import React, { useState } from 'react';

function AddProductPage() {
  const [productName, setProductName] = useState("");
  const [description, setDescription] = useState("");
  const [price, setPrice] = useState(0);
  const [stockQuantity, setStockQuantity] = useState(0);
  const [image, setImage] = useState(null);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  const token = localStorage.getItem('authToken');

  const handleAddProduct = async (e) => {
    e.preventDefault();
    const formData = new FormData();
    formData.append("productName", productName);
    formData.append("description", description);
    formData.append("price", price);
    formData.append("stockQuantity", stockQuantity);
    formData.append("image", image);

    try {
      const response = await fetch('http://localhost:5197/api/product', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
        body: formData,
      });
      if (response.ok) {
        setProductName("");
        setDescription("");
        setPrice(0);
        setStockQuantity(0);
        setImage(null);
        setSuccess(true);
        setError(null);
      } else {
        setSuccess(false);
        setError("Failed to add product");
      }
    } catch (error) {
      setSuccess(false);
      setError("An error occurred. Please try again.");
    }
  };

  return (
    <div className="container mt-5">
      <h1 className="mb-4">Add Product</h1>
      <form onSubmit={handleAddProduct}>
        <div className="mb-3">
          <label htmlFor="productName" className="form-label">Product Name</label>
          <input
            type="text"
            className="form-control"
            id="productName"
            value={productName}
            onChange={(e) => setProductName(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="description" className="form-label">Description</label>
          <input
            type="text"
            className="form-control"
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="price" className="form-label">Price</label>
          <input
            type="number"
            className="form-control"
            id="price"
            value={price}
            onChange={(e) => setPrice(parseFloat(e.target.value))}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="stockQuantity" className="form-label">Stock Quantity</label>
          <input
            type="number"
            className="form-control"
            id="stockQuantity"
            value={stockQuantity}
            onChange={(e) => setStockQuantity(parseInt(e.target.value))}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="image" className="form-label">Product Image</label>
          <input
            type="file"
            className="form-control"
            id="image"
            onChange={(e) => setImage(e.target.files[0])}
            required
          />
        </div>
        <button type="submit" className="btn btn-success">Add Product</button>
      </form>
      {success && <div className="alert alert-success mt-4">Product added successfully!</div>}
      {error && <div className="alert alert-danger mt-4">{error}</div>}
    </div>
  );
}

export default AddProductPage;