export async function apiLogin(email, password) {
  const response = await fetch('http://localhost:5197/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });
  if (!response.ok) {
    throw new Error('Login failed');
  }
  return response.json();
}

export async function apiRegister(username, email, password, fullname, role) {
  const response = await fetch('http://localhost:5197/api/user', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ username, email, password, fullname, role }),
  });

  if (response.status === 201 || response.status === 204) {
    // Return a success message or an empty object for successful registration
    return { message: 'Registration successful' };
  } else {
    // Handle error response
    const errorText = await response.text();
    let errorMessage = 'Registration failed';
    try {
      const errorJson = JSON.parse(errorText);
      if (errorJson && errorJson.message) {
        errorMessage = errorJson.message;
      }
    } catch (e) {
      errorMessage = errorText || errorMessage;
    }
    throw new Error(errorMessage);
  }
}


export async function apiGetUserProfile(userId, token) {
  const response = await fetch(`http://localhost:5197/api/user/${userId}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  if (!response.ok) {
    throw new Error('Failed to fetch profile');
  }
  return response.json();
}

export async function apiGetStores(token) {
  const response = await fetch('http://localhost:5197/api/store', {
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });
  if (!response.ok) {
    throw new Error('Failed to fetch stores');
  }
  return response.json();
}

export async function apiCreateStore(storeName, token) {
  const formData = new FormData();
  formData.append('storeName', storeName);

  const response = await fetch('http://localhost:5197/api/store', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
    },
    body: formData,
  });

  if (!response.ok) {
    throw new Error('Failed to create store');
  }
  return response.json();
}

export async function apiUpdateProduct(productId, formData, token) {
  const response = await fetch(`http://localhost:5197/api/product/${productId}`, {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${token}`,
    },
    body: formData,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to update product: ${errorText}`);
  }

  return response;
}




export async function apiDeleteProduct(productId, token) {
  const response = await fetch(`http://localhost:5197/api/product/${productId}`, {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to delete product: ${errorText}`);
  }

  // Expect no content status for successful delete
  if (response.status === 204) {
    return true;
  }

  return response.json();
}


export async function apiGetProductById(productId, token) {
  const response = await fetch(`http://localhost:5197/api/product/${productId}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  if (!response.ok) {
    throw new Error('Failed to fetch product');
  }
  return response.json();
}

export async function apiCreateOrder(orderData, token) {
  const response = await fetch('http://localhost:5197/api/order', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(orderData),
  });

  if (!response.ok) {
    throw new Error('Failed to create order');
  }
  return response.json();
}

export async function apiGetOrdersByStoreId(storeId, token) {
  const response = await fetch(`http://localhost:5197/api/order/by-store/${storeId}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error('Failed to fetch orders');
  }
  return response.json();
}

export async function apiGetOrdersByUserId(userId, token) {
  const response = await fetch(`http://localhost:5197/api/Order/by-user/${userId}`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to fetch orders: ${errorText}`);
  }

  return response.json();
}


export async function apiUpdateOrderStatus(orderId, status, token) {
  const response = await fetch(`http://localhost:5197/api/order/${orderId}/status`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(status),
  });

  if (!response.ok) {
    throw new Error('Failed to update order status');
  }
}



export async function apiGetOrdersByBuyer(token) {
  const response = await fetch(`http://localhost:5197/api/order/buyer`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  if (!response.ok) {
    throw new Error('Failed to fetch orders');
  }
  return response.json();
}