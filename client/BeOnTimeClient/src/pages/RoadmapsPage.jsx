import { useState, useEffect } from 'react';
import { DashboardLayout } from '../components/DashboardLayout';
import { RoadmapCard } from '../components/RoadmapCard';
import { RoadmapModal } from '../components/RoadmapModal';
import { roadmapService } from '../services/roadmapService';
import './Roadmaps.css';

export const RoadmapsPage = () => {
  const [roadmaps, setRoadmaps] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRoadmap, setEditingRoadmap] = useState(null);

  const fetchRoadmaps = async () => {
    setLoading(true);
    try {
      const data = await roadmapService.getAll();
      setRoadmaps(data || []);
    } catch (error) {
      console.warn("Backend unavailable, using mock data for roadmaps page");
      setRoadmaps([
        {
          id: 'mock-1',
          name: 'Вивчити React',
          description: 'Повний шлях від основ до просунутих концепцій React.',
          startDate: '2026-04-01T00:00:00',
          targetDate: '2026-06-30T00:00:00',
          totalTasks: 8,
          doneTasks: 5,
          progressPercent: 63,
        },
        {
          id: 'mock-2',
          name: 'Підготовка до іспитів',
          description: 'Розклад підготовки по всіх предметах.',
          startDate: '2026-05-01T00:00:00',
          targetDate: '2026-05-25T00:00:00',
          totalTasks: 12,
          doneTasks: 3,
          progressPercent: 25,
        },
        {
          id: 'mock-3',
          name: 'Фітнес план',
          description: null,
          startDate: null,
          targetDate: '2026-08-01T00:00:00',
          totalTasks: 4,
          doneTasks: 0,
          progressPercent: 0,
        },
      ]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoadmaps();
  }, []);

  const handleSaveRoadmap = async (data) => {
    try {
      if (data.id && !data.id.startsWith('mock-')) {
        const updated = await roadmapService.update(data.id, data);
        setRoadmaps(prev => prev.map(r => r.id === data.id ? updated : r));
      } else {
        const created = await roadmapService.create(data);
        setRoadmaps(prev => [...prev, created]);
      }
    } catch (error) {
      console.warn("Failed to save to backend, applying locally");
      if (data.id) {
        setRoadmaps(prev => prev.map(r => r.id === data.id ? { ...r, ...data } : r));
      } else {
        setRoadmaps(prev => [...prev, {
          ...data,
          id: `mock-${Date.now()}`,
          totalTasks: 0,
          doneTasks: 0,
          progressPercent: 0,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        }]);
      }
    }
    closeModal();
  };

  const handleDeleteRoadmap = async (id) => {
    if (!window.confirm('Ви впевнені, що хочете видалити цей роадмап?')) return;
    try {
      await roadmapService.delete(id);
      setRoadmaps(prev => prev.filter(r => r.id !== id));
    } catch (error) {
      console.warn("Failed to delete from backend, applying locally");
      setRoadmaps(prev => prev.filter(r => r.id !== id));
    }
  };

  const openModal = (roadmap = null) => {
    setEditingRoadmap(roadmap);
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingRoadmap(null);
  };

  return (
    <DashboardLayout>
      <div className="roadmaps-page">
        <div className="roadmaps-header">
          <h1>Роадмапи</h1>
          <button className="btn-new-roadmap" onClick={() => openModal()}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 4v16m8-8H4" />
            </svg>
            Новий роадмап
          </button>
        </div>

        {loading ? (
          <p style={{ textAlign: 'center', color: 'var(--ink-60)' }}>Завантаження роадмапів...</p>
        ) : roadmaps.length > 0 ? (
          <div className="roadmaps-grid">
            {roadmaps.map(roadmap => (
              <RoadmapCard
                key={roadmap.id}
                roadmap={roadmap}
                onDelete={handleDeleteRoadmap}
              />
            ))}
          </div>
        ) : (
          <div className="roadmaps-empty">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" width="56" height="56">
              <path d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2M9 5h6M9 14l2 2 4-4" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
            <p>Немає роадмапів. Створіть свій перший план!</p>
            <button
              className="btn-new-roadmap"
              style={{ margin: '24px auto 0' }}
              onClick={() => openModal()}
            >
              Створити роадмап
            </button>
          </div>
        )}
      </div>

      <RoadmapModal
        isOpen={isModalOpen}
        onClose={closeModal}
        onSave={handleSaveRoadmap}
        initialData={editingRoadmap}
      />
    </DashboardLayout>
  );
};
