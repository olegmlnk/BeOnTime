import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { Sidebar } from './Sidebar';
import { AuthContext } from '../context/AuthContext';
import { ThemeContext } from '../context/ThemeContext';
import { BrowserRouter, MemoryRouter } from 'react-router-dom';

describe('Sidebar Component', () => {
    const mockLogout = vi.fn();

    const renderWithProviders = (initialRoute = '/') => {
        return render(
            <MemoryRouter initialEntries={[initialRoute]}>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthContext.Provider value={{ user: { id: '1', name: 'Test' }, logout: mockLogout, isLoading: false }}>
                        <Sidebar />
                    </AuthContext.Provider>
                </ThemeContext.Provider>
            </MemoryRouter>
        );
    };

    it('повинен рендерити логотип BeOnTime', () => {
        renderWithProviders();
        expect(screen.getByText('BeOnTime')).toBeInTheDocument();
    });

    it('повинен рендерити всі навігаційні пункти', () => {
        renderWithProviders();
        expect(screen.getByText('Dashboard')).toBeInTheDocument();
        expect(screen.getByText('Завдання')).toBeInTheDocument();
        expect(screen.getByText('Ідеї')).toBeInTheDocument();
        expect(screen.getByText('Роадмапи')).toBeInTheDocument();
        expect(screen.getByText('Профіль')).toBeInTheDocument();
    });

    it('повинен рендерити кнопку виходу', () => {
        renderWithProviders();
        expect(screen.getByText('Вихід')).toBeInTheDocument();
    });

    it('повинен викликати logout при кліку на кнопку виходу', async () => {
        renderWithProviders();
        const user = userEvent.setup();

        await user.click(screen.getByText('Вихід'));
        expect(mockLogout).toHaveBeenCalled();
    });

    it('повинен підсвічувати активний пункт навігації для Dashboard', () => {
        renderWithProviders('/');
        const dashboardLink = screen.getByText('Dashboard').closest('.nav-item');
        expect(dashboardLink.classList.contains('active')).toBe(true);
    });

    it('повинен підсвічувати активний пункт навігації для Tasks', () => {
        renderWithProviders('/tasks');
        const tasksLink = screen.getByText('Завдання').closest('.nav-item');
        expect(tasksLink.classList.contains('active')).toBe(true);
    });

    it('повинен мати правильні посилання для навігаційних пунктів', () => {
        renderWithProviders();
        expect(screen.getByText('Dashboard').closest('a')).toHaveAttribute('href', '/');
        expect(screen.getByText('Завдання').closest('a')).toHaveAttribute('href', '/tasks');
        expect(screen.getByText('Ідеї').closest('a')).toHaveAttribute('href', '/ideas');
        expect(screen.getByText('Роадмапи').closest('a')).toHaveAttribute('href', '/roadmaps');
        expect(screen.getByText('Профіль').closest('a')).toHaveAttribute('href', '/profile');
    });
});
