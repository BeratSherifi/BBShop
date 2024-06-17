import React, { useState } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import StorePageSeller from './components/StorePageSeller';
import StorePageBuyer from './components/StorePageBuyer';
import CreateStorePage from './components/CreateStorePage';
import ProfilePage from './components/ProfilePage';
import Navbar from './components/Navbar';
import AddProductPage from './components/AddProductPage';
import EditProductPage from './components/EditProductPage';
import SearchPage from './components/SearchPage';
import OrderListPageSeller from './components/OrderListPageSeller';
import OrderListPageBuyer from './components/OrderListPageBuyer';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  const [token, setToken] = useState(localStorage.getItem('authToken'));
  const [role, setRole] = useState(sessionStorage.getItem('userRole'));

  const handleLoginSuccess = (token, role) => {
    setToken(token);
    setRole(role);
    localStorage.setItem('authToken', token);
    sessionStorage.setItem('userRole', role);
  };

  return (
    <Router>
      {token && <Navbar role={role} />}
      <Routes>
        <Route path="/login" element={<Login onLoginSuccess={handleLoginSuccess} />} />
        <Route path="/register" element={<Register />} />
        <Route path="/store-seller" element={token && role === 'seller' ? <StorePageSeller /> : <Navigate to="/login" />} />
        <Route path="/store/:storeId" element={<StorePageBuyer />} />
        <Route path="/create-store" element={token && role === 'seller' ? <CreateStorePage /> : <Navigate to="/login" />} />
        <Route path="/add-product" element={token && role === 'seller' ? <AddProductPage /> : <Navigate to="/login" />} />
        <Route path="/edit-product/:productId" element={token && role === 'seller' ? <EditProductPage /> : <Navigate to="/login" />} />
        <Route path="/profile" element={token ? <ProfilePage /> : <Navigate to="/login" />} />
        <Route path="/search" element={token ? <SearchPage /> : <Navigate to="/login" />} />
        <Route path="/orders-seller" element={token && role === 'seller' ? <OrderListPageSeller /> : <Navigate to="/login" />} />
        <Route path="/orders-buyer" element={token && role === 'buyer' ? <OrderListPageBuyer /> : <Navigate to="/login" />} />
        <Route path="*" element={<Navigate to="/login" />} />
      </Routes>
    </Router>
  );
}

export default App;
