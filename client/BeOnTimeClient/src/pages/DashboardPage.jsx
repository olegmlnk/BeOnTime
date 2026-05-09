import { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import { DashboardLayout } from '../components/DashboardLayout';
import { taskService } from '../services/taskService';
import { ideaService } from '../services/ideaService';
import { roadmapService } from '../services/roadmapService';

export const DashboardPage = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [allTasks, setAllTasks] = useState([]);
  const [ideas, setIdeas] = useState([]);
  const [roadmaps, setRoadmaps] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isMockData, setIsMockData] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [tasksData, ideasData, roadmapsData] = await Promise.all([
          taskService.getAll(),
          ideaService.getAll(),
          roadmapService.getAll(),
        ]);
        setAllTasks(tasksData || []);
        setIdeas(ideasData || []);
        setRoadmaps(roadmapsData || []);
        setIsMockData(false);
      } catch (error) {
        console.warn('Backend unavailable, using mock data.');
        setIsMockData(true);
        setAllTasks([
          { id: '1', title: 'Finish Quarterly Report', deadline: new Date(Date.now() + 86400000).toISOString(), status: 0, priority: 2 },
          { id: '2', title: 'Team Meeting Prep', deadline: new Date(Date.now() + 172800000).toISOString(), status: 0, priority: 1 },
          { id: '3', title: 'Review PRs', status: 2, priority: 0 },
          { id: '4', title: 'Update documentation', status: 1, priority: 1 },
        ]);
        setIdeas([
          { id: '1', title: 'New landing concept', content: 'Use more animations' },
          { id: '2', title: 'Blog post ideas', content: 'Time management tips' },
        ]);
        setRoadmaps([
          { id: '1', name: 'Вивчити React', progressPercent: 60, totalTasks: 5, doneTasks: 3 },
        ]);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  const handleToggleTask = async (task) => {
    const newStatus = task.status === 2 ? 0 : 2;
    setAllTasks(prev => prev.map(t => t.id === task.id ? { ...t, status: newStatus } : t));
    try {
      if (isMockData) return;
      await taskService.updateStatus(task.id, newStatus);
    } catch (err) {
      console.error('Failed to update task status', err);
      setAllTasks(prev => prev.map(t => t.id === task.id ? { ...t, status: task.status } : t));
    }
  };

  // Greeting based on time of day
  const getGreeting = () => {
    const hour = new Date().getHours();
    if (hour < 12) return 'Доброго ранку';
    if (hour < 18) return 'Доброго дня';
    return 'Доброго вечора';
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    
    if (d.toDateString() === today.toDateString()) return 'Сьогодні';
    if (d.toDateString() === tomorrow.toDateString()) return 'Завтра';
    return d.toLocaleDateString('uk-UA', { day: 'numeric', month: 'short' });
  };

  const getPriorityInfo = (priority) => {
    switch (priority) {
      case 2: return { label: 'Високий', color: 'var(--error)', bg: 'rgba(185, 64, 64, 0.1)' };
      case 1: return { label: 'Середній', color: 'var(--gold)', bg: 'rgba(200, 149, 60, 0.15)' };
      case 0: return { label: 'Низький', color: 'var(--teal)', bg: 'rgba(42, 107, 107, 0.1)' };
      default: return { label: '', color: 'var(--ink-60)', bg: 'var(--ink-08)' };
    }
  };

  if (loading) {
    return (
      <DashboardLayout>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '60vh' }}>
          <p style={{ color: 'var(--ink-60)', fontSize: '15px' }}>Завантаження...</p>
        </div>
      </DashboardLayout>
    );
  }

  // Stats
  const todoTasks = allTasks.filter(t => t.status === 0);
  const inProgressTasks = allTasks.filter(t => t.status === 1);
  const doneTasks = allTasks.filter(t => t.status === 2);
  const totalTasks = allTasks.length;
  const activeTasks = [...todoTasks, ...inProgressTasks].sort((a, b) => (b.priority || 0) - (a.priority || 0));

  return (
    <DashboardLayout>
      {/* Header */}
      <header className="dash-header">
        <div>
          <h1 className="dash-greeting">{getGreeting()}, {user?.name?.split(' ')[0] || 'Користувач'}!</h1>
          <p className="dash-subtitle">
            {new Date().toLocaleDateString('uk-UA', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })}
          </p>
        </div>
      </header>

      {/* Stats Cards */}
      <div className="dash-stats">
        <div className="stat-card" onClick={() => navigate('/tasks')}>
          <div className="stat-icon" style={{ background: 'rgba(200, 149, 60, 0.12)' }}>
            <svg viewBox="0 0 24 24" fill="none" stroke="var(--gold)" strokeWidth="2" width="22" height="22">
              <rect x="3" y="3" width="18" height="18" rx="2" />
              <path d="M3 9h18M9 21V9" strokeLinecap="round" />
            </svg>
          </div>
          <div className="stat-info">
            <span className="stat-number">{totalTasks}</span>
            <span className="stat-label">Усього завдань</span>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon" style={{ background: 'rgba(42, 107, 107, 0.1)' }}>
            <svg viewBox="0 0 24 24" fill="none" stroke="var(--teal)" strokeWidth="2" width="22" height="22">
              <path d="M12 22c5.523 0 10-4.477 10-10S17.523 2 12 2 2 6.477 2 12s4.477 10 10 10z" />
              <path d="M12 6v6l4 2" strokeLinecap="round" />
            </svg>
          </div>
          <div className="stat-info">
            <span className="stat-number">{todoTasks.length + inProgressTasks.length}</span>
            <span className="stat-label">Активних</span>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon" style={{ background: 'rgba(45, 122, 79, 0.1)' }}>
            <svg viewBox="0 0 24 24" fill="none" stroke="var(--success, #2d7a4f)" strokeWidth="2" width="22" height="22">
              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" strokeLinecap="round" />
              <path d="M22 4L12 14.01l-3-3" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </div>
          <div className="stat-info">
            <span className="stat-number">{doneTasks.length}</span>
            <span className="stat-label">Виконано</span>
          </div>
        </div>

        <div className="stat-card" onClick={() => navigate('/ideas')}>
          <div className="stat-icon" style={{ background: 'rgba(200, 149, 60, 0.08)' }}>
            <svg viewBox="0 0 24 24" fill="none" stroke="var(--gold)" strokeWidth="2" width="22" height="22">
              <path d="M9 21H15M12 18V21M12 3C8.68629 3 6 5.68629 6 9C6 11.0827 7.0583 12.9069 8.65342 14C9.44498 14.5422 10 15.4206 10 16.4V18H14V16.4C14 15.4206 14.555 14.5422 15.3466 14C16.9417 12.9069 18 11.0827 18 9C18 5.68629 15.3137 3 12 3Z" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </div>
          <div className="stat-info">
            <span className="stat-number">{ideas.length}</span>
            <span className="stat-label">Ідей</span>
          </div>
        </div>
      </div>

      {/* Main Grid */}
      <div className="dash-grid">
        {/* Active Tasks */}
        <div className="dash-section">
          <div className="dash-section-header">
            <h2>Активні завдання</h2>
            <a href="/tasks" className="dash-view-all">Переглянути всі →</a>
          </div>
          
          <div className="dash-task-list">
            {activeTasks.length > 0 ? (
              activeTasks.slice(0, 6).map(task => {
                const pri = getPriorityInfo(task.priority);
                const isDone = task.status === 2;
                return (
                  <div key={task.id} className={`dash-task-card${isDone ? ' done' : ''}`}>
                    <button
                      className={`dash-task-check${isDone ? ' checked' : ''}`}
                      onClick={() => handleToggleTask(task)}
                      title={isDone ? 'Повернути' : 'Позначити виконаним'}
                    >
                      {isDone && (
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" width="14" height="14">
                          <path d="M20 6L9 17l-5-5" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                      )}
                    </button>
                    <div className="dash-task-body">
                      <div className="dash-task-title">{task.title}</div>
                      <div className="dash-task-meta">
                        <span className="dash-task-priority" style={{ color: pri.color, background: pri.bg }}>
                          {pri.label}
                        </span>
                        {task.status === 1 && (
                          <span className="dash-task-status-badge">В процесі</span>
                        )}
                        {task.deadline && (
                          <span className="dash-task-deadline">
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="12" height="12">
                              <rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/>
                            </svg>
                            {formatDate(task.deadline)}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                );
              })
            ) : (
              <div className="dash-empty">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" width="40" height="40" style={{ opacity: 0.3 }}>
                  <path d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2M9 5h6M9 14l2 2 4-4" strokeLinecap="round" strokeLinejoin="round"/>
                </svg>
                <p>Немає активних завдань</p>
                <button className="dash-add-btn" onClick={() => navigate('/tasks')}>Створити завдання</button>
              </div>
            )}
          </div>
        </div>

        {/* Sidebar: Roadmaps + Ideas */}
        <div className="dash-sidebar-col">
          {/* Roadmaps Progress */}
          {roadmaps.length > 0 && (
            <div className="dash-widget">
              <div className="dash-section-header">
                <h2>Роадмапи</h2>
                <a href="/roadmaps" className="dash-view-all">Всі →</a>
              </div>
              <div className="dash-roadmap-list">
                {roadmaps.slice(0, 3).map(rm => (
                  <div key={rm.id} className="dash-roadmap-card" onClick={() => navigate(`/roadmaps/${rm.id}`)}>
                    <div className="dash-roadmap-name">{rm.name}</div>
                    <div className="dash-roadmap-progress">
                      <div className="dash-progress-bar">
                        <div className="dash-progress-fill" style={{ width: `${rm.progressPercent || 0}%` }} />
                      </div>
                      <span className="dash-progress-text">{rm.progressPercent || 0}%</span>
                    </div>
                    <div className="dash-roadmap-stats">{rm.doneTasks || 0} / {rm.totalTasks || 0} завдань</div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Recent Ideas */}
          <div className="dash-widget">
            <div className="dash-section-header">
              <h2>Останні ідеї</h2>
              <a href="/ideas" className="dash-view-all">Всі →</a>
            </div>
            <div className="dash-ideas-list">
              {ideas.length > 0 ? (
                ideas.slice(0, 4).map(idea => (
                  <div key={idea.id} className="dash-idea-card">
                    <h4>{idea.title}</h4>
                    {idea.content && idea.content.trim() && <p>{idea.content}</p>}
                  </div>
                ))
              ) : (
                <p className="dash-empty-text">Немає ідей</p>
              )}
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
};
