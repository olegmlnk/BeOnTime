import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { RoadmapCard } from './RoadmapCard';
import { BrowserRouter } from 'react-router-dom';

// Мокаємо useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('RoadmapCard Component', () => {
    const baseRoadmap = {
        id: '1',
        name: 'Вивчити React',
        description: 'Повний шлях від основ до просунутих концепцій.',
        progressPercent: 60,
        doneTasks: 3,
        totalTasks: 5,
        startDate: '2026-04-01T00:00:00',
        targetDate: '2026-06-30T00:00:00',
    };

    beforeEach(() => {
        mockNavigate.mockClear();
    });

    const renderCard = (roadmap = baseRoadmap, onDelete = null) => {
        return render(
            <BrowserRouter>
                <RoadmapCard roadmap={roadmap} onDelete={onDelete} />
            </BrowserRouter>
        );
    };

    it('повинен рендерити назву роадмапу', () => {
        renderCard();
        expect(screen.getByText('Вивчити React')).toBeInTheDocument();
    });

    it('повинен рендерити опис роадмапу', () => {
        renderCard();
        expect(screen.getByText('Повний шлях від основ до просунутих концепцій.')).toBeInTheDocument();
    });

    it('повинен не рендерити опис, якщо його немає', () => {
        renderCard({ ...baseRoadmap, description: null });
        expect(document.querySelector('.roadmap-card-desc')).toBeFalsy();
    });

    it('повинен відображати прогрес у відсотках', () => {
        renderCard();
        expect(screen.getByText('60%')).toBeInTheDocument();
    });

    it('повинен відображати кількість завдань', () => {
        renderCard();
        expect(screen.getByText('3 / 5 завдань')).toBeInTheDocument();
    });

    it('повинен відображати 0% для роадмапу без прогресу', () => {
        renderCard({ ...baseRoadmap, progressPercent: null, doneTasks: 0, totalTasks: 0 });
        expect(screen.getByText('0%')).toBeInTheDocument();
        expect(screen.getByText('0 / 0 завдань')).toBeInTheDocument();
    });

    it('повинен навігувати до деталей при кліку на карточку', async () => {
        renderCard();
        const user = userEvent.setup();

        await user.click(screen.getByText('Вивчити React'));
        expect(mockNavigate).toHaveBeenCalledWith('/roadmaps/1');
    });

    it('повинен рендерити кнопку видалення, якщо onDelete передано', () => {
        renderCard(baseRoadmap, vi.fn());
        expect(screen.getByTitle('Видалити')).toBeInTheDocument();
    });

    it('повинен не рендерити кнопку видалення, якщо onDelete не передано', () => {
        renderCard(baseRoadmap, null);
        expect(screen.queryByTitle('Видалити')).toBeFalsy();
    });

    it('повинен викликати onDelete з id роадмапу та не навігувати', async () => {
        const mockDelete = vi.fn();
        renderCard(baseRoadmap, mockDelete);
        const user = userEvent.setup();

        await user.click(screen.getByTitle('Видалити'));
        expect(mockDelete).toHaveBeenCalledWith('1');
        // Клік на кнопку не повинен навігувати (stopPropagation)
        expect(mockNavigate).not.toHaveBeenCalled();
    });

    it('повинен відображати дати початку та цільову', () => {
        renderCard();
        // Перевіряємо, що дати відрендерилися (формат uk-UA)
        const dateElements = document.querySelectorAll('.roadmap-date');
        expect(dateElements.length).toBe(2);
    });

    it('повинен не відображати дати, якщо їх немає', () => {
        renderCard({ ...baseRoadmap, startDate: null, targetDate: null });
        const dateElements = document.querySelectorAll('.roadmap-date');
        expect(dateElements.length).toBe(0);
    });
});
