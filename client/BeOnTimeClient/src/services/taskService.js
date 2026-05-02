import axios from 'axios';
import { getTokens } from '../utils/tokenStorage'; // assuming this exists based on typical structure

// Create an axios instance with base URL and auth header
const api = axios.create({
  baseURL: '/api/tasks', // Uses Vite proxy
});

api.interceptors.request.use((config) => {
  const { accessToken } = getTokens();
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

export const taskService = {
  // Get all tasks with optional filters
  getAll: async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.status) params.append('status', filters.status);
    if (filters.priority) params.append('priority', filters.priority);
    // ... other filters

    const response = await api.get(`?${params.toString()}`);
    return response.data;
  },

  // Get task by ID
  getById: async (id) => {
    const response = await api.get(`/${id}`);
    return response.data;
  },

  // Get overdue tasks
  getOverdue: async () => {
    const response = await api.get('/overdue');
    return response.data;
  },

  // Get upcoming tasks
  getUpcoming: async (days = 7) => {
    const response = await api.get(`/upcoming?days=${days}`);
    return response.data;
  },

  // Create new task
  create: async (taskData) => {
    const response = await api.post('/', taskData);
    return response.data;
  },

  // Update existing task
  update: async (id, taskData) => {
    const response = await api.put(`/${id}`, taskData);
    return response.data;
  },

  // Update task status
  updateStatus: async (id, status) => {
    const response = await api.patch(`/${id}/status`, { status });
    return response.data;
  },

  // Delete task
  delete: async (id) => {
    const response = await api.delete(`/${id}`);
    return response.data;
  }
};
