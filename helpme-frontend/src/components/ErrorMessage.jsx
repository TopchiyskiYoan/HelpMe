const s = {
  box: {
    color: '#dc2626',
    fontSize: '14px',
    padding: '10px 14px',
    background: '#fee2e2',
    borderRadius: '8px',
    border: '1px solid #fecaca',
  },
}

export default function ErrorMessage({ message }) {
  if (!message) return null
  return <div style={s.box}>{message}</div>
}
