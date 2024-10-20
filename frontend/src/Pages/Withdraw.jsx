import React, { useState, useEffect } from 'react';
import { Box, Typography, Slider, Button, TextField } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { fetchPortfolio, userWithdraw } from '../Services/api';
import { useAuth } from '../Services/AuthProvider';

const Withdraw = () => {
  const { userID } = useAuth();
  const [portfolioData, setPortfolioData] = useState(null);
  const [amount, setAmount] = useState(0);
  const navigate = useNavigate();

  useEffect(() => {
    const loadPortfolio = async () => {
      const data = await fetchPortfolio(userID);
      setPortfolioData(data);
    };
    
    loadPortfolio();
  }, [userID]);

  const handleSliderChange = (event, newValue) => {
    setAmount(newValue);
  };

  const handleInputChange = (event) => {
    const newValue = Number(event.target.value);
    if (newValue >= 0 && newValue <= portfolioData?.availableWalletBalance) {
      setAmount(newValue);
    }
  };

  const handleWithdrawClick = async () => {
    if (amount) {
      try {
        await userWithdraw({ userId: userID, withdrawAmount: parseFloat(amount) });
        setAmount(0);
        navigate('/portfolio');
      } catch (error) {
        console.error('Withdrawal error:', error);
      }
    } else {
      alert("Please enter a valid amount.");
    }
  };

  if (!portfolioData) {
    return <Typography>Loading...</Typography>;
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
        Withdraw Funds
      </Typography>
      <Typography variant="subtitle1">
        Available Wallet Balance: {portfolioData.availableWalletBalance.toFixed(2)} USD
      </Typography>
      <Typography variant="subtitle1" gutterBottom>
        Amount to Withdraw: {amount.toFixed(2)} USD
      </Typography>

      <TextField
        label="Amount"
        type="number"
        value={amount}
        onChange={handleInputChange}
        inputProps={{ min: 0, max: portfolioData.availableWalletBalance, step: 1 }}
        fullWidth
        sx={{ marginBottom: 3 }}
      />

      <Slider
        value={amount}
        onChange={handleSliderChange}
        aria-labelledby="amount-slider"
        valueLabelDisplay="auto"
        step={1}
        min={0}
        max={portfolioData.availableWalletBalance}
        sx={{ marginBottom: 3 }}
      />

      <Button
        variant="contained"
        onClick={handleWithdrawClick}
        disabled={amount === 0}
        sx={{ marginBottom: 2 }}
      >
        Withdraw {amount} USD
      </Button>
    </Box>
  );
};

export default Withdraw;
