import React, { useEffect, useState } from 'react';
import { Box, Typography, List, ListItem, ListItemText, Button, TextField } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { fetchCurrencyPairsByCategory } from '../Services/api';

const Dashboard = () => {
  const [activeTab, setActiveTab] = useState('Forex');
  const [searchTerm, setSearchTerm] = useState('');
  const [forexPairs, setForexPairs] = useState([]);
  const [cryptoPairs, setCryptoPairs] = useState([]);
  const [stockPairs, setStockPairs] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchAllCurrencyPairs = async () => {
      const forexData = await fetchCurrencyPairsByCategory('Forex');
      setForexPairs(forexData || []);
      
      const cryptoData = await fetchCurrencyPairsByCategory('Crypto');
      setCryptoPairs(cryptoData || []);
      
      const stockData = await fetchCurrencyPairsByCategory('Stocks');
      setStockPairs(stockData || []);
    };

    fetchAllCurrencyPairs();
  }, []);

  const handlePairClick = (pair) => {
    let route = '';
    switch (activeTab) {
      case 'Forex':
        route = `/forex-rate/${pair.pairId}`;
        break;
      case 'Crypto':
        route = `/crypto-rate/${pair.pairId}`;
        break;
      case 'Stocks':
        route = `/stock-rate/${pair.pairId}`;
        break;
      default:
        route = `/pair/${pair.pairId}`;
    }
    navigate(route, { state: { baseCurrency: pair.baseCurrency, quoteCurrency: pair.quoteCurrency } });
  };

  const handleTabChange = (category) => {
    setActiveTab(category);
    setSearchTerm('');
  };

  const filteredPairs = (() => {
    const pairs = activeTab === 'Forex' ? forexPairs :
                  activeTab === 'Crypto' ? cryptoPairs :
                  activeTab === 'Stocks' ? stockPairs : [];
    
    if (!Array.isArray(pairs)) {
      console.error('Pairs is not an array:', pairs);
      return [];
    }
  
    return pairs.filter(pair =>
      pair.baseCurrency.toLowerCase().includes(searchTerm.toLowerCase())
    );
  })();
  

  return (
    <Box sx={{ padding: 3, bgcolor: '#f5f5f5', borderRadius: 2 }}>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Box sx={{ display: 'flex', marginBottom: 2 }}>
        {['Forex', 'Crypto', 'Stocks'].map((category) => (
          <Button 
            key={category} 
            variant="contained" 
            onClick={() => handleTabChange(category)} 
            sx={{
              flex: 1,
              bgcolor: activeTab === category ? 'black' : 'primary.main',
              color: 'white',
              borderRadius: 1,
              marginRight: 1,
              '&:last-child': {
                marginRight: 0,
              }
            }}
          >
            {category}
          </Button>
        ))}
      </Box>
      <TextField
        label="Search by Base Currency"
        variant="outlined"
        fullWidth
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        sx={{ marginBottom: 2 }}
        autoComplete='off'
      />
      <List>
        {filteredPairs.map((pair) => (
          <ListItem 
            button 
            key={pair.pairId} 
            onClick={() => handlePairClick(pair)} 
            sx={{
              border: '1px solid #ccc', 
              borderRadius: 1, 
              marginBottom: 1, 
              bgcolor: 'white', 
              transition: 'background-color 0.2s',
              '&:hover': {
                backgroundColor: '#f0f0f0',
              }
            }}
          >
            <ListItemText primary={`${pair.baseCurrency}/${pair.quoteCurrency}`} />
          </ListItem>
        ))}
      </List>
    </Box>
  );
};

export default Dashboard;
