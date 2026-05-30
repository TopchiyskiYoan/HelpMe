import { useState } from 'react'
import { api } from '../services/api.js'
import StarRating from './StarRating.jsx'

const s = {
  card: {
    background: '#fff',
    border: '1px solid #fde68a',
    borderRadius: '14px',
    padding: '20px',
    boxShadow: '0 2px 8px rgba(217,119,6,0.08)',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '16px',
    fontWeight: 700,
    color: '#1c1917',
    marginBottom: '14px',
    letterSpacing: '-0.02em',
  },
  ratingRow: {
    display: 'flex',
    alignItems: 'center',
    gap: '10px',
    marginBottom: '14px',
  },
  label: { fontSize: '13px', color: '#78350f', fontWeight: 500 },
  textarea: {
    width: '100%',
    minHeight: '90px',
    padding: '10px 12px',
    borderRadius: '8px',
    border: '1px solid #fde68a',
    fontSize: '14px',
    color: '#1c1917',
    fontFamily: 'inherit',
    resize: 'vertical',
    outline: 'none',
    boxSizing: 'border-box',
    marginBottom: '12px',
    background: '#fffbf0',
  },
  btn: {
    padding: '9px 22px',
    borderRadius: '8px',
    border: 'none',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)',
    color: '#fff',
    fontSize: '14px',
    fontWeight: 700,
    cursor: 'pointer',
    boxShadow: '0 3px 10px rgba(217,119,6,0.3)',
  },
  error: {
    color: '#dc2626',
    fontSize: '13px',
    marginBottom: '10px',
    padding: '8px 12px',
    background: '#fee2e2',
    border: '1px solid #fecaca',
    borderRadius: '8px',
  },
  success: {
    color: '#15803d',
    fontSize: '14px',
    fontWeight: 500,
    padding: '12px 16px',
    background: '#dcfce7',
    border: '1px solid #bbf7d0',
    borderRadius: '10px',
  },
}

export default function ReviewForm({ jobId, onSubmitted }) {
  const [rating, setRating] = useState(0)
  const [comment, setComment] = useState('')
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [done, setDone] = useState(false)

  const submit = async () => {
    if (rating === 0) { setError('Моля изберете рейтинг.'); return }
    if (!comment.trim()) { setError('Моля напишете коментар.'); return }

    setError('')
    setSubmitting(true)
    try {
      await api.post('/reviews', { jobId, rating, comment })
      setDone(true)
      if (onSubmitted) onSubmitted()
    } catch (err) {
      setError(err.message || 'Грешка при изпращане.')
    } finally {
      setSubmitting(false)
    }
  }

  if (done) {
    return <div style={s.success}>✓ Отзивът е изпратен. Благодарим!</div>
  }

  return (
    <div style={s.card}>
      <div style={s.title}>Оставете отзив</div>
      <div style={s.ratingRow}>
        <span style={s.label}>Рейтинг:</span>
        <StarRating rating={rating} onChange={setRating} />
        {rating > 0 && <span style={{ fontSize: '13px', color: '#78350f' }}>{rating}/5</span>}
      </div>
      <textarea
        style={s.textarea}
        placeholder="Как беше работата с майстора?"
        value={comment}
        onChange={e => setComment(e.target.value)}
        maxLength={2000}
      />
      {error && <div style={s.error}>{error}</div>}
      <button style={s.btn} onClick={submit} disabled={submitting}>
        {submitting ? 'Изпращане...' : 'Изпрати отзив'}
      </button>
    </div>
  )
}
