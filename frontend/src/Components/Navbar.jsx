import React from 'react';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import { Link } from 'react-router-dom';
import { useAuth } from '../Services/AuthProvider';

const Navbar = () => {
    const { userID, logout } = useAuth();

    return (
        <AppBar position="static">
            <Toolbar>
                <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                    Trading App
                </Typography>
                <Button color="inherit" component={Link} to="/" sx={{ marginRight: 2 }}>
                    Dashboard
                </Button>
                <Button 
                    color="inherit" 
                    component={Link} 
                    to="/portfolio" 
                    sx={{ marginRight: 5 }} 
                    disabled={!userID}
                >
                    Portfolio
                </Button>
                {userID ? (
                    <Button color="inherit" onClick={logout} to="/" sx={{ marginRight: 5 }}>
                        Logout
                    </Button>
                ) : (
                    <Button color="inherit" component={Link} to="/login">
                        Login
                    </Button>
                )}
            </Toolbar>
        </AppBar>
    );
};

export default Navbar;
