import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { RoadmapDetailPage } from './RoadmapDetailPage';
import { AuthContext } from '../context/AuthContext';
import { ThemeContext } from '../context/ThemeContext';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо сервіси
vi.mock('../services/roadmapService', () => ({
    roadmapService: {
        getById: vi.fn(),
        update: vi.fn(),
        delete: vi.fn(),
        reorder: vi.fn(),
    }
}));

vi.mock('../services/taskService', () => ({
    taskService: {
        create: vi.fn(),
        updateStatus: vi.fn(),
    }
}));

import { roadmapService } from '../services/roadmapService';
import { taskService } from '../services/taskService';

// Мокаємо useParams та useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
        useParams: () => ({ id: 'test-roadmap-id' }),
    };
});

describe('RoadmapDetailPage Component', () => {
    const mockUser = { id: '1', name: 'Test User', email: 'test@test.com' };

    const mockRoadmap = {
        id: 'test-roadmap-id',
        name: 'Вивчити React',
        description: 'Повний шлях від основ до просунутих концепцій.',
        startDate: '2026-04-01T00:00:00',
        targetDate: '2026-06-30T00:00:00',
        totalTasks: 3,
        doneTasks: 1,
        progressPercent: 33,
        tasks: [
            { id: 't1', title: 'Вивчити JSX', status: 2, priority: 2, deadline: '2026-04-10T00:00:00', orderInRoadmap: 0 },
            { id: 't2', title: 'React Hooks', status: 1, priority: 1, deadline: '2026-04-20T00:00:00', orderInRoadmap: 1 },
            { id: 't3', title: 'Побудувати проект', status: 0, priority: 0, deadline: '2026-06-15T00:00:00', orderInRoadmap: 2 },
        ],
    };

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderWithProviders = () => {
        return render(
            <BrowserRouter>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthContext.Provider value={{ user: mockUser, login: vi.fn(), logout: vi.fn(), register: vi.fn(), isLoading: false }}>
                        <RoadmapDetailPage />
                    </AuthContext.Provider>
                </ThemeContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити назву роадмапу', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Вивчити React')).toBeInTheDocument();
        });
    });

    it('повинен рендерити опис роадмапу', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Повний шлях від основ до просунутих концепцій.')).toBeInTheDocument();
        });
    });

    it('повинен відображати прогрес роадмапу', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('33%')).toBeInTheDocument();
        });
        expect(screen.getByText('1 / 3 завдань')).toBeInTheDocument();
    });

    it('повинен відображати список завдань', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Вивчити JSX')).toBeInTheDocument();
        });
        expect(screen.getByText('React Hooks')).toBeInTheDocument();
        expect(screen.getByText('Побудувати проект')).toBeInTheDocument();
    });

    it('повинен відображати кількість завдань у заголовку секції', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Завдання (3)')).toBeInTheDocument();
        });
    });

    it('повинен відображати статуси завдань', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Done')).toBeInTheDocument();
        });
        expect(screen.getByText('In Progress')).toBeInTheDocument();
        expect(screen.getByText('To Do')).toBeInTheDocument();
    });

    it('повинен перемикати статус завдання при кліку', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);
        taskService.updateStatus.mockResolvedValueOnce({});

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Побудувати проект')).toBeInTheDocument();
        });

        // Кнопка чекбоксу для незавершеного завдання (status=0)
        const checkBtn = screen.getByText('Побудувати проект').closest('.roadmap-task-item').querySelector('.step-check-btn');
        await user.click(checkBtn);

        expect(taskService.updateStatus).toHaveBeenCalledWith('t3', 2);
    });

    it('повинен додавати новий крок через форму', async () => {
        roadmapService.getById
            .mockResolvedValueOnce(mockRoadmap)
            .mockResolvedValueOnce({ ...mockRoadmap, tasks: [...mockRoadmap.tasks, { id: 't4', title: 'Новий крок', status: 0, priority: 1, orderInRoadmap: 3 }] });
        taskService.create.mockResolvedValueOnce({});

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByPlaceholderText('Додати наступний крок...')).toBeInTheDocument();
        });

        await user.type(screen.getByPlaceholderText('Додати наступний крок...'), 'Новий крок');
        await user.click(screen.getByText('Додати'));

        await waitFor(() => {
            expect(taskService.create).toHaveBeenCalledWith(
                expect.objectContaining({ title: 'Новий крок', roadmapId: 'test-roadmap-id' })
            );
        });
    });

    it('повинен рендерити кнопки "Редагувати" та "Видалити"', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Редагувати')).toBeInTheDocument();
        });
        expect(screen.getByText('Видалити')).toBeInTheDocument();
    });

    it('повинен відкривати модалку редагування при кліку', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Редагувати')).toBeInTheDocument();
        });

        await user.click(screen.getByText('Редагувати'));

        expect(screen.getByText('Редагувати роадмап')).toBeInTheDocument();
        expect(screen.getByDisplayValue('Вивчити React')).toBeInTheDocument();
    });

    it('повинен видаляти роадмап та навігувати назад', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);
        roadmapService.delete.mockResolvedValueOnce({});
        vi.spyOn(window, 'confirm').mockReturnValueOnce(true);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByText('Видалити')).toBeInTheDocument();
        });

        await user.click(screen.getByText('Видалити'));

        expect(roadmapService.delete).toHaveBeenCalledWith('test-roadmap-id');
        expect(mockNavigate).toHaveBeenCalledWith('/roadmaps');
    });

    it('повинен показувати мок-дані при помилці API', async () => {
        roadmapService.getById.mockRejectedValueOnce(new Error('API Error'));

        renderWithProviders();

        await waitFor(() => {
            expect(screen.getByText('Вивчити React')).toBeInTheDocument();
        });
        // Мок-завдання з fallback
        expect(screen.getByText('Вивчити JSX та компоненти')).toBeInTheDocument();
    });

    it('повинен навігувати назад при кліку на кнопку "Назад"', async () => {
        roadmapService.getById.mockResolvedValueOnce(mockRoadmap);

        renderWithProviders();
        const user = userEvent.setup();

        await waitFor(() => {
            expect(screen.getByTitle('Назад до роадмапів')).toBeInTheDocument();
        });

        await user.click(screen.getByTitle('Назад до роадмапів'));
        expect(mockNavigate).toHaveBeenCalledWith('/roadmaps');
    });
});
