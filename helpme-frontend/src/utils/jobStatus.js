export const STATUS_LABELS = {
  Open: 'Отворена',
  AwaitingConfirmation: 'Чака потвърждение',
  InProgress: 'В прогрес',
  Completed: 'Завършена',
  Cancelled: 'Отменена',
}

export const STATUS_COLORS = {
  Open:                 { bg: '#fef3c7', color: '#d97706', border: '#fde68a' },
  AwaitingConfirmation: { bg: '#fef9c3', color: '#ca8a04', border: '#fde047' },
  InProgress:           { bg: '#dcfce7', color: '#15803d', border: '#bbf7d0' },
  Completed:            { bg: '#f3f4f6', color: '#6b7280', border: '#e5e7eb' },
  Cancelled:            { bg: '#fee2e2', color: '#dc2626', border: '#fecaca' },
}
