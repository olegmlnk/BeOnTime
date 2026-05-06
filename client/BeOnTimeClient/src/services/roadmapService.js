import axios from 'axios';
import { getTokens } from '../utils/tokenStorage';

const api = axios.create({
  baseURL: '/api/roadmaps',
});

api.interceptors.request.use((config) => {
  const { accessToken } = getTokens();
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

export const roadmapService = {
  getAll: async () => {
    const response = await api.get('/');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/${id}`);
    return response.data;
  },

  reorder: async (id, taskIds) => {
    const response = await api.put(`/${id}/reorder`, { taskIds });
    return response.data;
  },
};
