import { setTokens, removeTokens } from '../utils/tokenStorage';
import apiClient from './apiClient';

export const authService = {
  login: async (email, password) => {
    const response = await apiClient.post('/auth/login', { email, password });
    const { accessToken, refreshToken } = response.data;
    setTokens(accessToken, refreshToken);
    return response.data;
  },
  
  register: async (userName, email, password) => {
    const response = await apiClient.post('/auth/register', { userName, email, password });
    const { accessToken, refreshToken } = response.data;
    if (accessToken) {
        setTokens(accessToken, refreshToken);
    }
    return response.data;
  },
  
  logout: () => {
    removeTokens();
  }
};

export default apiClient;
