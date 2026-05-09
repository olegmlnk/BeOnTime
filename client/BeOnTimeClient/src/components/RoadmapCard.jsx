import { useNavigate } from 'react-router-dom';

export const RoadmapCard = ({ roadmap, onDelete }) => {
  const navigate = useNavigate();

  const formatDate = (dateStr) => {
    if (!dateStr) return null;
    return new Date(dateStr).toLocaleDateString('uk-UA', { day: 'numeric', month: 'short', year: 'numeric' });
  };

  const progress = roadmap.progressPercent ?? 0;

  return (
    <div className="roadmap-card" onClick={() => navigate(`/roadmaps/${roadmap.id}`)}>
      <div className="roadmap-card-header">
        <h3 className="roadmap-card-title">{roadmap.name}</h3>
        {onDelete && (
          <button 
            className="card-action-btn delete" 
            onClick={(e) => { e.stopPropagation(); onDelete(roadmap.id); }}
            title="Видалити"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
              <path d="M3 6h18M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2M10 11v6M14 11v6" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </button>
        )}
      </div>

      {roadmap.description && (
        <p className="roadmap-card-desc">{roadmap.description}</p>
      )}

      <div className="roadmap-progress-section">
        <div className="roadmap-progress-bar-bg">
          <div 
            className="roadmap-progress-bar-fill" 
            style={{ width: `${progress}%` }}
          />
        </div>
        <div className="roadmap-progress-info">
          <span className="roadmap-progress-text">{progress}%</span>
          <span className="roadmap-task-count">{roadmap.doneTasks ?? 0} / {roadmap.totalTasks ?? 0} завдань</span>
        </div>
      </div>

      <div className="roadmap-card-footer">
        {roadmap.startDate && (
          <span className="roadmap-date">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="14" height="14">
              <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/>
            </svg>
            {formatDate(roadmap.startDate)}
          </span>
        )}
        {roadmap.targetDate && (
          <span className="roadmap-date target">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="14" height="14">
              <path d="M5 12h14M12 5l7 7-7 7" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
            {formatDate(roadmap.targetDate)}
          </span>
        )}
      </div>
    </div>
  );
};
