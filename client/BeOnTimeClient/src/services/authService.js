import axios from 'axios';
import { getTokens, setTokens, removeTokens } from '../utils/tokenStorage';

const api = axios.create({
  baseURL: '/api'
});

// Додаємо токен до кожного запиту
api.interceptors.request.use((config) => {
  const { accessToken } = getTokens();
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

// Обробка простроченого токена (refresh logic)
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    // Якщо отримали 401 Unauthorized і ще не пробували оновити токен
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const { refreshToken } = getTokens();
      
      if (refreshToken) {
        try {
          const response = await axios.post('/api/auth/refresh', {
            refreshToken
          });
          
          const { accessToken: newAccess, refreshToken: newRefresh } = response.data;
          setTokens(newAccess, newRefresh);
          
          // Повторюємо оригінальний запит з новим токеном
          originalRequest.headers.Authorization = `Bearer ${newAccess}`;
          return api(originalRequest);
        } catch (refreshError) {
          // Якщо refresh також не вдався - розлогінюємо юзера
          removeTokens();
          window.location.href = '/login';
          return Promise.reject(refreshError);
        }
      } else {
        removeTokens();
        window.location.href = '/login';
      }
    }
    
    return Promise.reject(error);
  }
);

export const authService = {
  login: async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    const { accessToken, refreshToken } = response.data;
    setTokens(accessToken, refreshToken);
    return response.data;
  },
  
  register: async (userName, email, password) => {
    const response = await api.post('/auth/register', { userName, email, password });
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

export default api;
