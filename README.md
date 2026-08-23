# Prototipo: Reserva de entradas para eventos

Prototipo mínimo de reserva de entradas para un evento a partir de un código de evento y un código de usuario ya conocidos.

- **Backend**: .NET 8, ASP.NET Core Minimal APIs, arquitectura limpia (Domain / Application / Infrastructure / Api), EF Core Migrations sobre SQL Server.
- **Frontend**: React + TypeScript (Vite) — cliente mínimo de pruebas para invocar la API.

## Estructura

```
backend/
  TicketReservation.sln
  src/
    TicketReservation.Domain/          Entidades y excepciones de dominio
    TicketReservation.Application/     DTOs, interfaces, casos de uso (ReservationService)
    TicketReservation.Infrastructure/  EF Core, migraciones, repositorios, seed
    TicketReservation.Api/             Minimal API (Program.cs, endpoints)
frontend/
  src/
    api/                Cliente HTTP hacia el backend
    components/         EventsList, ReservationForm, ResultPanel
    App.tsx
```

## Requisitos

- .NET SDK 8.0+
- SQL Server LocalDB (incluido con Visual Studio, o instalable por separado)
- Node.js 20+ y npm

## Backend

```bash
cd backend

# aplicar la migración inicial (opcional: la Api también migra y siembra datos automáticamente al arrancar)
dotnet ef database update --project src/TicketReservation.Infrastructure --startup-project src/TicketReservation.Api

# ejecutar la API
dotnet run --project src/TicketReservation.Api
```

La API queda disponible en `http://localhost:5123` (ver `src/TicketReservation.Api/Properties/launchSettings.json`), con Swagger en `http://localhost:5123/swagger` en entorno de desarrollo.

Al arrancar, la API aplica las migraciones pendientes y siembra automáticamente estos datos de ejemplo:

**Eventos**

| Código  | Nombre                     | Aforo |
|---------|-----------------------------|-------|
| EVT-001 | Concierto de Rock en vivo   | 100   |
| EVT-002 | Conferencia de Tecnología   | 50    |
| EVT-003 | Obra de Teatro Clásico      | 5     |

**Usuarios**

| Código  | Nombre            |
|---------|-------------------|
| USR-001 | Ana García        |
| USR-002 | Luis Martínez     |
| USR-003 | Carla Rodríguez   |

### Endpoints

- `GET /api/events` — lista los eventos y su aforo disponible.
- `POST /api/reservations` — crea una reserva.

  ```json
  { "eventCode": "EVT-001", "userCode": "USR-001", "quantity": 2 }
  ```

  Respuestas: `201 Created` (reserva creada), `404` (evento o usuario inexistente), `409` (reserva duplicada del mismo usuario para el mismo evento, o aforo insuficiente), `400` (petición inválida).

### Reglas de negocio implementadas

- Control de aforo: la reserva falla si no hay entradas suficientes disponibles.
- No se permite más de una reserva del mismo usuario para el mismo evento.
- La cantidad de entradas por reserva es configurable en la petición.

## Frontend

```bash
cd frontend
npm install
npm run dev
```

Se sirve en `http://localhost:5173`. La URL del backend se configura en `frontend/.env` (`VITE_API_BASE_URL`).

El cliente muestra los eventos sembrados con su aforo, y permite reservar indicando código de evento, código de usuario y cantidad de entradas, mostrando el resultado (éxito o error) tras cada intento.

## Pruebas unitarias

```bash
cd backend
dotnet test tests/TicketReservation.Application.Tests
```

Proyecto xUnit (`backend/tests/TicketReservation.Application.Tests`) con pruebas de `ReservationService`,
`EventService` y de la jerarquía de excepciones de dominio, usando Moq para los repositorios/UnitOfWork y
FluentAssertions para las aserciones.

## Verificación rápida vía curl

```bash
curl http://localhost:5123/api/events

curl -X POST http://localhost:5123/api/reservations \
  -H "Content-Type: application/json" \
  -d '{"eventCode":"EVT-001","userCode":"USR-001","quantity":2}'
```
