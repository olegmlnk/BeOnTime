export const IdeaCard = ({ idea, onConvert, onDelete }) => {
  return (
    <div className="sticker-card">
      <h4 className="sticker-title">{idea.title}</h4>
      {idea.content && <p className="sticker-desc">{idea.content}</p>}
      
      {(onConvert || onDelete) && (
        <div className="sticker-actions">
          {onConvert && (
            <button 
              className="btn-icon convert" 
              onClick={() => onConvert(idea)}
              title="Перетворити на завдання"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                <path d="M5 12h14M12 5l7 7-7 7" strokeLinecap="round" strokeLinejoin="round"/>
              </svg>
            </button>
          )}
          {onDelete && (
            <button 
              className="btn-icon delete" 
              onClick={() => onDelete(idea.id)}
              title="Видалити"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                <path d="M3 6h18M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2M10 11v6M14 11v6" strokeLinecap="round" strokeLinejoin="round"/>
              </svg>
            </button>
          )}
        </div>
      )}
    </div>
  );
};
