import type { ReservationResponse } from '../api/reservationClient'

interface Props {
  reservation: ReservationResponse | null
  error: string | null
}

export default function ResultPanel({ reservation, error }: Props) {
  if (error) {
    return (
      <div className="result-panel error">
        <strong>No se pudo completar la reserva</strong>
        <p>{error}</p>
      </div>
    )
  }

  if (!reservation) {
    return null
  }

  return (
    <div className="result-panel success">
      <strong>Reserva confirmada</strong>
      <p>
        {reservation.quantity} entrada(s) para <b>{reservation.eventName}</b> ({reservation.eventCode}) a nombre del
        usuario <b>{reservation.userCode}</b>.
      </p>
      <p>Id de reserva: <code>{reservation.reservationId}</code></p>
      <p>Aforo restante: {reservation.remainingSeats}</p>
    </div>
  )
}
