import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { IdeasPage } from './IdeasPage';
import { AuthContext } from '../context/AuthContext';
import { ThemeContext } from '../context/ThemeContext';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо сервіс ідей
vi.mock('../services/ideaService', () => ({
    ideaService: {
        getAll: vi.fn(),
        create: vi.fn(),
        delete: vi.fn(),
        convertToTask: vi.fn(),
    }
}));

import { ideaService } from '../services/ideaService';

describe('IdeasPage Component', () => {
    const mockUser = { id: '1', name: 'Test User', email: 'test@test.com' };

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderWithProviders = () => {
        return render(
            <BrowserRouter>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthContext.Provider value={{ user: mockUser, login: vi.fn(), logout: vi.fn(), register: vi.fn(), isLoading: false }}>
                        <IdeasPage />
                    </AuthContext.Provider>
                </ThemeContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити заголовок сторінки', async () => {
        ideaService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Твої Ідеї')).toBeInTheDocument();
        });
        expect(screen.getByText(/Записуй думки/i)).toBeInTheDocument();
    });

    it('повинен відображати список ідей після завантаження', async () => {
        ideaService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'Refactor AuthContext', description: 'Clean up error handling' },
            { id: '2', title: 'Marketing campaign', description: 'Target students' },
        ]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Refactor AuthContext')).toBeInTheDocument();
        });
        expect(screen.getByText('Marketing campaign')).toBeInTheDocument();
    });

    it('повинен показувати порожній стан, якщо немає ідей', async () => {
        ideaService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText(/Немає ідей/i)).toBeInTheDocument();
        });
    });

    it('повинен розгортати поле вводу при фокусі', async () => {
        ideaService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByPlaceholderText('Нотатка...')).toBeInTheDocument();
        });

        await user.click(screen.getByPlaceholderText('Нотатка...'));

        // Після фокусу з'являється textarea
        expect(screen.getByPlaceholderText(/Додайте деталі/i)).toBeInTheDocument();
        expect(screen.getByText('Зберегти')).toBeInTheDocument();
    });

    it('повинен створювати нову ідею при кліку на Зберегти', async () => {
        ideaService.getAll.mockResolvedValueOnce([]);
        ideaService.create.mockResolvedValueOnce({ id: '99', title: 'New Idea', description: 'Details' });

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByPlaceholderText('Нотатка...')).toBeInTheDocument();
        });

        // Розгортаємо поле вводу
        await user.click(screen.getByPlaceholderText('Нотатка...'));
        await user.type(screen.getByPlaceholderText('Нотатка...'), 'New Idea');
        await user.type(screen.getByPlaceholderText(/Додайте деталі/i), 'Details');
        await user.click(screen.getByText('Зберегти'));

        expect(ideaService.create).toHaveBeenCalledWith({ title: 'New Idea', content: 'Details' });
    });

    it('повинен не зберігати ідею з порожнім заголовком', async () => {
        ideaService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByPlaceholderText('Нотатка...')).toBeInTheDocument();
        });

        await user.click(screen.getByPlaceholderText('Нотатка...'));

        // Кнопка "Зберегти" повинна бути disabled
        const saveBtn = screen.getByText('Зберегти');
        expect(saveBtn).toBeDisabled();
    });

    it('повинен видаляти ідею після підтвердження', async () => {
        ideaService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'Idea to delete', description: '' },
        ]);
        ideaService.delete.mockResolvedValueOnce({});

        // Мокаємо window.confirm
        vi.spyOn(window, 'confirm').mockReturnValueOnce(true);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Idea to delete')).toBeInTheDocument();
        });

        // Кнопка видалення
        const deleteBtn = screen.getByTitle('Видалити');
        await user.click(deleteBtn);

        expect(ideaService.delete).toHaveBeenCalledWith('1');
    });

    it('повинен конвертувати ідею в завдання', async () => {
        ideaService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'Convertible idea', description: 'desc' },
        ]);
        ideaService.convertToTask.mockResolvedValueOnce({});

        // Мокаємо window.alert
        vi.spyOn(window, 'alert').mockImplementation(() => {});

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Convertible idea')).toBeInTheDocument();
        });

        const convertBtn = screen.getByTitle('Перетворити на завдання');
        await user.click(convertBtn);

        expect(ideaService.convertToTask).toHaveBeenCalledWith('1');
    });

    it('повинен показувати мок-дані при помилці API', async () => {
        ideaService.getAll.mockRejectedValueOnce(new Error('API Error'));

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('New landing page concept')).toBeInTheDocument();
        });
    });
});
