import { describe, it, expect, vi, beforeEach } from 'vitest';
import { authService } from './authService';

// Мокаємо apiClient
vi.mock('./apiClient', () => ({
    default: {
        post: vi.fn(),
    }
}));

// Мокаємо tokenStorage
vi.mock('../utils/tokenStorage', () => ({
    setTokens: vi.fn(),
    removeTokens: vi.fn(),
}));

import apiClient from './apiClient';
import { setTokens, removeTokens } from '../utils/tokenStorage';

describe('authService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    describe('login', () => {
        it('повинен відправляти POST запит з email і password', async () => {
            apiClient.post.mockResolvedValueOnce({
                data: { accessToken: 'acc-token', refreshToken: 'ref-token' }
            });

            const result = await authService.login('test@test.com', 'password');

            expect(apiClient.post).toHaveBeenCalledWith('/auth/login', { email: 'test@test.com', password: 'password' });
            expect(setTokens).toHaveBeenCalledWith('acc-token', 'ref-token');
            expect(result.accessToken).toBe('acc-token');
        });

        it('повинен прокидувати помилку при невдалому логіні', async () => {
            apiClient.post.mockRejectedValueOnce(new Error('401'));

            await expect(authService.login('bad@test.com', 'wrong')).rejects.toThrow('401');
        });
    });

    describe('register', () => {
        it('повинен відправляти POST запит з даними реєстрації', async () => {
            apiClient.post.mockResolvedValueOnce({
                data: { accessToken: 'new-acc', refreshToken: 'new-ref' }
            });

            const result = await authService.register('Test User', 'test@test.com', 'pass123');

            expect(apiClient.post).toHaveBeenCalledWith('/auth/register', {
                userName: 'Test User', email: 'test@test.com', password: 'pass123'
            });
            expect(setTokens).toHaveBeenCalledWith('new-acc', 'new-ref');
            expect(result.accessToken).toBe('new-acc');
        });

        it('повинен не зберігати токен, якщо accessToken відсутній у відповіді', async () => {
            apiClient.post.mockResolvedValueOnce({
                data: { message: 'Registration pending' }
            });

            await authService.register('User', 'u@t.com', 'pass');
            expect(setTokens).not.toHaveBeenCalled();
        });
    });

    describe('logout', () => {
        it('повинен видаляти токени', () => {
            authService.logout();
            expect(removeTokens).toHaveBeenCalled();
        });
    });
});
