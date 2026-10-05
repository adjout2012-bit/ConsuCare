# ConsuCare — API Reference

Local base URL: `http://localhost:5180`. The hosted API is `https://consucare-api.onrender.com`.

Endpoints marked 🔒 need an `Authorization: Bearer <token>` header; the token comes from login or sign-up. Without it they return `401 Unauthorized`. Request and response bodies are JSON; enums are sent as numbers.

## Auth

Every endpoint marked 🔒 requires the bearer token returned by login or sign-up.

| Method | Path | Body | Returns |
|--------|------|------|---------|
| POST | `/api/auth/login` | `{ email, password }` | `{ user, token }` · `401` if the email or password is wrong |
| POST | `/api/auth/signup` | `{ fullName, email, community, password, role, conditionType?, diagnosisYear?, shareHealthDetails? }` | `{ user, token }` · `409` if email is taken. Supporters start Pending verification; health details are private unless sharing is explicitly opted into |

## Users 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/users/{id}` | Get a profile |
| PUT | `/api/users/{id}` | Update name, email, community, bio and photo |
| PUT | `/api/users/{id}/preferences` | Update notification and profile-visibility preferences; supporters can withdraw sharing consent |
| POST | `/api/users/{id}/change-password` | `{ currentPassword, newPassword }` |

## Supporters 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/supporters?search=` | Approved, public supporter profiles; search matches name, community, condition or management strategies |
| GET | `/api/supporters/{userId}` | Public supporter profile with consented health details and reviews |
| POST | `/api/supporters/{userId}/reviews` | `{ patientName, rating, text }` — add a review |

Only approved supporters who opted into sharing are discoverable. Supporter profile fields include `ConditionType`, `DiagnosisYear` and `ManagementStrategies`.

## Peer-support requests 🔒

| Method | Path | Body / purpose |
|--------|------|----------------|
| POST | `/api/support-requests` | `{ patientId, supporterUserId, careFocus, message, frequency }` — send a request; the supporter is notified |
| POST | `/api/support-requests/{id}/accept` | Accept; creates an active support connection and notifies the patient. `409` if already handled |
| POST | `/api/support-requests/{id}/decline` | Decline a request |

## Support connections

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/support-connections/patient/{patientId}` | Active, pending and past connections for the patient's Support Network |

## Goals 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/goals/patient/{patientId}` | A patient's care goals with milestones |
| POST | `/api/goals` | `{ patientId, title, type, supporterUserId?, milestones[] }` — blank milestones are ignored |
| POST | `/api/goals/milestones/{milestoneId}/toggle` | Tick / untick a milestone; returns the updated goal |

## Messages 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/messages/conversations/{userId}` | Conversation list with last message and unread count |
| GET | `/api/messages/thread/{userId}/{partnerId}` | Full message history between two users |

### Real-time chat — SignalR hub `/hubs/chat`

| Direction | Name | Payload |
|-----------|------|---------|
| client → server | `Register` | `userId` — join your personal group after connecting |
| client → server | `SendMessage` | `{ senderId, recipientId, content }` |
| server → client | `ReceiveMessage` | `{ id, senderId, recipientId, content, sentAt }` — sent to both people |

## Notifications 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/notifications/{userId}` | Newest first |
| POST | `/api/notifications/{userId}/read-all` | Mark all as read |

## Dashboards 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/dashboard/patient/{patientId}` | Stats, active supporters and goals for the Patient Hub |
| GET | `/api/dashboard/supporter/{supporterUserId}` | Stats, pending requests and supported patients for the Supporter Hub |

## Admin 🔒 (Admin role only — others get `403`)

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/admin/verifications` | Supporter applications |
| POST | `/api/admin/verifications/{profileId}/approve` | Approve — supporter appears in Discover only if sharing is enabled |
| POST | `/api/admin/verifications/{profileId}/reject` | Reject |
