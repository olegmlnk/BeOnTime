import { useState, useEffect } from 'react';

export const RoadmapModal = ({ isOpen, onClose, onSave, initialData = null }) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [startDate, setStartDate] = useState('');
  const [targetDate, setTargetDate] = useState('');

  useEffect(() => {
    if (initialData) {
      setName(initialData.name || '');
      setDescription(initialData.description || '');
      setStartDate(initialData.startDate ? new Date(initialData.startDate).toISOString().split('T')[0] : '');
      setTargetDate(initialData.targetDate ? new Date(initialData.targetDate).toISOString().split('T')[0] : '');
    } else {
      setName('');
      setDescription('');
      setStartDate('');
      setTargetDate('');
    }
  }, [initialData, isOpen]);

  if (!isOpen) return null;

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!name.trim()) return;

    onSave({
      id: initialData?.id,
      name: name.trim(),
      description: description.trim() || null,
      startDate: startDate ? new Date(startDate).toISOString() : null,
      targetDate: targetDate ? new Date(targetDate).toISOString() : null,
    });
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{initialData ? 'Редагувати роадмап' : 'Новий роадмап'}</h2>
          <button className="close-btn" onClick={onClose}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="24" height="24">
              <path d="M18 6L6 18M6 6l12 12" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </button>
        </div>

        <form className="modal-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="roadmapName">Назва роадмапу</label>
            <input 
              id="roadmapName"
              type="text" 
              className="form-input" 
              placeholder="Наприклад: Вивчити React" 
              value={name}
              onChange={(e) => setName(e.target.value)}
              autoFocus
              required
              maxLength={200}
            />
          </div>

          <div className="form-group">
            <label htmlFor="roadmapDesc">Опис (необов'язково)</label>
            <textarea 
              id="roadmapDesc"
              className="form-textarea" 
              placeholder="Додайте деталі про ваш план..." 
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              maxLength={2000}
            />
          </div>

          <div style={{ display: 'flex', gap: '16px' }}>
            <div className="form-group" style={{ flex: 1 }}>
              <label htmlFor="roadmapStart">Дата початку</label>
              <input 
                id="roadmapStart"
                type="date" 
                className="form-input" 
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
              />
            </div>

            <div className="form-group" style={{ flex: 1 }}>
              <label htmlFor="roadmapTarget">Цільова дата</label>
              <input 
                id="roadmapTarget"
                type="date" 
                className="form-input" 
                value={targetDate}
                onChange={(e) => setTargetDate(e.target.value)}
              />
            </div>
          </div>

          <div className="modal-footer">
            <button type="button" className="btn-secondary" onClick={onClose}>Скасувати</button>
            <button type="submit" className="btn-primary" disabled={!name.trim()}>
              <span>{initialData ? 'Зберегти' : 'Створити'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
