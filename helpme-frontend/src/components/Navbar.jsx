import { useState, useRef, useEffect } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import NotificationBell from './NotificationBell.jsx'

const nav = {
  nav: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '0 2rem',
    height: '58px',
    background: '#0f172a',
    boxShadow: '0 1px 0 rgba(255,255,255,0.06)',
    flexShrink: 0,
    position: 'sticky',
    top: 0,
    zIndex: 200,
  },
  left: { display: 'flex', alignItems: 'center', gap: '2rem' },
  logo: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontWeight: 800,
    fontSize: '20px',
    color: '#f59e0b',
    textDecoration: 'none',
    letterSpacing: '-0.04em',
  },
  links: { display: 'flex', alignItems: 'center', gap: '0.25rem' },
  link: {
    fontSize: '13.5px',
    fontWeight: 500,
    color: 'rgba(255,255,255,0.65)',
    textDecoration: 'none',
    padding: '5px 11px',
    borderRadius: '7px',
    transition: 'color 0.15s, background 0.15s',
  },
  linkActive: {
    fontSize: '13.5px',
    fontWeight: 600,
    color: '#ffffff',
    textDecoration: 'none',
    padding: '5px 11px',
    borderRadius: '7px',
    background: 'rgba(255,255,255,0.1)',
  },
  right: { display: 'flex', alignItems: 'center', gap: '0.75rem' },
  authLink: {
    fontSize: '13px',
    fontWeight: 500,
    color: 'rgba(255,255,255,0.65)',
    textDecoration: 'none',
    padding: '5px 12px',
    borderRadius: '7px',
    transition: 'color 0.15s',
  },
  authCta: {
    fontSize: '13px',
    fontWeight: 600,
    color: '#0f172a',
    textDecoration: 'none',
    padding: '6px 14px',
    borderRadius: '7px',
    background: '#f59e0b',
  },
  adminBadge: {
    fontSize: '12px',
    fontWeight: 600,
    color: '#f59e0b',
    textDecoration: 'none',
    padding: '5px 11px',
    borderRadius: '7px',
    border: '1px solid rgba(245,158,11,0.35)',
    background: 'rgba(245,158,11,0.08)',
  },
  avatar: {
    width: '33px',
    height: '33px',
    borderRadius: '50%',
    background: 'linear-gradient(135deg, #f59e0b, #d97706)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontSize: '13px',
    fontWeight: 700,
    color: '#fff',
    cursor: 'pointer',
    userSelect: 'none',
    border: '2px solid rgba(255,255,255,0.12)',
    flexShrink: 0,
  },
  dropWrapper: { position: 'relative' },
  drop: {
    position: 'absolute',
    top: 'calc(100% + 10px)',
    right: 0,
    background: '#1e293b',
    border: '1px solid rgba(255,255,255,0.1)',
    borderRadius: '12px',
    minWidth: '200px',
    boxShadow: '0 10px 32px rgba(0,0,0,0.35)',
    overflow: 'hidden',
    zIndex: 300,
  },
  dropHeader: {
    padding: '14px 16px 10px',
    borderBottom: '1px solid rgba(255,255,255,0.07)',
  },
  dropName: { fontSize: '14px', fontWeight: 600, color: '#f8fafc', marginBottom: '2px' },
  dropRole: { fontSize: '12px', color: 'rgba(255,255,255,0.4)', fontWeight: 400 },
  dropItem: {
    display: 'block',
    padding: '11px 16px',
    fontSize: '13.5px',
    color: 'rgba(255,255,255,0.75)',
    textDecoration: 'none',
    cursor: 'pointer',
    transition: 'background 0.12s',
    border: 'none',
    background: 'none',
    width: '100%',
    textAlign: 'left',
    fontFamily: 'inherit',
  },
  dropDivider: { borderTop: '1px solid rgba(255,255,255,0.07)', margin: '4px 0' },
}

const ROLE_LABEL = { Client: 'Клиент', Handyman: 'Майстор', Administrator: 'Администратор' }

export default function Navbar() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const [open, setOpen] = useState(false)
  const ref = useRef(null)

  useEffect(() => {
    const handler = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false) }
    document.addEventListener('mousedown', handler)
    return () => document.removeEventListener('mousedown', handler)
  }, [])

  const handleLogout = () => { setOpen(false); logout(); navigate('/login') }
  const initials = user ? `${user.firstName?.[0] ?? ''}${user.lastName?.[0] ?? ''}`.toUpperCase() : ''

  const NavLink = ({ to, children }) => (
    <Link to={to} style={nav.link}
      onMouseEnter={e => { e.currentTarget.style.color = '#fff'; e.currentTarget.style.background = 'rgba(255,255,255,0.08)' }}
      onMouseLeave={e => { e.currentTarget.style.color = 'rgba(255,255,255,0.65)'; e.currentTarget.style.background = 'transparent' }}>
      {children}
    </Link>
  )

  return (
    <nav style={nav.nav}>
      <div style={nav.left}>
        <Link to="/" style={nav.logo}>HelpMe</Link>
        {user && (
          <div style={nav.links}>
            {user.role === 'Client' && (
              <>
                <NavLink to="/jobs/my">Моите поръчки</NavLink>
                <NavLink to="/jobs/create">+ Нова поръчка</NavLink>
                <NavLink to="/handymen">Намери майстор</NavLink>
              </>
            )}
            {user.role === 'Handyman' && (
              <>
                <NavLink to="/jobs/feed">Поръчки</NavLink>
                <NavLink to="/handymen/me/interests">Мои оферти</NavLink>
                <NavLink to={`/handymen/${user.id}`}>Моят профил</NavLink>
              </>
            )}
            {user.role === 'Administrator' && (
              <Link to="/admin/users" style={nav.adminBadge}>Администрация</Link>
            )}
          </div>
        )}
      </div>

      <div style={nav.right}>
        {user ? (
          <>
            <NotificationBell />
            <div ref={ref} style={nav.dropWrapper}>
              <div style={nav.avatar} onClick={() => setOpen(o => !o)} title="Профил">
                {initials}
              </div>
              {open && (
                <div style={nav.drop}>
                  <div style={nav.dropHeader}>
                    <div style={nav.dropName}>{user.firstName} {user.lastName}</div>
                    <div style={nav.dropRole}>{ROLE_LABEL[user.role] ?? user.role}</div>
                  </div>
                  <Link to="/profile" style={nav.dropItem}
                    onClick={() => setOpen(false)}
                    onMouseEnter={e => e.currentTarget.style.background = 'rgba(255,255,255,0.07)'}
                    onMouseLeave={e => e.currentTarget.style.background = 'none'}>
                    Редактирай профил
                  </Link>
                  {user.role === 'Handyman' && (
                    <Link to={`/handymen/${user.id}`} style={nav.dropItem}
                      onClick={() => setOpen(false)}
                      onMouseEnter={e => e.currentTarget.style.background = 'rgba(255,255,255,0.07)'}
                      onMouseLeave={e => e.currentTarget.style.background = 'none'}>
                      Публичен профил
                    </Link>
                  )}
                  <div style={nav.dropDivider} />
                  <button style={{ ...nav.dropItem, color: '#f87171' }} onClick={handleLogout}
                    onMouseEnter={e => e.currentTarget.style.background = 'rgba(248,113,113,0.08)'}
                    onMouseLeave={e => e.currentTarget.style.background = 'none'}>
                    Изход
                  </button>
                </div>
              )}
            </div>
          </>
        ) : (
          <>
            <Link to="/login" style={nav.authLink}
              onMouseEnter={e => e.currentTarget.style.color = '#fff'}
              onMouseLeave={e => e.currentTarget.style.color = 'rgba(255,255,255,0.65)'}>
              Вход
            </Link>
            <Link to="/register" style={nav.authCta}>Регистрация</Link>
          </>
        )}
      </div>
    </nav>
  )
}
