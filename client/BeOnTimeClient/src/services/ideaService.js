import axios from 'axios';
import { getTokens } from '../utils/tokenStorage';

const api = axios.create({
  baseURL: '/api/ideas', // Uses Vite proxy
});

api.interceptors.request.use((config) => {
  const { accessToken } = getTokens();
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

export const ideaService = {
  // Get all ideas
  getAll: async () => {
    const response = await api.get('/');
    return response.data;
  },

  // Get idea by ID
  getById: async (id) => {
    const response = await api.get(`/${id}`);
    return response.data;
  },

  // Create new idea
  create: async (ideaData) => {
    const response = await api.post('/', ideaData);
    return response.data;
  },

  // Update existing idea
  update: async (id, ideaData) => {
    const response = await api.put(`/${id}`, ideaData);
    return response.data;
  },

  // Delete idea
  delete: async (id) => {
    const response = await api.delete(`/${id}`);
    return response.data;
  },

  // Convert idea to task
  convertToTask: async (id) => {
    const response = await api.post(`/${id}/convert`);
    return response.data;
  }
};
