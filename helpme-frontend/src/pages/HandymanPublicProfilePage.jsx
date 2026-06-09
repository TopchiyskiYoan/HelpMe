import { useState, useEffect, useCallback } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'
import StarRating from '../components/StarRating.jsx'
import ReviewList from '../components/ReviewList.jsx'
import { t } from '../theme.js'

const s = {
  page: { ...t.page, maxWidth: '760px' },
  back: {
    display: 'inline-flex', alignItems: 'center', gap: '6px',
    fontSize: '13px', fontWeight: 500, color: t.amberDark,
    textDecoration: 'none', marginBottom: '24px', cursor: 'pointer',
    background: 'none', border: 'none', padding: 0, fontFamily: 'inherit',
  },
  card: {
    background: t.card, border: `1px solid ${t.border}`,
    borderRadius: t.radiusLg, padding: '28px', marginBottom: '20px',
    boxShadow: t.shadow,
  },
  avatarRow: { display: 'flex', alignItems: 'flex-start', gap: '20px', marginBottom: '20px' },
  avatar: {
    width: '72px', height: '72px', borderRadius: '50%',
    background: 'linear-gradient(135deg, #f59e0b, #d97706)',
    display: 'flex', alignItems: 'center', justifyContent: 'center',
    fontSize: '26px', fontWeight: 800, color: '#fff', flexShrink: 0,
    boxShadow: '0 2px 8px rgba(217,119,6,0.35)',
  },
  nameBlock: { flex: 1 },
  name: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px', fontWeight: 800, color: t.text,
    letterSpacing: '-0.03em', marginBottom: '6px',
  },
  verifiedBadge: {
    display: 'inline-flex', alignItems: 'center', gap: '5px',
    fontSize: '12px', fontWeight: 600, padding: '4px 12px',
    borderRadius: t.radiusFull, background: t.greenBg,
    color: t.greenText, border: `1px solid ${t.greenBorder}`,
    marginBottom: '10px',
  },
  ratingRow: { display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px' },
  ratingText: { fontSize: '13px', color: t.textMuted, fontWeight: 500 },
  experience: {
    fontSize: '13px', color: t.textMuted,
    display: 'flex', alignItems: 'center', gap: '6px',
  },
  bio: {
    fontSize: '14.5px', color: t.text, lineHeight: 1.7,
    padding: '16px 18px', background: t.bg,
    border: `1px solid ${t.border}`, borderRadius: t.radius,
    marginBottom: '18px',
  },
  chipGroup: { marginBottom: '14px' },
  chipLabel: { fontSize: '12px', fontWeight: 600, color: t.textMuted, marginBottom: '7px', textTransform: 'uppercase', letterSpacing: '0.05em' },
  chips: { display: 'flex', flexWrap: 'wrap', gap: '6px' },
  chip: { ...t.chip },
  chipAmber: { ...t.chipAmber },
  sectionTitle: { ...t.section, fontSize: '17px' },
  loading: { ...t.fullPage, display: 'flex', alignItems: 'center', justifyContent: 'center', color: t.textMuted, fontSize: '14px' },
}

export default function HandymanPublicProfilePage() {
  const { userId } = useParams()
  const navigate = useNavigate()
  const { user } = useAuth()
  const [profile, setProfile] = useState(null)
  const [reviews, setReviews] = useState([])
  const [loading, setLoading] = useState(true)

  const isHandyman = user?.id === userId && user?.role === 'Handyman'

  const load = useCallback(async () => {
    try {
      const [profileData, reviewData] = await Promise.all([
        api.get(`/handymen/${userId}`),
        api.get(`/reviews/handyman/${userId}`)
      ])
      setProfile(profileData)
      setReviews(reviewData)
    } catch {
      navigate('/')
    } finally {
      setLoading(false)
    }
  }, [userId, navigate])

  useEffect(() => { load() }, [load])

  if (loading) return <div style={s.loading}>Зареждане...</div>
  if (!profile) return null

  const initials = `${profile.firstName?.[0] ?? ''}${profile.lastName?.[0] ?? ''}`.toUpperCase()

  return (
    <div style={s.page}>
      <button style={s.back} onClick={() => navigate(-1)}>← Назад</button>

      <div style={s.card}>
        <div style={s.avatarRow}>
          <div style={s.avatar}>{initials}</div>
          <div style={s.nameBlock}>
            <div style={s.name}>{profile.firstName} {profile.lastName}</div>
            {profile.isVerified && (
              <div style={s.verifiedBadge}>✓ Верифициран майстор</div>
            )}
            <div style={s.ratingRow}>
              <StarRating rating={Math.round(profile.averageRating ?? 0)} />
              <span style={s.ratingText}>
                {(profile.averageRating ?? 0) > 0
                  ? `${(profile.averageRating).toFixed(1)} (${profile.reviewCount} отзива)`
                  : 'Все още няма отзиви'}
              </span>
            </div>
            {profile.yearsOfExperience > 0 && (
              <div style={s.experience}>
                <span>🔧</span> {profile.yearsOfExperience} г. опит
              </div>
            )}
          </div>
        </div>

        {profile.bio && <div style={s.bio}>{profile.bio}</div>}

        {profile.subCategories?.length > 0 && (
          <div style={s.chipGroup}>
            <div style={s.chipLabel}>Специалности</div>
            <div style={s.chips}>
              {profile.subCategories.map(sc => (
                <span key={sc.id} style={s.chipAmber}>{sc.name}</span>
              ))}
            </div>
          </div>
        )}

        {profile.cities?.length > 0 && (
          <div style={s.chipGroup}>
            <div style={s.chipLabel}>Обслужвани градове</div>
            <div style={s.chips}>
              {profile.cities.map(c => (
                <span key={c.id} style={s.chip}>📍 {c.name}</span>
              ))}
            </div>
          </div>
        )}

        {isHandyman && (
          <div style={{ marginTop: '20px', paddingTop: '16px', borderTop: `1px solid ${t.border}` }}>
            <a href="/profile" style={{ ...t.btnSecondary, display: 'inline-block', textDecoration: 'none', fontSize: '13px' }}>
              Редактирай профила
            </a>
          </div>
        )}
      </div>

      <div style={{ marginTop: '8px' }}>
        <div style={s.sectionTitle}>Отзиви ({reviews.length})</div>
        <ReviewList reviews={reviews} canRespond={isHandyman} onResponded={load} />
      </div>
    </div>
  )
}
