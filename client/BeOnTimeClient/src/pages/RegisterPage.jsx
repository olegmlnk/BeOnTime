import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { AuthLayout } from '../components/AuthLayout';
import './AuthPages.css';

export const RegisterPage = () => {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [terms, setTerms] = useState(false);
  
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const { register } = useAuth();
  const navigate = useNavigate();

  const getStrengthScore = (val) => {
    let score = 0;
    if (!val) return score;
    if (val.length >= 8) score++;
    if (/[A-Z]/.test(val)) score++;
    if (/[0-9]/.test(val)) score++;
    if (/[^A-Za-z0-9]/.test(val)) score++;
    return score;
  };

  const strengthScore = getStrengthScore(password);
  const levels = ['', 'Слабкий', 'Нормальний', 'Хороший', 'Надійний'];
  const strengthClasses = ['', 's1', 's2', 's3', 's3'];

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!firstName || !lastName || !email || !password || !confirmPassword) {
      setError('Будь ласка, заповніть усі поля.');
      return;
    }

    if (password.length < 8) {
      setError('Пароль повинен містити щонайменше 8 символів.');
      return;
    }

    if (password !== confirmPassword) {
      setError('Паролі не збігаються. Спробуйте ще раз.');
      return;
    }

    if (!terms) {
      setError('Підтвердьте згоду з умовами використання.');
      return;
    }

    setLoading(true);

    try {
      const userName = `${firstName} ${lastName}`.trim();
      await register(userName, email, password);
      
      setSuccess('Акаунт створено! Перенаправлення...');
      setTimeout(() => navigate('/'), 1400);
    } catch (err) {
      let errorMessage = 'Помилка реєстрації. Спробуйте ще раз.';
      if (err.response?.data) {
        if (err.response.data.error) {
          errorMessage = err.response.data.error;
        } else if (err.response.data.errors) {
          const firstErrorKey = Object.keys(err.response.data.errors)[0];
          errorMessage = err.response.data.errors[firstErrorKey][0];
        } else if (err.response.data.title) {
        errorMessage = err.response.data.title;
        } else if (typeof err.response.data === 'string') {
          errorMessage = err.response.data.includes('<html') 
            ? 'Сервер не відповідає (можливо, бекенд вимкнений).' 
            : err.response.data;
        }
      } else if (err.message) {
        errorMessage = `Помилка: ${err.message}`;
      }
      console.error('Registration Error:', err);
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

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
          <h2>Створити акаунт</h2>
          <p>Приєднайтесь до BeOnTime і почніть планувати ефективніше.</p>
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

        <div className="field-row">
          <div className="field">
            <label htmlFor="regFirstName">Ім'я</label>
            <div className="input-wrap">
              <svg className="input-icon" viewBox="0 0 16 16" fill="none">
                <circle cx="8" cy="5.5" r="2.5" stroke="currentColor" strokeWidth="1.5" />
                <path d="M2.5 13.5c0-2.485 2.462-4.5 5.5-4.5s5.5 2.015 5.5 4.5" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
              </svg>
              <input
                type="text"
                id="regFirstName"
                placeholder="Ім'я"
                autoComplete="given-name"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                disabled={loading}
                className={error && !firstName ? 'input-error' : ''}
              />
            </div>
          </div>

          <div className="field">
            <label htmlFor="regLastName">Прізвище</label>
            <div className="input-wrap">
              <svg className="input-icon" viewBox="0 0 16 16" fill="none">
                <circle cx="8" cy="5.5" r="2.5" stroke="currentColor" strokeWidth="1.5" />
                <path d="M2.5 13.5c0-2.485 2.462-4.5 5.5-4.5s5.5 2.015 5.5 4.5" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
              </svg>
              <input
                type="text"
                id="regLastName"
                placeholder="Прізвище"
                autoComplete="family-name"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                disabled={loading}
                className={error && !lastName ? 'input-error' : ''}
              />
            </div>
          </div>
        </div>

        <div className="field">
          <label htmlFor="regEmail">Електронна пошта</label>
          <div className="input-wrap">
            <svg className="input-icon" viewBox="0 0 16 16" fill="none">
              <rect x="1.5" y="3.5" width="13" height="9" rx="1.5" stroke="currentColor" strokeWidth="1.5" />
              <path d="M1.5 5.5l6.5 4 6.5-4" stroke="currentColor" strokeWidth="1.5" strokeLinejoin="round" />
            </svg>
            <input
              type="email"
              id="regEmail"
              placeholder="you@example.com"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={loading}
              className={error && !email ? 'input-error' : ''}
            />
          </div>
        </div>

        <div className="field">
          <label htmlFor="regPass">Пароль</label>
          <div className="input-wrap">
            <svg className="input-icon" viewBox="0 0 16 16" fill="none">
              <rect x="3" y="7" width="10" height="7" rx="1.5" stroke="currentColor" strokeWidth="1.5" />
              <path d="M5 7V5a3 3 0 016 0v2" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
              <circle cx="8" cy="10.5" r="1" fill="currentColor" />
            </svg>
            <input
              type={showPassword ? 'text' : 'password'}
              id="regPass"
              placeholder="Мінімум 8 символів"
              autoComplete="new-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={loading}
              className={error && !password ? 'input-error' : ''}
            />
            <button
              type="button"
              className="eye-btn"
              onClick={() => setShowPassword(!showPassword)}
              aria-label="Показати пароль"
            >
              {showPassword ? EYE_CLOSE : EYE_OPEN}
            </button>
          </div>
          <div className="strength-bar">
            {[1, 2, 3, 4].map((i) => (
              <div 
                key={i} 
                className={`strength-seg ${strengthScore >= i ? strengthClasses[strengthScore] : ''}`} 
              />
            ))}
          </div>
          <div className="strength-label">
            {levels[strengthScore] || 'Введіть пароль'}
          </div>
        </div>

        <div className="field">
          <label htmlFor="regPassConfirm">Підтвердіть пароль</label>
          <div className="input-wrap">
            <svg className="input-icon" viewBox="0 0 16 16" fill="none">
              <rect x="3" y="7" width="10" height="7" rx="1.5" stroke="currentColor" strokeWidth="1.5" />
              <path d="M5 7V5a3 3 0 016 0v2" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
              <circle cx="8" cy="10.5" r="1" fill="currentColor" />
            </svg>
            <input
              type={showConfirmPassword ? 'text' : 'password'}
              id="regPassConfirm"
              placeholder="Повторіть пароль"
              autoComplete="new-password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              disabled={loading}
              className={error && password !== confirmPassword ? 'input-error' : ''}
            />
            <button
              type="button"
              className="eye-btn"
              onClick={() => setShowConfirmPassword(!showConfirmPassword)}
              aria-label="Показати пароль"
            >
              {showConfirmPassword ? EYE_CLOSE : EYE_OPEN}
            </button>
          </div>
        </div>

        <div className="checkbox-row">
          <input 
            type="checkbox" 
            id="terms" 
            checked={terms}
            onChange={(e) => setTerms(e.target.checked)}
            disabled={loading}
          />
          <label htmlFor="terms">Погоджуюсь з <a href="#" onClick={(e) => e.preventDefault()}>Умовами використання</a> та <a href="#" onClick={(e) => e.preventDefault()}>Політикою конфіденційності</a></label>
        </div>

        <button type="submit" className="btn-primary" disabled={loading}>
          <span>{loading ? 'Створюємо акаунт...' : 'Створити акаунт'}</span>
          <svg viewBox="0 0 18 18" fill="none">
            <path d="M9 3v12M3 9h12" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
          </svg>
        </button>

        <div className="switch-link">
          Вже є акаунт? <Link to="/login">Увійти</Link>
        </div>
      </form>
    </AuthLayout>
  );
};
