import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Dashboard from './Pages/Dashboard';
import PairDetail from './Pages/PairDetail';
import BuyOrderForex from './Pages/BuyOrderForex';
import BuyOrderCrypto from './Pages/BuyOrderCrypto';
import BuyOrderStocks from './Pages/BuyOrderStocks';
import SellOrder from './Pages/SellOrder';
import Login from './Pages/Login';
import Portfolio from './Pages/Portfolio';
import Signup from './Pages/Signup';
import Navbar from './Components/Navbar';
import Deposit from './Pages/Deposit';
import Withdraw from './Pages/Withdraw';
import { AuthProvider } from './Services/AuthProvider';

const App = () => {
  return (
    <AuthProvider>
      <Router>
        <Navbar />
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/portfolio" element={<Portfolio />} />
          <Route path="/deposit" element={<Deposit />} />
          <Route path="/withdraw" element={<Withdraw />} />
          <Route path="/login" element={<Login />} />
          <Route path="/signup" element={<Signup />} />
          <Route path="/:category/:pairId" element={<PairDetail />} />
          <Route path="/forex-rate/:pairId/buy" element={<BuyOrderForex />} />
          <Route path="/crypto-rate/:pairId/buy" element={<BuyOrderCrypto />} />
          <Route path="/stock-rate/:pairId/buy" element={<BuyOrderStocks />} />
          <Route path="/:category/:pairId/sell" element={<SellOrder />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
};

export default App;
