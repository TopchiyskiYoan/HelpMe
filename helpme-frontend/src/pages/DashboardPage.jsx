import { Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

export default function DashboardPage() {
  const { user } = useAuth()

  if (user?.role === 'Client') return <Navigate to="/jobs/my" replace />
  if (user?.role === 'Handyman') return <Navigate to="/jobs/feed" replace />
  if (user?.role === 'Administrator') return <Navigate to="/admin" replace />

  return null
}
