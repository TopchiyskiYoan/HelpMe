import { useState, useEffect, useCallback } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'
import StarRating from '../components/StarRating.jsx'
import ReviewList from '../components/ReviewList.jsx'

const s = {
  page: {
    padding: '2rem',
    flex: 1,
    maxWidth: '720px',
    margin: '0 auto',
    width: '100%',
    boxSizing: 'border-box',
    background: '#fffbf0',
  },
  back: {
    fontSize: '13px',
    fontWeight: 600,
    color: '#d97706',
    textDecoration: 'none',
    display: 'inline-flex',
    alignItems: 'center',
    gap: '4px',
    marginBottom: '20px',
    cursor: 'pointer',
  },
  profileCard: {
    background: '#fff',
    border: '1px solid #fde68a',
    borderRadius: '16px',
    padding: '24px',
    marginBottom: '24px',
    boxShadow: '0 2px 12px rgba(217,119,6,0.08)',
  },
  name: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px',
    fontWeight: 800,
    color: '#1c1917',
    letterSpacing: '-0.03em',
    marginBottom: '6px',
  },
  ratingRow: {
    display: 'flex',
    alignItems: 'center',
    gap: '10px',
    marginBottom: '12px',
  },
  ratingText: { fontSize: '13px', color: '#78350f', fontWeight: 500 },
  badge: {
    display: 'inline-flex',
    alignItems: 'center',
    gap: '4px',
    fontSize: '12px',
    fontWeight: 600,
    padding: '4px 12px',
    borderRadius: '20px',
    background: '#dcfce7',
    color: '#15803d',
    border: '1px solid #bbf7d0',
    marginBottom: '12px',
  },
  bio: {
    fontSize: '14px',
    color: '#1c1917',
    lineHeight: 1.65,
    padding: '14px 16px',
    background: '#fffbf0',
    border: '1px solid #fde68a',
    borderRadius: '10px',
    marginBottom: '14px',
  },
  chips: { display: 'flex', flexWrap: 'wrap', gap: '6px', marginBottom: '8px' },
  chip: {
    background: '#fef3c7',
    border: '1px solid #fde68a',
    borderRadius: '20px',
    padding: '4px 12px',
    fontSize: '12px',
    color: '#78350f',
    fontWeight: 500,
  },
  chipLabel: { fontSize: '12px', color: '#a16207', fontWeight: 600, marginBottom: '4px' },
  section: { marginBottom: '24px' },
  sectionTitle: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '18px',
    fontWeight: 700,
    color: '#1c1917',
    marginBottom: '14px',
    letterSpacing: '-0.02em',
  },
  loading: { padding: '4rem', textAlign: 'center', color: '#78350f', background: '#fffbf0', flex: 1 },
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

  return (
    <div style={s.page}>
      <a style={s.back} onClick={() => navigate(-1)}>← Назад</a>

      <div style={s.profileCard}>
        <div style={s.name}>{profile.firstName} {profile.lastName}</div>

        {profile.isVerified && (
          <div style={s.badge}>✓ Верифициран майстор</div>
        )}

        <div style={s.ratingRow}>
          <StarRating rating={Math.round(profile.averageRating)} />
          <span style={s.ratingText}>
            {profile.averageRating > 0
              ? `${profile.averageRating.toFixed(1)} (${profile.reviewCount} отзива)`
              : 'Все още няма отзиви'}
          </span>
        </div>

        {profile.bio && <div style={s.bio}>{profile.bio}</div>}

        {profile.yearsOfExperience > 0 && (
          <div style={{ fontSize: '13px', color: '#78350f', marginBottom: '12px' }}>
            🔧 {profile.yearsOfExperience} г. опит
          </div>
        )}

        {profile.subCategories?.length > 0 && (
          <div style={{ marginBottom: '10px' }}>
            <div style={s.chipLabel}>Специалности:</div>
            <div style={s.chips}>
              {profile.subCategories.map(sc => (
                <span key={sc.subCategoryId} style={s.chip}>{sc.subCategoryName}</span>
              ))}
            </div>
          </div>
        )}

        {profile.cities?.length > 0 && (
          <div>
            <div style={s.chipLabel}>Обслужвани градове:</div>
            <div style={s.chips}>
              {profile.cities.map(c => (
                <span key={c.cityId} style={s.chip}>📍 {c.cityName}</span>
              ))}
            </div>
          </div>
        )}
      </div>

      <div style={s.section}>
        <div style={s.sectionTitle}>Отзиви ({reviews.length})</div>
        <ReviewList
          reviews={reviews}
          canRespond={isHandyman}
          onResponded={load}
        />
      </div>
    </div>
  )
}
