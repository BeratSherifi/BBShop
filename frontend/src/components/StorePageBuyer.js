import React from 'react';
import { useParams } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import ProductList from './ProductList';

function StorePageBuyer() {
  const { storeId } = useParams();

  return (
    <div className="container mt-5">
      <h1 className="mb-4">Store Products</h1>
      <ProductList storeId={storeId} role="buyer" />
    </div>
  );
}

export default StorePageBuyer;
