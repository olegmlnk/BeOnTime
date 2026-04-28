import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import { PrivateRoute } from './components/PrivateRoute'
import { LoginPage } from './pages/LoginPage'
import { RegisterPage } from './pages/RegisterPage'
import { useAuth } from './hooks/useAuth'
import './App.css'
import { DashboardPage } from './pages/DashboardPage'
import { TasksPage } from './pages/TasksPage'
function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          {/* Публічні роути */}
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          
          {/* Захищені роути */}
          <Route 
            path="/" 
            element={
              <PrivateRoute>
                <DashboardPage />
              </PrivateRoute>
            } 
          />
          
          <Route 
            path="/tasks" 
            element={
              <PrivateRoute>
                <TasksPage />
              </PrivateRoute>
            } 
          />
          
          {/* Fallback */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Router>
    </AuthProvider>
  )
}

export default App
