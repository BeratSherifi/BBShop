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
  
  export async function apiRegister(username, email, password, role) {
    const response = await fetch('http://localhost:5197/api/user', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username, email, password, role }),
    });
    if (!response.ok) {
      throw new Error('Registration failed');
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
  