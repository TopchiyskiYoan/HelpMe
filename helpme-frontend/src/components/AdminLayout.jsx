import { NavLink, Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

const s = {
  wrap: { display: 'flex', flex: 1, background: '#fffbf0', minHeight: 0 },
  sidebar: {
    width: '220px',
    flexShrink: 0,
    background: '#1c1917',
    padding: '1.5rem 0',
    display: 'flex',
    flexDirection: 'column',
  },
  sideTitle: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontWeight: 800,
    fontSize: '13px',
    color: '#a8a29e',
    letterSpacing: '0.08em',
    textTransform: 'uppercase',
    padding: '0 1.25rem',
    marginBottom: '1rem',
  },
  link: {
    display: 'block',
    padding: '10px 1.25rem',
    color: 'rgba(255,255,255,0.7)',
    textDecoration: 'none',
    fontSize: '14px',
    fontWeight: 500,
    transition: 'background 0.15s',
    borderLeft: '3px solid transparent',
  },
  activeLink: {
    color: '#f59e0b',
    background: 'rgba(245,158,11,0.1)',
    borderLeftColor: '#f59e0b',
  },
  content: { flex: 1, padding: '2rem', overflow: 'auto' },
}

export default function AdminLayout({ children }) {
  const { user } = useAuth()

  if (!user || user.role !== 'Administrator') {
    return <Navigate to="/" replace />
  }

  return (
    <div style={s.wrap}>
      <aside style={s.sidebar}>
        <div style={s.sideTitle}>Администрация</div>
        <NavLink
          to="/admin"
          end
          style={({ isActive }) => ({ ...s.link, ...(isActive ? s.activeLink : {}) })}
        >
          Табло
        </NavLink>
        <NavLink
          to="/admin/users"
          style={({ isActive }) => ({ ...s.link, ...(isActive ? s.activeLink : {}) })}
        >
          Потребители
        </NavLink>
        <NavLink
          to="/admin/handymen/pending"
          style={({ isActive }) => ({ ...s.link, ...(isActive ? s.activeLink : {}) })}
        >
          Верификации
        </NavLink>
        <NavLink
          to="/admin/jobs"
          style={({ isActive }) => ({ ...s.link, ...(isActive ? s.activeLink : {}) })}
        >
          Поръчки
        </NavLink>
        <NavLink
          to="/admin/reviews"
          style={({ isActive }) => ({ ...s.link, ...(isActive ? s.activeLink : {}) })}
        >
          Отзиви
        </NavLink>
      </aside>
      <div style={s.content}>{children}</div>
    </div>
  )
}
