export const TaskCard = ({ task, onToggleStatus }) => {
  const isDone = task.status === 2; // Assuming 2 is Done based on Enum
  
  // Format priority based on enum (0: Low, 1: Medium, 2: High)
  const priorityMap = {
    0: 'Low',
    1: 'Medium',
    2: 'High'
  };
  const priorityText = priorityMap[task.priority] || 'Medium';

  return (
    <div className={`task-card ${isDone ? 'done' : ''}`}>
      <button 
        className={`task-checkbox ${isDone ? 'done' : ''}`}
        onClick={() => onToggleStatus(task)}
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3">
          <path d="M5 13l4 4L19 7" strokeLinecap="round" strokeLinejoin="round" />
        </svg>
      </button>
      
      <div className="task-content">
        <h3 className="task-title">{task.title}</h3>
        
        <div className="task-meta">
          <span className={`badge priority-${priorityText}`}>
            {priorityText} Priority
          </span>
          
          {task.deadline && (
            <span className="task-deadline">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <circle cx="12" cy="12" r="10" />
                <path d="M12 6v6l4 2" strokeLinecap="round" strokeLinejoin="round" />
              </svg>
              {new Date(task.deadline).toLocaleDateString()}
            </span>
          )}
        </div>
      </div>
    </div>
  );
};
