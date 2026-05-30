import { useState } from 'react'
import StarRating from './StarRating.jsx'
import { api } from '../services/api.js'

const s = {
  list: { display: 'flex', flexDirection: 'column', gap: '14px' },
  card: {
    background: '#fff',
    border: '1px solid #fde68a',
    borderRadius: '12px',
    padding: '16px 18px',
    boxShadow: '0 1px 4px rgba(217,119,6,0.06)',
  },
  header: { display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' },
  clientName: { fontSize: '14px', fontWeight: 600, color: '#1c1917' },
  date: { fontSize: '12px', color: '#a16207' },
  comment: { fontSize: '14px', color: '#1c1917', lineHeight: 1.6, marginBottom: '10px' },
  response: {
    marginTop: '10px',
    padding: '10px 14px',
    background: '#fef3c7',
    border: '1px solid #fde68a',
    borderRadius: '8px',
    fontSize: '13px',
    color: '#78350f',
  },
  responseLabel: { fontWeight: 600, marginBottom: '4px', fontSize: '12px', color: '#d97706' },
  respondForm: { marginTop: '10px', display: 'flex', flexDirection: 'column', gap: '8px' },
  textarea: {
    width: '100%',
    minHeight: '70px',
    padding: '8px 10px',
    borderRadius: '8px',
    border: '1px solid #fde68a',
    fontSize: '13px',
    fontFamily: 'inherit',
    resize: 'vertical',
    outline: 'none',
    boxSizing: 'border-box',
    background: '#fffbf0',
  },
  btnSmall: {
    alignSelf: 'flex-start',
    padding: '6px 16px',
    borderRadius: '7px',
    border: 'none',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)',
    color: '#fff',
    fontSize: '13px',
    fontWeight: 600,
    cursor: 'pointer',
  },
  empty: { fontSize: '14px', color: '#78350f', padding: '10px 0' },
}

function ReviewCard({ review, canRespond, onResponded }) {
  const [replying, setReplying] = useState(false)
  const [content, setContent] = useState('')
  const [submitting, setSubmitting] = useState(false)

  const submitResponse = async () => {
    if (!content.trim()) return
    setSubmitting(true)
    try {
      await api.post(`/reviews/${review.id}/respond`, { content })
      setReplying(false)
      if (onResponded) onResponded()
    } catch {
      // ignore
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div style={s.card}>
      <div style={s.header}>
        <span style={s.clientName}>{review.clientName || 'Клиент'}</span>
        <span style={s.date}>{new Date(review.createdAt).toLocaleDateString('bg-BG')}</span>
      </div>
      <StarRating rating={review.rating} />
      <div style={{ ...s.comment, marginTop: '8px' }}>{review.comment}</div>

      {review.response ? (
        <div style={s.response}>
          <div style={s.responseLabel}>Отговор на майстора:</div>
          {review.response.content}
        </div>
      ) : canRespond && !replying && (
        <button
          style={{ ...s.btnSmall, background: 'transparent', color: '#d97706', border: '1px solid #fde68a' }}
          onClick={() => setReplying(true)}
        >
          Отговори
        </button>
      )}

      {replying && (
        <div style={s.respondForm}>
          <textarea
            style={s.textarea}
            placeholder="Напишете отговор..."
            value={content}
            onChange={e => setContent(e.target.value)}
            maxLength={2000}
          />
          <div style={{ display: 'flex', gap: '8px' }}>
            <button style={s.btnSmall} onClick={submitResponse} disabled={submitting}>
              {submitting ? '...' : 'Изпрати'}
            </button>
            <button
              style={{ ...s.btnSmall, background: 'transparent', color: '#78350f', border: '1px solid #fde68a' }}
              onClick={() => setReplying(false)}
            >
              Откажи
            </button>
          </div>
        </div>
      )}
    </div>
  )
}

export default function ReviewList({ reviews, canRespond = false, onResponded }) {
  if (reviews.length === 0) {
    return <div style={s.empty}>Все още няма отзиви.</div>
  }

  return (
    <div style={s.list}>
      {reviews.map(r => (
        <ReviewCard
          key={r.id}
          review={r}
          canRespond={canRespond}
          onResponded={onResponded}
        />
      ))}
    </div>
  )
}
