import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import './Dashboard.css';

export const Sidebar = () => {
  const { logout } = useAuth();
  const location = useLocation();

  const navItems = [
    { name: 'Dashboard', path: '/' },
    { name: 'Tasks', path: '/tasks' },
    { name: 'Ideas', path: '/ideas' },
    { name: 'Roadmaps', path: '/roadmaps' },
    { name: 'Settings', path: '/settings' },
  ];

  return (
    <aside className="sidebar">
      <div className="brand">
        <div className="brand-icon">
          <svg viewBox="0 0 22 22" fill="none">
            <circle cx="11" cy="11" r="9" stroke="#0f0e0c" strokeWidth="2" />
            <line x1="11" y1="5" x2="11" y2="11.5" stroke="#0f0e0c" strokeWidth="2" strokeLinecap="round" />
            <line x1="11" y1="11.5" x2="14.5" y2="14.5" stroke="#0f0e0c" strokeWidth="2" strokeLinecap="round" />
            <circle cx="11" cy="11" r="1.5" fill="#0f0e0c" />
          </svg>
        </div>
        <span className="brand-name">BeOnTime</span>
      </div>

      <nav className="sidebar-nav">
        {navItems.map((item) => (
          <Link
            key={item.name}
            to={item.path}
            className={`nav-item ${location.pathname === item.path ? 'active' : ''}`}
          >
            {item.name}
          </Link>
        ))}
      </nav>

      <div className="sidebar-footer">
        <button className="logout-btn" onClick={logout}>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path strokeLinecap="round" strokeLinejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
          Вихід
        </button>
      </div>
    </aside>
  );
};
