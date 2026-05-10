import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { RoadmapModal } from './RoadmapModal';

describe('RoadmapModal Component', () => {
    const mockOnSave = vi.fn();
    const mockOnClose = vi.fn();

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderModal = (props = {}) => {
        return render(
            <RoadmapModal
                isOpen={true}
                onClose={mockOnClose}
                onSave={mockOnSave}
                initialData={null}
                {...props}
            />
        );
    };

    it('повинен не рендеритися, якщо isOpen=false', () => {
        renderModal({ isOpen: false });
        expect(screen.queryByText('Новий роадмап')).toBeFalsy();
    });

    it('повинен рендерити заголовок "Новий роадмап" для нового', () => {
        renderModal();
        expect(screen.getByText('Новий роадмап')).toBeInTheDocument();
    });

    it('повинен рендерити заголовок "Редагувати роадмап" для існуючого', () => {
        renderModal({ initialData: { id: '1', name: 'Test' } });
        expect(screen.getByText('Редагувати роадмап')).toBeInTheDocument();
    });

    it('повинен рендерити всі поля форми', () => {
        renderModal();
        expect(screen.getByLabelText('Назва роадмапу')).toBeInTheDocument();
        expect(screen.getByLabelText("Опис (необов'язково)")).toBeInTheDocument();
        expect(screen.getByLabelText('Дата початку')).toBeInTheDocument();
        expect(screen.getByLabelText('Цільова дата')).toBeInTheDocument();
    });

    it('повинен показувати кнопку "Створити" для нового роадмапу', () => {
        renderModal();
        expect(screen.getByText('Створити')).toBeInTheDocument();
    });

    it('повинен показувати кнопку "Зберегти" для існуючого роадмапу', () => {
        renderModal({ initialData: { id: '1', name: 'Test' } });
        expect(screen.getByText('Зберегти')).toBeInTheDocument();
    });

    it('повинен заповнювати форму даними при редагуванні', () => {
        renderModal({
            initialData: {
                id: '1',
                name: 'Мій план',
                description: 'Опис плану',
                startDate: '2026-05-01T00:00:00',
                targetDate: '2026-06-01T00:00:00',
            }
        });
        expect(screen.getByDisplayValue('Мій план')).toBeInTheDocument();
        expect(screen.getByDisplayValue('Опис плану')).toBeInTheDocument();
        // Перевіряємо, що поля дат заповнені (формат може варіюватися залежно від часового поясу)
        const startInput = screen.getByLabelText('Дата початку');
        const targetInput = screen.getByLabelText('Цільова дата');
        expect(startInput.value).toBeTruthy();
        expect(targetInput.value).toBeTruthy();
    });

    it('повинен мати disabled кнопку "Створити" при порожній назві', () => {
        renderModal();
        const submitBtn = screen.getByRole('button', { name: 'Створити' });
        expect(submitBtn).toBeDisabled();
    });

    it('повинен викликати onSave з даними при сабміті', async () => {
        renderModal();
        const user = userEvent.setup();

        await user.type(screen.getByLabelText('Назва роадмапу'), 'Новий план');
        await user.type(screen.getByLabelText("Опис (необов'язково)"), 'Деталі плану');
        await user.click(screen.getByText('Створити'));

        expect(mockOnSave).toHaveBeenCalledTimes(1);
        const callArgs = mockOnSave.mock.calls[0][0];
        expect(callArgs.name).toBe('Новий план');
        expect(callArgs.description).toBe('Деталі плану');
        expect(callArgs.id).toBeUndefined();
    });

    it('повинен не сабмітити форму без назви', async () => {
        renderModal();

        const submitBtn = screen.getByRole('button', { name: 'Створити' });
        expect(submitBtn).toBeDisabled();
        expect(mockOnSave).not.toHaveBeenCalled();
    });

    it('повинен викликати onClose при кліку на "Скасувати"', async () => {
        renderModal();
        const user = userEvent.setup();

        await user.click(screen.getByText('Скасувати'));
        expect(mockOnClose).toHaveBeenCalled();
    });

    it('повинен викликати onClose при кліку на overlay', async () => {
        renderModal();
        const user = userEvent.setup();

        const overlay = document.querySelector('.modal-overlay');
        await user.click(overlay);
        expect(mockOnClose).toHaveBeenCalled();
    });

    it('повинен не закриватися при кліку на контент модалки', async () => {
        renderModal();
        const user = userEvent.setup();

        const content = document.querySelector('.modal-content');
        await user.click(content);
        expect(mockOnClose).not.toHaveBeenCalled();
    });

    it('повинен передавати id при редагуванні', async () => {
        renderModal({ initialData: { id: 'abc-123', name: 'Існуючий' } });
        const user = userEvent.setup();

        await user.click(screen.getByText('Зберегти'));

        expect(mockOnSave).toHaveBeenCalledTimes(1);
        expect(mockOnSave.mock.calls[0][0].id).toBe('abc-123');
    });
});
