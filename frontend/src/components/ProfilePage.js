
import React, { useEffect, useState } from 'react';
import { apiGetUserProfile } from '../api';

function ProfilePage() {
  const [profile, setProfile] = useState({});
  const [error, setError] = useState(null);
  const token = localStorage.getItem('authToken');
  const userId = sessionStorage.getItem('userId');

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const profileData = await apiGetUserProfile(userId, token);
        setProfile(profileData);
      } catch (error) {
        setError("Failed to fetch profile");
      }
    };

    fetchProfile();
  }, [userId, token]);

  return (
    <div className="container mt-5">
      <h1>Profile</h1>
      {error && <div className="alert alert-danger">{error}</div>}
      {profile && (
        <div className="card">
          <div className="card-body">
            <h5 className="card-title">User Information</h5>
            <p className="card-text"><strong>Username:</strong> {profile.username}</p>
            <p className="card-text"><strong>Email:</strong> {profile.email}</p>
            <p className="card-text"><strong>Full Name:</strong> {profile.fullName}</p>
            <p className="card-text"><strong>Role:</strong> {profile.role}</p>
          </div>
        </div>
      )}
    </div>
  );
}

export default ProfilePage;