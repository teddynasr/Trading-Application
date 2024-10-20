import React, { createContext, useContext, useState } from 'react';
import { logout as apiLogout } from './api';

const AuthContext = createContext();

export const useAuth = () => {
    return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
    const [userID, setUserID] = useState(null);
    const [authToken, setAuthToken] = useState(null);

    const login = (newUserID, token) => {
        setUserID(newUserID);
        setAuthToken(token);
    };

    const logout = async () => {
        try {
            await apiLogout(userID);
            setUserID(null);
            setAuthToken(null);
        } catch (error) {
            console.error('Logout failed:', error);
        }
    };

    return (
        <AuthContext.Provider value={{ userID, authToken, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};
