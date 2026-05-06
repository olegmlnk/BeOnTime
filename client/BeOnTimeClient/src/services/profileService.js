import axios from 'axios';
import { getTokens } from '../utils/tokenStorage';

const api = axios.create({
  baseURL: '/api/profile',
});

api.interceptors.request.use((config) => {
  const { accessToken } = getTokens();
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

export const profileService = {
  getProfile: async () => {
    const response = await api.get('/');
    return response.data;
  },

  updateUsername: async (userName) => {
    const response = await api.put('/username', { userName });
    return response.data;
  },

  updateEmail: async (email, currentPassword) => {
    const response = await api.put('/email', { email, currentPassword });
    return response.data;
  },

  changePassword: async (currentPassword, newPassword, confirmNewPassword) => {
    const response = await api.put('/password', { currentPassword, newPassword, confirmNewPassword });
    return response.data;
  },

  deleteAccount: async (currentPassword) => {
    const response = await api.delete('/', { data: { currentPassword } });
    return response.data;
  },
};
