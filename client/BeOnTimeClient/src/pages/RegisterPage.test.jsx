import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { RegisterPage } from './RegisterPage';
import { AuthContext } from '../context/AuthContext';
import { ThemeContext } from '../context/ThemeContext';
import { BrowserRouter } from 'react-router-dom';

// Мок роутера
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('RegisterPage Component', () => {
    let mockRegister;

    beforeEach(() => {
        mockRegister = vi.fn();
        mockNavigate.mockClear();
    });

    const renderWithProviders = () => {
        return render(
            <BrowserRouter>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthContext.Provider value={{ register: mockRegister }}>
                        <RegisterPage />
                    </AuthContext.Provider>
                </ThemeContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен показувати помилку, якщо паролі не збігаються', async () => {
        renderWithProviders();
        const user = userEvent.setup();

        await user.type(screen.getByLabelText('Ім\'я'), 'Іван');
        await user.type(screen.getByLabelText('Прізвище'), 'Рябець');
        await user.type(screen.getByLabelText('Електронна пошта'), 'test@test.com');

        // Вводимо різні паролі
        await user.type(screen.getByLabelText('Пароль'), 'Password123!');
        await user.type(screen.getByLabelText('Підтвердіть пароль'), 'Password123?');

        // Відмічаємо чекбокс 
        await user.click(screen.getByRole('checkbox'));

        await user.click(screen.getByRole('button', { name: /Створити акаунт/i }));

        // Перевіряємо помилку з RegisterPage.jsx
        expect(screen.getByText('Паролі не збігаються. Спробуйте ще раз.')).toBeInTheDocument();
        expect(mockRegister).not.toHaveBeenCalled();
    });

    it('повинен показувати помилку, якщо не відмічено чекбокс умов', async () => {
        renderWithProviders();
        const user = userEvent.setup();

        await user.type(screen.getByLabelText('Ім\'я'), 'Іван');
        await user.type(screen.getByLabelText('Прізвище'), 'Рябець');
        await user.type(screen.getByLabelText('Електронна пошта'), 'test@test.com');
        await user.type(screen.getByLabelText('Пароль'), 'Password123!');
        await user.type(screen.getByLabelText('Підтвердіть пароль'), 'Password123!');

        // СПЕЦІАЛЬНО не клікаємо чекбокс   
        await user.click(screen.getByRole('button', { name: /Створити акаунт/i }));

        expect(screen.getByText('Підтвердьте згоду з умовами використання.')).toBeInTheDocument();
    });

    it('повинен правильно оцінювати міцність пароля', async () => {
        renderWithProviders();
        const user = userEvent.setup();
        const passInput = screen.getByLabelText('Пароль');

        // За замовчуванням
        expect(screen.getByText('Введіть пароль')).toBeInTheDocument();

        // 1. Короткий пароль
        await user.type(passInput, '12345');
        expect(screen.getByText('Слабкий')).toBeInTheDocument();

        // 2. Довгий пароль + велика літера (2 бали)
        await user.clear(passInput);
        await user.type(passInput, 'Abcdefghij');
        expect(screen.getByText('Нормальний')).toBeInTheDocument();

        // 3. Літери (з великою) + цифри    
        await user.clear(passInput);
        await user.type(passInput, 'Password123');
        expect(screen.getByText('Хороший')).toBeInTheDocument();

        // 4. Літери + цифри + спецсимволи
        await user.clear(passInput);
        await user.type(passInput, 'StrongP@ss123!');
        expect(screen.getByText('Надійний')).toBeInTheDocument();
    });

    it('повинен успішно реєструвати користувача при валідних даних', async () => {
        renderWithProviders();
        const user = userEvent.setup();

        await user.type(screen.getByLabelText('Ім\'я'), 'Іван');
        await user.type(screen.getByLabelText('Прізвище'), 'Рябець');
        await user.type(screen.getByLabelText('Електронна пошта'), 'test@test.com');
        await user.type(screen.getByLabelText('Пароль'), 'StrongP@ss123!');
        await user.type(screen.getByLabelText('Підтвердіть пароль'), 'StrongP@ss123!');

        await user.click(screen.getByRole('checkbox'));

        await user.click(screen.getByRole('button', { name: /Створити акаунт/i }));

        // Метод register приймає: (userName, email, password)
        // У RegisterPage.jsx він склеює Ім'я та Прізвище: `${firstName} ${lastName}`.trim()
        expect(mockRegister).toHaveBeenCalledWith('Іван Рябець', 'test@test.com', 'StrongP@ss123!');
    });
});