import { useNavigate, useLocation } from 'react-router-dom';
import { ThemeToggle } from './ThemeToggle';
import '../pages/AuthPages.css';

export const AuthLayout = ({ children }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const isLogin = location.pathname === '/login';

  return (
    <div className="auth-container">
      {/* ── LEFT ── */}
      <div className="panel-left">
        <div className="deco-circles">
          <div className="deco-c" style={{ width: '200px', height: '200px', top: '30%', left: '-60px', borderColor: 'rgba(108,92,231,0.12)' }}></div>
          <div className="deco-c" style={{ width: '120px', height: '120px', top: '55%', right: '20px', borderColor: 'rgba(0,206,201,0.15)' }}></div>
          <div className="deco-c" style={{ width: '60px', height: '60px', top: '20%', right: '60px', borderColor: 'rgba(108,92,231,0.1)' }}></div>
        </div>

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
        </div>

        <div className="panel-hero">
          <h1>Плануй<br /><em>розумніше.</em><br />Живи більше.</h1>
          <p>Інструмент для тих, хто цінує кожну хвилину. Організуйте задачі, встановлюйте пріоритети та досягайте цілей.</p>
        </div>

        <div className="features">
          <div className="feature-item">
            <div className="feature-dot">
              <svg viewBox="0 0 16 16" fill="none">
                <rect x="2" y="3" width="12" height="11" rx="2" stroke="currentColor" strokeWidth="1.5" />
                <path d="M5 1v3M11 1v3" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
                <path d="M5 8h6M5 11h4" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
              </svg>
            </div>
            <div className="feature-text">
              <strong>Розумний планер</strong>
              Щоденник, тижневий і місячний огляд задач
            </div>
          </div>

          <div className="feature-item">
            <div className="feature-dot">
              <svg viewBox="0 0 16 16" fill="none">
                <circle cx="8" cy="8" r="5.5" stroke="currentColor" strokeWidth="1.5" />
                <path d="M8 5v3.5l2.5 1.5" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
              </svg>
            </div>
            <div className="feature-text">
              <strong>Таймер Помодоро</strong>
              Фокус-сесії та нагадування вбудовано
            </div>
          </div>

          <div className="feature-item">
            <div className="feature-dot">
              <svg viewBox="0 0 16 16" fill="none">
                <path d="M3 8l3 3 7-7" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
              </svg>
            </div>
            <div className="feature-text">
              <strong>Прогрес та аналітика</strong>
              Відстежуйте продуктивність у реальному часі
            </div>
          </div>
        </div>

        <div className="panel-tagline">
          <div className="clock-ring"></div>
          <span>Час — єдиний ресурс,<br />який не відновлюється</span>
        </div>
      </div>

      {/* ── RIGHT ── */}
      <div className="panel-right">
        <div className="auth-top-bar">
          <ThemeToggle />
        </div>
        <div className="auth-card">
          <div className="tab-switcher">
            <button
              className={`tab-btn ${isLogin ? 'active' : ''}`}
              onClick={() => navigate('/login')}
            >
              Вхід
            </button>
            <button
              className={`tab-btn ${!isLogin ? 'active' : ''}`}
              onClick={() => navigate('/register')}
            >
              Реєстрація
            </button>
          </div>

          {children}
        </div>
      </div>
    </div>
  );
};
