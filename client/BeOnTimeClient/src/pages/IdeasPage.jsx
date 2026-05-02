import { useState, useEffect, useRef } from 'react';
import { DashboardLayout } from '../components/DashboardLayout';
import { IdeaCard } from '../components/IdeaCard';
import { ideaService } from '../services/ideaService';
import './Ideas.css';

export const IdeasPage = () => {
  const [ideas, setIdeas] = useState([]);
  const [loading, setLoading] = useState(true);
  
  // Quick Input State
  const [isInputExpanded, setIsInputExpanded] = useState(false);
  const [newTitle, setNewTitle] = useState('');
  const [newDesc, setNewDesc] = useState('');
  const [isSaving, setIsSaving] = useState(false);
  
  const inputContainerRef = useRef(null);

  const fetchIdeas = async () => {
    setLoading(true);
    try {
      const data = await ideaService.getAll();
      setIdeas(data || []);
    } catch (error) {
      console.warn("Backend unavailable, using mock data for ideas page");
      setIdeas([
        { id: '1', title: 'New landing page concept', description: 'Use more animations on scroll. Maybe a parallax effect for the hero section.' },
        { id: '2', title: 'Blog post ideas', description: '- Time management techniques\n- How to use Kanban effectively\n- Pomodoro for developers' },
        { id: '3', title: 'Refactor AuthContext', description: 'Need to clean up the error handling logic' },
        { id: '4', title: 'Marketing campaign', description: 'Target students before exam week' },
        { id: '5', title: 'Fix mobile layout', description: '' },
      ]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchIdeas();
    
    // Click outside handler to collapse input
    const handleClickOutside = (event) => {
      if (inputContainerRef.current && !inputContainerRef.current.contains(event.target)) {
        if (!newTitle.trim() && !newDesc.trim()) {
          setIsInputExpanded(false);
        }
      }
    };
    
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [newTitle, newDesc]);

  const handleSaveIdea = async () => {
    if (!newTitle.trim()) {
      setIsInputExpanded(false);
      return;
    }
    
    setIsSaving(true);
    const newIdeaObj = { title: newTitle.trim(), description: newDesc.trim() || null };
    
    try {
      const createdIdea = await ideaService.create(newIdeaObj);
      setIdeas(prev => [createdIdea, ...prev]);
    } catch (error) {
      console.warn("Failed to save idea to backend, adding locally");
      setIdeas(prev => [{ ...newIdeaObj, id: Date.now().toString() }, ...prev]);
    } finally {
      setIsSaving(false);
      setNewTitle('');
      setNewDesc('');
      setIsInputExpanded(false);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Enter' && e.ctrlKey) {
      handleSaveIdea();
    }
  };

  const handleDeleteIdea = async (id) => {
    if (!window.confirm('Видалити цю ідею?')) return;
    try {
      await ideaService.delete(id);
      setIdeas(prev => prev.filter(i => i.id !== id));
    } catch (error) {
      console.warn("Failed to delete from backend, applying locally");
      setIdeas(prev => prev.filter(i => i.id !== id));
    }
  };

  const handleConvertToTask = async (idea) => {
    try {
      await ideaService.convertToTask(idea.id);
      // Remove from ideas list (it moved to tasks)
      setIdeas(prev => prev.filter(i => i.id !== idea.id));
      // Optionally show a toast notification here
      alert(`Ідея "${idea.title}" перетворена на завдання! Шукайте її на дошці Tasks.`);
    } catch (error) {
      console.warn("Conversion failed, applying mock conversion");
      setIdeas(prev => prev.filter(i => i.id !== idea.id));
      alert(`[МOК] Ідея "${idea.title}" перетворена на завдання!`);
    }
  };

  return (
    <DashboardLayout>
      <div className="ideas-page">
        <div className="ideas-header">
          <h1>Твої Ідеї</h1>
          <p>Записуй думки, щоб нічого не забути. Згодом перетвори їх на завдання.</p>
        </div>

        <div className="quick-input-container" ref={inputContainerRef}>
          <div className={`quick-input-box ${isInputExpanded ? 'focused' : ''}`}>
            <input
              type="text"
              className="quick-input-title"
              placeholder="Нотатка..."
              value={newTitle}
              onChange={(e) => setNewTitle(e.target.value)}
              onFocus={() => setIsInputExpanded(true)}
              onKeyDown={handleKeyDown}
              disabled={isSaving}
            />
            
            {isInputExpanded && (
              <>
                <textarea
                  className="quick-input-desc"
                  placeholder="Додайте деталі... (Ctrl + Enter щоб зберегти)"
                  value={newDesc}
                  onChange={(e) => setNewDesc(e.target.value)}
                  onKeyDown={handleKeyDown}
                  disabled={isSaving}
                  autoFocus
                />
                <div className="quick-input-actions">
                  <button 
                    className="btn-save-idea" 
                    onClick={handleSaveIdea}
                    disabled={!newTitle.trim() || isSaving}
                  >
                    {isSaving ? 'Збереження...' : 'Зберегти'}
                  </button>
                </div>
              </>
            )}
          </div>
        </div>

        <div style={{ marginTop: '48px' }}>
          {loading ? (
            <p style={{ textAlign: 'center', color: 'var(--ink-60)' }}>Завантаження ідей...</p>
          ) : ideas.length > 0 ? (
            <div className="ideas-masonry">
              {ideas.map(idea => (
                <IdeaCard 
                  key={idea.id} 
                  idea={idea} 
                  onDelete={handleDeleteIdea}
                  onConvert={handleConvertToTask}
                />
              ))}
            </div>
          ) : (
            <div style={{ textAlign: 'center', marginTop: '64px', color: 'var(--ink-60)' }}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" width="48" height="48" style={{ marginBottom: '16px', opacity: 0.5 }}>
                <path d="M9 21H15M12 18V21M12 3C8.68629 3 6 5.68629 6 9C6 11.0827 7.0583 12.9069 8.65342 14C9.44498 14.5422 10 15.4206 10 16.4V18H14V16.4C14 15.4206 14.555 14.5422 15.3466 14C16.9417 12.9069 18 11.0827 18 9C18 5.68629 15.3137 3 12 3Z" strokeLinecap="round" strokeLinejoin="round"/>
              </svg>
              <p>Немає ідей. Напишіть щось зверху!</p>
            </div>
          )}
        </div>
      </div>
    </DashboardLayout>
  );
};
