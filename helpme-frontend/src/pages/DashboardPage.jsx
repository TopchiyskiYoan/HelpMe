import { useAuth } from '../context/AuthContext.jsx'

export default function DashboardPage() {
  const { user } = useAuth()

  return (
    <div style={{ padding: '2rem' }}>
      <h2>Добре дошли, {user?.firstName}!</h2>
      <p style={{ color: 'var(--text)', marginTop: '0.5rem' }}>
        Роля: <strong>{user?.role}</strong>
      </p>
    </div>
  )
}
