import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { DashboardLayout } from '../components/DashboardLayout';
import { useAuth } from '../hooks/useAuth';
import { profileService } from '../services/profileService';
import './Profile.css';

export const ProfilePage = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState(null);

  // Editable fields
  const [editUsername, setEditUsername] = useState('');
  const [editEmail, setEditEmail] = useState('');
  const [emailPassword, setEmailPassword] = useState('');
  
  // Password change
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  // Delete account
  const [deletePassword, setDeletePassword] = useState('');
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

  useEffect(() => {
    const fetchProfile = async () => {
      setLoading(true);
      try {
        const data = await profileService.getProfile();
        setProfile(data);
        setEditUsername(data.userName || '');
        setEditEmail(data.email || '');
      } catch (error) {
        console.warn("Backend unavailable, using mock profile");
        const mockProfile = {
          id: user?.id || '123',
          userName: user?.name || 'User',
          email: user?.email || 'user@example.com',
          role: 0,
          createdAt: '2026-03-15T10:00:00',
        };
        setProfile(mockProfile);
        setEditUsername(mockProfile.userName);
        setEditEmail(mockProfile.email);
      } finally {
        setLoading(false);
      }
    };
    fetchProfile();
  }, [user]);

  const showMsg = (text, type = 'success') => {
    setMessage({ text, type });
    setTimeout(() => setMessage(null), 4000);
  };

  const handleUpdateUsername = async () => {
    if (!editUsername.trim() || editUsername === profile.userName) return;
    try {
      const result = await profileService.updateUsername(editUsername.trim());
      setProfile(prev => ({ ...prev, userName: editUsername.trim() }));
      showMsg("Ім'я користувача оновлено!");
    } catch (error) {
      showMsg(error.response?.data?.error || 'Помилка оновлення імені', 'error');
    }
  };

  const handleUpdateEmail = async () => {
    if (!editEmail.trim() || !emailPassword || editEmail === profile.email) return;
    try {
      await profileService.updateEmail(editEmail.trim(), emailPassword);
      setProfile(prev => ({ ...prev, email: editEmail.trim() }));
      setEmailPassword('');
      showMsg('Email оновлено!');
    } catch (error) {
      showMsg(error.response?.data?.error || 'Помилка оновлення email', 'error');
    }
  };

  const handleChangePassword = async (e) => {
    e.preventDefault();
    if (!currentPassword || !newPassword || !confirmPassword) return;
    if (newPassword !== confirmPassword) {
      showMsg('Паролі не співпадають', 'error');
      return;
    }
    try {
      await profileService.changePassword(currentPassword, newPassword, confirmPassword);
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
      showMsg('Пароль змінено успішно!');
    } catch (error) {
      showMsg(error.response?.data?.error || 'Помилка зміни паролю', 'error');
    }
  };

  const handleDeleteAccount = async () => {
    if (!deletePassword) return;
    try {
      await profileService.deleteAccount(deletePassword);
      logout();
      navigate('/login');
    } catch (error) {
      showMsg(error.response?.data?.error || 'Помилка видалення акаунту', 'error');
    }
  };

  const getInitials = (name) => {
    if (!name) return '?';
    return name.split(' ').map(n => n[0]).join('').slice(0, 2);
  };

  const getRoleLabel = (role) => {
    switch(role) {
      case 0: return 'User';
      case 1: return 'Admin';
      default: return 'User';
    }
  };

  if (loading) {
    return (
      <DashboardLayout>
        <p style={{ textAlign: 'center', color: 'var(--ink-60)', paddingTop: '80px' }}>Завантаження профілю...</p>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="profile-page">
        <div className="profile-page-header">
          <h1>Профіль</h1>
          <p>Керуйте вашим акаунтом та налаштуваннями.</p>
        </div>

        {message && (
          <div className={`profile-message ${message.type}`}>
            {message.text}
          </div>
        )}

        {/* User Info Card */}
        <div className="profile-info-card">
          <div className="profile-avatar">
            <span>{getInitials(profile?.userName)}</span>
          </div>
          <div className="profile-user-details">
            <h2>{profile?.userName}</h2>
            <p className="profile-email">{profile?.email}</p>
            <div className="profile-badges">
              <span className="profile-role-badge">{getRoleLabel(profile?.role)}</span>
              {profile?.createdAt && (
                <span className="profile-date-badge">
                  З {new Date(profile.createdAt).toLocaleDateString('uk-UA', { month: 'long', year: 'numeric' })}
                </span>
              )}
            </div>
          </div>
        </div>

        {/* Update Username */}
        <div className="profile-section">
          <h3 className="profile-section-title">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
              <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" strokeLinecap="round" strokeLinejoin="round"/>
              <circle cx="12" cy="7" r="4"/>
            </svg>
            Ім'я користувача
          </h3>
          <div className="profile-field">
            <label htmlFor="editUsername">Нове ім'я</label>
            <div className="profile-field-row">
              <input
                id="editUsername"
                type="text"
                className="form-input"
                value={editUsername}
                onChange={(e) => setEditUsername(e.target.value)}
              />
              <button
                className="btn-save-field"
                onClick={handleUpdateUsername}
                disabled={!editUsername.trim() || editUsername === profile?.userName}
              >
                Зберегти
              </button>
            </div>
          </div>
        </div>

        {/* Update Email */}
        <div className="profile-section">
          <h3 className="profile-section-title">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
              <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"/>
              <polyline points="22,6 12,13 2,6"/>
            </svg>
            Email
          </h3>
          <div className="profile-field">
            <label htmlFor="editEmail">Новий email</label>
            <input
              id="editEmail"
              type="email"
              className="form-input"
              value={editEmail}
              onChange={(e) => setEditEmail(e.target.value)}
            />
          </div>
          <div className="profile-field">
            <label htmlFor="emailPwd">Поточний пароль (для підтвердження)</label>
            <div className="profile-field-row">
              <input
                id="emailPwd"
                type="password"
                className="form-input"
                placeholder="Введіть поточний пароль"
                value={emailPassword}
                onChange={(e) => setEmailPassword(e.target.value)}
              />
              <button
                className="btn-save-field"
                onClick={handleUpdateEmail}
                disabled={!editEmail.trim() || !emailPassword || editEmail === profile?.email}
              >
                Зберегти
              </button>
            </div>
          </div>
        </div>

        {/* Change Password */}
        <div className="profile-section">
          <h3 className="profile-section-title">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
              <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
              <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
            </svg>
            Змінити пароль
          </h3>
          <form className="password-form" onSubmit={handleChangePassword}>
            <div className="profile-field">
              <label htmlFor="curPwd">Поточний пароль</label>
              <input
                id="curPwd"
                type="password"
                className="form-input"
                placeholder="Введіть поточний пароль"
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
              />
            </div>
            <div className="profile-field">
              <label htmlFor="newPwd">Новий пароль</label>
              <input
                id="newPwd"
                type="password"
                className="form-input"
                placeholder="Введіть новий пароль"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
              />
            </div>
            <div className="profile-field">
              <label htmlFor="confirmPwd">Підтвердити пароль</label>
              <input
                id="confirmPwd"
                type="password"
                className="form-input"
                placeholder="Повторіть новий пароль"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
              />
            </div>
            <div className="password-actions">
              <button
                type="submit"
                className="btn-save-field"
                disabled={!currentPassword || !newPassword || !confirmPassword}
              >
                Змінити пароль
              </button>
            </div>
          </form>
        </div>

        {/* Danger Zone */}
        <div className="profile-section danger">
          <h3 className="profile-section-title">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="20" height="20">
              <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/>
              <line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/>
            </svg>
            Небезпечна зона
          </h3>
          <p className="danger-description">
            Видалення акаунту є незворотнім. Всі ваші дані (завдання, роадмапи, ідеї) будуть видалені назавжди.
          </p>

          {!showDeleteConfirm ? (
            <button className="btn-danger" onClick={() => setShowDeleteConfirm(true)}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" width="16" height="16">
                <path d="M3 6h18M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" strokeLinecap="round" strokeLinejoin="round"/>
              </svg>
              Видалити акаунт
            </button>
          ) : (
            <div className="profile-field">
              <label htmlFor="deletePwd">Введіть пароль для підтвердження</label>
              <div className="profile-field-row">
                <input
                  id="deletePwd"
                  type="password"
                  className="form-input"
                  placeholder="Ваш пароль"
                  value={deletePassword}
                  onChange={(e) => setDeletePassword(e.target.value)}
                />
                <button
                  className="btn-danger"
                  onClick={handleDeleteAccount}
                  disabled={!deletePassword}
                  style={{ whiteSpace: 'nowrap' }}
                >
                  Підтвердити видалення
                </button>
                <button
                  className="btn-secondary"
                  onClick={() => { setShowDeleteConfirm(false); setDeletePassword(''); }}
                  style={{ whiteSpace: 'nowrap' }}
                >
                  Скасувати
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </DashboardLayout>
  );
};
