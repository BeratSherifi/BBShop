import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

function SearchPage() {
  const [searchQuery, setSearchQuery] = useState('');
  const [stores, setStores] = useState([]);
  const [error, setError] = useState(null);
  const navigate = useNavigate();
  const token = localStorage.getItem('authToken');
  const role = sessionStorage.getItem('userRole');

  const handleSearch = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch(`http://localhost:5197/api/Store/search/${searchQuery}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setStores(data);
        setError(null);
      } else {
        setError('Failed to fetch stores');
      }
    } catch (error) {
      setError('An error occurred. Please try again.');
    }
  };

  const handleStoreClick = (storeId) => {
    navigate(`/store/${storeId}`, { state: { role } });
  };

  return (
    <div className="container mt-5">
      <h1>Search Stores</h1>
      <form onSubmit={handleSearch} className="mb-4">
        <input
          type="text"
          className="form-control"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          placeholder="Enter store name"
          required
        />
        <button type="submit" className="btn btn-primary mt-3">Search</button>
      </form>
      {error && <div className="alert alert-danger">{error}</div>}
      <div className="row">
        {stores.map((store) => (
          <div className="col-md-4 mb-4" key={store.storeId}>
            <div className="card" onClick={() => handleStoreClick(store.storeId)}>
              <div className="card-body">
                <h5 className="card-title">{store.storeName}</h5>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default SearchPage;
