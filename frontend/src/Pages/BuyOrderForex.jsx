import React, { useState, useEffect } from 'react';
import { Box, Typography, Slider, Button, TextField } from '@mui/material';
import { useAuth } from '../Services/AuthProvider';
import { atMarketForexRate, fetchPortfolio, userOrder } from '../Services/api';
import { useLocation, useNavigate } from 'react-router-dom';

const BuyOrderForex = () => {
    const { userID } = useAuth();
    const { state } = useLocation();
    const { baseCurrency, quoteCurrency } = state;
    const [forexData, setForexData] = useState(null);
    const [portfolio, setPortfolio] = useState(null);
    const [amount, setAmount] = useState(0);
    const [totalValue, setTotalValue] = useState(0); 
    const navigate = useNavigate();

    useEffect(() => {
        async function fetchData() {
            const forexResponse = await atMarketForexRate(baseCurrency, quoteCurrency);
            setForexData(forexResponse);

            const portfolioData = await fetchPortfolio(userID);
            setPortfolio(portfolioData);
        }

        fetchData();
    }, [baseCurrency, quoteCurrency, userID]);

    const handleSliderChange = (event, newValue) => {
        setAmount(newValue);
        if (forexData) {
            setTotalValue(newValue * forexData.midPrice);
        }
    };

    const handleInputChange = (event) => {
        const newValue = Number(event.target.value);
        if (newValue >= 0 && forexData && newValue <= maxAmount) {
            setAmount(newValue);
            setTotalValue(newValue * forexData.midPrice);
        }
    };

    const handleBuy = async () => {
        const path = window.location.pathname;
        const uuidRegex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/;
        const match = path.match(uuidRegex);

        const currentCurrencyPairID = match[0];

        await userOrder({
            userId: userID,
            currencyPairID: currentCurrencyPairID,
            orderAmount: amount,
            orderUnitPrice: totalValue / amount,
            orderType: 'buy'
        });

        navigate('/');
    };

    if (!forexData || !portfolio) {
        return <div>Loading...</div>;
    }

    const maxAmount = Math.floor(portfolio.availableWalletBalance / forexData.midPrice);

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
                Buy Forex ({baseCurrency}/{quoteCurrency})
            </Typography>
            <Typography variant="subtitle1">
                Available Wallet Balance: {portfolio.availableWalletBalance.toFixed(2)} USD
            </Typography>
            <Typography variant="subtitle1" gutterBottom>
                Total Value: {totalValue.toFixed(2)} {quoteCurrency}
            </Typography>

            <TextField
                label="Amount"
                type="number"
                value={amount}
                onChange={handleInputChange}
                inputProps={{ min: 0, max: maxAmount, step: 0.00000001 }}
                fullWidth
                sx={{ marginBottom: 3 }}
            />

            <Slider
                value={amount}
                onChange={handleSliderChange}
                aria-labelledby="amount-slider"
                valueLabelDisplay="auto"
                step={0.00000001}
                min={0}
                max={maxAmount}
                sx={{ marginBottom: 3 }}
            />

            <Button
                variant="contained"
                onClick={handleBuy}
                disabled={totalValue >= portfolio.availableWalletBalance || amount === 0}
            >
                Buy {amount} Units
            </Button>
        </Box>
    );
};

export default BuyOrderForex;
