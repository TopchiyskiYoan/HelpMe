import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'
import { t } from '../theme.js'

const s = {
  wrapper: {
    display: 'flex', justifyContent: 'center', alignItems: 'flex-start',
    flex: 1, padding: '2rem', background: t.bg,
  },
  card: {
    width: '100%', maxWidth: '480px',
    padding: '36px 32px',
    border: `1px solid ${t.border}`,
    borderRadius: t.radiusLg,
    background: t.card,
    boxShadow: t.shadowMd,
    textAlign: 'left',
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
  row: { display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '14px' },
  field: { marginBottom: '16px' },
  label: { ...t.label },
  input: (err) => ({ ...t.input, borderColor: err ? t.red : t.border }),
  select: { ...t.input, cursor: 'pointer', appearance: 'auto' },
  fieldError: { fontSize: '12px', color: t.redText, marginTop: '4px' },
  serverError: {
    fontSize: '13px', color: t.redText, marginBottom: '18px',
    padding: '11px 14px', background: t.redBg,
    border: `1px solid ${t.redBorder}`, borderRadius: t.radius,
  },
  roleCards: { display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '10px', marginBottom: '16px' },
  roleCard: (active) => ({
    padding: '14px',
    borderRadius: t.radius,
    border: `2px solid ${active ? t.amber : t.border}`,
    background: active ? t.amberBg : t.card,
    cursor: 'pointer',
    textAlign: 'center',
    transition: 'all 0.15s',
  }),
  roleIcon: { fontSize: '22px', marginBottom: '6px' },
  roleName: { fontSize: '13px', fontWeight: 700, color: t.text, marginBottom: '2px' },
  roleDesc: { fontSize: '11px', color: t.textMuted },
  btn: { ...t.btnPrimary, width: '100%', padding: '12px', fontSize: '15px', marginTop: '8px' },
  footer: { marginTop: '24px', fontSize: '13px', color: t.textMuted, textAlign: 'center' },
  footerLink: { color: t.amberDark, fontWeight: 600, textDecoration: 'none' },
}

function validate(form) {
  const errors = {}
  if (!form.firstName.trim()) errors.firstName = 'Задължително.'
  if (!form.lastName.trim()) errors.lastName = 'Задължително.'
  if (!form.email.trim()) errors.email = 'Задължително.'
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) errors.email = 'Невалиден email.'
  if (!form.phoneNumber.trim()) errors.phoneNumber = 'Задължително.'
  else if (!/^(08\d{8}|\+359\d{9})$/.test(form.phoneNumber)) errors.phoneNumber = 'Формат: 08XXXXXXXX'
  if (!form.password) errors.password = 'Задължително.'
  else if (form.password.length < 6) errors.password = 'Мин. 6 символа.'
  else if (!/[A-Z]/.test(form.password)) errors.password = 'Нужна главна буква.'
  else if (!/[0-9]/.test(form.password)) errors.password = 'Нужна цифра.'
  else if (!/[^A-Za-z0-9]/.test(form.password)) errors.password = 'Нужен спец. символ.'
  return errors
}

const INITIAL = { firstName: '', lastName: '', email: '', phoneNumber: '', password: '', role: 'Client' }

export default function RegisterPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState(INITIAL)
  const [fieldErrors, setFieldErrors] = useState({})
  const [serverError, setServerError] = useState('')
  const [loading, setLoading] = useState(false)

  const set = (f) => (e) => {
    setForm(prev => ({ ...prev, [f]: e.target.value }))
    if (fieldErrors[f]) setFieldErrors(fe => ({ ...fe, [f]: '' }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setServerError('')
    const errors = validate(form)
    if (Object.keys(errors).length > 0) { setFieldErrors(errors); return }
    setLoading(true)
    try {
      const data = await api.post('/auth/register', form)
      login({ id: data.userId, firstName: data.firstName, lastName: data.lastName, email: data.email, role: data.role }, data.token)
      navigate('/')
    } catch (err) {
      const msg = err.message || ''
      if (msg.includes('EMAIL') || msg.includes('already')) setServerError('Този email адрес вече е регистриран.')
      else if (msg.includes('PHONE') || msg.includes('phone')) setServerError('Невалиден телефонен номер.')
      else setServerError('Регистрацията не успя. Моля, опитайте отново.')
    } finally {
      setLoading(false)
    }
  }

  const fe = (f) => fieldErrors[f]

  return (
    <div style={s.wrapper}>
      <div style={s.card}>
        <div style={s.brand}>HelpMe</div>
        <div style={s.title}>Създайте акаунт</div>
        <div style={s.subtitle}>Регистрирайте се безплатно в платформата</div>

        {serverError && <div style={s.serverError}>{serverError}</div>}

        <form onSubmit={handleSubmit} noValidate>
          <div style={{ marginBottom: '18px' }}>
            <div style={{ ...s.label, marginBottom: '10px' }}>Регистрирам се като</div>
            <div style={s.roleCards}>
              {[
                { value: 'Client', icon: '🔍', name: 'Клиент', desc: 'Търся майстор' },
                { value: 'Handyman', icon: '🔧', name: 'Майстор', desc: 'Предлагам услуги' },
              ].map(r => (
                <div key={r.value} style={s.roleCard(form.role === r.value)} onClick={() => setForm(f => ({ ...f, role: r.value }))}>
                  <div style={s.roleIcon}>{r.icon}</div>
                  <div style={s.roleName}>{r.name}</div>
                  <div style={s.roleDesc}>{r.desc}</div>
                </div>
              ))}
            </div>
          </div>

          <div style={s.row}>
            <div style={s.field}>
              <label style={s.label}>Име</label>
              <input style={s.input(fe('firstName'))} value={form.firstName} onChange={set('firstName')}
                onFocus={e => e.target.style.borderColor = t.amber}
                onBlur={e => e.target.style.borderColor = fe('firstName') ? t.red : t.border} />
              {fe('firstName') && <span style={s.fieldError}>{fe('firstName')}</span>}
            </div>
            <div style={s.field}>
              <label style={s.label}>Фамилия</label>
              <input style={s.input(fe('lastName'))} value={form.lastName} onChange={set('lastName')}
                onFocus={e => e.target.style.borderColor = t.amber}
                onBlur={e => e.target.style.borderColor = fe('lastName') ? t.red : t.border} />
              {fe('lastName') && <span style={s.fieldError}>{fe('lastName')}</span>}
            </div>
          </div>

          <div style={s.field}>
            <label style={s.label}>Email адрес</label>
            <input style={s.input(fe('email'))} type="email" value={form.email} onChange={set('email')}
              placeholder="you@example.com"
              onFocus={e => e.target.style.borderColor = t.amber}
              onBlur={e => e.target.style.borderColor = fe('email') ? t.red : t.border} />
            {fe('email') && <span style={s.fieldError}>{fe('email')}</span>}
          </div>

          <div style={s.field}>
            <label style={s.label}>Телефон</label>
            <input style={s.input(fe('phoneNumber'))} type="tel" value={form.phoneNumber} onChange={set('phoneNumber')}
              placeholder="08XXXXXXXX"
              onFocus={e => e.target.style.borderColor = t.amber}
              onBlur={e => e.target.style.borderColor = fe('phoneNumber') ? t.red : t.border} />
            {fe('phoneNumber') && <span style={s.fieldError}>{fe('phoneNumber')}</span>}
          </div>

          <div style={s.field}>
            <label style={s.label}>Парола</label>
            <input style={s.input(fe('password'))} type="password" value={form.password} onChange={set('password')}
              placeholder="Мин. 6 символа с главна буква и цифра"
              onFocus={e => e.target.style.borderColor = t.amber}
              onBlur={e => e.target.style.borderColor = fe('password') ? t.red : t.border} />
            {fe('password') && <span style={s.fieldError}>{fe('password')}</span>}
          </div>

          <button style={s.btn} type="submit" disabled={loading}>
            {loading ? 'Зареждане...' : 'Създай акаунт'}
          </button>
        </form>

        <div style={s.footer}>
          Вече имате акаунт?{' '}
          <Link to="/login" style={s.footerLink}>Влезте тук</Link>
        </div>
      </div>
    </div>
  )
}
