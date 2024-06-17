import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

function CreateStorePage() {
  const [storeName, setStoreName] = useState("");
  const [logo, setLogo] = useState(null);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const navigate = useNavigate();

  const token = localStorage.getItem('authToken');

  const handleCreateStore = async (e) => {
    e.preventDefault();
    const formData = new FormData();
    formData.append("storeName", storeName);
    formData.append("logo", logo);

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
        setLogo(null);
        setSuccess(true);
        setError(null);
        navigate('/store');
      } else {
        const errorText = await response.text();
        setSuccess(false);
        setError(`Failed to create store: ${errorText}`);
      }
    } catch (error) {
      setSuccess(false);
      setError("An error occurred. Please try again.");
    }
  };

  return (
    <div className="container mt-5">
      <h1 className="mb-4">Create Your Store</h1>
      <form onSubmit={handleCreateStore}>
        <div className="mb-3">
          <label htmlFor="storeName" className="form-label">Store Name</label>
          <input
            type="text"
            className="form-control"
            id="storeName"
            value={storeName}
            onChange={(e) => setStoreName(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="storeLogo" className="form-label">Store Logo</label>
          <input
            type="file"
            className="form-control"
            id="storeLogo"
            onChange={(e) => setLogo(e.target.files[0])}
            required
          />
        </div>
        <button type="submit" className="btn btn-success">Create Store</button>
      </form>
      {success && <div className="alert alert-success mt-4">Store created successfully!</div>}
      {error && <div className="alert alert-danger mt-4">{error}</div>}
    </div>
  );
}

export default CreateStorePage;