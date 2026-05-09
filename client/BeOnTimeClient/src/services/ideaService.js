import apiClient from './apiClient';

export const ideaService = {
  // Get all ideas
  getAll: async () => {
    const response = await apiClient.get('/ideas');
    return response.data;
  },

  // Get idea by ID
  getById: async (id) => {
    const response = await apiClient.get(`/ideas/${id}`);
    return response.data;
  },

  // Create new idea
  create: async (ideaData) => {
    const response = await apiClient.post('/ideas', ideaData);
    return response.data;
  },

  // Update existing idea
  update: async (id, ideaData) => {
    const response = await apiClient.put(`/ideas/${id}`, ideaData);
    return response.data;
  },

  // Delete idea
  delete: async (id) => {
    const response = await apiClient.delete(`/ideas/${id}`);
    return response.data;
  },

  // Convert idea to task
  convertToTask: async (id) => {
    const response = await apiClient.post(`/ideas/${id}/convert`);
    return response.data;
  }
};
