import React, { useState } from 'react';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import { useNavigate } from 'react-router-dom';
import { apiRegister } from '../api';

function Register() {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("buyer");
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleOnSubmit = async (e) => {
    e.preventDefault();
    try {
      await apiRegister(username, email, password, role);
      setUsername("");
      setEmail("");
      setPassword("");
      setRole("buyer");
      setError(null);
      navigate('/login');
    } catch (error) {
      setError("Registration failed. Please try again.");
    }
  };

  return (
    <div>
      <form onSubmit={handleOnSubmit}>
        <TextField
          variant="outlined"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          label="Username"
          fullWidth
          margin="normal"
        />
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
        <TextField
          variant="outlined"
          value={role}
          onChange={(e) => setRole(e.target.value)}
          label="Role"
          fullWidth
          margin="normal"
        />
        {error && <div style={{ color: 'red' }}>{error}</div>}
        <Button type="submit" variant="contained" color="primary">
          Register
        </Button>
      </form>
    </div>
  );
}

export default Register;
