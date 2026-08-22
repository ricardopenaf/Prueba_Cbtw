import type { EventSummary } from '../api/reservationClient'

interface Props {
  events: EventSummary[]
  loading: boolean
}

export default function EventsList({ events, loading }: Props) {
  if (loading) {
    return <p>Cargando eventos...</p>
  }

  if (events.length === 0) {
    return <p>No hay eventos disponibles.</p>
  }

  return (
    <table className="events-table">
      <thead>
        <tr>
          <th>Código</th>
          <th>Evento</th>
          <th>Lugar</th>
          <th>Fecha</th>
          <th>Aforo disponible</th>
        </tr>
      </thead>
      <tbody>
        {events.map((event) => (
          <tr key={event.code}>
            <td>
              <code>{event.code}</code>
            </td>
            <td>{event.name}</td>
            <td>{event.venue}</td>
            <td>{new Date(event.eventDateUtc).toLocaleDateString()}</td>
            <td>
              {event.availableSeats} / {event.totalCapacity}
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
