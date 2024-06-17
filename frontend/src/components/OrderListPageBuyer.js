// OrderListPageBuyer.js
import React, { useState, useEffect } from 'react';
import { apiGetOrdersByBuyer } from '../api';

function OrderListPageBuyer() {
  const [orders, setOrders] = useState([]);
  const [error, setError] = useState(null);
  const token = localStorage.getItem('authToken');

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const data = await apiGetOrdersByBuyer(token);
        setOrders(data);
      } catch (error) {
        setError('Failed to fetch orders');
      }
    };

    fetchOrders();
  }, [token]);

  return (
    <div className="container mt-5">
      <h1>Orders</h1>
      {error && <div className="alert alert-danger">{error}</div>}
      <div className="row">
        {orders.length > 0 ? (
          orders.map((order) => (
            <div className="col-md-4 mb-4" key={order.orderId}>
              <div className="card">
                <div className="card-body">
                  <h5 className="card-title">Order ID: {order.orderId}</h5>
                  <p className="card-text"><strong>Status:</strong> {order.status}</p>
                  <p className="card-text"><strong>Order Date:</strong> {order.orderDate}</p>
                  <p className="card-text"><strong>Products:</strong></p>
                  <ul>
                    {order.orderItems.map(item => (
                      <li key={item.orderItemId}>{item.productName} - Quantity: {item.quantity}</li>
                    ))}
                  </ul>
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

export default OrderListPageBuyer;
