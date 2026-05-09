import apiClient from './apiClient';

export const roadmapService = {
  getAll: async () => {
    const response = await apiClient.get('/roadmaps');
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`/roadmaps/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await apiClient.post('/roadmaps', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await apiClient.put(`/roadmaps/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await apiClient.delete(`/roadmaps/${id}`);
    return response.data;
  },

  reorder: async (id, taskIds) => {
    const response = await apiClient.put(`/roadmaps/${id}/reorder`, { taskIds });
    return response.data;
  },
};
