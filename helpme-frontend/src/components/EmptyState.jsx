const s = {
  wrap: {
    textAlign: 'center',
    padding: '4rem 2rem',
    color: '#78350f',
    fontSize: '15px',
    lineHeight: 1.7,
  },
  icon: { fontSize: '40px', marginBottom: '12px', display: 'block' },
}

export default function EmptyState({ icon = '📭', message = 'Няма намерени резултати.' }) {
  return (
    <div style={s.wrap}>
      <span style={s.icon}>{icon}</span>
      {message}
    </div>
  )
}
