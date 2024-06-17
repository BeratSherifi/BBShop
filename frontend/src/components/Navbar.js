import React from 'react';
import { Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import logo from '../logo.png';

function Navbar({ role }) {
  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
      <div className="container-fluid justify-content-center">
        <Link className="navbar-brand mx-auto" to="/profile">
          <img src={logo} alt="BB Shop Logo" style={{ width: '150px', height: 'auto' }} />
        </Link>
        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav ms-auto">
            <li className="nav-item">
              <Link className="nav-link" to="/profile">Profile</Link>
            </li>
            {role === 'seller' && (
              <>
                <li className="nav-item">
                  <Link className="nav-link" to="/store-seller">Store</Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/orders-seller">Orders</Link>
                </li>
              </>
            )}
            {role === 'buyer' && (
              <li className="nav-item">
                <Link className="nav-link" to="/orders-buyer">Orders</Link>
              </li>
            )}
            <li className="nav-item">
              <Link className="nav-link" to="/search">Search</Link>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
