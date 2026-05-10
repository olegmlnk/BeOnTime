import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { RoadmapsPage } from './RoadmapsPage';
import { AuthContext } from '../context/AuthContext';
import { ThemeContext } from '../context/ThemeContext';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо сервіс роадмапів
vi.mock('../services/roadmapService', () => ({
    roadmapService: {
        getAll: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        delete: vi.fn(),
    }
}));

import { roadmapService } from '../services/roadmapService';

// Мокаємо useNavigate для RoadmapCard
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('RoadmapsPage Component', () => {
    const mockUser = { id: '1', name: 'Test User', email: 'test@test.com' };

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderWithProviders = () => {
        return render(
            <BrowserRouter>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthContext.Provider value={{ user: mockUser, login: vi.fn(), logout: vi.fn(), register: vi.fn(), isLoading: false }}>
                        <RoadmapsPage />
                    </AuthContext.Provider>
                </ThemeContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити заголовок сторінки', async () => {
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByRole('heading', { level: 1, name: 'Роадмапи' })).toBeInTheDocument();
        });
    });

    it('повинен рендерити кнопку створення нового роадмапу', async () => {
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Новий роадмап')).toBeInTheDocument();
        });
    });

    it('повинен відображати список роадмапів після завантаження', async () => {
        roadmapService.getAll.mockResolvedValueOnce([
            { id: '1', name: 'Вивчити React', progressPercent: 60, totalTasks: 5, doneTasks: 3 },
            { id: '2', name: 'Фітнес план', progressPercent: 25, totalTasks: 8, doneTasks: 2 },
        ]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Вивчити React')).toBeInTheDocument();
        });
        expect(screen.getByText('Фітнес план')).toBeInTheDocument();
    });

    it('повинен показувати порожній стан, якщо немає роадмапів', async () => {
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText(/Немає роадмапів/i)).toBeInTheDocument();
        });
    });

    it('повинен відкривати модалку при кліку "Новий роадмап"', async () => {
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Новий роадмап')).toBeInTheDocument();
        });

        await user.click(screen.getByText('Новий роадмап'));

        expect(screen.getByText('Назва роадмапу')).toBeInTheDocument();
        expect(screen.getByText('Створити')).toBeInTheDocument();
    });

    it('повинен створювати новий роадмап через модалку', async () => {
        roadmapService.getAll.mockResolvedValueOnce([]);
        roadmapService.create.mockResolvedValueOnce({
            id: 'new-1', name: 'Новий план', progressPercent: 0, totalTasks: 0, doneTasks: 0,
        });

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Новий роадмап')).toBeInTheDocument();
        });

        await user.click(screen.getByText('Новий роадмап'));
        await user.type(screen.getByLabelText('Назва роадмапу'), 'Новий план');
        await user.click(screen.getByText('Створити'));

        await waitFor(() => {
            expect(roadmapService.create).toHaveBeenCalled();
        });
    });

    it('повинен видаляти роадмап після підтвердження', async () => {
        roadmapService.getAll.mockResolvedValueOnce([
            { id: '1', name: 'Роадмап для видалення', progressPercent: 0, totalTasks: 0, doneTasks: 0 },
        ]);
        roadmapService.delete.mockResolvedValueOnce({});

        vi.spyOn(window, 'confirm').mockReturnValueOnce(true);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Роадмап для видалення')).toBeInTheDocument();
        });

        await user.click(screen.getByTitle('Видалити'));

        expect(roadmapService.delete).toHaveBeenCalledWith('1');
    });

    it('повинен показувати мок-дані при помилці API', async () => {
        roadmapService.getAll.mockRejectedValueOnce(new Error('API Error'));

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Вивчити React')).toBeInTheDocument();
        });
        expect(screen.getByText('Підготовка до іспитів')).toBeInTheDocument();
    });
});
