import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

const s = {
  nav: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '0 2rem',
    height: '56px',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)',
    boxShadow: '0 2px 12px rgba(217,119,6,0.28)',
    flexShrink: 0,
  },
  logo: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontWeight: 800,
    fontSize: '19px',
    color: '#fff',
    textDecoration: 'none',
    letterSpacing: '-0.03em',
    textShadow: '0 1px 4px rgba(0,0,0,0.15)',
  },
  right: { display: 'flex', alignItems: 'center', gap: '1.25rem' },
  link: {
    fontSize: '13px',
    fontWeight: 500,
    color: 'rgba(255,255,255,0.88)',
    textDecoration: 'none',
  },
  ctaLink: {
    fontSize: '13px',
    fontWeight: 700,
    color: '#fff',
    textDecoration: 'none',
    background: 'rgba(255,255,255,0.22)',
    padding: '5px 14px',
    borderRadius: '7px',
    backdropFilter: 'blur(4px)',
  },
  name: {
    fontSize: '13px',
    color: 'rgba(255,255,255,0.88)',
    fontWeight: 500,
  },
  btn: {
    fontSize: '12px',
    fontWeight: 500,
    padding: '4px 12px',
    borderRadius: '6px',
    border: '1px solid rgba(255,255,255,0.5)',
    background: 'transparent',
    color: '#fff',
    cursor: 'pointer',
  },
  authLink: {
    fontSize: '13px',
    fontWeight: 600,
    color: '#fff',
    textDecoration: 'none',
    background: 'rgba(255,255,255,0.18)',
    padding: '5px 14px',
    borderRadius: '7px',
  },
}

export default function Navbar() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => { logout(); navigate('/login') }

  return (
    <nav style={s.nav}>
      <Link to="/" style={s.logo}>HelpMe</Link>
      <div style={s.right}>
        {user ? (
          <>
            {user.role === 'Client' && (
              <>
                <Link to="/jobs/my" style={s.link}>Моите поръчки</Link>
                <Link to="/jobs/create" style={s.ctaLink}>+ Нова поръчка</Link>
              </>
            )}
            {user.role === 'Handyman' && (
              <>
                <Link to="/jobs/feed" style={s.link}>Поръчки</Link>
                <Link to="/handymen/me/interests" style={s.link}>Офертите ми</Link>
              </>
            )}
            <span style={s.name}>{user.firstName} {user.lastName}</span>
            <button style={s.btn} onClick={handleLogout}>Изход</button>
          </>
        ) : (
          <>
            <Link to="/login" style={s.link}>Вход</Link>
            <Link to="/register" style={s.authLink}>Регистрация</Link>
          </>
        )}
      </div>
    </nav>
  )
}
