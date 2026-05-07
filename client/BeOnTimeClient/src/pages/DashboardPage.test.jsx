import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { DashboardPage } from './DashboardPage';
import { AuthContext } from '../context/AuthContext';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо сервіси
vi.mock('../services/taskService', () => ({
    taskService: {
        getUpcoming: vi.fn(),
        updateStatus: vi.fn(),
    }
}));

vi.mock('../services/ideaService', () => ({
    ideaService: {
        getAll: vi.fn(),
        create: vi.fn(),
    }
}));

import { taskService } from '../services/taskService';
import { ideaService } from '../services/ideaService';

describe('DashboardPage Component', () => {
    const mockUser = { id: '1', name: 'Test User', email: 'test@test.com' };

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderWithProviders = (user = mockUser) => {
        return render(
            <BrowserRouter>
                <AuthContext.Provider value={{ user, login: vi.fn(), logout: vi.fn(), register: vi.fn(), isLoading: false }}>
                    <DashboardPage />
                </AuthContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити привітання з іменем користувача', async () => {
        taskService.getUpcoming.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText(/Good morning, Test/i)).toBeInTheDocument();
        });
    });

    it('повинен відображати завдання після завантаження', async () => {
        const mockTasks = [
            { id: '1', title: 'Finish Report', deadline: new Date(Date.now() + 86400000).toISOString(), status: 0, priority: 2 },
            { id: '2', title: 'Review PRs', deadline: new Date().toISOString(), status: 2, priority: 0 },
        ];
        taskService.getUpcoming.mockResolvedValueOnce(mockTasks);
        ideaService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Finish Report')).toBeInTheDocument();
        });
        expect(screen.getByText('Review PRs')).toBeInTheDocument();
    });

    it('повинен відображати ідеї після завантаження', async () => {
        taskService.getUpcoming.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'New landing page concept', description: 'Use more animations' },
        ]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('New landing page concept')).toBeInTheDocument();
        });
    });

    it('повинен відображати повідомлення, якщо немає завдань', async () => {
        taskService.getUpcoming.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText(/No upcoming tasks/i)).toBeInTheDocument();
        });
    });

    it('повинен перемикати статус завдання при кліку на чекбокс', async () => {
        const mockTasks = [
            { id: '1', title: 'Task to toggle', status: 0, priority: 1 },
        ];
        taskService.getUpcoming.mockResolvedValueOnce(mockTasks);
        ideaService.getAll.mockResolvedValueOnce([]);
        taskService.updateStatus.mockResolvedValueOnce({});

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Task to toggle')).toBeInTheDocument();
        });

        // TaskCard рендерить кнопку-чекбокс
        const checkboxBtn = screen.getByText('Task to toggle').closest('.task-card').querySelector('.task-checkbox');
        await user.click(checkboxBtn);

        expect(taskService.updateStatus).toHaveBeenCalledWith('1', 2);
    });

    it('повинен показувати мок-дані при помилці API', async () => {
        taskService.getUpcoming.mockRejectedValueOnce(new Error('API Error'));
        ideaService.getAll.mockRejectedValueOnce(new Error('API Error'));

        renderWithProviders();

        // Мок-дані з catch-блоку DashboardPage
        await waitFor(() => {
            expect(screen.getByText('Finish Quarterly Report')).toBeInTheDocument();
        });
        expect(screen.getByText('New landing page concept')).toBeInTheDocument();
    });

    it('повинен додавати нову ідею при натисканні Enter', async () => {
        taskService.getUpcoming.mockResolvedValueOnce([]);
        ideaService.getAll.mockResolvedValueOnce([]);
        ideaService.create.mockResolvedValueOnce({ id: '99', title: 'My new idea' });

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByPlaceholderText(/Jot down an idea/i)).toBeInTheDocument();
        });

        const input = screen.getByPlaceholderText(/Jot down an idea/i);
        await user.type(input, 'My new idea{Enter}');

        await waitFor(() => {
            expect(ideaService.create).toHaveBeenCalledWith({ title: 'My new idea' });
        });
    });

    it('повинен рендерити розділ Completed для виконаних завдань', async () => {
        const mockTasks = [
            { id: '1', title: 'Pending Task', status: 0, priority: 1 },
            { id: '2', title: 'Done Task', status: 2, priority: 0 },
        ];
        taskService.getUpcoming.mockResolvedValueOnce(mockTasks);
        ideaService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Completed')).toBeInTheDocument();
        });
        expect(screen.getByText('Done Task')).toBeInTheDocument();
    });
});
