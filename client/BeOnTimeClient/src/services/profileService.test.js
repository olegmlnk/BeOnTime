import { describe, it, expect, vi, beforeEach } from 'vitest';
import { profileService } from './profileService';

vi.mock('./apiClient', () => ({
    default: {
        get: vi.fn(),
        put: vi.fn(),
        delete: vi.fn(),
    }
}));

import apiClient from './apiClient';

describe('profileService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('getProfile повинен отримувати профіль', async () => {
        apiClient.get.mockResolvedValueOnce({ data: { id: '1', userName: 'Test' } });

        const result = await profileService.getProfile();

        expect(apiClient.get).toHaveBeenCalledWith('/profile');
        expect(result.userName).toBe('Test');
    });

    it('updateUsername повинен оновлювати ім\'я', async () => {
        apiClient.put.mockResolvedValueOnce({ data: {} });

        await profileService.updateUsername('NewName');

        expect(apiClient.put).toHaveBeenCalledWith('/profile/username', { userName: 'NewName' });
    });

    it('updateEmail повинен оновлювати email з паролем', async () => {
        apiClient.put.mockResolvedValueOnce({ data: {} });

        await profileService.updateEmail('new@test.com', 'password');

        expect(apiClient.put).toHaveBeenCalledWith('/profile/email', { email: 'new@test.com', currentPassword: 'password' });
    });

    it('changePassword повинен змінювати пароль', async () => {
        apiClient.put.mockResolvedValueOnce({ data: {} });

        await profileService.changePassword('oldPass', 'newPass', 'newPass');

        expect(apiClient.put).toHaveBeenCalledWith('/profile/password', {
            currentPassword: 'oldPass',
            newPassword: 'newPass',
            confirmNewPassword: 'newPass',
        });
    });

    it('deleteAccount повинен видаляти акаунт з паролем', async () => {
        apiClient.delete.mockResolvedValueOnce({ data: {} });

        await profileService.deleteAccount('mypassword');

        expect(apiClient.delete).toHaveBeenCalledWith('/profile', { data: { currentPassword: 'mypassword' } });
    });
});
