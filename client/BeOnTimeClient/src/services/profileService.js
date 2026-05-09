import apiClient from './apiClient';

export const profileService = {
  getProfile: async () => {
    const response = await apiClient.get('/profile');
    return response.data;
  },

  updateUsername: async (userName) => {
    const response = await apiClient.put('/profile/username', { userName });
    return response.data;
  },

  updateEmail: async (email, currentPassword) => {
    const response = await apiClient.put('/profile/email', { email, currentPassword });
    return response.data;
  },

  changePassword: async (currentPassword, newPassword, confirmNewPassword) => {
    const response = await apiClient.put('/profile/password', { currentPassword, newPassword, confirmNewPassword });
    return response.data;
  },

  deleteAccount: async (currentPassword) => {
    const response = await apiClient.delete('/profile', { data: { currentPassword } });
    return response.data;
  },
};
