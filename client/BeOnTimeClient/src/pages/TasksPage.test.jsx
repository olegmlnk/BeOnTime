import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { TasksPage } from './TasksPage';
import { AuthContext } from '../context/AuthContext';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо сервіс завдань
vi.mock('../services/taskService', () => ({
    taskService: {
        getAll: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        updateStatus: vi.fn(),
        delete: vi.fn(),
    }
}));

import { taskService } from '../services/taskService';

describe('TasksPage Component', () => {
    const mockUser = { id: '1', name: 'Test User', email: 'test@test.com' };

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderWithProviders = () => {
        return render(
            <BrowserRouter>
                <AuthContext.Provider value={{ user: mockUser, login: vi.fn(), logout: vi.fn(), register: vi.fn(), isLoading: false }}>
                    <TasksPage />
                </AuthContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити заголовок та кнопку створення', async () => {
        taskService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Всі завдання')).toBeInTheDocument();
        });
        expect(screen.getByText('Нове завдання')).toBeInTheDocument();
    });

    it('повинен рендерити три колонки Kanban-дошки', async () => {
        taskService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('To Do')).toBeInTheDocument();
        });
        expect(screen.getByText('In Progress')).toBeInTheDocument();
        expect(screen.getByText('Done')).toBeInTheDocument();
    });

    it('повинен розміщувати завдання у правильних колонках', async () => {
        taskService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'Todo Task', status: 0, priority: 1 },
            { id: '2', title: 'Progress Task', status: 1, priority: 2 },
            { id: '3', title: 'Done Task', status: 2, priority: 0 },
        ]);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Todo Task')).toBeInTheDocument();
        });
        expect(screen.getByText('Progress Task')).toBeInTheDocument();
        expect(screen.getByText('Done Task')).toBeInTheDocument();
    });

    it('повинен відкривати модальне вікно при кліку на "Нове завдання"', async () => {
        taskService.getAll.mockResolvedValueOnce([]);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Нове завдання')).toBeInTheDocument();
        });

        await user.click(screen.getByText('Нове завдання'));

        // TaskModal заголовок для нового завдання
        expect(screen.getByText('Нове завдання', { selector: 'h2' })).toBeInTheDocument();
        expect(screen.getByLabelText('Назва завдання')).toBeInTheDocument();
    });

    it('повинен створювати нове завдання через модальне вікно', async () => {
        taskService.getAll.mockResolvedValueOnce([]);
        taskService.create.mockResolvedValueOnce({ id: '99', title: 'Brand new task', status: 0, priority: 1 });

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Нове завдання')).toBeInTheDocument();
        });

        // Відкриваємо модалку
        await user.click(screen.getByText('Нове завдання'));

        // Заповнюємо форму
        await user.type(screen.getByLabelText('Назва завдання'), 'Brand new task');
        await user.click(screen.getByText('Створити'));

        await waitFor(() => {
            expect(taskService.create).toHaveBeenCalled();
        });
    });

    it('повинен відкривати модалку редагування при кліку на кнопку', async () => {
        taskService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'Existing task', status: 0, priority: 1 },
        ]);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Existing task')).toBeInTheDocument();
        });

        // Кнопка редагування
        const editBtn = screen.getByTitle('Редагувати');
        await user.click(editBtn);

        // Модалка з заголовком "Редагувати завдання"
        expect(screen.getByText('Редагувати завдання')).toBeInTheDocument();
        expect(screen.getByDisplayValue('Existing task')).toBeInTheDocument();
    });

    it('повинен видаляти завдання після підтвердження', async () => {
        taskService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'Task to delete', status: 0, priority: 0 },
        ]);
        taskService.delete.mockResolvedValueOnce({});

        vi.spyOn(window, 'confirm').mockReturnValueOnce(true);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Task to delete')).toBeInTheDocument();
        });

        const deleteBtn = screen.getByTitle('Видалити');
        await user.click(deleteBtn);

        expect(taskService.delete).toHaveBeenCalledWith('1');
    });

    it('повинен не видаляти завдання при скасуванні підтвердження', async () => {
        taskService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'Task not to delete', status: 0, priority: 0 },
        ]);

        vi.spyOn(window, 'confirm').mockReturnValueOnce(false);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Task not to delete')).toBeInTheDocument();
        });

        const deleteBtn = screen.getByTitle('Видалити');
        await user.click(deleteBtn);

        expect(taskService.delete).not.toHaveBeenCalled();
    });

    it('повинен показувати мок-дані при помилці API', async () => {
        taskService.getAll.mockRejectedValueOnce(new Error('API Error'));

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Finish Quarterly Report')).toBeInTheDocument();
        });
    });

    it('повинен показувати правильні лічильники колонок', async () => {
        taskService.getAll.mockResolvedValueOnce([
            { id: '1', title: 'T1', status: 0, priority: 0 },
            { id: '2', title: 'T2', status: 0, priority: 1 },
            { id: '3', title: 'T3', status: 1, priority: 2 },
        ]);

        renderWithProviders();

        await waitFor(() => {
            // Шукаємо лічильники в колонках
            const counts = screen.getAllByClassName ? null : null;
            expect(screen.getByText('T1')).toBeInTheDocument();
        });

        // Перевіряємо, що лічильники відображаються правильно
        const columnCounts = document.querySelectorAll('.column-count');
        expect(columnCounts[0].textContent).toBe('2'); // To Do: 2
        expect(columnCounts[1].textContent).toBe('1'); // In Progress: 1
        expect(columnCounts[2].textContent).toBe('0'); // Done: 0
    });
});
