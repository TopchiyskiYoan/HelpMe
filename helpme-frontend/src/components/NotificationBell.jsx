import { useState, useEffect, useRef } from 'react'
import { api } from '../services/api.js'
import NotificationDropdown from './NotificationDropdown.jsx'

const s = {
  wrap: { position: 'relative' },
  btn: {
    background: 'rgba(255,255,255,0.18)',
    border: 'none',
    borderRadius: '8px',
    padding: '5px 10px',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    gap: '4px',
    color: '#fff',
    fontSize: '16px',
    position: 'relative',
  },
  badge: {
    position: 'absolute',
    top: '-4px',
    right: '-4px',
    background: '#dc2626',
    color: '#fff',
    borderRadius: '10px',
    fontSize: '10px',
    fontWeight: 700,
    minWidth: '16px',
    height: '16px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '0 3px',
  },
}

export default function NotificationBell() {
  const [unreadCount, setUnreadCount] = useState(0)
  const [open, setOpen] = useState(false)
  const wrapRef = useRef(null)

  const fetchCount = () => {
    api.get('/notifications/unread-count')
      .then(data => setUnreadCount(data.count))
      .catch(() => {})
  }

  useEffect(() => {
    fetchCount()
    const interval = setInterval(fetchCount, 30000)
    return () => clearInterval(interval)
  }, [])

  useEffect(() => {
    function handleOutside(e) {
      if (wrapRef.current && !wrapRef.current.contains(e.target))
        setOpen(false)
    }
    document.addEventListener('mousedown', handleOutside)
    return () => document.removeEventListener('mousedown', handleOutside)
  }, [])

  const handleOpen = () => {
    setOpen(prev => !prev)
    if (!open) fetchCount()
  }

  return (
    <div style={s.wrap} ref={wrapRef}>
      <button style={s.btn} onClick={handleOpen} title="Известия">
        🔔
        {unreadCount > 0 && (
          <span style={s.badge}>{unreadCount > 99 ? '99+' : unreadCount}</span>
        )}
      </button>
      {open && (
        <NotificationDropdown
          onRead={() => setUnreadCount(0)}
          onClose={() => setOpen(false)}
        />
      )}
    </div>
  )
}
