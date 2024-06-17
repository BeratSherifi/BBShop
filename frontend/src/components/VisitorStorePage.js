import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import ProductListVisitor from './ProductListVisitor';

function VisitorStorePage() {
  const { storeId } = useParams();
  const [store, setStore] = useState(null);
  const [error, setError] = useState(null);
  const token = localStorage.getItem('authToken');
  const role = sessionStorage.getItem('userRole');

  const fetchStore = async () => {
    try {
      const response = await fetch(`http://localhost:5197/api/Store/${storeId}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setStore(data);
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
    if (storeId) {
      fetchStore();
    }
  }, [storeId]);

  return (
    <div className="container mt-5">
      {store ? (
        <div>
          <h1 className="mb-4">{store.storeName}</h1>
          {store.logoUrl && (
            <div>
              <h6>Store Logo:</h6>
              <img src={`http://localhost:5197${store.logoUrl}`} alt="Store Logo" className="img-fluid" />
            </div>
          )}
          <ProductListVisitor storeId={store.storeId} role={role} />
        </div>
      ) : (
        <div>
          <h1 className="mb-4">Store</h1>
          {error && <div className="alert alert-danger">{error}</div>}
        </div>
      )}
    </div>
  );
}

export default VisitorStorePage;
