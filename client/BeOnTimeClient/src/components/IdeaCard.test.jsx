import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { IdeaCard } from './IdeaCard';

describe('IdeaCard Component', () => {
    const baseIdea = {
        id: '1',
        title: 'Test Idea',
        description: 'Some description',
        content: 'Some description',
    };

    it('повинен рендерити заголовок ідеї', () => {
        render(<IdeaCard idea={baseIdea} />);
        expect(screen.getByText('Test Idea')).toBeInTheDocument();
    });

    it('повинен рендерити опис ідеї', () => {
        render(<IdeaCard idea={baseIdea} />);
        expect(screen.getByText('Some description')).toBeInTheDocument();
    });

    it('повинен не рендерити опис, якщо його немає', () => {
        render(<IdeaCard idea={{ id: '1', title: 'No desc' }} />);
        expect(screen.queryByClassName?.('sticker-desc')).toBeFalsy();
        const descEl = document.querySelector('.sticker-desc');
        expect(descEl).toBeFalsy();
    });

    it('повинен не рендерити кнопки дій, якщо callbacks не передані', () => {
        render(<IdeaCard idea={baseIdea} />);
        expect(screen.queryByTitle('Видалити')).toBeFalsy();
        expect(screen.queryByTitle('Перетворити на завдання')).toBeFalsy();
    });

    it('повинен рендерити кнопку видалення, якщо onDelete передано', () => {
        render(<IdeaCard idea={baseIdea} onDelete={vi.fn()} />);
        expect(screen.getByTitle('Видалити')).toBeInTheDocument();
    });

    it('повинен рендерити кнопку конвертації, якщо onConvert передано', () => {
        render(<IdeaCard idea={baseIdea} onConvert={vi.fn()} />);
        expect(screen.getByTitle('Перетворити на завдання')).toBeInTheDocument();
    });

    it('повинен викликати onDelete з id ідеї', async () => {
        const mockDelete = vi.fn();
        render(<IdeaCard idea={baseIdea} onDelete={mockDelete} />);
        const user = userEvent.setup();

        await user.click(screen.getByTitle('Видалити'));
        expect(mockDelete).toHaveBeenCalledWith('1');
    });

    it('повинен викликати onConvert з об\'єктом ідеї', async () => {
        const mockConvert = vi.fn();
        render(<IdeaCard idea={baseIdea} onConvert={mockConvert} />);
        const user = userEvent.setup();

        await user.click(screen.getByTitle('Перетворити на завдання'));
        expect(mockConvert).toHaveBeenCalledWith(baseIdea);
    });

    it('повинен рендерити обидві кнопки, якщо обидва callbacks передано', () => {
        render(<IdeaCard idea={baseIdea} onDelete={vi.fn()} onConvert={vi.fn()} />);
        expect(screen.getByTitle('Видалити')).toBeInTheDocument();
        expect(screen.getByTitle('Перетворити на завдання')).toBeInTheDocument();
    });
});
