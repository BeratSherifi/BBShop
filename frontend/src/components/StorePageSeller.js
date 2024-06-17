// StorePageSeller.js
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import ProductListSeller from './ProductListSeller';

function StorePageSeller() {
  const [store, setStore] = useState(null);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const userId = sessionStorage.getItem('userId');
  const token = localStorage.getItem('authToken');

  const fetchStore = async () => {
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
        setStore(data);
        localStorage.setItem('storeId', data.storeId); // Store the storeId in localStorage
        setError(null);
      } else if (response.status === 404) {
        setStore(null);
        setError('No store found');
      } else {
        const errorText = await response.text();
        setError(`Failed to fetch store: ${errorText}`);
      }
    } catch (error) {
      setStore(null);
      setError('An error occurred. Please try again.');
    }
  };

  useEffect(() => {
    fetchStore();
  }, []);

  const handleCreateStoreClick = () => {
    navigate('/create-store');
  };

  const handleAddProductClick = () => {
    navigate('/add-product');
  };

  return (
    <div className="container mt-5">
      {store ? (
        <div>
          <h1 className="mb-4">Your Store</h1>
          <div className="card mb-4">
            <div className="card-body">
              <h5 className="card-title">Store Name: {store.storeName}</h5>
              {store.logoUrl && (
                <div>
                  <h6>Store Logo:</h6>
                  <img src={`http://localhost:5197${store.logoUrl}`} alt="Store Logo" className="img-fluid" />
                </div>
              )}
              <button onClick={handleAddProductClick} className="btn btn-primary mt-3">Add Product</button>
            </div>
          </div>
          <ProductListSeller storeId={store.storeId} />
        </div>
      ) : (
        <div>
          <h1 className="mb-4">Your Store</h1>
          <p className="text-muted">You still don't have a store.</p>
          <button onClick={handleCreateStoreClick} className="btn btn-primary mb-4">Create Store</button>
        </div>
      )}
      {error && <div className="alert alert-danger">{error}</div>}
    </div>
  );
}

export default StorePageSeller;
