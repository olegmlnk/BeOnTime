import { useState, useEffect } from 'react';

export const TaskModal = ({ isOpen, onClose, onSave, initialData = null }) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [deadline, setDeadline] = useState('');
  const [priority, setPriority] = useState('1'); // 1 = Medium
  const [status, setStatus] = useState('0'); // 0 = Todo

  useEffect(() => {
    if (initialData) {
      setTitle(initialData.title || '');
      setDescription(initialData.description || '');
      setDeadline(initialData.deadline ? new Date(initialData.deadline).toISOString().split('T')[0] : '');
      setPriority(initialData.priority !== undefined ? String(initialData.priority) : '1');
      setStatus(initialData.status !== undefined ? String(initialData.status) : '0');
    } else {
      setTitle('');
      setDescription('');
      setDeadline('');
      setPriority('1');
      setStatus('0');
    }
  }, [initialData, isOpen]);

  if (!isOpen) return null;

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!title.trim()) return;

    onSave({
      id: initialData?.id,
      title: title.trim(),
      description: description.trim() || null,
      deadline: deadline ? new Date(deadline).toISOString() : null,
      priority: parseInt(priority, 10),
      status: parseInt(status, 10)
    });
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{initialData ? 'Редагувати завдання' : 'Нове завдання'}</h2>
          <button className="close-btn" onClick={onClose}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="24" height="24">
              <path d="M18 6L6 18M6 6l12 12" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </button>
        </div>

        <form className="modal-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="taskTitle">Назва завдання</label>
            <input 
              id="taskTitle"
              type="text" 
              className="form-input" 
              placeholder="Що потрібно зробити?" 
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              autoFocus
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="taskDesc">Опис (необов'язково)</label>
            <textarea 
              id="taskDesc"
              className="form-textarea" 
              placeholder="Додайте деталі..." 
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </div>

          <div className="form-group">
            <label htmlFor="taskDeadline">Дедлайн (необов'язково)</label>
            <input 
              id="taskDeadline"
              type="date" 
              className="form-input" 
              value={deadline}
              onChange={(e) => setDeadline(e.target.value)}
            />
          </div>

          <div style={{ display: 'flex', gap: '16px' }}>
            <div className="form-group" style={{ flex: 1 }}>
              <label htmlFor="taskPriority">Пріоритет</label>
              <select 
                id="taskPriority"
                className="form-select" 
                value={priority}
                onChange={(e) => setPriority(e.target.value)}
              >
                <option value="0">Low</option>
                <option value="1">Medium</option>
                <option value="2">High</option>
              </select>
            </div>

            <div className="form-group" style={{ flex: 1 }}>
              <label htmlFor="taskStatus">Статус</label>
              <select 
                id="taskStatus"
                className="form-select" 
                value={status}
                onChange={(e) => setStatus(e.target.value)}
              >
                <option value="0">To Do</option>
                <option value="1">In Progress</option>
                <option value="2">Done</option>
                <option value="3">Cancelled</option>
              </select>
            </div>
          </div>

          <div className="modal-footer">
            <button type="button" className="btn-secondary" onClick={onClose}>Скасувати</button>
            <button type="submit" className="btn-primary" disabled={!title.trim()}>
              <span>{initialData ? 'Зберегти' : 'Створити'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
