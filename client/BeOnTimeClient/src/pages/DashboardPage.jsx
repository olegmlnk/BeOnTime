import { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { DashboardLayout } from '../components/DashboardLayout';
import { TaskCard } from '../components/TaskCard';
import { IdeaCard } from '../components/IdeaCard';
import { taskService } from '../services/taskService';
import { ideaService } from '../services/ideaService';

export const DashboardPage = () => {
  const { user } = useAuth();
  const [tasks, setTasks] = useState([]);
  const [ideas, setIdeas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isMockData, setIsMockData] = useState(false);

  // Function to load actual data from API or set fallback
  useEffect(() => {
    const fetchData = async () => {
      try {
        // Try to fetch actual tasks and ideas
        // If the backend isn't fully ready, this will fallback to mock data in the catch block
        const tasksData = await taskService.getUpcoming(7);
        const ideasData = await ideaService.getAll();
        
        setTasks(tasksData || []);
        setIdeas(ideasData || []);
        setIsMockData(false);
      } catch (error) {
        console.warn('Backend API not reachable or returned error, using mock data for UI layout display.');
        setIsMockData(true);
        
        // Mock data matching backend entities for UI demonstration
        setTasks([
          { id: 'c3d4e5f6-0001-4000-8000-000000000001', title: 'Finish Quarterly Report', deadline: new Date(Date.now() + 86400000).toISOString(), status: 0, priority: 2 },
          { id: 'c3d4e5f6-0001-4000-8000-000000000002', title: 'Team Meeting Prep', deadline: new Date(Date.now() + 172800000).toISOString(), status: 0, priority: 1 },
          { id: 'c3d4e5f6-0001-4000-8000-000000000003', title: 'Review PRs', deadline: new Date().toISOString(), status: 2, priority: 0 },
        ]);
        
        setIdeas([
          { id: 'c3d4e5f6-0002-4000-8000-000000000001', title: 'New landing page concept', description: 'Use more animations on scroll' },
          { id: 'c3d4e5f6-0002-4000-8000-000000000002', title: 'Blog post ideas', description: 'Write about time management techniques' },
        ]);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleToggleTask = async (task) => {
    const newStatus = task.status === 2 ? 0 : 2; // Toggle between Todo(0) and Done(2)
    
    // Optimistic UI update
    setTasks(prev => prev.map(t => t.id === task.id ? { ...t, status: newStatus } : t));
    
    try {
      if (isMockData) return; // skip backend for mock data
      await taskService.updateStatus(task.id, newStatus);
    } catch (err) {
      // Revert if failed
      console.error('Failed to update task status', err);
      setTasks(prev => prev.map(t => t.id === task.id ? { ...t, status: task.status } : t));
    }
  };

  const handleAddIdea = async (e) => {
    if (e.key === 'Enter' && e.target.value.trim()) {
      const newTitle = e.target.value.trim();
      e.target.value = ''; // clear input
      
      try {
        if (isMockData) throw new Error('mock mode');
        const newIdea = await ideaService.create({ title: newTitle });
        setIdeas([newIdea, ...ideas]);
      } catch (err) {
        if (!isMockData) console.warn('Failed to save idea to backend, adding mock to UI', err);
        setIdeas([{ id: crypto.randomUUID(), title: newTitle }, ...ideas]);
      }
    }
  };

  if (loading) return null; // Or a sleek loader

  // Splitting tasks based on status for the UI
  const pendingTasks = tasks.filter(t => t.status !== 2);
  const doneTasks = tasks.filter(t => t.status === 2);

  return (
    <DashboardLayout>
      <header className="dashboard-header">
        <div className="header-greeting">
          <h1>Good morning, {user?.name?.split(' ')[0] || 'User'}!</h1>
          <p>Here's what's on your plate for today, {new Date().toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })}</p>
        </div>
        
        <button className="btn-pomodoro">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="12" cy="12" r="10" />
            <path d="M12 6v6l4 2" strokeLinecap="round" strokeLinejoin="round" />
            <path d="M12 2v2" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
          Start Session
        </button>
      </header>

      <div className="dashboard-grid">
        <div className="main-column">
          <div className="section-header">
            <h2>Upcoming Tasks</h2>
            <a href="/tasks" className="view-all">View all</a>
          </div>
          
          <div className="tasks-list">
            {pendingTasks.length > 0 ? (
              pendingTasks.map(task => (
                <TaskCard key={task.id} task={task} onToggleStatus={handleToggleTask} />
              ))
            ) : (
              <p style={{ color: 'var(--ink-60)', fontStyle: 'italic' }}>No upcoming tasks. You're all caught up!</p>
            )}
            
            {doneTasks.length > 0 && (
              <div style={{ marginTop: '32px' }}>
                <h3 style={{ fontSize: '16px', color: 'var(--ink-60)', marginBottom: '16px' }}>Completed</h3>
                {doneTasks.map(task => (
                  <TaskCard key={task.id} task={task} onToggleStatus={handleToggleTask} />
                ))}
              </div>
            )}
          </div>
        </div>
        
        <div className="side-column">
          <div className="ideas-widget">
            <div className="section-header" style={{ marginBottom: '16px' }}>
              <h2>Quick Ideas</h2>
              <svg viewBox="0 0 24 24" fill="none" stroke="var(--gold)" strokeWidth="2" style={{ width: '20px', height: '20px' }}>
                <path d="M9 21H15M12 18V21M12 3C8.68629 3 6 5.68629 6 9C6 11.0827 7.0583 12.9069 8.65342 14C9.44498 14.5422 10 15.4206 10 16.4V18H14V16.4C14 15.4206 14.555 14.5422 15.3466 14C16.9417 12.9069 18 11.0827 18 9C18 5.68629 15.3137 3 12 3Z" strokeLinecap="round" strokeLinejoin="round"/>
              </svg>
            </div>
            
            <div className="ideas-list">
              {ideas.length > 0 ? (
                ideas.slice(0, 5).map(idea => (
                  <IdeaCard key={idea.id} idea={idea} />
                ))
              ) : (
                <p style={{ color: 'var(--ink-60)', fontStyle: 'italic', fontSize: '13px' }}>No ideas yet.</p>
              )}
            </div>
            
            <input 
              type="text" 
              className="add-idea-input" 
              placeholder="+ Jot down an idea and hit Enter..." 
              onKeyDown={handleAddIdea}
            />
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
};
