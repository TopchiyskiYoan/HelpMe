import { useState } from 'react'

export default function InterestedHandymanCard({ interest, canSelect, onSelect }) {
  const [hovered, setHovered] = useState(false)

  const card = {
    border: `1px solid ${hovered ? '#fbbf24' : '#fde68a'}`,
    borderRadius: '12px',
    padding: '14px 18px',
    background: '#ffffff',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    gap: '1rem',
    textAlign: 'left',
    flexWrap: 'wrap',
    boxShadow: hovered ? '0 4px 14px rgba(217,119,6,0.12)' : '0 1px 4px rgba(217,119,6,0.06)',
    transition: 'all 0.18s ease',
  }

  const s = {
    left: { display: 'flex', flexDirection: 'column', gap: '4px' },
    name: { fontSize: '15px', fontWeight: 700, color: '#1c1917', letterSpacing: '-0.01em' },
    price: { fontSize: '17px', fontWeight: 700, color: '#d97706' },
    note: { fontSize: '13px', color: '#78350f', fontStyle: 'italic', marginTop: '2px' },
    btn: {
      padding: '8px 20px',
      borderRadius: '8px',
      border: 'none',
      background: 'linear-gradient(135deg, #d97706, #f59e0b)',
      color: '#fff',
      fontSize: '13px',
      fontWeight: 700,
      cursor: 'pointer',
      boxShadow: '0 3px 10px rgba(217,119,6,0.3)',
      flexShrink: 0,
    },
    selectedBadge: {
      fontSize: '12px',
      fontWeight: 600,
      padding: '4px 12px',
      borderRadius: '20px',
      background: '#dcfce7',
      color: '#15803d',
      border: '1px solid #bbf7d0',
    },
    rejectedBadge: {
      fontSize: '12px',
      fontWeight: 500,
      padding: '4px 12px',
      borderRadius: '20px',
      background: '#f3f4f6',
      color: '#6b7280',
      border: '1px solid #e5e7eb',
    },
  }

  return (
    <div
      style={card}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <div style={s.left}>
        <div style={s.name}>{interest.handymanName}</div>
        <div style={s.price}>{interest.proposedPrice} лв.</div>
        {interest.note && <div style={s.note}>„{interest.note}"</div>}
      </div>
      <div>
        {canSelect && interest.status === 'Pending' && (
          <button style={s.btn} onClick={() => onSelect(interest.id)}>Избери</button>
        )}
        {interest.status === 'Selected' && <span style={s.selectedBadge}>Избран ✓</span>}
        {interest.status === 'Rejected' && <span style={s.rejectedBadge}>Отхвърлен</span>}
      </div>
    </div>
  )
}
