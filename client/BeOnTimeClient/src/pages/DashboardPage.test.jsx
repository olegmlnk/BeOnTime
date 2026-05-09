import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { DashboardPage } from './DashboardPage';
import { AuthContext } from '../context/AuthContext';
import { ThemeContext } from '../context/ThemeContext';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо сервіси
vi.mock('../services/taskService', () => ({
    taskService: {
        getAll: vi.fn(),
        updateStatus: vi.fn(),
    }
}));

vi.mock('../services/ideaService', () => ({
    ideaService: {
        getAll: vi.fn(),
    }
}));

vi.mock('../services/roadmapService', () => ({
    roadmapService: {
        getAll: vi.fn(),
    }
}));

import { taskService } from '../services/taskService';
import { ideaService } from '../services/ideaService';
import { roadmapService } from '../services/roadmapService';

describe('DashboardPage Component', () => {
    const mockUser = { id: '1', name: 'Test User', email: 'test@test.com' };

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderWithProviders = (user = mockUser) => {
        return render(
            <BrowserRouter>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthContext.Provider value={{ user, login: vi.fn(), logout: vi.fn(), register: vi.fn(), isLoading: false }}>
                        <DashboardPage />
                    </AuthContext.Provider>
                </ThemeContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити привітання з іменем користувача', async () => {
        taskService.getAll.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([]);
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            // Привітання залежить від часу доби: "Доброго ранку/дня/вечора, Test!"
            expect(screen.getByText(/Test!/)).toBeInTheDocument();
        });
    });

    it('повинен відображати статистику завдань після завантаження', async () => {
        const mockTasks = [
            { id: '1', title: 'Finish Report', deadline: new Date(Date.now() + 86400000).toISOString(), status: 0, priority: 2 },
            { id: '2', title: 'Review PRs', status: 2, priority: 0 },
            { id: '3', title: 'In progress task', status: 1, priority: 1 },
        ];
        taskService.getAll.mockResolvedValueOnce(mockTasks);
        ideaService.getAll.mockResolvedValueOnce([]);
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            // Усього завдань: 3
            expect(screen.getByText('3')).toBeInTheDocument();
        });
        // Виконано: 1
        expect(screen.getByText('1')).toBeInTheDocument();
        // Активних: 2
        expect(screen.getByText('2')).toBeInTheDocument();
    });

    it('повинен відображати активні завдання після завантаження', async () => {
        const mockTasks = [
            { id: '1', title: 'Finish Report', deadline: new Date(Date.now() + 86400000).toISOString(), status: 0, priority: 2 },
            { id: '2', title: 'Review PRs', status: 2, priority: 0 },
        ];
        taskService.getAll.mockResolvedValueOnce(mockTasks);
        ideaService.getAll.mockResolvedValueOnce([]);
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Finish Report')).toBeInTheDocument();
        });
    });

    it('повинен відображати ідеї після завантаження', async () => {
        taskService.getAll.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'New landing concept', content: 'Use more animations' },
        ]);
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('New landing concept')).toBeInTheDocument();
        });
        expect(screen.getByText('Use more animations')).toBeInTheDocument();
    });

    it('повинен відображати повідомлення, якщо немає активних завдань', async () => {
        taskService.getAll.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([]);
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Немає активних завдань')).toBeInTheDocument();
        });
    });

    it('повинен перемикати статус завдання при кліку на чекбокс', async () => {
        const mockTasks = [
            { id: '1', title: 'Task to toggle', status: 0, priority: 1 },
        ];
        taskService.getAll.mockResolvedValueOnce(mockTasks);
        ideaService.getAll.mockResolvedValueOnce([]);
        roadmapService.getAll.mockResolvedValueOnce([]);
        taskService.updateStatus.mockResolvedValueOnce({});

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Task to toggle')).toBeInTheDocument();
        });

        // Нові CSS-класи: .dash-task-card і .dash-task-check
        const checkboxBtn = screen.getByText('Task to toggle').closest('.dash-task-card').querySelector('.dash-task-check');
        await user.click(checkboxBtn);

        expect(taskService.updateStatus).toHaveBeenCalledWith('1', 2);
    });

    it('повинен показувати мок-дані при помилці API', async () => {
        taskService.getAll.mockRejectedValueOnce(new Error('API Error'));
        ideaService.getAll.mockRejectedValueOnce(new Error('API Error'));
        roadmapService.getAll.mockRejectedValueOnce(new Error('API Error'));

        renderWithProviders();

        // Мок-дані з catch-блоку DashboardPage
        await waitFor(() => {
            expect(screen.getByText('Finish Quarterly Report')).toBeInTheDocument();
        });
        expect(screen.getByText('New landing concept')).toBeInTheDocument();
    });

    it('повинен відображати роадмапи з прогресом', async () => {
        taskService.getAll.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([]);
        roadmapService.getAll.mockResolvedValueOnce([
            { id: '1', name: 'Вивчити React', progressPercent: 60, totalTasks: 5, doneTasks: 3 },
        ]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Вивчити React')).toBeInTheDocument();
        });
        expect(screen.getByText('60%')).toBeInTheDocument();
        expect(screen.getByText('3 / 5 завдань')).toBeInTheDocument();
    });

    it('повинен відображати "Немає ідей" при порожньому списку', async () => {
        taskService.getAll.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([]);
        roadmapService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Немає ідей')).toBeInTheDocument();
        });
    });
});
