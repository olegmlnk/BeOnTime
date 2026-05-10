import React, { createContext, useState, useEffect } from 'react';
import axios from 'axios';
import { authService } from '../services/authService';
import { getTokens, setTokens } from '../utils/tokenStorage';
import { jwtDecode } from 'jwt-decode';

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      const { accessToken, refreshToken } = getTokens();
      if (accessToken) {
        try {
          const decoded = jwtDecode(accessToken);
          // Перевіряємо чи токен не прострочений (exp в секундах, Date.now() в мілісекундах)
          if (decoded.exp * 1000 > Date.now()) {
            setUser({
              id: decoded.sub || decoded.nameid,
              email: decoded.email,
              name: decoded.name || decoded.unique_name
            });
          } else if (refreshToken) {
            // Access token expired but refresh token exists — try to refresh
            try {
              const response = await axios.post('/api/auth/refresh', { refreshToken });
              const { accessToken: newAccess, refreshToken: newRefresh } = response.data;
              setTokens(newAccess, newRefresh);
              const newDecoded = jwtDecode(newAccess);
              setUser({
                id: newDecoded.sub || newDecoded.nameid,
                email: newDecoded.email,
                name: newDecoded.name || newDecoded.unique_name
              });
            } catch (refreshError) {
              console.warn("Refresh token expired, logging out", refreshError);
              authService.logout();
            }
          } else {
            authService.logout();
          }
        } catch (error) {
          console.error("Invalid token format", error);
          authService.logout();
        }
      }
      setIsLoading(false);
    };

    initAuth();
  }, []);

  const login = async (email, password) => {
    try {
      const data = await authService.login(email, password);
      const decoded = jwtDecode(data.accessToken);
      setUser({
        id: decoded.sub || decoded.nameid,
        email: decoded.email,
        name: decoded.name || decoded.unique_name
      });
      return data;
    } catch (error) {
      if (import.meta.env.DEV) {
        console.warn("Backend unavailable or error occurred, using mock login.", error);
        const fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjMiLCJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJuYW1lIjoiVGVzdCBVc2VyIiwiZXhwIjo5OTk5OTk5OTk5fQ.mock_signature";
        setTokens(fakeToken, fakeToken);
        setUser({ id: '123', email, name: email.split('@')[0] });
        return { accessToken: fakeToken };
      }
      throw error;
    }
  };

  const register = async (userName, email, password) => {
    try {
      const data = await authService.register(userName, email, password);
      if (data.accessToken) {
          const decoded = jwtDecode(data.accessToken);
          setUser({
            id: decoded.sub || decoded.nameid,
            email: decoded.email,
            name: decoded.name || decoded.unique_name
          });
      }
      return data;
    } catch (error) {
      if (import.meta.env.DEV) {
        console.warn("Backend unavailable or error occurred, using mock register.", error);
        const fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjMiLCJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJuYW1lIjoiVGVzdCBVc2VyIiwiZXhwIjo5OTk5OTk5OTk5fQ.mock_signature";
        setTokens(fakeToken, fakeToken);
        setUser({ id: '123', email, name: userName });
        return { accessToken: fakeToken };
      }
      throw error;
    }
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
