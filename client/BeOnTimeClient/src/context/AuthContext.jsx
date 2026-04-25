import React, { createContext, useState, useEffect } from 'react';
import { authService } from '../services/authService';
import { getTokens } from '../utils/tokenStorage';
import { jwtDecode } from 'jwt-decode';

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Перевіряємо токен при завантаженні додатку
    const { accessToken } = getTokens();
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
        } else {
          authService.logout();
        }
      } catch (error) {
        console.error("Invalid token format", error);
        authService.logout();
      }
    }
    setIsLoading(false);
  }, []);

  const login = async (email, password) => {
    const data = await authService.login(email, password);
    const decoded = jwtDecode(data.accessToken);
    setUser({
      id: decoded.sub || decoded.nameid,
      email: decoded.email,
      name: decoded.name || decoded.unique_name
    });
    return data;
  };

  const register = async (userName, email, password) => {
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
