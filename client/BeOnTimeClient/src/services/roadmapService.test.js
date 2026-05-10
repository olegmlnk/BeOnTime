import { describe, it, expect, vi, beforeEach } from 'vitest';
import { roadmapService } from './roadmapService';

vi.mock('./apiClient', () => ({
    default: {
        get: vi.fn(),
        post: vi.fn(),
        put: vi.fn(),
        delete: vi.fn(),
    }
}));

import apiClient from './apiClient';

describe('roadmapService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('getAll повинен отримувати всі роадмапи', async () => {
        apiClient.get.mockResolvedValueOnce({ data: [{ id: '1', name: 'Roadmap 1' }] });

        const result = await roadmapService.getAll();

        expect(apiClient.get).toHaveBeenCalledWith('/roadmaps');
        expect(result).toEqual([{ id: '1', name: 'Roadmap 1' }]);
    });

    it('getById повинен отримувати роадмап за ID', async () => {
        apiClient.get.mockResolvedValueOnce({ data: { id: '5', name: 'Specific' } });

        const result = await roadmapService.getById('5');

        expect(apiClient.get).toHaveBeenCalledWith('/roadmaps/5');
        expect(result.id).toBe('5');
    });

    it('create повинен створювати роадмап', async () => {
        const data = { name: 'New Roadmap' };
        apiClient.post.mockResolvedValueOnce({ data: { id: '99', ...data } });

        const result = await roadmapService.create(data);

        expect(apiClient.post).toHaveBeenCalledWith('/roadmaps', data);
        expect(result.name).toBe('New Roadmap');
    });

    it('update повинен оновлювати роадмап', async () => {
        apiClient.put.mockResolvedValueOnce({ data: { id: '1', name: 'Updated' } });

        await roadmapService.update('1', { name: 'Updated' });

        expect(apiClient.put).toHaveBeenCalledWith('/roadmaps/1', { name: 'Updated' });
    });

    it('delete повинен видаляти роадмап', async () => {
        apiClient.delete.mockResolvedValueOnce({ data: {} });

        await roadmapService.delete('1');

        expect(apiClient.delete).toHaveBeenCalledWith('/roadmaps/1');
    });

    it('reorder повинен змінювати порядок завдань', async () => {
        const taskIds = ['t3', 't1', 't2'];
        apiClient.put.mockResolvedValueOnce({ data: {} });

        await roadmapService.reorder('roadmap-1', taskIds);

        expect(apiClient.put).toHaveBeenCalledWith('/roadmaps/roadmap-1/reorder', { taskIds });
    });
});
