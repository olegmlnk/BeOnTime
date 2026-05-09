import { describe, it, expect, vi, beforeEach } from 'vitest';

// Мокаємо localStorage
const mockStorage = {};
const localStorageMock = {
    getItem: vi.fn((key) => mockStorage[key] || null),
    setItem: vi.fn((key, value) => { mockStorage[key] = value; }),
    removeItem: vi.fn((key) => { delete mockStorage[key]; }),
};

Object.defineProperty(globalThis, 'localStorage', {
    value: localStorageMock,
    writable: true,
});

import { getTokens, setTokens, removeTokens } from './tokenStorage';

describe('tokenStorage', () => {
    beforeEach(() => {
        vi.clearAllMocks();
        Object.keys(mockStorage).forEach(key => delete mockStorage[key]);
    });

    describe('getTokens', () => {
        it('повинен повертати null, якщо токенів немає', () => {
            const { accessToken, refreshToken } = getTokens();
            expect(accessToken).toBeNull();
            expect(refreshToken).toBeNull();
            expect(localStorageMock.getItem).toHaveBeenCalledWith('accessToken');
            expect(localStorageMock.getItem).toHaveBeenCalledWith('refreshToken');
        });

        it('повинен повертати збережені токени', () => {
            mockStorage['accessToken'] = 'test-access';
            mockStorage['refreshToken'] = 'test-refresh';

            const { accessToken, refreshToken } = getTokens();
            expect(accessToken).toBe('test-access');
            expect(refreshToken).toBe('test-refresh');
        });
    });

    describe('setTokens', () => {
        it('повинен зберігати обидва токени', () => {
            setTokens('access-123', 'refresh-456');
            expect(localStorageMock.setItem).toHaveBeenCalledWith('accessToken', 'access-123');
            expect(localStorageMock.setItem).toHaveBeenCalledWith('refreshToken', 'refresh-456');
        });

        it('повинен не зберігати refreshToken, якщо він falsy', () => {
            setTokens('access-only', null);
            expect(localStorageMock.setItem).toHaveBeenCalledWith('accessToken', 'access-only');
            expect(localStorageMock.setItem).toHaveBeenCalledTimes(1);
        });
    });

    describe('removeTokens', () => {
        it('повинен видаляти обидва токени', () => {
            removeTokens();
            expect(localStorageMock.removeItem).toHaveBeenCalledWith('accessToken');
            expect(localStorageMock.removeItem).toHaveBeenCalledWith('refreshToken');
        });
    });
});
