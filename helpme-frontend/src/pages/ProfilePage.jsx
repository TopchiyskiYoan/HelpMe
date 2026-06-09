import { useState, useEffect } from 'react'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'
import { t } from '../theme.js'

const s = {
  page: { ...t.fullPage },
  inner: { maxWidth: '680px', margin: '0 auto' },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px', fontWeight: 800, color: t.text,
    letterSpacing: '-0.03em', marginBottom: '6px',
  },
  subtitle: { fontSize: '13.5px', color: t.textMuted, marginBottom: '32px' },
  card: {
    background: t.card, border: `1px solid ${t.border}`,
    borderRadius: t.radiusLg, padding: '28px', marginBottom: '20px',
    boxShadow: t.shadow,
  },
  cardTitle: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '16px', fontWeight: 700, color: t.text,
    marginBottom: '20px', letterSpacing: '-0.01em',
  },
  row: { display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' },
  field: { marginBottom: '16px' },
  label: { ...t.label },
  input: { ...t.input },
  btn: { ...t.btnPrimary, padding: '10px 24px' },
  success: {
    fontSize: '13px', color: t.greenText, padding: '10px 14px',
    background: t.greenBg, border: `1px solid ${t.greenBorder}`,
    borderRadius: t.radius, marginBottom: '16px',
  },
  error: {
    fontSize: '13px', color: t.redText, padding: '10px 14px',
    background: t.redBg, border: `1px solid ${t.redBorder}`,
    borderRadius: t.radius, marginBottom: '16px',
  },
  avatarSection: {
    display: 'flex', alignItems: 'center', gap: '18px', marginBottom: '24px',
    paddingBottom: '24px', borderBottom: `1px solid ${t.border}`,
  },
  avatar: {
    width: '72px', height: '72px', borderRadius: '50%',
    background: 'linear-gradient(135deg, #f59e0b, #d97706)',
    display: 'flex', alignItems: 'center', justifyContent: 'center',
    fontSize: '26px', fontWeight: 800, color: '#fff',
    boxShadow: '0 2px 8px rgba(217,119,6,0.35)', flexShrink: 0,
  },
  avatarInfo: { flex: 1 },
  avatarName: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '18px', fontWeight: 800, color: t.text, marginBottom: '3px',
  },
  avatarRole: { fontSize: '13px', color: t.textMuted },
  divider: { borderTop: `1px solid ${t.border}`, margin: '20px 0' },
}

const ROLE_LABEL = { Client: 'Клиент', Handyman: 'Майстор', Administrator: 'Администратор' }

function useField(initial = '') {
  const [value, setValue] = useState(initial)
  return [value, (e) => setValue(e.target.value), setValue]
}

export default function ProfilePage() {
  const { user, login, token } = useAuth()
  const [loading, setLoading] = useState(false)
  const [pwdLoading, setPwdLoading] = useState(false)
  const [profileMsg, setProfileMsg] = useState({ type: '', text: '' })
  const [pwdMsg, setPwdMsg] = useState({ type: '', text: '' })

  const [firstName, onFirstName, setFirstName] = useField('')
  const [lastName, onLastName, setLastName] = useField('')
  const [phoneNumber, onPhoneNumber, setPhoneNumber] = useField('')
  const [profilePictureUrl, onProfilePictureUrl, setProfilePictureUrl] = useField('')

  const [currentPwd, onCurrentPwd, setCurrentPwd] = useField('')
  const [newPwd, onNewPwd, setNewPwd] = useField('')
  const [confirmPwd, onConfirmPwd, setConfirmPwd] = useField('')

  useEffect(() => {
    if (!user) return
    setFirstName(user.firstName || '')
    setLastName(user.lastName || '')
    setPhoneNumber(user.phoneNumber || '')
    setProfilePictureUrl(user.profilePictureUrl || '')
  }, [user]) // eslint-disable-line react-hooks/exhaustive-deps

  const handleProfile = async (e) => {
    e.preventDefault()
    setProfileMsg({ type: '', text: '' })
    setLoading(true)
    try {
      const updated = await api.put('/users/me', { firstName, lastName, phoneNumber: phoneNumber || null, profilePictureUrl: profilePictureUrl || null })
      login({ ...user, firstName: updated.firstName, lastName: updated.lastName, phoneNumber: updated.phoneNumber, profilePictureUrl: updated.profilePictureUrl }, token)
      setProfileMsg({ type: 'success', text: 'Профилът е обновен успешно!' })
    } catch {
      setProfileMsg({ type: 'error', text: 'Грешка при обновяването. Моля, опитайте отново.' })
    } finally {
      setLoading(false)
    }
  }

  const handlePassword = async (e) => {
    e.preventDefault()
    setPwdMsg({ type: '', text: '' })
    if (newPwd !== confirmPwd) { setPwdMsg({ type: 'error', text: 'Новите пароли не съвпадат.' }); return }
    if (newPwd.length < 6) { setPwdMsg({ type: 'error', text: 'Паролата трябва да е поне 6 символа.' }); return }
    setPwdLoading(true)
    try {
      await api.put('/users/me/password', { currentPassword: currentPwd, newPassword: newPwd })
      setPwdMsg({ type: 'success', text: 'Паролата е сменена успешно!' })
      setCurrentPwd(''); setNewPwd(''); setConfirmPwd('')
    } catch {
      setPwdMsg({ type: 'error', text: 'Текущата парола е грешна или новата не отговаря на изискванията.' })
    } finally {
      setPwdLoading(false)
    }
  }

  const initials = `${user?.firstName?.[0] ?? ''}${user?.lastName?.[0] ?? ''}`.toUpperCase()

  const focusIn = (e) => { e.target.style.borderColor = t.amber }
  const focusOut = (e) => { e.target.style.borderColor = t.border }

  return (
    <div style={s.page}>
      <div style={s.inner}>
        <div style={s.title}>Настройки на профила</div>
        <div style={s.subtitle}>Управлявайте вашата лична информация и сигурност</div>

        <div style={s.card}>
          <div style={s.avatarSection}>
            <div style={s.avatar}>{initials}</div>
            <div style={s.avatarInfo}>
              <div style={s.avatarName}>{user?.firstName} {user?.lastName}</div>
              <div style={s.avatarRole}>{ROLE_LABEL[user?.role] ?? user?.role} · {user?.email}</div>
            </div>
          </div>

          <div style={s.cardTitle}>Лична информация</div>
          {profileMsg.text && (
            <div style={profileMsg.type === 'success' ? s.success : s.error}>{profileMsg.text}</div>
          )}
          <form onSubmit={handleProfile}>
            <div style={s.row}>
              <div style={s.field}>
                <label style={s.label}>Име</label>
                <input style={s.input} value={firstName} onChange={onFirstName} required
                  onFocus={focusIn} onBlur={focusOut} />
              </div>
              <div style={s.field}>
                <label style={s.label}>Фамилия</label>
                <input style={s.input} value={lastName} onChange={onLastName} required
                  onFocus={focusIn} onBlur={focusOut} />
              </div>
            </div>
            <div style={s.field}>
              <label style={s.label}>Телефон</label>
              <input style={s.input} type="tel" value={phoneNumber} onChange={onPhoneNumber}
                placeholder="08XXXXXXXX" onFocus={focusIn} onBlur={focusOut} />
            </div>
            <div style={s.field}>
              <label style={s.label}>URL на профилна снимка (незадължително)</label>
              <input style={s.input} type="url" value={profilePictureUrl} onChange={onProfilePictureUrl}
                placeholder="https://example.com/photo.jpg" onFocus={focusIn} onBlur={focusOut} />
            </div>
            <button style={s.btn} type="submit" disabled={loading}>
              {loading ? 'Запазване...' : 'Запази промените'}
            </button>
          </form>
        </div>

        <div style={s.card}>
          <div style={s.cardTitle}>Смяна на парола</div>
          {pwdMsg.text && (
            <div style={pwdMsg.type === 'success' ? s.success : s.error}>{pwdMsg.text}</div>
          )}
          <form onSubmit={handlePassword}>
            <div style={s.field}>
              <label style={s.label}>Текуща парола</label>
              <input style={s.input} type="password" value={currentPwd} onChange={onCurrentPwd} required
                onFocus={focusIn} onBlur={focusOut} />
            </div>
            <div style={s.field}>
              <label style={s.label}>Нова парола</label>
              <input style={s.input} type="password" value={newPwd} onChange={onNewPwd} required
                placeholder="Мин. 6 символа" onFocus={focusIn} onBlur={focusOut} />
            </div>
            <div style={s.field}>
              <label style={s.label}>Потвърди новата парола</label>
              <input style={s.input} type="password" value={confirmPwd} onChange={onConfirmPwd} required
                onFocus={focusIn} onBlur={focusOut} />
            </div>
            <button style={s.btn} type="submit" disabled={pwdLoading}>
              {pwdLoading ? 'Смяна...' : 'Смени паролата'}
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}
