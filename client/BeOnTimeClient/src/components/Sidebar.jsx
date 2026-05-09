import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { ThemeToggle } from './ThemeToggle';
import './Dashboard.css';

export const Sidebar = () => {
  const { logout } = useAuth();
  const location = useLocation();

  const navItems = [
    { 
      name: 'Dashboard', 
      path: '/',
      icon: (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
          <rect x="3" y="3" width="7" height="7" rx="1" strokeLinecap="round" strokeLinejoin="round"/>
          <rect x="14" y="3" width="7" height="4" rx="1" strokeLinecap="round" strokeLinejoin="round"/>
          <rect x="14" y="11" width="7" height="10" rx="1" strokeLinecap="round" strokeLinejoin="round"/>
          <rect x="3" y="14" width="7" height="7" rx="1" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
      )
    },
    { 
      name: 'Завдання', 
      path: '/tasks',
      icon: (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
          <path d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2M9 5h6M9 14l2 2 4-4" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
      )
    },
    { 
      name: 'Ідеї', 
      path: '/ideas',
      icon: (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
          <path d="M9 21H15M12 18V21M12 3C8.68629 3 6 5.68629 6 9C6 11.0827 7.0583 12.9069 8.65342 14C9.44498 14.5422 10 15.4206 10 16.4V18H14V16.4C14 15.4206 14.555 14.5422 15.3466 14C16.9417 12.9069 18 11.0827 18 9C18 5.68629 15.3137 3 12 3Z" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
      )
    },
    { 
      name: 'Роадмапи', 
      path: '/roadmaps',
      icon: (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
          <path d="M3 3v18h18" strokeLinecap="round" strokeLinejoin="round"/>
          <path d="M7 16l4-8 4 4 5-9" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
      )
    },
    { 
      name: 'Профіль', 
      path: '/profile',
      icon: (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
          <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" strokeLinecap="round" strokeLinejoin="round"/>
          <circle cx="12" cy="7" r="4"/>
        </svg>
      )
    },
  ];

  return (
    <aside className="sidebar">
      <div className="brand">
        <div className="brand-icon">
          <svg viewBox="0 0 22 22" fill="none">
            <circle cx="11" cy="11" r="9" stroke="currentColor" strokeWidth="2" />
            <line x1="11" y1="5" x2="11" y2="11.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
            <line x1="11" y1="11.5" x2="14.5" y2="14.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
            <circle cx="11" cy="11" r="1.5" fill="currentColor" />
          </svg>
        </div>
        <span className="brand-name">BeOnTime</span>
        <ThemeToggle className="brand-theme-toggle" />
      </div>

      <nav className="sidebar-nav">
        {navItems.map((item) => (
          <Link
            key={item.name}
            to={item.path}
            className={`nav-item ${location.pathname === item.path || (item.path !== '/' && location.pathname.startsWith(item.path)) ? 'active' : ''}`}
          >
            {item.icon}
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
