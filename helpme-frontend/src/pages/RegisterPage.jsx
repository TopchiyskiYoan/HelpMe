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
    maxWidth: '440px',
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
  row: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '1rem',
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
  input: (hasError) => ({
    padding: '10px 12px',
    borderRadius: '8px',
    border: `1px solid ${hasError ? '#e53e3e' : 'var(--border)'}`,
    background: 'var(--bg)',
    color: 'var(--text-h)',
    fontSize: '14px',
    outline: 'none',
  }),
  select: {
    padding: '10px 12px',
    borderRadius: '8px',
    border: '1px solid var(--border)',
    background: 'var(--bg)',
    color: 'var(--text-h)',
    fontSize: '14px',
    outline: 'none',
    cursor: 'pointer',
  },
  fieldError: {
    fontSize: '12px',
    color: '#e53e3e',
    marginTop: '2px',
  },
  hint: {
    fontSize: '12px',
    color: 'var(--text)',
    marginTop: '2px',
  },
  serverError: {
    fontSize: '13px',
    color: '#e53e3e',
    marginBottom: '1rem',
    padding: '10px 12px',
    background: 'rgba(229,62,62,0.08)',
    borderRadius: '6px',
    borderLeft: '3px solid #e53e3e',
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

const SERVER_ERRORS = {
  'Email already in use.': 'Този email адрес вече е регистриран. Моля, влезте в акаунта си.',
  'Invalid role. Use \'Client\' or \'Handyman\'.': 'Невалидна роля.',
  'Invalid phone number. Use 08XXXXXXXX or +359XXXXXXXXX.': 'Невалиден телефонен номер. Формат: 08XXXXXXXX или +359XXXXXXXXX',
  'Registration failed.': 'Регистрацията не успя. Моля, опитайте отново.',
}

function validate(form) {
  const errors = {}
  if (!form.firstName.trim()) errors.firstName = 'Името е задължително.'
  if (!form.lastName.trim()) errors.lastName = 'Фамилията е задължителна.'
  if (!form.email.trim()) errors.email = 'Email адресът е задължителен.'
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) errors.email = 'Невалиден email адрес.'
  if (!form.phoneNumber.trim()) errors.phoneNumber = 'Телефонният номер е задължителен.'
  else if (!/^(08\d{8}|\+359\d{9})$/.test(form.phoneNumber)) errors.phoneNumber = 'Невалиден номер. Формат: 08XXXXXXXX или +359XXXXXXXXX'
  if (!form.password) errors.password = 'Паролата е задължителна.'
  else if (form.password.length < 6) errors.password = 'Паролата трябва да е поне 6 символа.'
  else if (!/[A-Z]/.test(form.password)) errors.password = 'Паролата трябва да съдържа поне една главна буква.'
  else if (!/[0-9]/.test(form.password)) errors.password = 'Паролата трябва да съдържа поне една цифра.'
  else if (!/[^A-Za-z0-9]/.test(form.password)) errors.password = 'Паролата трябва да съдържа поне един специален символ (!@#$...).'
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

  const set = (field) => (e) => {
    setForm(f => ({ ...f, [field]: e.target.value }))
    if (fieldErrors[field]) setFieldErrors(fe => ({ ...fe, [field]: '' }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setServerError('')

    const errors = validate(form)
    if (Object.keys(errors).length > 0) {
      setFieldErrors(errors)
      return
    }

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

  const f = (field) => fieldErrors[field]

  return (
    <div style={styles.wrapper}>
      <div style={styles.card}>
        <div style={styles.title}>Регистрация</div>
        {serverError && <div style={styles.serverError}>{serverError}</div>}
        <form onSubmit={handleSubmit} noValidate>
          <div style={styles.row}>
            <div style={styles.field}>
              <label style={styles.label}>Име</label>
              <input style={styles.input(f('firstName'))} value={form.firstName} onChange={set('firstName')} />
              {f('firstName') && <span style={styles.fieldError}>{f('firstName')}</span>}
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Фамилия</label>
              <input style={styles.input(f('lastName'))} value={form.lastName} onChange={set('lastName')} />
              {f('lastName') && <span style={styles.fieldError}>{f('lastName')}</span>}
            </div>
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Email</label>
            <input style={styles.input(f('email'))} type="email" value={form.email} onChange={set('email')} />
            {f('email') && <span style={styles.fieldError}>{f('email')}</span>}
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Телефон</label>
            <input style={styles.input(f('phoneNumber'))} type="tel" value={form.phoneNumber} onChange={set('phoneNumber')} />
            {f('phoneNumber') && <span style={styles.fieldError}>{f('phoneNumber')}</span>}
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Парола</label>
            <input style={styles.input(f('password'))} type="password" value={form.password} onChange={set('password')} />
            {f('password') && <span style={styles.fieldError}>{f('password')}</span>}
          </div>
          <div style={styles.field}>
            <label style={styles.label}>Роля</label>
            <select style={styles.select} value={form.role} onChange={set('role')}>
              <option value="Client">Клиент — търся майстор</option>
              <option value="Handyman">Майстор — предлагам услуги</option>
            </select>
          </div>
          <button style={styles.btn} type="submit" disabled={loading}>
            {loading ? 'Зареждане...' : 'Регистрация'}
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
