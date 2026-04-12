import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

const styles = {
  nav: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '0 2rem',
    height: '56px',
    borderBottom: '1px solid var(--border)',
    background: 'var(--bg)',
  },
  logo: {
    fontWeight: 600,
    fontSize: '18px',
    color: 'var(--accent)',
    textDecoration: 'none',
  },
  right: {
    display: 'flex',
    alignItems: 'center',
    gap: '1rem',
  },
  name: {
    fontSize: '14px',
    color: 'var(--text)',
  },
  link: {
    fontSize: '14px',
    color: 'var(--accent)',
    textDecoration: 'none',
  },
  btn: {
    fontSize: '13px',
    padding: '6px 14px',
    borderRadius: '6px',
    border: '1px solid var(--border)',
    background: 'transparent',
    color: 'var(--text)',
    cursor: 'pointer',
  },
}

export default function Navbar() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <nav style={styles.nav}>
      <Link to="/" style={styles.logo}>HelpMe</Link>
      <div style={styles.right}>
        {user ? (
          <>
            <span style={styles.name}>{user.firstName} {user.lastName}</span>
            <button style={styles.btn} onClick={handleLogout}>Изход</button>
          </>
        ) : (
          <>
            <Link to="/login" style={styles.link}>Вход</Link>
            <Link to="/register" style={styles.link}>Регистрация</Link>
          </>
        )}
      </div>
    </nav>
  )
}
