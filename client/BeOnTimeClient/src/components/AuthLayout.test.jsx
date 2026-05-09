import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { AuthLayout } from './AuthLayout';
import { ThemeContext } from '../context/ThemeContext';
import { MemoryRouter } from 'react-router-dom';

// Мокаємо useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('AuthLayout Component', () => {
    beforeEach(() => {
        mockNavigate.mockClear();
    });

    const renderWithProviders = (route = '/login', children = <div>Form Content</div>) => {
        return render(
            <MemoryRouter initialEntries={[route]}>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthLayout>{children}</AuthLayout>
                </ThemeContext.Provider>
            </MemoryRouter>
        );
    };

    it('повинен рендерити бренд BeOnTime', () => {
        renderWithProviders();
        expect(screen.getByText('BeOnTime')).toBeInTheDocument();
    });

    it('повинен рендерити слоган', () => {
        renderWithProviders();
        expect(screen.getByText(/Плануй/)).toBeInTheDocument();
        expect(screen.getByText(/Живи більше/)).toBeInTheDocument();
    });

    it('повинен рендерити таби "Вхід" та "Реєстрація"', () => {
        renderWithProviders();
        expect(screen.getByText('Вхід')).toBeInTheDocument();
        expect(screen.getByText('Реєстрація')).toBeInTheDocument();
    });

    it('повинен мати активний таб "Вхід" на сторінці /login', () => {
        renderWithProviders('/login');
        const loginTab = screen.getByText('Вхід');
        expect(loginTab.classList.contains('active')).toBe(true);
    });

    it('повинен мати активний таб "Реєстрація" на сторінці /register', () => {
        renderWithProviders('/register');
        const regTab = screen.getByText('Реєстрація');
        expect(regTab.classList.contains('active')).toBe(true);
    });

    it('повинен рендерити дочірній контент', () => {
        renderWithProviders('/login', <div>My Form</div>);
        expect(screen.getByText('My Form')).toBeInTheDocument();
    });

    it('повинен рендерити ThemeToggle', () => {
        renderWithProviders();
        expect(screen.getByTitle('Темна тема')).toBeInTheDocument();
    });

    it('повинен рендерити features', () => {
        renderWithProviders();
        expect(screen.getByText('Розумний планер')).toBeInTheDocument();
        expect(screen.getByText('Таймер Помодоро')).toBeInTheDocument();
        expect(screen.getByText('Прогрес та аналітика')).toBeInTheDocument();
    });

    it('повинен навігувати до /login при кліку на таб "Вхід"', async () => {
        renderWithProviders('/register');
        const user = userEvent.setup();

        await user.click(screen.getByText('Вхід'));
        expect(mockNavigate).toHaveBeenCalledWith('/login');
    });

    it('повинен навігувати до /register при кліку на таб "Реєстрація"', async () => {
        renderWithProviders('/login');
        const user = userEvent.setup();

        await user.click(screen.getByText('Реєстрація'));
        expect(mockNavigate).toHaveBeenCalledWith('/register');
    });
});
