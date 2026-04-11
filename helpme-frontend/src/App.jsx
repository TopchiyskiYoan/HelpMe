import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from './context/AuthContext.jsx'

// Pages — added progressively per phase
// Phase 1.3 will populate these imports
const Placeholder = ({ name }) => (
  <div style={{ padding: '2rem' }}>
    <h2>{name}</h2>
    <p>Coming in Phase 1.3</p>
  </div>
)

function App() {
  const { user } = useAuth()

  return (
    <Routes>
      <Route path="/login" element={<Placeholder name="Login" />} />
      <Route path="/register" element={<Placeholder name="Register" />} />
      <Route
        path="/"
        element={user ? <Placeholder name="Dashboard" /> : <Navigate to="/login" replace />}
      />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default App
