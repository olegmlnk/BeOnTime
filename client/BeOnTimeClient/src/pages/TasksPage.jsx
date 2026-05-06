import { useState, useEffect } from 'react';
import { DashboardLayout } from '../components/DashboardLayout';
import { TaskModal } from '../components/TaskModal';
import { taskService } from '../services/taskService';
import './Tasks.css';

export const TasksPage = () => {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isMockData, setIsMockData] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState(null);

  // Columns definition mapping to TaskItemStatus enum
  const COLUMNS = [
    { id: 0, title: 'To Do' },
    { id: 1, title: 'In Progress' },
    { id: 2, title: 'Done' }
  ];

  const fetchTasks = async () => {
    setLoading(true);
    try {
      const data = await taskService.getAll();
      setTasks(data || []);
      setIsMockData(false);
    } catch (error) {
      console.warn("Backend unavailable, using mock data for tasks page");
      setIsMockData(true);
      setTasks([
        { id: 'b2c3d4e5-0001-4000-8000-000000000001', title: 'Finish Quarterly Report', description: 'Include Q3 metrics', status: 0, priority: 2 },
        { id: 'b2c3d4e5-0001-4000-8000-000000000002', title: 'Update homepage design', description: 'Use new color tokens', status: 1, priority: 1 },
        { id: 'b2c3d4e5-0001-4000-8000-000000000003', title: 'Review PRs', status: 2, priority: 0 },
        { id: 'b2c3d4e5-0001-4000-8000-000000000004', title: 'Team Sync', status: 0, priority: 1 },
      ]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTasks();
  }, []);

  const handleSaveTask = async (taskData) => {
    try {
      if (isMockData) throw new Error('mock mode');
      if (taskData.id) {
        // Edit existing
        await taskService.update(taskData.id, taskData);
        setTasks(prev => prev.map(t => t.id === taskData.id ? { ...t, ...taskData } : t));
      } else {
        // Create new
        const newTask = await taskService.create(taskData);
        setTasks(prev => [...prev, newTask]);
      }
    } catch (error) {
      if (!isMockData) console.warn("Failed to save to backend, applying locally");
      if (taskData.id) {
        setTasks(prev => prev.map(t => t.id === taskData.id ? { ...t, ...taskData } : t));
      } else {
        setTasks(prev => [...prev, { ...taskData, id: Date.now().toString() }]);
      }
    }
    closeModal();
  };

  const handleDeleteTask = async (id) => {
    if (!window.confirm('Ви впевнені, що хочете видалити це завдання?')) return;
    try {
      if (isMockData) throw new Error('mock mode');
      await taskService.delete(id);
      setTasks(prev => prev.filter(t => t.id !== id));
    } catch (error) {
      if (!isMockData) console.warn("Failed to delete from backend, applying locally");
      setTasks(prev => prev.filter(t => t.id !== id));
    }
  };

  const handleDragStart = (e, taskId) => {
    e.dataTransfer.setData('taskId', taskId);
  };

  const handleDrop = async (e, newStatus) => {
    e.preventDefault();
    const taskId = e.dataTransfer.getData('taskId');
    if (!taskId) return;

    const task = tasks.find(t => t.id === taskId);
    if (!task || task.status === newStatus) return;

    // Optimistic UI update
    setTasks(prev => prev.map(t => t.id === taskId ? { ...t, status: newStatus } : t));

    try {
      if (isMockData) return; // skip backend for mock data
      await taskService.updateStatus(taskId, newStatus);
    } catch (error) {
      console.warn("Status update failed, reverting...");
      setTasks(prev => prev.map(t => t.id === taskId ? { ...t, status: task.status } : t));
    }
  };

  const handleDragOver = (e) => {
    e.preventDefault(); // necessary to allow dropping
  };

  const openModal = (task = null) => {
    setEditingTask(task);
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingTask(null);
  };

  const getPriorityColor = (priority) => {
    switch (priority) {
      case 2: return 'var(--error)';
      case 1: return 'var(--gold)';
      case 0: return 'var(--teal)';
      default: return 'var(--ink-60)';
    }
  };

  return (
    <DashboardLayout>
      <div className="tasks-page">
        <div className="tasks-header">
          <h1>Всі завдання</h1>
          <button className="btn-new-task" onClick={() => openModal()}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 4v16m8-8H4" />
            </svg>
            Нове завдання
          </button>
        </div>

        <div className="kanban-board">
          {COLUMNS.map(column => {
            const columnTasks = tasks.filter(t => t.status === column.id);
            return (
              <div 
                key={column.id} 
                className="kanban-column"
                onDrop={(e) => handleDrop(e, column.id)}
                onDragOver={handleDragOver}
              >
                <div className="column-header">
                  <span>{column.title}</span>
                  <span className="column-count">{columnTasks.length}</span>
                </div>
                
                <div className="kanban-column-content">
                  {columnTasks.map(task => (
                    <div 
                      key={task.id} 
                      className="kanban-card"
                      draggable
                      onDragStart={(e) => handleDragStart(e, task.id)}
                    >
                      <div className="card-actions">
                        <button className="card-action-btn" onClick={() => openModal(task)} title="Редагувати">
                          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                          </svg>
                        </button>
                        <button className="card-action-btn delete" onClick={() => handleDeleteTask(task.id)} title="Видалити">
                          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                            <path d="M3 6h18M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2M10 11v6M14 11v6" />
                          </svg>
                        </button>
                      </div>
                      
                      <div className="kanban-card-title">{task.title}</div>
                      {task.description && <div className="kanban-card-desc">{task.description}</div>}
                      
                      <div className="kanban-card-footer">
                        <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                          <div style={{ width: '8px', height: '8px', borderRadius: '50%', backgroundColor: getPriorityColor(task.priority) }} />
                          <span style={{ fontSize: '12px', color: 'var(--ink-60)', fontWeight: '500' }}>
                            {task.priority === 2 ? 'High' : task.priority === 1 ? 'Med' : 'Low'}
                          </span>
                        </div>
                        {task.deadline && (
                          <span style={{ fontSize: '12px', color: 'var(--ink-60)' }}>
                            {new Date(task.deadline).toLocaleDateString('uk-UA')}
                          </span>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            );
          })}
        </div>
      </div>

      <TaskModal 
        isOpen={isModalOpen} 
        onClose={closeModal} 
        onSave={handleSaveTask}
        initialData={editingTask}
      />
    </DashboardLayout>
  );
};
