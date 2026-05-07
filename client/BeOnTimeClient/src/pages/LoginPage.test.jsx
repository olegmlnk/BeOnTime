import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { LoginPage } from './LoginPage';
import { AuthContext } from '../context/AuthContext';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо хук useNavigate з react-router-dom
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('LoginPage Component', () => {
    let mockLogin;

    beforeEach(() => {
        mockLogin = vi.fn();
        mockNavigate.mockClear();
    });

    const renderWithProviders = () => {
        return render(
            <BrowserRouter>
                <AuthContext.Provider value={{ login: mockLogin }}>
                    <LoginPage />
                </AuthContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити форму входу (поля email, password та кнопку)', () => {
        renderWithProviders();
        // Використовуємо точні рядки замість регулярних виразів
        expect(screen.getByLabelText('Електронна пошта')).toBeInTheDocument();
        expect(screen.getByLabelText('Пароль')).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /Увійти/i })).toBeInTheDocument();
    });

    it('повинен показувати помилку, якщо відправити порожню форму', async () => {
        renderWithProviders();
        const user = userEvent.setup();

        await user.click(screen.getByRole('button', { name: /Увійти/i }));

        expect(screen.getByText('Будь ласка, заповніть усі поля.')).toBeInTheDocument();
        expect(mockLogin).not.toHaveBeenCalled();
    });

    it('повинен викликати метод login при введенні правильних даних', async () => {
        renderWithProviders();
        const user = userEvent.setup();

        await user.type(screen.getByLabelText('Електронна пошта'), 'test@example.com');
        await user.type(screen.getByLabelText('Пароль'), 'password123');
        await user.click(screen.getByRole('button', { name: /Увійти/i }));

        expect(mockLogin).toHaveBeenCalledWith('test@example.com', 'password123');
    });

    it('повинен показувати помилку сервера, якщо логін не вдався', async () => {
        mockLogin.mockRejectedValueOnce({
            response: { data: { error: 'Невірний пароль' } }
        });

        renderWithProviders();
        const user = userEvent.setup();

        await user.type(screen.getByLabelText('Електронна пошта'), 'test@example.com');
        await user.type(screen.getByLabelText('Пароль'), 'wrongpass');
        await user.click(screen.getByRole('button', { name: /Увійти/i }));

        await waitFor(() => {
            expect(screen.getByText('Невірний пароль')).toBeInTheDocument();
        });
    });
});