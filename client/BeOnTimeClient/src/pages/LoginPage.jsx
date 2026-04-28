import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { AuthLayout } from '../components/AuthLayout';
import './AuthPages.css';

export const LoginPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    
    if (!email || !password) {
      setError('Будь ласка, заповніть усі поля.');
      return;
    }

    setLoading(true);

    try {
      await login(email, password);
      setSuccess('Вхід успішний! Перенаправлення...');
      setTimeout(() => navigate('/'), 1200);
    } catch (err) {
      setError(err.response?.data?.error || 'Помилка входу. Перевірте дані.');
    } finally {
      setLoading(false);
    }
  };

  const togglePass = () => setShowPassword(!showPassword);

  const EYE_OPEN = (
    <svg viewBox="0 0 16 16" fill="none">
      <path d="M1 8s2.5-5 7-5 7 5 7 5-2.5 5-7 5-7-5-7-5z" stroke="currentColor" strokeWidth="1.5"/>
      <circle cx="8" cy="8" r="2" stroke="currentColor" strokeWidth="1.5"/>
    </svg>
  );

  const EYE_CLOSE = (
    <svg viewBox="0 0 16 16" fill="none">
      <path d="M2 2l12 12M6.5 6.6A2 2 0 0011 9.5M1 8s2-4.5 7-4.5c.76 0 1.48.1 2.15.27M9.5 11.8C8.4 12.4 7.2 12.5 6 12.5c-5 0-6.5-4.5-6.5-4.5" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round"/>
    </svg>
  );

  return (
    <AuthLayout>
      <form className="form-panel active" onSubmit={handleSubmit} noValidate>
        <div className="form-heading">
          <h2>З поверненням</h2>
          <p>Раді бачити вас знову. Введіть дані для входу.</p>
        </div>

        {(error || success) && (
          <div className={`msg ${error ? 'error' : 'success'}`}>
            <svg viewBox="0 0 16 16" fill="none">
              <circle cx="8" cy="8" r="6" stroke="currentColor" strokeWidth="1.5" />
              <path d="M8 5v3M8 10.5v.5" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
            </svg>
            <span>{error || success}</span>
          </div>
        )}

        <div className="field">
          <label htmlFor="loginEmail">Електронна пошта</label>
          <div className="input-wrap">
            <svg className="input-icon" viewBox="0 0 16 16" fill="none">
              <rect x="1.5" y="3.5" width="13" height="9" rx="1.5" stroke="currentColor" strokeWidth="1.5" />
              <path d="M1.5 5.5l6.5 4 6.5-4" stroke="currentColor" strokeWidth="1.5" strokeLinejoin="round" />
            </svg>
            <input
              type="email"
              id="loginEmail"
              placeholder="you@example.com"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={error && !email ? 'input-error' : ''}
              disabled={loading}
            />
          </div>
        </div>

        <div className="field">
          <div className="forgot-row">
            <label htmlFor="loginPass">Пароль</label>
            <a href="#" className="forgot-link" onClick={(e) => e.preventDefault()}>Забули пароль?</a>
          </div>
          <div className="input-wrap">
            <svg className="input-icon" viewBox="0 0 16 16" fill="none">
              <rect x="3" y="7" width="10" height="7" rx="1.5" stroke="currentColor" strokeWidth="1.5" />
              <path d="M5 7V5a3 3 0 016 0v2" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
              <circle cx="8" cy="10.5" r="1" fill="currentColor" />
            </svg>
            <input
              type={showPassword ? 'text' : 'password'}
              id="loginPass"
              placeholder="••••••••"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className={error && !password ? 'input-error' : ''}
              disabled={loading}
            />
            <button
              type="button"
              className="eye-btn"
              onClick={togglePass}
              aria-label="Показати пароль"
            >
              {showPassword ? EYE_CLOSE : EYE_OPEN}
            </button>
          </div>
        </div>

        <div className="checkbox-row">
          <input type="checkbox" id="rememberMe" />
          <label htmlFor="rememberMe">Запам'ятати мене на цьому пристрої</label>
        </div>

        <button type="submit" className="btn-primary" disabled={loading}>
          <span>{loading ? 'Входимо...' : 'Увійти'}</span>
          <svg viewBox="0 0 18 18" fill="none">
            <path d="M3 9h12M10 5l4 4-4 4" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        </button>

        <div className="switch-link">
          Немає акаунту? <Link to="/register">Зареєструватись</Link>
        </div>
      </form>
    </AuthLayout>
  );
};
