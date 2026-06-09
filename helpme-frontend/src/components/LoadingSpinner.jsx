const s = {
  wrap: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    padding: '3rem',
  },
  spinner: {
    width: '36px',
    height: '36px',
    border: '3px solid #fde68a',
    borderTopColor: '#d97706',
    borderRadius: '50%',
    animation: 'spin 0.7s linear infinite',
  },
}

export default function LoadingSpinner() {
  return (
    <div style={s.wrap}>
      <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
      <div style={s.spinner} />
    </div>
  )
}
