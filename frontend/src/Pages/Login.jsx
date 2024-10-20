import React, { useState } from 'react';
import { Box, Button, TextField, Typography, Link } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../Services/AuthProvider';
import { login as apiLogin } from '../Services/api';

const Login = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSignupRedirect = () => {
    navigate('/signup');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try{
      const response = await apiLogin({ username, password });
      const { id } = response.data;
      login(id);
      navigate('/');
    }
    catch(err){
      setError('Incorrect Credentials.');
    }
};


  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        height: '80vh',
      }}
    >
      <Typography variant="h4" gutterBottom>
        Login
      </Typography>
      <Box
        component="form"
        sx={{
          width: '300px',
          display: 'flex',
          flexDirection: 'column',
          gap: '20px',
        }}
        autoComplete="off"
        onSubmit={handleSubmit}
      >
        <TextField
          label="Username"
          variant="outlined"
          fullWidth
          required
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          autoComplete="off"
        />
        <TextField
          label="Password"
          variant="outlined"
          type="password"
          fullWidth
          required
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          autoComplete="off"
        />
        {error && (
          <Typography variant="body2" color="error">
            {error}
          </Typography>
        )}
        <Button type="submit" variant="contained" color="primary" fullWidth>
          Login
        </Button>
      </Box>
      <Typography variant="body2" sx={{ marginTop: '10px' }}>
        Don't have an account?{' '}
        <Link component="button" variant="body2" onClick={handleSignupRedirect}>
          Sign up here
        </Link>
      </Typography>
    </Box>
  );
};

export default Login;
