import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'

const styles = {
  wrapper: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    flex: 1,
    padding: '2rem',
  },
  card: {
    width: '100%',
    maxWidth: '400px',
    padding: '2rem',
    border: '1px solid var(--border)',
    borderRadius: '12px',
    textAlign: 'left',
  },
  title: {
    fontSize: '22px',
    fontWeight: 600,
    color: 'var(--text-h)',
    marginBottom: '1.5rem',
  },
  field: {
    display: 'flex',
    flexDirection: 'column',
    gap: '6px',
    marginBottom: '1rem',
  },
  label: {
    fontSize: '13px',
    color: 'var(--text)',
  },
  input: {
    padding: '10px 12px',
    borderRadius: '8px',
    border: '1px solid var(--border)',
    background: 'var(--bg)',
    color: 'var(--text-h)',
    fontSize: '14px',
    outline: 'none',
  },
  btn: {
    width: '100%',
    padding: '10px',
    borderRadius: '8px',
    border: 'none',
    background: 'var(--accent)',
    color: '#fff',
    fontSize: '14px',
    fontWeight: 500,
    cursor: 'pointer',
    marginTop: '0.5rem',
  },
  error: {
    fontSize: '13px',
    color: '#e53e3e',
    marginBottom: '1rem',
    padding: '8px 12px',
    background: 'rgba(229,62,62,0.08)',
    borderRadius: '6px',
  },
  footer: {
    marginTop: '1.5rem',
    fontSize: '13px',
    color: 'var(--text)',
    textAlign: 'center',
  },
  footerLink: {
    color: 'var(--accent)',
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
    <div style={styles.wrapper}>
      <div style={styles.card}>
        <div style={styles.title}>Вход</div>
        {error && <div style={styles.error}>{error}</div>}
        <form onSubmit={handleSubmit}>
          <div style={styles.field}>
            <label style={styles.label}>Email</label>
            <input style={styles.input} type="email" required value={form.email}
              onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Парола</label>
            <input style={styles.input} type="password" required value={form.password}
              onChange={e => setForm(f => ({ ...f, password: e.target.value }))} />
          </div>
          <button style={styles.btn} type="submit" disabled={loading}>
            {loading ? 'Зареждане...' : 'Вход'}
          </button>
        </form>
        <div style={styles.footer}>
          Нямате акаунт?{' '}
          <Link to="/register" style={styles.footerLink}>Регистрирайте се</Link>
        </div>
      </div>
    </div>
  )
}
