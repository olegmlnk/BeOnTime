import apiClient from './apiClient';

export const taskService = {
  // Get all tasks with optional filters
  getAll: async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.status) params.append('status', filters.status);
    if (filters.priority) params.append('priority', filters.priority);
    // ... other filters

    const response = await apiClient.get(`/tasks?${params.toString()}`);
    return response.data;
  },

  // Get task by ID
  getById: async (id) => {
    const response = await apiClient.get(`/tasks/${id}`);
    return response.data;
  },

  // Get overdue tasks
  getOverdue: async () => {
    const response = await apiClient.get('/tasks/overdue');
    return response.data;
  },

  // Get upcoming tasks
  getUpcoming: async (days = 7) => {
    const response = await apiClient.get(`/tasks/upcoming?days=${days}`);
    return response.data;
  },

  // Create new task
  create: async (taskData) => {
    const response = await apiClient.post('/tasks', taskData);
    return response.data;
  },

  // Update existing task
  update: async (id, taskData) => {
    const response = await apiClient.put(`/tasks/${id}`, taskData);
    return response.data;
  },

  // Update task status
  updateStatus: async (id, status) => {
    const response = await apiClient.patch(`/tasks/${id}/status`, { status });
    return response.data;
  },

  // Delete task
  delete: async (id) => {
    const response = await apiClient.delete(`/tasks/${id}`);
    return response.data;
  }
};
