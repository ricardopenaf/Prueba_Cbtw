import { useState } from 'react'
import type { FormEvent } from 'react'
import { ApiError, listReservations } from '../api/reservationClient'
import type { ReservationListItem } from '../api/reservationClient'

function toInputDate(date: Date): string {
  return date.toISOString().slice(0, 10)
}

const now = new Date()
const defaultTo = toInputDate(now)
const defaultFrom = toInputDate(new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000))

export default function ReservationsList() {
  const [from, setFrom] = useState(defaultFrom)
  const [to, setTo] = useState(defaultTo)
  const [reservations, setReservations] = useState<ReservationListItem[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [searched, setSearched] = useState(false)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    if (from > to) {
      setError('La fecha inicial no puede ser posterior a la fecha final.')
      return
    }
    setLoading(true)
    setError(null)
    try {
      setReservations(await listReservations(from, to))
      setSearched(true)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'No se pudo cargar el listado de reservaciones.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <>
      <form onSubmit={handleSubmit} className="date-range-form">
        <div className="field">
          <label htmlFor="fromDate">Fecha inicial</label>
          <input id="fromDate" type="date" value={from} max={to} onChange={(e) => setFrom(e.target.value)} required />
        </div>

        <div className="field">
          <label htmlFor="toDate">Fecha final</label>
          <input id="toDate" type="date" value={to} min={from} onChange={(e) => setTo(e.target.value)} required />
        </div>

        <button type="submit" disabled={loading}>
          {loading ? 'Buscando...' : 'Buscar'}
        </button>
      </form>

      {error && <p className="events-error">{error}</p>}

      {loading ? (
        <p>Cargando reservaciones...</p>
      ) : (
        searched &&
        !error &&
        (reservations.length === 0 ? (
          <p>No hay reservaciones en el rango seleccionado.</p>
        ) : (
          <table className="events-table">
            <thead>
              <tr>
                <th>Fecha de reserva</th>
                <th>Evento</th>
                <th>Usuario</th>
                <th>Entradas</th>
              </tr>
            </thead>
            <tbody>
              {reservations.map((reservation) => (
                <tr key={reservation.reservationId}>
                  <td>{new Date(reservation.reservedAtUtc).toLocaleString()}</td>
                  <td>
                    {reservation.eventName} (<code>{reservation.eventCode}</code>)
                  </td>
                  <td>
                    {reservation.userFullName} (<code>{reservation.userCode}</code>)
                  </td>
                  <td>{reservation.quantity}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ))
      )}
    </>
  )
}
