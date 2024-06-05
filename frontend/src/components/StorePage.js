import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function StorePage() {
  const [storeName, setStoreName] = useState("");
  const [stores, setStores] = useState([]);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const token = localStorage.getItem('authToken');

  const fetchStores = async () => {
    try {
      const response = await fetch('http://localhost:5197/api/store', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      });
      if (response.ok) {
        const data = await response.json();
        setStores(data);
        setError(null);
      } else {
        setError("Failed to fetch stores");
      }
    } catch (error) {
      setError("An error occurred. Please try again.");
    }
  };

  useEffect(() => {
    fetchStores();
  }, []);

  const handleCreateStore = async (e) => {
    e.preventDefault();
    const formData = new FormData();
    formData.append("storeName", storeName);

    try {
      const response = await fetch('http://localhost:5197/api/store', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
        body: formData,
      });
      if (response.ok) {
        setStoreName("");
        setError(null);
        fetchStores(); // Refresh the store list
      } else {
        setError("Failed to create store");
      }
    } catch (error) {
      setError("An error occurred. Please try again.");
    }
  };

  return (
    <div>
      <form onSubmit={handleCreateStore}>
        <input
          type="text"
          value={storeName}
          onChange={(e) => setStoreName(e.target.value)}
          placeholder="Store Name"
        />
        <button type="submit">CREATE STORE</button>
      </form>
      {error && <div style={{ color: 'red' }}>{error}</div>}
      <div>
        <h2>Your Stores</h2>
        <ul>
          {stores.map(store => (
            <li key={store.storeId}>{store.storeName}</li>
          ))}
        </ul>
      </div>
    </div>
  );
}

export default StorePage;
