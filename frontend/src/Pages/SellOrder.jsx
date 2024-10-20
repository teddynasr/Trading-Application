import React, { useState, useEffect } from 'react';
import { Box, Typography, Slider, Button, TextField } from '@mui/material';
import { useAuth } from '../Services/AuthProvider';
import { updatePositionIfExists, userOrder } from '../Services/api';
import { useLocation, useNavigate } from 'react-router-dom';

const SellOrder = () => {
    const { userID } = useAuth();
    const { state } = useLocation();
    const { baseCurrency, quoteCurrency } = state;
    const [positionData, setPositionData] = useState(null);
    const [sellAmount, setSellAmount] = useState(0);
    const [totalRevenue, setTotalRevenue] = useState(0);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchPositionData = async () => {
            const path = window.location.pathname;
            const uuidRegex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/;
            const match = path.match(uuidRegex);

            if (match) {
                const currencyPairID = match[0];
                const position = await updatePositionIfExists(currencyPairID, userID);
                setPositionData(position);
            }
        };

        fetchPositionData();
    }, [userID]);

    const handleAmountChange = (event) => {
        const value = Math.max(0, Math.min(positionData?.amount || 0, parseFloat(event.target.value)));
        setSellAmount(value);
    };

    const unitPrice = positionData ? positionData.currentValueUSD / positionData.amount : 0;
    useEffect(() => {
        setTotalRevenue(unitPrice * sellAmount);
    }, [sellAmount, unitPrice]);

    const handleSell = async () => {
        await userOrder({
            userId: userID,
            currencyPairID: positionData.currencyPairID,
            orderAmount: sellAmount,
            orderUnitPrice: unitPrice,
            orderType: 'sell',
        });

        navigate('/');
    };

    if (!positionData) {
        return <div>Loading...</div>;
    }

    return (
        <Box sx={{ padding: 3, maxWidth: 500, margin: 'auto' }}>
            <Button
                variant="outlined"
                onClick={() => navigate(-1)}
                sx={{ marginBottom: 2 }}
            >
                Back
            </Button>

            <Typography variant="h5" gutterBottom>
                Sell Forex ({baseCurrency}/{quoteCurrency})
            </Typography>
            <Typography variant="subtitle1">
                Unit Price: ${unitPrice.toFixed(2)}
            </Typography>
            <Typography variant="subtitle1">
                Available Amount: {positionData.amount.toFixed(8)} {baseCurrency}
            </Typography>

            <Typography gutterBottom>Amount:</Typography>
            <TextField
                type="number"
                value={sellAmount}
                onChange={handleAmountChange}
                inputProps={{
                    min: 0,
                    max: positionData.amount,
                    step: 0.00000001,
                    style: { textAlign: 'center' },
                }}
                sx={{ width: '100%', marginBottom: 2 }}
            />

            <Slider
                value={sellAmount}
                min={0}
                max={positionData.amount}
                step={0.00000001}
                onChange={(e, value) => setSellAmount(value)}
                valueLabelDisplay="auto"
                sx={{ marginBottom: 2 }}
            />

            <Typography variant="h6">Total Revenue: ${totalRevenue.toFixed(2)}</Typography>

            <Button
                variant="contained"
                color="primary"
                disabled={sellAmount <= 0}
                onClick={handleSell}
                sx={{ marginTop: 2 }}
            >
                Confirm Sale
            </Button>
        </Box>
    );
};

export default SellOrder;
