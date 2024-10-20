import React, { useState } from 'react';
import { Box, Button, TextField, Typography, Link } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { signup } from '../Services/api';
import { useAuth } from '../Services/AuthProvider';

const Signup = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    passwordHash: '',
    firstName: '',
    lastName: '',
  });
  const [error, setError] = useState('');

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await signup(formData);
      const { id } = response.data;
      login(id);
      navigate('/');
    } catch (err) {
      setError('Signup failed. Please try again.');
    }
  };

  const handleLoginRedirect = () => {
    navigate('/login');
  };

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        height: '90vh',
      }}
    >
      <Typography variant="h4" gutterBottom>
        Sign Up
      </Typography>
      {error && <Typography color="error">{error}</Typography>}
      <Box
        component="form"
        onSubmit={handleSubmit}
        sx={{
          width: '300px',
          display: 'flex',
          flexDirection: 'column',
          gap: '20px',
        }}
        autoComplete="off"
      >
        <TextField 
          label="Username" 
          variant="outlined" 
          fullWidth 
          required 
          autoComplete="new-username" 
          name="username"
          onChange={handleInputChange}
        />
        <TextField 
          label="Email" 
          variant="outlined" 
          type="email" 
          fullWidth 
          required 
          autoComplete="new-email" 
          name="email"
          onChange={handleInputChange}
        />
        <TextField
          label="Password"
          variant="outlined"
          type="password"
          fullWidth
          required
          autoComplete="new-password"
          name="passwordHash"
          onChange={handleInputChange}
        />
        <TextField 
          label="First Name" 
          variant="outlined" 
          fullWidth 
          required 
          name="firstName"
          onChange={handleInputChange}
        />
        <TextField 
          label="Last Name" 
          variant="outlined" 
          fullWidth 
          required 
          name="lastName"
          onChange={handleInputChange}
        />
        <Button variant="contained" color="primary" fullWidth type="submit">
          Sign Up
        </Button>
      </Box>
      <Typography variant="body2" sx={{ marginTop: '10px' }}>
        Already have an account?{' '}
        <Link component="button" variant="body2" onClick={handleLoginRedirect}>
          Log in
        </Link>
      </Typography>
    </Box>
  );
};

export default Signup;
