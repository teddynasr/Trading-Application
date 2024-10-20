import React, { useEffect, useState } from 'react';
import { Box, Button, Typography } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import Chart from 'react-apexcharts';
import { useAuth } from '../Services/AuthProvider';
import { fetchHistoricalData, findPosition, fetchPortfolio } from '../Services/api';

const PairDetail = () => {
  const navigate = useNavigate();
  const { state } = useLocation();
  const { userID } = useAuth();
  const location = useLocation();

  const [candles, setCandles] = useState([]);
  const [activeTab, setActiveTab] = useState('');
  const [loading, setLoading] = useState(true);
  const [positionData, setPositionData] = useState(null);
  const [portfolioData, setPortfolioData] = useState(null);

  const baseCurrency = state?.baseCurrency || 'EUR';
  const quoteCurrency = state?.quoteCurrency || 'USD';

  useEffect(() => {
    const path = location.pathname;
    if (path.includes('crypto-rate')) {
      setActiveTab('Crypto');
    } else if (path.includes('forex-rate')) {
      setActiveTab('Forex');
    } else if (path.includes('stock-rate')) {
      setActiveTab('Stocks');
    }

    const uuidRegex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/;
    const match = path.match(uuidRegex);

    if (match) {
      const currencyPairID = match[0];
      const fetchPosition = async () => {
        const position = await findPosition(currencyPairID, userID);
        setPositionData(position);

        const portfolio = await fetchPortfolio(userID);
        setPortfolioData(portfolio);
      };

      fetchPosition();
    }
  }, [location.pathname, userID]);

  useEffect(() => {
    const fetchData = async () => {
      const startDate = new Date();
      startDate.setDate(1);
      const formattedStartDate = startDate.getFullYear() + '-' +
        String(startDate.getMonth() + 1).padStart(2, '0') + '-' +
        String(startDate.getDate()).padStart(2, '0');

      const endDate = new Date();
      endDate.setDate(endDate.getDate() + 1);
      const formattedEndDate = endDate.getFullYear() + '-' +
        String(endDate.getMonth() + 1).padStart(2, '0') + '-' +
        String(endDate.getDate()).padStart(2, '0');

      let url = '';

      switch (activeTab) {
        case 'Forex':
          url = `/CurrencyPair/history-forex-rate?baseCurrency=${baseCurrency}&quoteCurrency=${quoteCurrency}&startDate=${formattedStartDate}&endDate=${formattedEndDate}&candleSize=1day`;
          break;
        case 'Crypto':
          url = `/CurrencyPair/history-crypto-rate?baseCurrency=${baseCurrency}&quoteCurrency=${quoteCurrency}&startDate=${formattedStartDate}&endDate=${formattedEndDate}&candleSize=1day`;
          break;
        case 'Stocks':
          url = `/CurrencyPair/history-stock-rate?stock=${baseCurrency}&startDate=${formattedStartDate}&endDate=${formattedEndDate}&candleSize=6hour`;
          break;
        default:
          return;
      }

      const transformCryptoData = (cryptoResponse) => {
        return cryptoResponse[0].priceData.map((entry) => {
          return {
            date: entry.date,
            open: entry.open,
            high: entry.high,
            low: entry.low,
            close: entry.close
          };
        });
      };

      try {
        let response = await fetchHistoricalData(url);

        if (activeTab === 'Crypto') {
          response = transformCryptoData(response);
        }

        const parsedData = response.map(candle => ({
          ...candle,
          date: new Date(candle.date).getTime(),
        }));
        setCandles(parsedData);
      } catch (error) {
        console.error('Error fetching data:', error);
      } finally {
        setLoading(false);
      }
    };

    if (activeTab) {
      fetchData();
    }
  }, [activeTab, baseCurrency, quoteCurrency]);

  const chartData = {
    series: [{
      data: candles.map(candle => ({
        x: candle.date,
        y: [candle.open, candle.high, candle.low, candle.close]
      })),
    }],
  };

  const options = {
    chart: {
      type: 'candlestick',
      height: 350,
    },
    title: {
      text: `${baseCurrency} / ${quoteCurrency}`,
      align: 'left',
    },
    xaxis: {
      type: 'datetime',
    },
    yaxis: {
      tooltip: {
        enabled: true,
      },
    },
  };

  return (
    <Box sx={{ padding: 3, bgcolor: '#f5f5f5', borderRadius: 2 }}>
      <Button onClick={() => navigate(-1)} variant="outlined" sx={{ marginBottom: 2 }}>
        Back
      </Button>

      {loading ? (
        <Typography>Loading...</Typography>
      ) : (
        <Box sx={{ marginBottom: 3, maxWidth: 800 }}>
          <Chart options={options} series={chartData.series} type="candlestick" height={350} />
        </Box>
      )}

      <Box sx={{ display: 'flex', gap: 2 }}>
        <Button 
          variant="contained" 
          color="primary" 
          disabled={!userID || (portfolioData && portfolioData.availableWalletBalance === 0)} // Disable if wallet balance is 0
          onClick={() => navigate(`${window.location.pathname}/buy`, { state: { baseCurrency, quoteCurrency } })}
        >
          Buy
        </Button>
        <Button 
          variant="contained" 
          color="error" 
          disabled={!userID || !positionData} 
          onClick={() => navigate(`${window.location.pathname}/sell`, { state: { baseCurrency, quoteCurrency } })}
        >
          Sell
        </Button>
      </Box>
    </Box>
  );
};

export default PairDetail;
