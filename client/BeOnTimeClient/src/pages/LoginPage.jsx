import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import './AuthPages.css';

export const LoginPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(email, password);
      navigate('/'); // Redirect to dashboard or home
    } catch (err) {
      setError(err.response?.data?.error || 'Failed to login. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-layout">
        <div className="auth-form-section">
          <div className="auth-card">
            <h2 className="auth-title">Welcome Back</h2>
            <p className="auth-subtitle">Sign in to your account</p>
            
            {error && <div className="auth-error">{error}</div>}
            
            <form onSubmit={handleSubmit} className="auth-form">
              <div className="form-group">
                <label htmlFor="email">Email</label>
                <input
                  type="email"
                  id="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="Enter your email"
                  required
                  disabled={loading}
                />
              </div>
              
              <div className="form-group">
                <label htmlFor="password">Password</label>
                <input
                  type="password"
                  id="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="Enter your password"
                  required
                  disabled={loading}
                />
              </div>
              
              <button type="submit" className="auth-btn" disabled={loading}>
                {loading ? 'Signing in...' : 'Sign In'}
              </button>
            </form>
            
            <div className="auth-footer">
              Don't have an account? <Link to="/register">Sign up here</Link>
            </div>
          </div>
        </div>
        
        <div className="auth-info-section">
          <div className="auth-info-content">
            <h1>Stay Organized.<br/>Achieve More.</h1>
            <p>Welcome to BeOnTime! The smartest way to manage your schedules, track your tasks, and stay on top of your daily goals without feeling overwhelmed.</p>
          </div>
        </div>
      </div>
    </div>
  );
};
