import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import './AuthPages.css';

export const RegisterPage = () => {
  const [userName, setUserName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    
    if (password.length < 8) {
      setError('Password must be at least 8 characters long');
      return;
    }

    setLoading(true);

    try {
      await register(userName, email, password);
      // Wait for 1 second to show success, maybe redirect to login or dashboard directly.
      // Usually register returns token as well in our backend!
      navigate('/');
    } catch (err) {
      setError(err.response?.data?.error || 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-layout">
        <div className="auth-form-section">
          <div className="auth-card">
            <h2 className="auth-title">Create Account</h2>
            <p className="auth-subtitle">Join BeOnTime today</p>
            
            {error && <div className="auth-error">{error}</div>}
            
            <form onSubmit={handleSubmit} className="auth-form">
              <div className="form-group">
                <label htmlFor="userName">Username</label>
                <input
                  type="text"
                  id="userName"
                  value={userName}
                  onChange={(e) => setUserName(e.target.value)}
                  placeholder="Choose a username"
                  required
                  minLength={3}
                  maxLength={55}
                  disabled={loading}
                />
              </div>

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
                  placeholder="Create a password (min 8 chars)"
                  required
                  minLength={8}
                  disabled={loading}
                />
              </div>
              
              <button type="submit" className="auth-btn" disabled={loading}>
                {loading ? 'Creating account...' : 'Sign Up'}
              </button>
            </form>
            
            <div className="auth-footer">
              Already have an account? <Link to="/login">Sign in</Link>
            </div>
          </div>
        </div>

        <div className="auth-info-section">
          <div className="auth-info-content">
            <h1>Start Your Journey<br/>With Us.</h1>
            <p>Join thousands of users who have transformed the way they work and live. Experience seamless productivity starting today.</p>
          </div>
        </div>
      </div>
    </div>
  );
};
