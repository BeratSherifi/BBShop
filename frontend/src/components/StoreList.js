import React from 'react';

function StoreList({ stores }) {
  return (
    <ul>
      {stores.map(store => (
        <li key={store.storeId}>{store.storeName}</li>
      ))}
    </ul>
  );
}

export default StoreList;
