import React, { useState } from 'react';
import { Box, Typography, Button, TextField } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { userDeposit } from '../Services/api';
import { useAuth } from '../Services/AuthProvider';

const Deposit = () => {
  const { userID } = useAuth();
  const [amount, setAmount] = useState('');
  const navigate = useNavigate();

  const handleDepositChange = (event) => {
    setAmount(event.target.value);
  };

  const handleDepositClick = async () => {
    if (amount) {
      try {
        await userDeposit({ userId: userID, depositAmount: parseFloat(amount) });
        setAmount('');
        navigate('/portfolio')
      } catch (error) {
        console.error('Deposit error:', error);
      }
    } else {
      alert("Please enter a valid amount.");
    }
  };

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
        Deposit Funds
      </Typography>

      <TextField
        label="Deposit Amount"
        type="number"
        value={amount}
        onChange={handleDepositChange}
        fullWidth
        required
        inputProps={{ min: 0, step: 1 }}
        sx={{ marginBottom: 3 }}
        autoComplete='off'
      />

      <Button
        variant="contained"
        onClick={handleDepositClick}
        disabled={amount === ''}
        sx={{ marginBottom: 2 }}
      >
        Deposit {amount} USD
      </Button>
    </Box>
  );
};

export default Deposit;
