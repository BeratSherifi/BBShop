import React, { useState } from 'react';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import { useNavigate } from 'react-router-dom';
import { apiLogin } from '../api';

function Login({ onLoginSuccess }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleOnSubmit = async (e) => {
    e.preventDefault();
    try {
      const data = await apiLogin(email, password);
      onLoginSuccess(data.token);
      setEmail("");
      setPassword("");
      setError(null);
      navigate('/store');
    } catch (error) {
      setError("Login failed. Please check your credentials and try again.");
    }
  };

  return (
    <div>
      <form onSubmit={handleOnSubmit}>
        <TextField
          variant="outlined"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          label="Email"
          fullWidth
          margin="normal"
        />
        <TextField
          variant="outlined"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          label="Password"
          fullWidth
          margin="normal"
        />
        {error && <div style={{ color: 'red' }}>{error}</div>}
        <Button type="submit" variant="contained" color="primary">
          Login
        </Button>
      </form>
    </div>
  );
}

export default Login;
