import { describe, it, expect, vi, beforeEach } from 'vitest';
import { taskService } from './taskService';

vi.mock('./apiClient', () => ({
    default: {
        get: vi.fn(),
        post: vi.fn(),
        put: vi.fn(),
        patch: vi.fn(),
        delete: vi.fn(),
    }
}));

import apiClient from './apiClient';

describe('taskService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('getAll повинен отримувати всі завдання', async () => {
        apiClient.get.mockResolvedValueOnce({ data: [{ id: '1', title: 'Task 1' }] });

        const result = await taskService.getAll();

        expect(apiClient.get).toHaveBeenCalledWith('/tasks?');
        expect(result).toEqual([{ id: '1', title: 'Task 1' }]);
    });

    it('getAll повинен передавати фільтри', async () => {
        apiClient.get.mockResolvedValueOnce({ data: [] });

        await taskService.getAll({ status: '1', priority: '2' });

        expect(apiClient.get).toHaveBeenCalledWith('/tasks?status=1&priority=2');
    });

    it('getById повинен отримувати завдання за ID', async () => {
        apiClient.get.mockResolvedValueOnce({ data: { id: '5', title: 'Specific' } });

        const result = await taskService.getById('5');

        expect(apiClient.get).toHaveBeenCalledWith('/tasks/5');
        expect(result.id).toBe('5');
    });

    it('getOverdue повинен отримувати прострочені завдання', async () => {
        apiClient.get.mockResolvedValueOnce({ data: [] });

        await taskService.getOverdue();

        expect(apiClient.get).toHaveBeenCalledWith('/tasks/overdue');
    });

    it('getUpcoming повинен отримувати найближчі завдання', async () => {
        apiClient.get.mockResolvedValueOnce({ data: [] });

        await taskService.getUpcoming(14);

        expect(apiClient.get).toHaveBeenCalledWith('/tasks/upcoming?days=14');
    });

    it('create повинен створювати завдання', async () => {
        const taskData = { title: 'New', priority: 1 };
        apiClient.post.mockResolvedValueOnce({ data: { id: '99', ...taskData } });

        const result = await taskService.create(taskData);

        expect(apiClient.post).toHaveBeenCalledWith('/tasks', taskData);
        expect(result.title).toBe('New');
    });

    it('update повинен оновлювати завдання', async () => {
        apiClient.put.mockResolvedValueOnce({ data: { id: '1', title: 'Updated' } });

        await taskService.update('1', { title: 'Updated' });

        expect(apiClient.put).toHaveBeenCalledWith('/tasks/1', { title: 'Updated' });
    });

    it('updateStatus повинен оновлювати статус завдання', async () => {
        apiClient.patch.mockResolvedValueOnce({ data: {} });

        await taskService.updateStatus('1', 2);

        expect(apiClient.patch).toHaveBeenCalledWith('/tasks/1/status', { status: 2 });
    });

    it('delete повинен видаляти завдання', async () => {
        apiClient.delete.mockResolvedValueOnce({ data: {} });

        await taskService.delete('1');

        expect(apiClient.delete).toHaveBeenCalledWith('/tasks/1');
    });
});
