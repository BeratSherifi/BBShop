import React, { useEffect, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';
import { apiDeleteProduct } from '../api';

function ProductListSeller({ storeId }) {
  const [products, setProducts] = useState([]);
  const [error, setError] = useState(null);
  const token = localStorage.getItem('authToken');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await fetch(`http://localhost:5197/api/Product/by-store-id/${storeId}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
        });

        if (response.ok) {
          const data = await response.json();
          setProducts(data);
        } else {
          setError('Failed to fetch products');
        }
      } catch (error) {
        setError('An error occurred. Please try again.');
      }
    };

    if (storeId) {
      fetchProducts();
    }
  }, [storeId, token]);

  const handleEditProduct = (productId) => {
    navigate(`/edit-product/${productId}`);
  };

  const handleDeleteProduct = async (productId) => {
    const confirmed = window.confirm('Are you sure you want to delete this product?');
    if (confirmed) {
      try {
        await apiDeleteProduct(productId, token);
        setProducts(products.filter(product => product.productId !== productId));
      } catch (error) {
        setError('Failed to delete product');
      }
    }
  };

  return (
    <div className="container mt-5">
      <h2>Products</h2>
      {error && <div className="alert alert-danger">{error}</div>}
      <div className="row">
        {products.length > 0 ? (
          products.map((product) => (
            <div className="col-md-4 mb-4" key={product.productId}>
              <div className="card">
                <img src={`http://localhost:5197${product.imageUrl}`} className="card-img-top" alt={product.productName} />
                <div className="card-body">
                  <h5 className="card-title">{product.productName}</h5>
                  <p className="card-text">{product.description}</p>
                  <p className="card-text"><strong>Price:</strong> ${product.price}</p>
                  <p className="card-text"><strong>Stock:</strong> {product.stockQuantity}</p>
                  <button className="btn btn-warning mr-2" onClick={() => handleEditProduct(product.productId)}>Edit</button>
                  <button className="btn btn-danger" onClick={() => handleDeleteProduct(product.productId)}>Delete</button>
                </div>
              </div>
            </div>
          ))
        ) : (
          <p>No products found for this store.</p>
        )}
      </div>
    </div>
  );
}

export default ProductListSeller;
