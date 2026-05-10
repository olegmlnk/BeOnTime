import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { ProfilePage } from './ProfilePage';
import { AuthContext } from '../context/AuthContext';
import { ThemeContext } from '../context/ThemeContext';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо сервіс профілю
vi.mock('../services/profileService', () => ({
    profileService: {
        getProfile: vi.fn(),
        updateUsername: vi.fn(),
        updateEmail: vi.fn(),
        changePassword: vi.fn(),
        deleteAccount: vi.fn(),
    }
}));

import { profileService } from '../services/profileService';

// Мокаємо useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('ProfilePage Component', () => {
    const mockUser = { id: '1', name: 'Test User', email: 'test@test.com' };
    const mockLogout = vi.fn();

    const mockProfile = {
        id: '1',
        userName: 'TestUser',
        email: 'test@test.com',
        role: 0,
        createdAt: '2026-03-15T10:00:00',
    };

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderWithProviders = () => {
        return render(
            <BrowserRouter>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthContext.Provider value={{ user: mockUser, login: vi.fn(), logout: mockLogout, register: vi.fn(), isLoading: false }}>
                        <ProfilePage />
                    </AuthContext.Provider>
                </ThemeContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити заголовок сторінки', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Профіль')).toBeInTheDocument();
        });
        expect(screen.getByText('Керуйте вашим акаунтом та налаштуваннями.')).toBeInTheDocument();
    });

    it('повинен відображати інформацію користувача', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('TestUser')).toBeInTheDocument();
        });
        expect(screen.getByText('test@test.com')).toBeInTheDocument();
        expect(screen.getByText('User')).toBeInTheDocument();
    });

    it('повинен відображати ініціали в аватарі', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();

        await waitFor(() => {
            const avatar = document.querySelector('.profile-avatar span');
            expect(avatar.textContent).toBe('T');
        });
    });

    it('повинен відображати секції для редагування', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByRole('heading', { name: /Ім'я користувача/ })).toBeInTheDocument();
            expect(screen.getByRole('heading', { name: /Email/ })).toBeInTheDocument();
            expect(screen.getByRole('heading', { name: /Змінити пароль/ })).toBeInTheDocument();
            expect(screen.getByRole('heading', { name: /Небезпечна зона/ })).toBeInTheDocument();
        });
    });

    it('повинен заповнювати поле імені даними з профілю', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByDisplayValue('TestUser')).toBeInTheDocument();
        });
    });

    it('повинен оновлювати ім\'я користувача', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);
        profileService.updateUsername.mockResolvedValueOnce({});

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByDisplayValue('TestUser')).toBeInTheDocument();
        });

        const nameInput = screen.getByDisplayValue('TestUser');
        await user.clear(nameInput);
        await user.type(nameInput, 'NewName');

        // Кнопка "Зберегти" для імені
        const saveButtons = screen.getAllByText('Зберегти');
        await user.click(saveButtons[0]);

        expect(profileService.updateUsername).toHaveBeenCalledWith('NewName');
    });

    it('повинен мати disabled кнопку збереження імені, якщо значення не змінилося', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByDisplayValue('TestUser')).toBeInTheDocument();
        });

        const saveButtons = screen.getAllByText('Зберегти');
        expect(saveButtons[0]).toBeDisabled();
    });

    it('повинен рендерити форму зміни паролю', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByLabelText('Поточний пароль')).toBeInTheDocument();
        });
        expect(screen.getByLabelText('Новий пароль')).toBeInTheDocument();
        expect(screen.getByLabelText('Підтвердити пароль')).toBeInTheDocument();
    });

    it('повинен змінювати пароль при валідних даних', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);
        profileService.changePassword.mockResolvedValueOnce({});

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByLabelText('Поточний пароль')).toBeInTheDocument();
        });

        await user.type(screen.getByLabelText('Поточний пароль'), 'OldPass123');
        await user.type(screen.getByLabelText('Новий пароль'), 'NewPass456!');
        await user.type(screen.getByLabelText('Підтвердити пароль'), 'NewPass456!');
        await user.click(screen.getByRole('button', { name: 'Змінити пароль' }));

        expect(profileService.changePassword).toHaveBeenCalledWith('OldPass123', 'NewPass456!', 'NewPass456!');
    });

    it('повинен показувати помилку, якщо паролі не співпадають', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByLabelText('Поточний пароль')).toBeInTheDocument();
        });

        await user.type(screen.getByLabelText('Поточний пароль'), 'OldPass');
        await user.type(screen.getByLabelText('Новий пароль'), 'NewPass1');
        await user.type(screen.getByLabelText('Підтвердити пароль'), 'NewPass2');
        await user.click(screen.getByRole('button', { name: 'Змінити пароль' }));

        expect(screen.getByText('Паролі не співпадають')).toBeInTheDocument();
        expect(profileService.changePassword).not.toHaveBeenCalled();
    });

    it('повинен показувати кнопку видалення акаунту', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Видалити акаунт')).toBeInTheDocument();
        });
    });

    it('повинен показувати підтвердження при кліку на "Видалити акаунт"', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Видалити акаунт')).toBeInTheDocument();
        });

        await user.click(screen.getByText('Видалити акаунт'));

        expect(screen.getByText('Введіть пароль для підтвердження')).toBeInTheDocument();
        expect(screen.getByText('Підтвердити видалення')).toBeInTheDocument();
        expect(screen.getByText('Скасувати')).toBeInTheDocument();
    });

    it('повинен видаляти акаунт, викликати logout та навігувати', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);
        profileService.deleteAccount.mockResolvedValueOnce({});

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Видалити акаунт')).toBeInTheDocument();
        });

        await user.click(screen.getByText('Видалити акаунт'));
        await user.type(screen.getByPlaceholderText('Ваш пароль'), 'mypassword');
        await user.click(screen.getByText('Підтвердити видалення'));

        await waitFor(() => {
            expect(profileService.deleteAccount).toHaveBeenCalledWith('mypassword');
        });
        expect(mockLogout).toHaveBeenCalled();
        expect(mockNavigate).toHaveBeenCalledWith('/login');
    });

    it('повинен скасовувати видалення акаунту', async () => {
        profileService.getProfile.mockResolvedValueOnce(mockProfile);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Видалити акаунт')).toBeInTheDocument();
        });

        await user.click(screen.getByText('Видалити акаунт'));
        expect(screen.getByText('Підтвердити видалення')).toBeInTheDocument();

        await user.click(screen.getByText('Скасувати'));
        expect(screen.queryByText('Підтвердити видалення')).toBeFalsy();
    });

    it('повинен показувати мок-дані при помилці API', async () => {
        profileService.getProfile.mockRejectedValueOnce(new Error('API Error'));

        renderWithProviders();

        await waitFor(() => {
            // Мок профіль із AuthContext user
            expect(screen.getByText('Test User')).toBeInTheDocument();
        });
    });
});
