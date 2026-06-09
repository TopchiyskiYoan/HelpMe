import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api.js'
import StarRating from '../components/StarRating.jsx'
import { t } from '../theme.js'

const s = {
  page: { ...t.fullPage },
  inner: { maxWidth: '960px', margin: '0 auto' },
  header: { marginBottom: '28px' },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '28px', fontWeight: 800, color: t.text,
    letterSpacing: '-0.03em', marginBottom: '6px',
  },
  subtitle: { fontSize: '14px', color: t.textMuted },
  searchBar: {
    display: 'flex', gap: '10px', marginBottom: '24px', flexWrap: 'wrap',
  },
  searchInput: {
    ...t.input, maxWidth: '360px', flex: 1,
    padding: '10px 14px',
    backgroundImage: 'url("data:image/svg+xml,%3Csvg xmlns=\'http://www.w3.org/2000/svg\' width=\'16\' height=\'16\' viewBox=\'0 0 24 24\' fill=\'none\' stroke=\'%2394a3b8\' stroke-width=\'2\'%3E%3Ccircle cx=\'11\' cy=\'11\' r=\'8\'/%3E%3Cline x1=\'21\' y1=\'21\' x2=\'16.65\' y2=\'16.65\'/%3E%3C/svg%3E")',
    backgroundRepeat: 'no-repeat',
    backgroundPosition: '12px center',
    paddingLeft: '38px',
  },
  grid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
    gap: '16px',
  },
  card: {
    background: t.card, border: `1px solid ${t.border}`,
    borderRadius: t.radiusLg, padding: '20px',
    boxShadow: t.shadow, textDecoration: 'none',
    display: 'block', transition: 'all 0.18s',
  },
  avatarRow: { display: 'flex', alignItems: 'center', gap: '14px', marginBottom: '14px' },
  avatar: {
    width: '52px', height: '52px', borderRadius: '50%',
    background: 'linear-gradient(135deg, #f59e0b, #d97706)',
    display: 'flex', alignItems: 'center', justifyContent: 'center',
    fontSize: '18px', fontWeight: 800, color: '#fff', flexShrink: 0,
  },
  nameBlock: { flex: 1, minWidth: 0 },
  name: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '16px', fontWeight: 700, color: t.text,
    letterSpacing: '-0.02em', marginBottom: '3px',
    whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis',
  },
  verifiedBadge: {
    display: 'inline-flex', alignItems: 'center', gap: '4px',
    fontSize: '11px', fontWeight: 600, padding: '2px 9px',
    borderRadius: t.radiusFull, background: t.greenBg,
    color: t.greenText, border: `1px solid ${t.greenBorder}`,
  },
  ratingRow: { display: 'flex', alignItems: 'center', gap: '6px', marginBottom: '10px' },
  ratingText: { fontSize: '12px', color: t.textMuted },
  bio: {
    fontSize: '13px', color: t.textMuted, lineHeight: 1.55,
    marginBottom: '12px', display: '-webkit-box', WebkitLineClamp: 2,
    WebkitBoxOrient: 'vertical', overflow: 'hidden',
  },
  chips: { display: 'flex', flexWrap: 'wrap', gap: '5px', marginBottom: '12px' },
  chip: { ...t.chipAmber, fontSize: '11px', padding: '3px 9px' },
  cityChip: { ...t.chip, fontSize: '11px', padding: '3px 9px' },
  footer: {
    display: 'flex', alignItems: 'center', justifyContent: 'space-between',
    paddingTop: '12px', borderTop: `1px solid ${t.border}`,
  },
  exp: { fontSize: '12px', color: t.textMuted },
  viewBtn: {
    fontSize: '12px', fontWeight: 600, color: t.amberDark,
    textDecoration: 'none', padding: '5px 12px',
    border: `1px solid ${t.amberBorder}`, borderRadius: t.radiusSm,
    background: t.amberBg,
  },
  empty: { textAlign: 'center', padding: '5rem 2rem', color: t.textMuted, fontSize: '15px' },
  emptyIcon: { fontSize: '44px', marginBottom: '14px', display: 'block' },
}

export default function HandymanListPage() {
  const [handymen, setHandymen] = useState([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    api.get('/handymen')
      .then(setHandymen)
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const filtered = handymen.filter(h => {
    if (!search.trim()) return true
    const q = search.toLowerCase()
    return (
      h.firstName?.toLowerCase().includes(q) ||
      h.lastName?.toLowerCase().includes(q) ||
      h.bio?.toLowerCase().includes(q) ||
      h.subCategories?.some(sc => sc.name?.toLowerCase().includes(q)) ||
      h.cities?.some(c => c.name?.toLowerCase().includes(q))
    )
  })

  return (
    <div style={s.page}>
      <div style={s.inner}>
        <div style={s.header}>
          <div style={s.title}>Намери майстор</div>
          <div style={s.subtitle}>{handymen.length} верифицирани майстори на платформата</div>
        </div>

        <div style={s.searchBar}>
          <input
            style={s.searchInput}
            placeholder="Търси по специалност, град или име..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            onFocus={e => e.target.style.borderColor = t.amber}
            onBlur={e => e.target.style.borderColor = t.border}
          />
        </div>

        {loading && <div style={s.empty}>Зареждане...</div>}
        {!loading && filtered.length === 0 && (
          <div style={s.empty}>
            <span style={s.emptyIcon}>🔍</span>
            {search ? 'Няма майстори, отговарящи на търсенето.' : 'Все още няма верифицирани майстори.'}
          </div>
        )}

        <div style={s.grid}>
          {filtered.map(h => (
            <HandymanCard key={h.userId} handyman={h} />
          ))}
        </div>
      </div>
    </div>
  )
}

function HandymanCard({ handyman: h }) {
  const [hovered, setHovered] = useState(false)
  const initials = `${h.firstName?.[0] ?? ''}${h.lastName?.[0] ?? ''}`.toUpperCase()

  return (
    <div
      style={{ ...s.card, borderColor: hovered ? t.amberBorder : t.border, boxShadow: hovered ? t.shadowMd : t.shadow, transform: hovered ? 'translateY(-2px)' : 'none' }}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <div style={s.avatarRow}>
        <div style={s.avatar}>{initials}</div>
        <div style={s.nameBlock}>
          <div style={s.name}>{h.firstName} {h.lastName}</div>
          {h.isVerified && <div style={s.verifiedBadge}>✓ Верифициран</div>}
        </div>
      </div>

      {(h.averageRating ?? 0) > 0 && (
        <div style={s.ratingRow}>
          <StarRating rating={Math.round(h.averageRating)} />
          <span style={s.ratingText}>{h.averageRating.toFixed(1)} ({h.reviewCount} отзива)</span>
        </div>
      )}

      {h.bio && <div style={s.bio}>{h.bio}</div>}

      {h.subCategories?.length > 0 && (
        <div style={s.chips}>
          {h.subCategories.slice(0, 3).map(sc => (
            <span key={sc.id} style={s.chip}>{sc.name}</span>
          ))}
          {h.subCategories.length > 3 && (
            <span style={s.chip}>+{h.subCategories.length - 3}</span>
          )}
        </div>
      )}

      {h.cities?.length > 0 && (
        <div style={{ ...s.chips, marginBottom: '14px' }}>
          {h.cities.slice(0, 2).map(c => (
            <span key={c.id} style={s.cityChip}>📍 {c.name}</span>
          ))}
          {h.cities.length > 2 && <span style={s.cityChip}>+{h.cities.length - 2}</span>}
        </div>
      )}

      <div style={s.footer}>
        <span style={s.exp}>🔧 {h.yearsOfExperience} г. опит</span>
        <Link to={`/handymen/${h.userId}`} style={s.viewBtn}>Виж профил →</Link>
      </div>
    </div>
  )
}
