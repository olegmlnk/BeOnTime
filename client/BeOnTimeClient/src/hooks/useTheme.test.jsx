import { renderHook } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { useTheme } from './useTheme';
import { ThemeContext } from '../context/ThemeContext';

describe('useTheme Hook', () => {
    it('повинен повертати контекст теми', () => {
        const mockValue = { theme: 'dark', toggleTheme: () => {} };

        const { result } = renderHook(() => useTheme(), {
            wrapper: ({ children }) => (
                <ThemeContext.Provider value={mockValue}>
                    {children}
                </ThemeContext.Provider>
            ),
        });

        expect(result.current.theme).toBe('dark');
        expect(typeof result.current.toggleTheme).toBe('function');
    });

    it('повинен кидати помилку поза ThemeProvider', () => {
        expect(() => {
            renderHook(() => useTheme());
        }).toThrow('useTheme must be used within a ThemeProvider');
    });
});
