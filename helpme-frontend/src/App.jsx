import { Routes, Route, Navigate } from 'react-router-dom'
import Navbar from './components/Navbar.jsx'
import ProtectedRoute from './components/ProtectedRoute.jsx'
import LoginPage from './pages/LoginPage.jsx'
import RegisterPage from './pages/RegisterPage.jsx'
import DashboardPage from './pages/DashboardPage.jsx'
import JobCreatePage from './pages/JobCreatePage.jsx'
import ClientDashboardPage from './pages/ClientDashboardPage.jsx'
import HandymanFeedPage from './pages/HandymanFeedPage.jsx'
import JobDetailPage from './pages/JobDetailPage.jsx'
import PendingConfirmationsPage from './pages/PendingConfirmationsPage.jsx'
import HandymanPublicProfilePage from './pages/HandymanPublicProfilePage.jsx'

function App() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100svh' }}>
      <Navbar />
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route path="/" element={<ProtectedRoute><DashboardPage /></ProtectedRoute>} />

        <Route path="/jobs/create" element={
          <ProtectedRoute roles={['Client']}><JobCreatePage /></ProtectedRoute>
        } />
        <Route path="/jobs/my" element={
          <ProtectedRoute roles={['Client']}><ClientDashboardPage /></ProtectedRoute>
        } />
        <Route path="/jobs/feed" element={
          <ProtectedRoute roles={['Handyman']}><HandymanFeedPage /></ProtectedRoute>
        } />
        <Route path="/jobs/:id" element={
          <ProtectedRoute><JobDetailPage /></ProtectedRoute>
        } />
        <Route path="/handymen/me/interests" element={
          <ProtectedRoute roles={['Handyman']}><PendingConfirmationsPage /></ProtectedRoute>
        } />
        <Route path="/handymen/:userId" element={
          <ProtectedRoute><HandymanPublicProfilePage /></ProtectedRoute>
        } />

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </div>
  )
}

export default App
