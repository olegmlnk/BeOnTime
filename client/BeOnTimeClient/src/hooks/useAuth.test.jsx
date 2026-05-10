import { renderHook } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { useAuth } from './useAuth';
import { AuthContext } from '../context/AuthContext';

describe('useAuth Hook', () => {
    it('повинен повертати контекст авторизації', () => {
        const mockValue = { user: { id: '1', name: 'Test' }, login: () => {}, logout: () => {}, isLoading: false };

        const { result } = renderHook(() => useAuth(), {
            wrapper: ({ children }) => (
                <AuthContext.Provider value={mockValue}>
                    {children}
                </AuthContext.Provider>
            ),
        });

        expect(result.current.user).toEqual(mockValue.user);
        expect(result.current.isLoading).toBe(false);
    });

    it('повинен кидати помилку поза AuthProvider', () => {
        expect(() => {
            renderHook(() => useAuth());
        }).toThrow('useAuth must be used within an AuthProvider');
    });
});
