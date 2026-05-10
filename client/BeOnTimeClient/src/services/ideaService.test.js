import { describe, it, expect, vi, beforeEach } from 'vitest';
import { ideaService } from './ideaService';

vi.mock('./apiClient', () => ({
    default: {
        get: vi.fn(),
        post: vi.fn(),
        put: vi.fn(),
        delete: vi.fn(),
    }
}));

import apiClient from './apiClient';

describe('ideaService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('getAll повинен отримувати всі ідеї', async () => {
        apiClient.get.mockResolvedValueOnce({ data: [{ id: '1', title: 'Idea 1' }] });

        const result = await ideaService.getAll();

        expect(apiClient.get).toHaveBeenCalledWith('/ideas');
        expect(result).toEqual([{ id: '1', title: 'Idea 1' }]);
    });

    it('getById повинен отримувати ідею за ID', async () => {
        apiClient.get.mockResolvedValueOnce({ data: { id: '5', title: 'Specific' } });

        const result = await ideaService.getById('5');

        expect(apiClient.get).toHaveBeenCalledWith('/ideas/5');
        expect(result.id).toBe('5');
    });

    it('create повинен створювати ідею', async () => {
        const ideaData = { title: 'New idea', content: 'Details' };
        apiClient.post.mockResolvedValueOnce({ data: { id: '99', ...ideaData } });

        const result = await ideaService.create(ideaData);

        expect(apiClient.post).toHaveBeenCalledWith('/ideas', ideaData);
        expect(result.title).toBe('New idea');
    });

    it('update повинен оновлювати ідею', async () => {
        apiClient.put.mockResolvedValueOnce({ data: { id: '1', title: 'Updated' } });

        await ideaService.update('1', { title: 'Updated' });

        expect(apiClient.put).toHaveBeenCalledWith('/ideas/1', { title: 'Updated' });
    });

    it('delete повинен видаляти ідею', async () => {
        apiClient.delete.mockResolvedValueOnce({ data: {} });

        await ideaService.delete('1');

        expect(apiClient.delete).toHaveBeenCalledWith('/ideas/1');
    });

    it('convertToTask повинен конвертувати ідею в завдання', async () => {
        apiClient.post.mockResolvedValueOnce({ data: { taskId: 'new-task' } });

        const result = await ideaService.convertToTask('1');

        expect(apiClient.post).toHaveBeenCalledWith('/ideas/1/convert');
        expect(result.taskId).toBe('new-task');
    });
});
