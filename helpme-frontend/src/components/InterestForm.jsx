import { useState } from 'react'
import { api } from '../services/api.js'

const s = {
  box: {
    border: '1px solid #fde68a',
    borderRadius: '14px',
    padding: '20px 24px',
    background: '#fff',
    textAlign: 'left',
    boxShadow: '0 4px 16px rgba(217,119,6,0.10)',
  },
  title: {
    fontSize: '16px',
    fontWeight: 700,
    color: '#1c1917',
    letterSpacing: '-0.02em',
    marginBottom: '16px',
  },
  field: { display: 'flex', flexDirection: 'column', gap: '5px', marginBottom: '12px' },
  label: {
    fontSize: '11px',
    fontWeight: 600,
    color: '#78350f',
    textTransform: 'uppercase',
    letterSpacing: '0.06em',
  },
  input: {
    padding: '10px 12px',
    borderRadius: '8px',
    border: '1px solid #fde68a',
    background: '#fffbf0',
    color: '#1c1917',
    fontSize: '14px',
    outline: 'none',
    width: '100%',
    boxSizing: 'border-box',
  },
  textarea: {
    padding: '10px 12px',
    borderRadius: '8px',
    border: '1px solid #fde68a',
    background: '#fffbf0',
    color: '#1c1917',
    fontSize: '14px',
    outline: 'none',
    width: '100%',
    boxSizing: 'border-box',
    resize: 'vertical',
    minHeight: '80px',
    fontFamily: 'inherit',
  },
  btn: {
    padding: '10px 24px',
    borderRadius: '8px',
    border: 'none',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)',
    color: '#fff',
    fontSize: '14px',
    fontWeight: 700,
    cursor: 'pointer',
    boxShadow: '0 4px 14px rgba(217,119,6,0.35)',
  },
  error: {
    fontSize: '13px',
    color: '#dc2626',
    marginBottom: '12px',
    padding: '8px 12px',
    background: '#fee2e2',
    border: '1px solid #fecaca',
    borderRadius: '8px',
  },
}

export default function InterestForm({ jobId, onSubmitted }) {
  const [price, setPrice] = useState('')
  const [note, setNote] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!price || Number(price) <= 0) { setError('Въведете валидна цена.'); return }
    setError('')
    setLoading(true)
    try {
      await api.post(`/jobs/${jobId}/interests`, { proposedPrice: Number(price), note: note || null })
      onSubmitted?.()
    } catch (err) {
      setError(err.message || 'Грешка при изпращане.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={s.box}>
      <div style={s.title}>Изрази интерес</div>
      {error && <div style={s.error}>{error}</div>}
      <form onSubmit={handleSubmit}>
        <div style={s.field}>
          <label style={s.label}>Предложена цена (лв.)</label>
          <input style={s.input} type="number" min="0.01" step="0.01" value={price}
            onChange={e => setPrice(e.target.value)} required />
        </div>
        <div style={s.field}>
          <label style={s.label}>Бележка (незадължително)</label>
          <textarea style={s.textarea} value={note}
            onChange={e => setNote(e.target.value)}
            placeholder="Кога можете, какви материали носите..." />
        </div>
        <button style={s.btn} type="submit" disabled={loading}>
          {loading ? 'Изпращане...' : 'Изпрати оферта'}
        </button>
      </form>
    </div>
  )
}
