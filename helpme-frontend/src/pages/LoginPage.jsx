import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'

const s = {
  wrapper: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    flex: 1,
    padding: '2rem',
    background: '#fffbf0',
  },
  card: {
    width: '100%',
    maxWidth: '400px',
    padding: '32px 28px',
    border: '1px solid #fde68a',
    borderRadius: '16px',
    background: '#fff',
    boxShadow: '0 8px 32px rgba(217,119,6,0.14)',
    textAlign: 'left',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px',
    fontWeight: 800,
    color: '#1c1917',
    letterSpacing: '-0.03em',
    marginBottom: '4px',
  },
  subtitle: {
    fontSize: '13px',
    color: '#78350f',
    marginBottom: '24px',
  },
  field: {
    display: 'flex',
    flexDirection: 'column',
    gap: '5px',
    marginBottom: '14px',
  },
  label: {
    fontSize: '11px',
    fontWeight: 600,
    color: '#78350f',
    textTransform: 'uppercase',
    letterSpacing: '0.06em',
  },
  input: {
    padding: '11px 13px',
    borderRadius: '8px',
    border: '1px solid #fde68a',
    background: '#fffbf0',
    color: '#1c1917',
    fontSize: '14px',
    outline: 'none',
    width: '100%',
    boxSizing: 'border-box',
  },
  btn: {
    width: '100%',
    padding: '12px',
    borderRadius: '9px',
    border: 'none',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)',
    color: '#fff',
    fontSize: '15px',
    fontWeight: 700,
    cursor: 'pointer',
    marginTop: '8px',
    boxShadow: '0 4px 14px rgba(217,119,6,0.35)',
    letterSpacing: '0.01em',
  },
  error: {
    fontSize: '13px',
    color: '#dc2626',
    marginBottom: '14px',
    padding: '10px 13px',
    background: '#fee2e2',
    border: '1px solid #fecaca',
    borderRadius: '8px',
  },
  footer: {
    marginTop: '20px',
    fontSize: '13px',
    color: '#78350f',
    textAlign: 'center',
  },
  footerLink: {
    color: '#d97706',
    fontWeight: 600,
    textDecoration: 'none',
  },
}

export default function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const data = await api.post('/auth/login', form)
      login({ id: data.userId, firstName: data.firstName, lastName: data.lastName, email: data.email, role: data.role }, data.token)
      navigate('/')
    } catch (err) {
      setError(err.message || 'Грешен email или парола.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={s.wrapper}>
      <div style={s.card}>
        <div style={s.title}>Добре дошли</div>
        <div style={s.subtitle}>Влезте в акаунта си</div>
        {error && <div style={s.error}>{error}</div>}
        <form onSubmit={handleSubmit}>
          <div style={s.field}>
            <label style={s.label}>Email</label>
            <input style={s.input} type="email" required value={form.email}
              onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
          </div>
          <div style={s.field}>
            <label style={s.label}>Парола</label>
            <input style={s.input} type="password" required value={form.password}
              onChange={e => setForm(f => ({ ...f, password: e.target.value }))} />
          </div>
          <button style={s.btn} type="submit" disabled={loading}>
            {loading ? 'Зареждане...' : 'Вход →'}
          </button>
        </form>
        <div style={s.footer}>
          Нямате акаунт?{' '}
          <Link to="/register" style={s.footerLink}>Регистрирайте се</Link>
        </div>
      </div>
    </div>
  )
}
