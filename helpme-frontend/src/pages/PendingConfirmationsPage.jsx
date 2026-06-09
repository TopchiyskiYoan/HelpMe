import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api.js'
import { t } from '../theme.js'

const s = {
  page: { ...t.fullPage },
  inner: { maxWidth: '760px', margin: '0 auto' },
  header: { marginBottom: '28px' },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px', fontWeight: 800, color: t.text,
    letterSpacing: '-0.03em', marginBottom: '4px',
  },
  subtitle: { fontSize: '13.5px', color: t.textMuted },
  list: { display: 'flex', flexDirection: 'column', gap: '10px' },
  empty: {
    textAlign: 'center', padding: '5rem 2rem',
    color: t.textMuted, fontSize: '15px', lineHeight: 1.8,
  },
  emptyIcon: { fontSize: '44px', marginBottom: '14px', display: 'block' },
}

export default function PendingConfirmationsPage() {
  const [interests, setInterests] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    api.get('/handymen/me/interests')
      .then(data => setInterests(data.filter(i => i.status === 'Selected')))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  return (
    <div style={s.page}>
      <div style={s.inner}>
        <div style={s.header}>
          <div style={s.title}>Чакащи потвърждение</div>
          <div style={s.subtitle}>Поръчки, за които сте избрани — потвърдете или откажете</div>
        </div>

        {loading && <div style={s.empty}>Зареждане...</div>}
        {!loading && interests.length === 0 && (
          <div style={s.empty}>
            <span style={s.emptyIcon}>✅</span>
            Няма поръчки, чакащи вашето потвърждение.
          </div>
        )}
        {!loading && (
          <div style={s.list}>
            {interests.map(i => <InterestCard key={i.id} interest={i} />)}
          </div>
        )}
      </div>
    </div>
  )
}

function InterestCard({ interest: i }) {
  const [hovered, setHovered] = useState(false)

  return (
    <div
      style={{
        border: `1px solid ${hovered ? t.amberBorder : t.border}`,
        borderRadius: t.radius,
        padding: '18px 22px',
        background: t.card,
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        gap: '1rem',
        flexWrap: 'wrap',
        boxShadow: hovered ? t.shadowMd : t.shadow,
        transition: 'all 0.18s ease',
        transform: hovered ? 'translateY(-1px)' : 'none',
      }}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <div style={{ display: 'flex', flexDirection: 'column', gap: '5px' }}>
        <Link
          to={`/jobs/${i.jobId}`}
          style={{ fontSize: '15px', fontWeight: 700, color: t.text, letterSpacing: '-0.01em', textDecoration: 'none' }}
        >
          Поръчка #{i.jobId}
        </Link>
        <div style={{ fontSize: '13px', color: t.textMuted }}>
          Вашата оферта: <span style={{ fontWeight: 700, color: t.amberDark }}>{i.proposedPrice} лв.</span>
        </div>
      </div>

      <Link
        to={`/jobs/${i.jobId}`}
        style={{
          ...t.btnPrimary, textDecoration: 'none', display: 'inline-flex',
          alignItems: 'center', gap: '5px', fontSize: '13px',
        }}
      >
        Потвърди / Откажи →
      </Link>
    </div>
  )
}
