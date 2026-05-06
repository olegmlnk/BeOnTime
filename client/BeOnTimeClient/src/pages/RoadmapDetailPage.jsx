import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { DashboardLayout } from '../components/DashboardLayout';
import { RoadmapModal } from '../components/RoadmapModal';
import { roadmapService } from '../services/roadmapService';
import './RoadmapDetail.css';

export const RoadmapDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [roadmap, setRoadmap] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [dragIdx, setDragIdx] = useState(null);
  const [dragOverIdx, setDragOverIdx] = useState(null);

  const fetchRoadmap = async () => {
    setLoading(true);
    try {
      const data = await roadmapService.getById(id);
      setRoadmap(data);
    } catch (error) {
      console.warn("Backend unavailable, using mock data for roadmap detail");
      setRoadmap({
        id,
        name: 'Вивчити React',
        description: 'Повний шлях від основ до просунутих концепцій React. Включає вивчення хуків, контексту, маршрутизації та state management.',
        startDate: '2026-04-01T00:00:00',
        targetDate: '2026-06-30T00:00:00',
        totalTasks: 5,
        doneTasks: 3,
        progressPercent: 60,
        tasks: [
          { id: 'mt-1', title: 'Вивчити JSX та компоненти', status: 2, priority: 2, deadline: '2026-04-10T00:00:00', orderInRoadmap: 0 },
          { id: 'mt-2', title: 'React Hooks (useState, useEffect)', status: 2, priority: 2, deadline: '2026-04-20T00:00:00', orderInRoadmap: 1 },
          { id: 'mt-3', title: 'React Router DOM', status: 2, priority: 1, deadline: '2026-05-01T00:00:00', orderInRoadmap: 2 },
          { id: 'mt-4', title: 'Context API та useReducer', status: 1, priority: 1, deadline: '2026-05-15T00:00:00', orderInRoadmap: 3 },
          { id: 'mt-5', title: 'Побудувати фінальний проект', status: 0, priority: 0, deadline: '2026-06-15T00:00:00', orderInRoadmap: 4 },
        ],
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoadmap();
  }, [id]);

  const handleEditSave = async (data) => {
    try {
      const updated = await roadmapService.update(id, data);
      setRoadmap(prev => ({ ...prev, ...updated }));
    } catch (error) {
      console.warn("Failed to update, applying locally");
      setRoadmap(prev => ({ ...prev, ...data }));
    }
    setIsEditModalOpen(false);
  };

  const handleDelete = async () => {
    if (!window.confirm('Ви впевнені, що хочете видалити цей роадмап?')) return;
    try {
      await roadmapService.delete(id);
    } catch (error) {
      console.warn("Failed to delete from backend");
    }
    navigate('/roadmaps');
  };

  // Drag & Drop Reorder
  const handleDragStart = (idx) => {
    setDragIdx(idx);
  };

  const handleDragOver = (e, idx) => {
    e.preventDefault();
    if (idx !== dragOverIdx) setDragOverIdx(idx);
  };

  const handleDrop = async (e, dropIdx) => {
    e.preventDefault();
    if (dragIdx === null || dragIdx === dropIdx) {
      setDragIdx(null);
      setDragOverIdx(null);
      return;
    }

    const tasks = [...roadmap.tasks];
    const [moved] = tasks.splice(dragIdx, 1);
    tasks.splice(dropIdx, 0, moved);

    // Update order
    const reorderedTasks = tasks.map((t, i) => ({ ...t, orderInRoadmap: i }));
    setRoadmap(prev => ({ ...prev, tasks: reorderedTasks }));
    setDragIdx(null);
    setDragOverIdx(null);

    try {
      await roadmapService.reorder(id, reorderedTasks.map(t => t.id));
    } catch (error) {
      console.warn("Failed to save reorder to backend");
    }
  };

  const handleDragEnd = () => {
    setDragIdx(null);
    setDragOverIdx(null);
  };

  const getStatusInfo = (status) => {
    switch (status) {
      case 0: return { label: 'To Do', className: 'todo' };
      case 1: return { label: 'In Progress', className: 'in-progress' };
      case 2: return { label: 'Done', className: 'done' };
      case 3: return { label: 'Cancelled', className: 'cancelled' };
      default: return { label: 'Unknown', className: 'todo' };
    }
  };

  const getPriorityColor = (priority) => {
    switch (priority) {
      case 2: return 'var(--error)';
      case 1: return 'var(--gold)';
      case 0: return 'var(--teal)';
      default: return 'var(--ink-60)';
    }
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return null;
    return new Date(dateStr).toLocaleDateString('uk-UA', { day: 'numeric', month: 'short', year: 'numeric' });
  };

  if (loading) {
    return (
      <DashboardLayout>
        <p style={{ textAlign: 'center', color: 'var(--ink-60)', paddingTop: '80px' }}>Завантаження...</p>
      </DashboardLayout>
    );
  }

  if (!roadmap) {
    return (
      <DashboardLayout>
        <p style={{ textAlign: 'center', color: 'var(--ink-60)', paddingTop: '80px' }}>Роадмап не знайдено.</p>
      </DashboardLayout>
    );
  }

  const progress = roadmap.progressPercent ?? 0;
  const tasks = roadmap.tasks || [];

  return (
    <DashboardLayout>
      <div className="roadmap-detail">
        <div className="roadmap-detail-top">
          <button className="btn-back" onClick={() => navigate('/roadmaps')} title="Назад до роадмапів">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
              <path d="M19 12H5M12 19l-7-7 7-7" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </button>
          <h1 style={{ fontSize: '14px', color: 'var(--ink-60)', fontWeight: 500 }}>Роадмапи</h1>
        </div>

        <div className="roadmap-detail-header">
          <div className="roadmap-detail-title-row">
            <h2 className="roadmap-detail-title">{roadmap.name}</h2>
            <div className="roadmap-detail-actions">
              <button className="btn-edit-roadmap" onClick={() => setIsEditModalOpen(true)}>
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                  <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                  <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
                </svg>
                Редагувати
              </button>
              <button className="btn-delete-roadmap" onClick={handleDelete}>
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                  <path d="M3 6h18M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2M10 11v6M14 11v6" strokeLinecap="round" strokeLinejoin="round"/>
                </svg>
                Видалити
              </button>
            </div>
          </div>

          {roadmap.description && (
            <p className="roadmap-detail-desc">{roadmap.description}</p>
          )}

          <div className="roadmap-detail-meta">
            {roadmap.startDate && (
              <div className="roadmap-meta-item">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                  <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/>
                </svg>
                Початок: {formatDate(roadmap.startDate)}
              </div>
            )}
            {roadmap.targetDate && (
              <div className="roadmap-meta-item">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                  <path d="M5 12h14M12 5l7 7-7 7" strokeLinecap="round" strokeLinejoin="round"/>
                </svg>
                Ціль: {formatDate(roadmap.targetDate)}
              </div>
            )}
            <div className="roadmap-meta-item">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                <path d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2M9 5h6" strokeLinecap="round" strokeLinejoin="round"/>
              </svg>
              {roadmap.doneTasks ?? 0} / {roadmap.totalTasks ?? 0} завдань
            </div>
          </div>

          <div className="roadmap-detail-progress">
            <div className="roadmap-progress-bar-bg">
              <div className="roadmap-progress-bar-fill" style={{ width: `${progress}%` }} />
            </div>
            <span className="roadmap-detail-progress-text">{progress}%</span>
          </div>
        </div>

        <div className="roadmap-tasks-section">
          <h2>Завдання ({tasks.length})</h2>

          {tasks.length > 0 ? (
            <div className="roadmap-task-list">
              {tasks.map((task, idx) => {
                const statusInfo = getStatusInfo(task.status);
                return (
                  <div
                    key={task.id}
                    className={`roadmap-task-item${dragIdx === idx ? ' dragging' : ''}${dragOverIdx === idx ? ' drag-over' : ''}`}
                    draggable
                    onDragStart={() => handleDragStart(idx)}
                    onDragOver={(e) => handleDragOver(e, idx)}
                    onDrop={(e) => handleDrop(e, idx)}
                    onDragEnd={handleDragEnd}
                  >
                    <div className="drag-handle" title="Перетягніть для зміни порядку">
                      <span /><span /><span />
                    </div>

                    <span className="roadmap-task-order">{idx + 1}</span>

                    <div className="roadmap-task-info">
                      <div className={`roadmap-task-title${task.status === 2 ? ' done' : ''}`}>
                        {task.title}
                      </div>
                      <div className="roadmap-task-badges">
                        <span className={`status-badge ${statusInfo.className}`}>{statusInfo.label}</span>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                          <div style={{ width: '6px', height: '6px', borderRadius: '50%', backgroundColor: getPriorityColor(task.priority) }} />
                          <span style={{ fontSize: '11px', color: 'var(--ink-60)', fontWeight: 500 }}>
                            {task.priority === 2 ? 'High' : task.priority === 1 ? 'Med' : 'Low'}
                          </span>
                        </div>
                        {task.deadline && (
                          <span style={{ fontSize: '11px', color: 'var(--ink-60)' }}>
                            {formatDate(task.deadline)}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          ) : (
            <div className="roadmap-tasks-empty">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" width="48" height="48" style={{ opacity: 0.4 }}>
                <path d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2M9 5h6M9 14l2 2 4-4" strokeLinecap="round" strokeLinejoin="round"/>
              </svg>
              <p>У цьому роадмапі ще немає завдань. Додайте завдання через сторінку Tasks.</p>
            </div>
          )}
        </div>
      </div>

      <RoadmapModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSave={handleEditSave}
        initialData={roadmap}
      />
    </DashboardLayout>
  );
};
