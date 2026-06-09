import { useState, useEffect } from 'react'
import { api } from '../services/api.js'

const s = {
  dropdown: {
    position: 'absolute',
    top: 'calc(100% + 8px)',
    right: 0,
    width: '320px',
    background: '#fff',
    borderRadius: '12px',
    boxShadow: '0 8px 32px rgba(0,0,0,0.18)',
    border: '1px solid #fde68a',
    zIndex: 1000,
    overflow: 'hidden',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '12px 16px',
    borderBottom: '1px solid #fef3c7',
    background: '#fffbf0',
  },
  headerTitle: { fontWeight: 700, fontSize: '13px', color: '#1c1917' },
  readAll: {
    fontSize: '12px',
    color: '#d97706',
    background: 'none',
    border: 'none',
    cursor: 'pointer',
    fontWeight: 600,
  },
  list: { maxHeight: '360px', overflowY: 'auto' },
  item: (isRead) => ({
    padding: '12px 16px',
    borderBottom: '1px solid #fef3c7',
    background: isRead ? '#fff' : '#fffbf0',
    cursor: 'pointer',
    transition: 'background 0.15s',
  }),
  title: { fontSize: '13px', fontWeight: 700, color: '#1c1917', marginBottom: '3px' },
  message: { fontSize: '12px', color: '#78350f', lineHeight: 1.5 },
  date: { fontSize: '11px', color: '#a8a29e', marginTop: '4px' },
  empty: {
    padding: '24px',
    textAlign: 'center',
    color: '#78350f',
    fontSize: '13px',
  },
}

function formatDate(dateStr) {
  const d = new Date(dateStr)
  return d.toLocaleDateString('bg-BG', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })
}

export default function NotificationDropdown({ onRead }) {
  const [notifications, setNotifications] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    api.get('/notifications')
      .then(setNotifications)
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const handleMarkRead = async (id) => {
    await api.post(`/notifications/${id}/read`).catch(() => {})
    setNotifications(prev => prev.map(n => n.id === id ? { ...n, isRead: true } : n))
    onRead?.()
  }

  const handleMarkAll = async () => {
    await api.post('/notifications/read-all').catch(() => {})
    setNotifications(prev => prev.map(n => ({ ...n, isRead: true })))
    onRead?.()
  }

  const unread = notifications.filter(n => !n.isRead).length

  return (
    <div style={s.dropdown}>
      <div style={s.header}>
        <span style={s.headerTitle}>Известия</span>
        {unread > 0 && (
          <button style={s.readAll} onClick={handleMarkAll}>
            Маркирай всички като прочетени
          </button>
        )}
      </div>
      <div style={s.list}>
        {loading && <div style={s.empty}>Зареждане...</div>}
        {!loading && notifications.length === 0 && (
          <div style={s.empty}>Нямате известия.</div>
        )}
        {!loading && notifications.map(n => (
          <div
            key={n.id}
            style={s.item(n.isRead)}
            onClick={() => !n.isRead && handleMarkRead(n.id)}
          >
            <div style={s.title}>{n.title}</div>
            <div style={s.message}>{n.message}</div>
            <div style={s.date}>{formatDate(n.createdAt)}</div>
          </div>
        ))}
      </div>
    </div>
  )
}
