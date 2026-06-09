import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'
import { t } from '../theme.js'

const s = {
  wrapper: {
    display: 'flex', justifyContent: 'center', alignItems: 'center',
    flex: 1, padding: '2rem', background: t.bg, minHeight: 0,
  },
  card: {
    width: '100%', maxWidth: '400px',
    padding: '36px 32px',
    border: `1px solid ${t.border}`,
    borderRadius: t.radiusLg,
    background: t.card,
    boxShadow: t.shadowMd,
  },
  brand: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '15px', fontWeight: 800, color: t.amber,
    letterSpacing: '-0.02em', marginBottom: '24px',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px', fontWeight: 800, color: t.text,
    letterSpacing: '-0.03em', marginBottom: '4px',
  },
  subtitle: { fontSize: '13.5px', color: t.textMuted, marginBottom: '28px' },
  field: { marginBottom: '16px' },
  label: { ...t.label },
  input: { ...t.input },
  btn: { ...t.btnPrimary, width: '100%', padding: '12px', fontSize: '15px', marginTop: '8px' },
  error: {
    fontSize: '13px', color: t.redText, marginBottom: '16px',
    padding: '11px 14px', background: t.redBg,
    border: `1px solid ${t.redBorder}`, borderRadius: t.radius,
  },
  footer: { marginTop: '24px', fontSize: '13px', color: t.textMuted, textAlign: 'center' },
  footerLink: { color: t.amberDark, fontWeight: 600, textDecoration: 'none' },
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
      const msg = err.message || ''
      if (msg.includes('заблокиран') || msg.includes('BANNED')) setError('Акаунтът ви е временно блокиран. Свържете се с поддръжката.')
      else setError('Невалиден email или парола.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={s.wrapper}>
      <div style={s.card}>
        <div style={s.brand}>HelpMe</div>
        <div style={s.title}>Добре дошли</div>
        <div style={s.subtitle}>Влезте в акаунта си, за да продължите</div>
        {error && <div style={s.error}>{error}</div>}
        <form onSubmit={handleSubmit}>
          <div style={s.field}>
            <label style={s.label}>Email адрес</label>
            <input style={s.input} type="email" required value={form.email}
              placeholder="you@example.com"
              onFocus={e => e.target.style.borderColor = t.amber}
              onBlur={e => e.target.style.borderColor = t.border}
              onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
          </div>
          <div style={s.field}>
            <label style={s.label}>Парола</label>
            <input style={s.input} type="password" required value={form.password}
              placeholder="••••••••"
              onFocus={e => e.target.style.borderColor = t.amber}
              onBlur={e => e.target.style.borderColor = t.border}
              onChange={e => setForm(f => ({ ...f, password: e.target.value }))} />
          </div>
          <button style={s.btn} type="submit" disabled={loading}>
            {loading ? 'Зареждане...' : 'Вход'}
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
