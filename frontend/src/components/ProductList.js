import React, { useEffect, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';

function ProductList({ storeId, role }) {
  const [products, setProducts] = useState([]);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [quantities, setQuantities] = useState({});
  const token = localStorage.getItem('authToken');

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

  const handleQuantityChange = (productId, value) => {
    setQuantities({
      ...quantities,
      [productId]: value,
    });
  };

  const handleBuyProduct = async (productId) => {
    const quantity = quantities[productId] || 1;

    try {
      const response = await fetch('http://localhost:5197/api/Order', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify({
          storeId,
          orderItems: [
            {
              productId,
              quantity,
            },
          ],
        }),
      });

      if (response.ok) {
        setSuccess('Order placed successfully!');
        setError(null);
      } else {
        setError('Failed to place order');
        setSuccess(null);
      }
    } catch (error) {
      setError('An error occurred. Please try again.');
      setSuccess(null);
    }
  };

  return (
    <div className="container mt-5">
      <h2>Products</h2>
      {error && <div className="alert alert-danger">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
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
                  {role === 'seller' ? (
                    <>
                      <button className="btn btn-warning mr-2">Edit</button>
                      <button className="btn btn-danger">Delete</button>
                    </>
                  ) : (
                    <>
                      <div className="mb-3">
                        <label htmlFor={`quantity-${product.productId}`} className="form-label">Quantity</label>
                        <input
                          type="number"
                          id={`quantity-${product.productId}`}
                          className="form-control"
                          value={quantities[product.productId] || 1}
                          onChange={(e) => handleQuantityChange(product.productId, parseInt(e.target.value))}
                          min="1"
                          max={product.stockQuantity}
                        />
                      </div>
                      <button onClick={() => handleBuyProduct(product.productId)} className="btn btn-success">Buy</button>
                    </>
                  )}
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

export default ProductList;
