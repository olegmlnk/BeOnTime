import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { ThemeToggle } from './ThemeToggle';
import { ThemeContext } from '../context/ThemeContext';

describe('ThemeToggle Component', () => {
    const renderWithTheme = (theme = 'light') => {
        const mockToggle = vi.fn();
        render(
            <ThemeContext.Provider value={{ theme, toggleTheme: mockToggle }}>
                <ThemeToggle />
            </ThemeContext.Provider>
        );
        return { mockToggle };
    };

    it('повинен рендерити кнопку перемикання теми', () => {
        renderWithTheme();
        const btn = screen.getByRole('button');
        expect(btn).toBeInTheDocument();
        expect(btn.classList.contains('theme-toggle')).toBe(true);
    });

    it('повинен показувати aria-label для темної теми у світлому режимі', () => {
        renderWithTheme('light');
        expect(screen.getByLabelText('Увімкнути темну тему')).toBeInTheDocument();
    });

    it('повинен показувати aria-label для світлої теми у темному режимі', () => {
        renderWithTheme('dark');
        expect(screen.getByLabelText('Увімкнути світлу тему')).toBeInTheDocument();
    });

    it('повинен показувати title "Темна тема" у світлому режимі', () => {
        renderWithTheme('light');
        expect(screen.getByTitle('Темна тема')).toBeInTheDocument();
    });

    it('повинен показувати title "Світла тема" у темному режимі', () => {
        renderWithTheme('dark');
        expect(screen.getByTitle('Світла тема')).toBeInTheDocument();
    });

    it('повинен викликати toggleTheme при кліку', async () => {
        const { mockToggle } = renderWithTheme('light');
        const user = userEvent.setup();

        await user.click(screen.getByRole('button'));
        expect(mockToggle).toHaveBeenCalledTimes(1);
    });

    it('повинен мати клас "dark" на thumb у темному режимі', () => {
        renderWithTheme('dark');
        const thumb = document.querySelector('.theme-toggle-thumb');
        expect(thumb.classList.contains('dark')).toBe(true);
    });

    it('повинен мати клас "light" на thumb у світлому режимі', () => {
        renderWithTheme('light');
        const thumb = document.querySelector('.theme-toggle-thumb');
        expect(thumb.classList.contains('light')).toBe(true);
    });

    it('повинен приймати додатковий className', () => {
        render(
            <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                <ThemeToggle className="custom-class" />
            </ThemeContext.Provider>
        );
        const btn = screen.getByRole('button');
        expect(btn.classList.contains('custom-class')).toBe(true);
    });
});
