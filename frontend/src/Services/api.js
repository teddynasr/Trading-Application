import axios from 'axios';
import config from '../config';

const api = axios.create({
  headers: {
    'Content-Type': 'application/json',
  },
});

const setAuthToken = (authToken) => {
  if (authToken) {
    api.defaults.headers.common['Authorization'] = `Bearer ${authToken}`;
  } else {
    delete api.defaults.headers.common['Authorization'];
  }
};

export const signup = async (signupData) => {
  try {
    const response = await api.post(`${config.userService}/users`, signupData);
    setAuthToken(response.data.token);
    return response;
  } catch (error) {
    console.error('Signup failed:', error);
    throw error;
  }
};

export const login = async (loginData) => {
  try {
    const response = await api.post(`${config.userService}/users/login`, loginData);
    setAuthToken(response.data.token);
    return response;
  } catch (error) {
    console.error('Login failed:', error);
    throw error;
  }
};

export const logout = async (userID) => {
  try {
    await api.post(`${config.userService}/users/logout`, userID);
    setAuthToken(null);
  } catch (error) {
    console.error('Logout failed:', error);
  }
};

export const fetchCurrencyPairsByCategory = async (categoryName) => {
  try {
    const response = await api.get(`${config.marketDataService}/currencyPair/category/${categoryName}`);
    return response.data;
  } catch (error) {
    console.error(`Fetching ${categoryName} pairs failed:`, error);
  }
};

export const fetchHistoricalData = async (endpoint) => {
  try {
    const response = await api.get(`${config.marketDataService}${endpoint}`);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const refreshPositions = async (userId) => {
  try {
    const response = await api.put(`${config.portfolioService}/UserPortfolio/${userId}`);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const fetchPortfolio = async (userId) => {
  try {
    const response = await api.get(`${config.portfolioService}/UserPortfolio/${userId}`);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const fetchPositions = async (userId) => {
  try {
    const response = await api.get(`${config.portfolioService}/position/user/${userId}`);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const fetchCurrencyPairs = async () => {
  try {
    const response = await api.get(`${config.marketDataService}/currencyPair`);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const userDeposit = async (depositBody) => {
  try {
    const response = await api.post(`${config.portfolioService}/userPortfolio/deposit`, depositBody);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const userWithdraw = async (withdrawBody) => {
  try {
    const response = await api.post(`${config.portfolioService}/userPortfolio/withdraw`, withdrawBody);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const atMarketForexRate = async (baseCurrency, quoteCurrency) => {
  try {
    const response = await api.get(`${config.marketDataService}/CurrencyPair/forex-rate?baseCurrency=${baseCurrency}&quoteCurrency=${quoteCurrency}`);
    return response.data[0];
  } catch (error) {
    console.error(`error:`);
  }
}

export const atMarketCryptoRate = async (baseCurrency, quoteCurrency) => {
  try {
    const response = await api.get(`${config.marketDataService}/CurrencyPair/crypto-rate?baseCurrency=${baseCurrency}&quoteCurrency=${quoteCurrency}`);
    return response.data[0];
  } catch (error) {
    console.error(`error:`);
  }
}

export const atMarketStockRate = async (stock) => {
  try {
    const response = await api.get(`${config.marketDataService}/CurrencyPair/stock-rate?stock=${stock}`);
    return response.data[0];
  } catch (error) {
    console.error(`error:`);
  }
}

export const updatePositionIfExists = async (currencyPairId, userId) => {
  try {
    const response = await api.get(`${config.portfolioService}/position/currencyPair/${currencyPairId}/user/${userId}/updated`);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const findPosition = async (currencyPairId, userId) => {
  try {
    const response = await api.get(`${config.portfolioService}/position/currencyPair/${currencyPairId}/user/${userId}`);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}

export const userOrder = async (userOrderBody) => {
  try {
    const response = await api.post(`${config.orderService}/order`, userOrderBody);
    return response.data;
  } catch (error) {
    console.error(`error:`);
  }
}