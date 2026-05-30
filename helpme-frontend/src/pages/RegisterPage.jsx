import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'

const field = {
  display: 'flex',
  flexDirection: 'column',
  gap: '5px',
  marginBottom: '13px',
}
const label = {
  fontSize: '11px',
  fontWeight: 600,
  color: '#78350f',
  textTransform: 'uppercase',
  letterSpacing: '0.06em',
}
const baseInput = {
  padding: '11px 13px',
  borderRadius: '8px',
  background: '#fffbf0',
  color: '#1c1917',
  fontSize: '14px',
  outline: 'none',
  width: '100%',
  boxSizing: 'border-box',
}

const styles = {
  wrapper: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'flex-start',
    flex: 1,
    padding: '2rem',
    background: '#fffbf0',
  },
  card: {
    width: '100%',
    maxWidth: '460px',
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
    marginBottom: '22px',
  },
  row: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '12px',
  },
  field,
  label,
  input: (hasError) => ({
    ...baseInput,
    border: `1px solid ${hasError ? '#fca5a5' : '#fde68a'}`,
  }),
  select: {
    ...baseInput,
    border: '1px solid #fde68a',
    cursor: 'pointer',
  },
  fieldError: {
    fontSize: '12px',
    color: '#dc2626',
    marginTop: '2px',
  },
  serverError: {
    fontSize: '13px',
    color: '#dc2626',
    marginBottom: '14px',
    padding: '10px 13px',
    background: '#fee2e2',
    border: '1px solid #fecaca',
    borderRadius: '8px',
    borderLeft: '3px solid #dc2626',
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

const SERVER_ERRORS = {
  'Email already in use.': 'Този email адрес вече е регистриран. Моля, влезте в акаунта си.',
  "Invalid role. Use 'Client' or 'Handyman'.": 'Невалидна роля.',
  'Invalid phone number. Use 08XXXXXXXX or +359XXXXXXXXX.': 'Невалиден телефонен номер.',
  'Registration failed.': 'Регистрацията не успя. Моля, опитайте отново.',
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
      setServerError(SERVER_ERRORS[msg] ?? 'Възникна грешка. Моля, опитайте отново.')
    } finally {
      setLoading(false)
    }
  }

  const fe = (f) => fieldErrors[f]

  return (
    <div style={styles.wrapper}>
      <div style={styles.card}>
        <div style={styles.title}>Създайте акаунт</div>
        <div style={styles.subtitle}>Регистрирайте се безплатно</div>
        {serverError && <div style={styles.serverError}>{serverError}</div>}
        <form onSubmit={handleSubmit} noValidate>
          <div style={styles.row}>
            <div style={styles.field}>
              <label style={styles.label}>Име</label>
              <input style={styles.input(fe('firstName'))} value={form.firstName} onChange={set('firstName')} />
              {fe('firstName') && <span style={styles.fieldError}>{fe('firstName')}</span>}
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Фамилия</label>
              <input style={styles.input(fe('lastName'))} value={form.lastName} onChange={set('lastName')} />
              {fe('lastName') && <span style={styles.fieldError}>{fe('lastName')}</span>}
            </div>
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Email</label>
            <input style={styles.input(fe('email'))} type="email" value={form.email} onChange={set('email')} />
            {fe('email') && <span style={styles.fieldError}>{fe('email')}</span>}
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Телефон</label>
            <input style={styles.input(fe('phoneNumber'))} type="tel" value={form.phoneNumber} onChange={set('phoneNumber')} placeholder="08XXXXXXXX" />
            {fe('phoneNumber') && <span style={styles.fieldError}>{fe('phoneNumber')}</span>}
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Парола</label>
            <input style={styles.input(fe('password'))} type="password" value={form.password} onChange={set('password')} />
            {fe('password') && <span style={styles.fieldError}>{fe('password')}</span>}
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Роля</label>
            <select style={styles.select} value={form.role} onChange={set('role')}>
              <option value="Client">Клиент — търся майстор</option>
              <option value="Handyman">Майстор — предлагам услуги</option>
            </select>
          </div>
          <button style={styles.btn} type="submit" disabled={loading}>
            {loading ? 'Зареждане...' : 'Регистрация →'}
          </button>
        </form>
        <div style={styles.footer}>
          Вече имате акаунт?{' '}
          <Link to="/login" style={styles.footerLink}>Влезте</Link>
        </div>
      </div>
    </div>
  )
}
