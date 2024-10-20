import React, { useEffect, useState } from 'react';
import { refreshPositions, fetchPortfolio, fetchPositions, fetchCurrencyPairs } from '../Services/api';
import { CardContent, Typography, Card, Box, Button } from '@mui/material';
import { useAuth } from '../Services/AuthProvider';
import { useNavigate } from 'react-router-dom';

const PortfolioPage = () => {
  const { userID } = useAuth();
  const [portfolio, setPortfolio] = useState(null);
  const [positions, setPositions] = useState([]);
  const [currencyPairs, setCurrencyPairs] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      if (!userID) return;
      setLoading(true);
      try {
        await refreshPositions(userID);

        const portfolioData = await fetchPortfolio(userID);
        setPortfolio(portfolioData);

        const positionsData = await fetchPositions(userID);
        setPositions(positionsData);

        const currencyPairsData = await fetchCurrencyPairs();
        setCurrencyPairs(currencyPairsData);
      } catch (error) {
        console.error('Error fetching data:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [userID]);

  if (loading) {
    return <Typography variant="h6">Loading...</Typography>;
  }

  const totalBalance = (portfolio?.availableWalletBalance || 0) + (portfolio?.investedBalance || 0);
  const getBaseCurrency = (currencyPairID) => {
    const pair = currencyPairs.find(pair => pair.pairId === currencyPairID);
    return pair ? pair.baseCurrency : 'Unknown';
  };

  return (
    <Box sx={{ padding: 2 }}>
      <Typography variant="h4">Portfolio Overview</Typography>
      <Typography variant="h6">Total Balance: ${totalBalance.toFixed(2)}</Typography>
      <Typography variant="h6">Available Wallet Balance (Uninvested Funds): ${portfolio.availableWalletBalance.toFixed(2)}</Typography>
      <Typography variant="h6">Invested Balance: ${portfolio.investedBalance.toFixed(2)}</Typography>
      <Box display="flex" justifyContent="flex-end" sx={{ marginTop: 2 }}>
        <Box sx={{ marginLeft: 1 }}>
          <Button 
            variant="contained" 
            color="primary" 
            onClick={() => navigate('/deposit')}
          >
            Deposit
          </Button>
        </Box>
        <Box sx={{ marginLeft: 1 }}>
          <Button 
            variant="contained" 
            color="error" 
            onClick={() => navigate('/withdraw')}
          >
            Withdraw
          </Button>
        </Box>
      </Box>
      <Typography variant="h5" sx={{ marginTop: 2 }}>Positions:</Typography>
      {positions.map((position) => (
        <Card key={position.positionID} sx={{ marginTop: 2 }}>
          <CardContent>
            <Typography variant="h6">Base Currency: {getBaseCurrency(position.currencyPairID)}</Typography>
            <Typography>Amount: {position.amount}</Typography>
            <Typography>Current Value (USD): ${position.currentValueUSD.toFixed(2)}</Typography>
          </CardContent>
        </Card>
      ))}
    </Box>
  );
};

export default PortfolioPage;
