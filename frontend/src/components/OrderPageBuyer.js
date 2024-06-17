import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { apiCreateOrder } from '../api';

function OrderPageBuyer() {
  const [storeId, setStoreId] = useState('');
  const [productId, setProductId] = useState('');
  const [quantity, setQuantity] = useState(1);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const token = localStorage.getItem('authToken');

  const handleCreateOrder = async (e) => {
    e.preventDefault();
    const orderData = {
      storeId,
      orderItems: [
        {
          productId,
          quantity,
        },
      ],
    };

    try {
      await apiCreateOrder(orderData, token);
      setSuccess(true);
      setError(null);
      setStoreId('');
      setProductId('');
      setQuantity(1);
    } catch (error) {
      setSuccess(false);
      setError('Failed to create order');
    }
  };

  return (
    <div className="container mt-5">
      <h1 className="mb-4">Create Order</h1>
      <form onSubmit={handleCreateOrder}>
        <div className="mb-3">
          <label htmlFor="storeId" className="form-label">Store ID</label>
          <input
            type="text"
            className="form-control"
            id="storeId"
            value={storeId}
            onChange={(e) => setStoreId(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="productId" className="form-label">Product ID</label>
          <input
            type="text"
            className="form-control"
            id="productId"
            value={productId}
            onChange={(e) => setProductId(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="quantity" className="form-label">Quantity</label>
          <input
            type="number"
            className="form-control"
            id="quantity"
            value={quantity}
            onChange={(e) => setQuantity(parseInt(e.target.value))}
            required
          />
        </div>
        <button type="submit" className="btn btn-success">Create Order</button>
      </form>
      {success && <div className="alert alert-success mt-4">Order created successfully!</div>}
      {error && <div className="alert alert-danger mt-4">{error}</div>}
    </div>
  );
}

export default OrderPageBuyer;
