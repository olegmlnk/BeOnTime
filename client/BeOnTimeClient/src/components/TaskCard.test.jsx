import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { TaskCard } from './TaskCard';

describe('TaskCard Component', () => {
    const mockToggle = vi.fn();

    const baseTask = {
        id: '1',
        title: 'Test Task',
        status: 0,
        priority: 1,
    };

    it('повинен рендерити назву завдання', () => {
        render(<TaskCard task={baseTask} onToggleStatus={mockToggle} />);
        expect(screen.getByText('Test Task')).toBeInTheDocument();
    });

    it('повинен відображати пріоритет Low', () => {
        render(<TaskCard task={{ ...baseTask, priority: 0 }} onToggleStatus={mockToggle} />);
        expect(screen.getByText('Low Priority')).toBeInTheDocument();
    });

    it('повинен відображати пріоритет Medium', () => {
        render(<TaskCard task={{ ...baseTask, priority: 1 }} onToggleStatus={mockToggle} />);
        expect(screen.getByText('Medium Priority')).toBeInTheDocument();
    });

    it('повинен відображати пріоритет High', () => {
        render(<TaskCard task={{ ...baseTask, priority: 2 }} onToggleStatus={mockToggle} />);
        expect(screen.getByText('High Priority')).toBeInTheDocument();
    });

    it('повинен додавати клас done для виконаного завдання', () => {
        const { container } = render(<TaskCard task={{ ...baseTask, status: 2 }} onToggleStatus={mockToggle} />);
        expect(container.querySelector('.task-card.done')).toBeTruthy();
    });

    it('повинен не мати клас done для невиконаного завдання', () => {
        const { container } = render(<TaskCard task={{ ...baseTask, status: 0 }} onToggleStatus={mockToggle} />);
        expect(container.querySelector('.task-card.done')).toBeFalsy();
    });

    it('повинен викликати onToggleStatus при кліку на чекбокс', async () => {
        render(<TaskCard task={baseTask} onToggleStatus={mockToggle} />);
        const user = userEvent.setup();

        const checkbox = document.querySelector('.task-checkbox');
        await user.click(checkbox);

        expect(mockToggle).toHaveBeenCalledWith(baseTask);
    });

    it('повинен відображати дедлайн, якщо він є', () => {
        const taskWithDeadline = {
            ...baseTask,
            deadline: '2026-05-10T00:00:00.000Z',
        };
        render(<TaskCard task={taskWithDeadline} onToggleStatus={mockToggle} />);

        // Перевіряємо, що дата відображається (формат залежить від локалі)
        const deadlineEl = document.querySelector('.task-deadline');
        expect(deadlineEl).toBeTruthy();
    });

    it('повинен не відображати дедлайн, якщо його немає', () => {
        render(<TaskCard task={baseTask} onToggleStatus={mockToggle} />);
        const deadlineEl = document.querySelector('.task-deadline');
        expect(deadlineEl).toBeFalsy();
    });
});
