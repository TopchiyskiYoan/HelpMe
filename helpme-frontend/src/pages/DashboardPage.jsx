import { Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

export default function DashboardPage() {
  const { user } = useAuth()

  if (user?.role === 'Client') return <Navigate to="/jobs/my" replace />
  if (user?.role === 'Handyman') return <Navigate to="/jobs/feed" replace />

  return (
    <div style={{ padding: '2rem', textAlign: 'left' }}>
      <h2>Добре дошли, {user?.firstName}!</h2>
      <p style={{ color: 'var(--text)', marginTop: '0.5rem' }}>
        Роля: <strong>{user?.role}</strong>
      </p>
    </div>
  )
}
