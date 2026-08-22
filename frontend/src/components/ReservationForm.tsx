import { useState } from 'react'
import type { FormEvent } from 'react'

interface Props {
  onSubmit: (eventCode: string, userCode: string, quantity: number) => void
  submitting: boolean
}

export default function ReservationForm({ onSubmit, submitting }: Props) {
  const [eventCode, setEventCode] = useState('')
  const [userCode, setUserCode] = useState('')
  const [quantity, setQuantity] = useState(1)

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    onSubmit(eventCode.trim(), userCode.trim(), quantity)
  }

  return (
    <form onSubmit={handleSubmit} className="reservation-form">
      <div className="field">
        <label htmlFor="eventCode">Código de evento</label>
        <input
          id="eventCode"
          value={eventCode}
          onChange={(e) => setEventCode(e.target.value)}
          placeholder="EVT-001"
          required
        />
      </div>

      <div className="field">
        <label htmlFor="userCode">Código de usuario</label>
        <input
          id="userCode"
          value={userCode}
          onChange={(e) => setUserCode(e.target.value)}
          placeholder="USR-001"
          required
        />
      </div>

      <div className="field">
        <label htmlFor="quantity">Cantidad de entradas</label>
        <input
          id="quantity"
          type="number"
          min={1}
          value={quantity}
          onChange={(e) => setQuantity(Number(e.target.value))}
          required
        />
      </div>

      <button type="submit" disabled={submitting}>
        {submitting ? 'Reservando...' : 'Reservar'}
      </button>
    </form>
  )
}
