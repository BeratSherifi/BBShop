import React, { useState, useEffect } from 'react';
import { apiGetOrdersByStoreId, apiUpdateOrderStatus } from '../api';

function OrderListPageSeller() {
  const [orders, setOrders] = useState([]);
  const [error, setError] = useState(null);
  const [storeId, setStoreId] = useState(null);

  const token = localStorage.getItem('authToken');
  const userId = sessionStorage.getItem('userId');

  const fetchStoreId = async () => {
    try {
      const response = await fetch(`http://localhost:5197/api/Store/by-user-id/${userId}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setStoreId(data.storeId);
      } else {
        const errorText = await response.text();
        setError(`Failed to fetch store ID: ${errorText}`);
      }
    } catch (error) {
      setError('An error occurred while fetching store ID. Please try again.');
    }
  };

  const fetchOrders = async () => {
    try {
      const orders = await apiGetOrdersByStoreId(storeId, token);
      setOrders(orders);
    } catch (error) {
      setError('Failed to fetch orders');
    }
  };

  useEffect(() => {
    if (userId && token) {
      fetchStoreId();
    }
  }, [userId, token]);

  useEffect(() => {
    if (storeId) {
      fetchOrders();
    }
  }, [storeId]);

  const handleUpdateStatus = async (orderId, newStatus) => {
    try {
      await apiUpdateOrderStatus(orderId, newStatus, token);
      fetchOrders();
    } catch (error) {
      setError('Failed to update order status');
    }
  };

  return (
    <div className="container">
      <h1 className="mb-4">Orders</h1>
      {error && <div className="alert alert-danger">{error}</div>}
      <div className="row">
        {orders.length > 0 ? (
          orders.map(order => (
            <div className="col-md-4 mb-4" key={order.orderId}>
              <div className="card">
                <div className="card-body">
                  <h5 className="card-title">Order ID: {order.orderId}</h5>
                  <p className="card-text"><strong>Status:</strong> {order.status}</p>
                  <p className="card-text"><strong>Order Date:</strong> {order.orderDate}</p>
                  <p className="card-text"><strong>User ID:</strong> {order.userId}</p>
                  <p className="card-text"><strong>Products:</strong></p>
                  <ul>
                    {order.orderItems.map(item => (
                      <li key={item.orderItemId}>{item.productName} - Quantity: {item.quantity}</li>
                    ))}
                  </ul>
                  <button onClick={() => handleUpdateStatus(order.orderId, 'Completed')} className="btn btn-primary">Update Status</button>
                </div>
              </div>
            </div>
          ))
        ) : (
          <p>No orders found.</p>
        )}
      </div>
    </div>
  );
}

export default OrderListPageSeller;
