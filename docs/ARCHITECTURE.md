# ConsuCare — Architecture

ConsuCare is a peer-support platform for people navigating chronic illness. Patients can discover approved supporters with relevant lived experience, request peer support, chat and track care goals with milestones. ConsuCare does not provide medical advice.

## Components

```
Browser (Blazor WebAssembly)
   │  HTTPS + JWT bearer token            WebSocket (SignalR)
   ▼                                          ▼
ASP.NET Core Web API  ───────────────  ChatHub  (/hubs/chat)
   │  Entity Framework Core
   ▼
PostgreSQL (Supabase)   — or an in-memory database for local development
```

| Project | Purpose |
|---------|---------|
| `src/ConsuCare.Client` | Blazor WebAssembly front end: patient, supporter and admin screens; layouts, shared components and typed API client |
| `src/ConsuCare.Api` | REST controllers, SignalR chat hub, JWT auth, EF Core `AppDbContext`, migrations, seed data |
| `src/ConsuCare.Shared` | Entities and DTOs used by both client and server, so both sides agree on the data shapes |
| `tests/ConsuCare.Api.Tests` | xUnit integration tests that boot the real API against an in-memory database |

## Deployment

| Part | Host | Deployed from |
|------|------|---------------|
| Front end | Existing Vercel host — https://mentorlink-client.vercel.app (legacy hostname) | `build-vercel.sh` publishes the client; `vercel.json` sends every route to `index.html` so refreshing a page works |
| API + chat hub | Existing Render host — https://project-mentorlink.onrender.com (legacy hostname) | Root `Dockerfile` |
| Database | Supabase PostgreSQL (session pooler) | EF Core migrations run automatically when the API starts |

The client reads the API address from `wwwroot/appsettings.json` (`ApiBaseUrl`). For local runs `appsettings.Development.json` blanks it so the client calls the API it is served from. These legacy deployment domains remain configured for compatibility; replace them only when ConsuCare deployment URLs are available.

## Data model

| Entity | Key fields |
|--------|------------|
| `User` | name, email, hashed password, role (Patient / Supporter / Admin), community, bio, profile visibility and notification preferences |
| `SupporterProfile` | condition type, diagnosis year, management strategies, availability, verification status (Pending / Approved / Rejected) |
| `Review` | supporter, patient name, 1–5 rating, text |
| `SupportRequest` | patient, supporter, care focus, message, preferred frequency, status (Pending / Accepted / Declined) |
| `SupportConnection` | patient, supporter, status (Active / Completed), start date, last check-in |
| `Goal` / `Milestone` | a patient's care goal, optionally linked to a supporter, broken into milestones that can be ticked off |
| `ChatMessage` | sender, recipient, content, sent time, read flag |
| `Notification` | recipient, kind (request received / accepted, new message, milestone completed …), title, body, read flag |

## Key flows

**Sign in.** `POST /api/auth/login` checks the PBKDF2 password hash and returns a JWT. The client stores the session in `localStorage` (so a refresh keeps you signed in) and `AuthHeaderHandler` attaches the token to every API call.

**Peer-support request.** A patient sends a request to an approved supporter who has opted into health-detail sharing → the supporter is notified → they accept (a `SupportConnection` is created and the patient is notified) or decline.

**Privacy.** Supporter health details are not shared by default on signup. Public directory and profile endpoints require both approval and explicit sharing consent; supporters can withdraw consent in Settings.

**Chat.** The Messages screen connects to `/hubs/chat` and calls `Register(userId)`. `SendMessage` saves the message, creates a notification and pushes `ReceiveMessage` to both people instantly.

**Goals.** Ticking a milestone recalculates progress; when every milestone is done the goal is marked Completed. Each completed milestone creates a notification.

## Responsive layout

All screens work on phones and tablets. Below 860px the sidebar becomes a top bar with a menu button, and Messages shows either the conversation list or one open chat (with a back button). Styles live in `src/ConsuCare.Client/wwwroot/css/app.css`.
