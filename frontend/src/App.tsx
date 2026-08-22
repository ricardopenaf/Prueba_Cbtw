import { useEffect, useState } from 'react'
import { ApiError, listEvents, reserveTicket } from './api/reservationClient'
import type { EventSummary, ReservationResponse } from './api/reservationClient'
import EventsList from './components/EventsList'
import ReservationForm from './components/ReservationForm'
import ResultPanel from './components/ResultPanel'
import './App.css'

function App() {
  const [events, setEvents] = useState<EventSummary[]>([])
  const [loadingEvents, setLoadingEvents] = useState(true)
  const [eventsError, setEventsError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)
  const [reservation, setReservation] = useState<ReservationResponse | null>(null)
  const [error, setError] = useState<string | null>(null)

  async function loadEvents() {
    setLoadingEvents(true)
    try {
      setEvents(await listEvents())
      setEventsError(null)
    } catch {
      setEventsError('No se pudo cargar la lista de eventos. Verifica que el backend esté en ejecución en ' + import.meta.env.VITE_API_BASE_URL + '.')
    } finally {
      setLoadingEvents(false)
    }
  }

  useEffect(() => {
    loadEvents()
  }, [])

  async function handleReserve(eventCode: string, userCode: string, quantity: number) {
    setSubmitting(true)
    setReservation(null)
    setError(null)
    try {
      const result = await reserveTicket(eventCode, userCode, quantity)
      setReservation(result)
      await loadEvents()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error de conexión con el servidor.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="app">
      <h1>Cliente de pruebas · Reserva de entradas</h1>

      <section>
        <h2>Eventos disponibles</h2>
        {eventsError ? <p className="events-error">{eventsError}</p> : <EventsList events={events} loading={loadingEvents} />}
      </section>

      <section>
        <h2>Nueva reserva</h2>
        <ReservationForm onSubmit={handleReserve} submitting={submitting} />
      </section>

      <ResultPanel reservation={reservation} error={error} />
    </div>
  )
}

export default App
