import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiRegister } from '../api';

function Register() {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [fullname, setFullname] = useState("");
  const [role, setRole] = useState("buyer");
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const navigate = useNavigate();

  const handleOnSubmit = async (e) => {
    e.preventDefault();
    try {
      const data = await apiRegister(username, email, password, fullname, role);
      // Clear the form fields
      setUsername("");
      setEmail("");
      setPassword("");
      setFullname("");
      setRole("buyer");
      setError(null);
      setSuccess(data.message);
      // Navigate to the login page after a short delay
      setTimeout(() => navigate('/login'), 2000);
    } catch (error) {
      setError(error.message);
      setSuccess(null);
    }
  };

  return (
    <div className="container mt-5">
      <h1>Register</h1>
      <form onSubmit={handleOnSubmit}>
        <div className="form-group mb-3">
          <label htmlFor="username">Username</label>
          <input
            type="text"
            className="form-control"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div className="form-group mb-3">
          <label htmlFor="email">Email</label>
          <input
            type="email"
            className="form-control"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div className="form-group mb-3">
          <label htmlFor="password">Password</label>
          <input
            type="password"
            className="form-control"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <div className="form-group mb-3">
          <label htmlFor="fullname">Full Name</label>
          <input
            type="text"
            className="form-control"
            id="fullname"
            value={fullname}
            onChange={(e) => setFullname(e.target.value)}
            required
          />
        </div>
        <div className="form-group mb-3">
          <label htmlFor="role">Role</label>
          <select
            className="form-control"
            id="role"
            value={role}
            onChange={(e) => setRole(e.target.value)}
            required
          >
            <option value="buyer">Buyer</option>
            <option value="seller">Seller</option>
          </select>
        </div>
        {error && <div className="alert alert-danger">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}
        <button type="submit" className="btn btn-primary">
          Register
        </button>
      </form>
    </div>
  );
}

export default Register;