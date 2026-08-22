const API_BASE_URL = import.meta.env.VITE_API_BASE_URL as string

export interface EventSummary {
  code: string
  name: string
  venue: string
  eventDateUtc: string
  totalCapacity: number
  availableSeats: number
}

export interface ReservationResponse {
  reservationId: string
  eventCode: string
  eventName: string
  userCode: string
  quantity: number
  reservedAtUtc: string
  remainingSeats: number
}

interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
}

export class ApiError extends Error {
  status: number

  constructor(message: string, status: number) {
    super(message)
    this.status = status
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const problem = (await response.json().catch(() => null)) as ProblemDetails | null
    throw new ApiError(problem?.detail ?? `Error inesperado (HTTP ${response.status})`, response.status)
  }
  return (await response.json()) as T
}

export function listEvents(): Promise<EventSummary[]> {
  return fetch(`${API_BASE_URL}/api/events`).then((r) => handleResponse<EventSummary[]>(r))
}

export function reserveTicket(eventCode: string, userCode: string, quantity: number): Promise<ReservationResponse> {
  return fetch(`${API_BASE_URL}/api/reservations`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ eventCode, userCode, quantity }),
  }).then((r) => handleResponse<ReservationResponse>(r))
}
