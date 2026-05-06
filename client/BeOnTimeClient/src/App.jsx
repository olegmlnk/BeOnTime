import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import { PrivateRoute } from './components/PrivateRoute'
import { LoginPage } from './pages/LoginPage'
import { RegisterPage } from './pages/RegisterPage'
import { DashboardPage } from './pages/DashboardPage'
import { TasksPage } from './pages/TasksPage'
import { IdeasPage } from './pages/IdeasPage'
import { RoadmapsPage } from './pages/RoadmapsPage'
import { RoadmapDetailPage } from './pages/RoadmapDetailPage'
import { ProfilePage } from './pages/ProfilePage'
import './App.css'

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

          <Route 
            path="/ideas" 
            element={
              <PrivateRoute>
                <IdeasPage />
              </PrivateRoute>
            } 
          />

          <Route 
            path="/roadmaps" 
            element={
              <PrivateRoute>
                <RoadmapsPage />
              </PrivateRoute>
            } 
          />

          <Route 
            path="/roadmaps/:id" 
            element={
              <PrivateRoute>
                <RoadmapDetailPage />
              </PrivateRoute>
            } 
          />

          <Route 
            path="/profile" 
            element={
              <PrivateRoute>
                <ProfilePage />
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
