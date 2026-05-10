import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { TaskModal } from './TaskModal';

describe('TaskModal Component', () => {
    const mockOnClose = vi.fn();
    const mockOnSave = vi.fn();

    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('повинен не рендерити нічого, якщо isOpen=false', () => {
        const { container } = render(
            <TaskModal isOpen={false} onClose={mockOnClose} onSave={mockOnSave} />
        );
        expect(container.querySelector('.modal-overlay')).toBeFalsy();
    });

    it('повинен рендерити модалку, якщо isOpen=true', () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        expect(screen.getByText('Нове завдання')).toBeInTheDocument();
    });

    it('повинен показувати заголовок "Редагувати завдання" при редагуванні', () => {
        render(
            <TaskModal
                isOpen={true}
                onClose={mockOnClose}
                onSave={mockOnSave}
                initialData={{ id: '1', title: 'Existing', status: 0, priority: 1 }}
            />
        );
        expect(screen.getByText('Редагувати завдання')).toBeInTheDocument();
    });

    it('повинен заповнювати форму даними при редагуванні', () => {
        render(
            <TaskModal
                isOpen={true}
                onClose={mockOnClose}
                onSave={mockOnSave}
                initialData={{ id: '1', title: 'Edit me', description: 'Details', status: 1, priority: 2 }}
            />
        );
        expect(screen.getByDisplayValue('Edit me')).toBeInTheDocument();
        expect(screen.getByDisplayValue('Details')).toBeInTheDocument();
    });

    it('повинен показувати кнопку "Створити" для нового завдання', () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        expect(screen.getByText('Створити')).toBeInTheDocument();
    });

    it('повинен показувати кнопку "Зберегти" при редагуванні', () => {
        render(
            <TaskModal
                isOpen={true}
                onClose={mockOnClose}
                onSave={mockOnSave}
                initialData={{ id: '1', title: 'Task', status: 0, priority: 1 }}
            />
        );
        expect(screen.getByText('Зберегти')).toBeInTheDocument();
    });

    it('повинен викликати onClose при кліку на "Скасувати"', async () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        const user = userEvent.setup();

        await user.click(screen.getByText('Скасувати'));
        expect(mockOnClose).toHaveBeenCalled();
    });

    it('повинен викликати onClose при кліку на overlay', async () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        const user = userEvent.setup();

        const overlay = document.querySelector('.modal-overlay');
        await user.click(overlay);
        expect(mockOnClose).toHaveBeenCalled();
    });

    it('повинен не закриватися при кліку на контент модалки', async () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        const user = userEvent.setup();

        const content = document.querySelector('.modal-content');
        await user.click(content);
        expect(mockOnClose).not.toHaveBeenCalled();
    });

    it('повинен викликати onSave з даними при сабміті форми', async () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        const user = userEvent.setup();

        await user.type(screen.getByLabelText('Назва завдання'), 'New task title');
        await user.type(screen.getByLabelText(/Опис/), 'Task description');
        await user.click(screen.getByText('Створити'));

        expect(mockOnSave).toHaveBeenCalledWith(
            expect.objectContaining({
                title: 'New task title',
                description: 'Task description',
            })
        );
    });

    it('повинен не сабмітити форму без назви', async () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);

        // Кнопка "Створити" має бути disabled коли назва порожня
        const submitBtn = screen.getByText('Створити').closest('button');
        expect(submitBtn).toBeDisabled();
    });

    it('повинен рендерити поля пріоритету та статусу', () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        expect(screen.getByLabelText('Пріоритет')).toBeInTheDocument();
        expect(screen.getByLabelText('Статус')).toBeInTheDocument();
    });

    it('повинен рендерити всі опції пріоритету', () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        expect(screen.getByText('Low')).toBeInTheDocument();
        expect(screen.getByText('Medium')).toBeInTheDocument();
        expect(screen.getByText('High')).toBeInTheDocument();
    });

    it('повинен рендерити всі опції статусу', () => {
        render(<TaskModal isOpen={true} onClose={mockOnClose} onSave={mockOnSave} />);
        expect(screen.getByText('To Do')).toBeInTheDocument();
        expect(screen.getByText('In Progress')).toBeInTheDocument();
        expect(screen.getByText('Done')).toBeInTheDocument();
        expect(screen.getByText('Cancelled')).toBeInTheDocument();
    });
});
