import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { apiGetProductById, apiUpdateProduct } from '../api';

function EditProductPage() {
  const { productId } = useParams();
  const [productName, setProductName] = useState("");
  const [description, setDescription] = useState("");
  const [price, setPrice] = useState(0);
  const [stockQuantity, setStockQuantity] = useState(0);
  const [image, setImage] = useState(null);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const navigate = useNavigate();
  const token = localStorage.getItem('authToken');

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        const productData = await apiGetProductById(productId, token);
        setProductName(productData.productName);
        setDescription(productData.description);
        setPrice(productData.price);
        setStockQuantity(productData.stockQuantity);
        setImage(null);
      } catch (error) {
        setError("Failed to fetch product");
      }
    };

    fetchProduct();
  }, [productId, token]);

  const handleUpdateProduct = async (e) => {
    e.preventDefault();
    const formData = new FormData();
    formData.append("productName", productName);
    formData.append("description", description);
    formData.append("price", price);
    formData.append("stockQuantity", stockQuantity);
    if (image) {
      formData.append("image", image);
    }

    try {
      const response = await apiUpdateProduct(productId, formData, token);
      if (response.status === 204) {
        setSuccess(true);
        setError(null);
        navigate('/store-seller'); // Redirect to store page after successful update
      } else {
        setSuccess(false);
        setError("Failed to update product");
      }
    } catch (error) {
      setSuccess(false);
      setError("Failed to update product");
    }
  };

  return (
    <div className="container mt-5">
      <h1 className="mb-4">Edit Product</h1>
      <form onSubmit={handleUpdateProduct}>
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
          />
        </div>
        <button type="submit" className="btn btn-success">Update Product</button>
      </form>
      {success && <div className="alert alert-success mt-4">Product updated successfully!</div>}
      {error && <div className="alert alert-danger mt-4">{error}</div>}
    </div>
  );
}

export default EditProductPage;