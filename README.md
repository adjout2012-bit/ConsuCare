# ConsuCare

Peer-support platform helping people newly diagnosed with chronic illness connect with approved supporters who share relevant lived experience. Patients can request practical or emotional peer support, track care goals and chat in real time. ConsuCare is not medical advice or a substitute for professional care.

**Stack:** ASP.NET Core Web API · Blazor WebAssembly · SignalR · Entity Framework Core · PostgreSQL (Supabase)

## Project layout

```
ConsuCare.sln
src/
  ConsuCare.Shared/    Entities + DTOs shared by client and server
  ConsuCare.Api/       ASP.NET Core Web API + SignalR hub + EF Core (hosts the client)
  ConsuCare.Client/    Blazor WebAssembly front end (all 14 screens, phone / tablet / desktop)
tests/
  ConsuCare.Api.Tests/ xUnit integration tests for the API
docs/                   Architecture, API reference, user guide, test plan
```

## Recent project updates

- API and client projects are fully wired together with JWT authentication, authorization, and CORS for the Vercel frontend.
- The app includes Patient and Supporter sign-up/login, opt-in supporter health-profile sharing, supporter approval, peer-support requests, dashboards, care goals, notifications, and SignalR live chat.
- Default runtime behaviour uses an in-memory EF Core database, so PostgreSQL/Supabase setup is optional. The API still requires a JWT signing key from User Secrets or an environment variable.
- Supporters choose whether their condition, diagnosis year and management strategies may be shown in Discover; sharing can be withdrawn in Settings.
- The Blazor client provides patient, supporter and admin experiences across the 14 screens listed below.
- Automated xUnit tests cover authentication, password hashing, access control, supporter privacy, peer-support flow, goals and notifications.

## Run it

Before the first run, configure the local JWT key using the instructions in [Local JWT signing key](#local-jwt-signing-key).

```bash
dotnet run --project src/ConsuCare.Api
```

Then open http://localhost:5180. The Blazor client is served by the API, so that one command runs everything.

**Demo accounts** (password `demo1234`):

| Role    | Email                  | Lands on              |
|---------|------------------------|-----------------------|
| Patient | `amara@patient.dev`    | Patient Hub           |
| Supporter | `nnamdi@supporter.dev` | Supporter Hub       |
| Admin   | `admin@consucare.dev`  | Supporter Verification |

Tip: open two browser windows (one normal, one private), sign in as Amara in one and Nnamdi in the other, and chat — messages arrive live over SignalR.

## Live site

Existing Vercel and Render deployments retain the legacy hostnames `https://mentorlink-client.vercel.app` and `https://project-mentorlink.onrender.com`. Those hostnames remain configured for compatibility; no new ConsuCare deployment URLs have been provided.

## Tests

```bash
dotnet test
```

40 automated tests boot the API against an in-memory database and cover login and sign-up, password hashing, access control, supporter privacy consent, the peer-support request → accept / decline flow, goals and notifications. The manual checklist is in [docs/TEST_PLAN.md](docs/TEST_PLAN.md).

## Documentation

- [Architecture](docs/ARCHITECTURE.md) — components, deployment, data model, key flows
- [API reference](docs/API_REFERENCE.md) — every endpoint and the SignalR chat hub
- [User guide](docs/USER_GUIDE.md) — how patients, supporters and admins use each screen
- [Test plan](docs/TEST_PLAN.md) — automated tests and manual checklist

## Database — Supabase (PostgreSQL)

Out of the box the app uses an **in-memory database** with seed data, so it runs with zero setup (data resets on restart).

To configure the API to use Supabase:

1. In your Supabase project go to **Settings → Database → Connection string** and copy the direct connection values.
2. Store the Npgsql-formatted connection string in local .NET User Secrets, or set `ConnectionStrings__DefaultConnection` in the process environment. Do not put credentials in `appsettings.json`.

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<Npgsql connection string>" --project src\ConsuCare.Api\ConsuCare.Api.csproj
```

3. Restart the app. On startup EF Core applies any pending migrations.

> If your network blocks direct connections (IPv6), use Supabase's **session pooler** connection string instead — same format, host like `aws-0-REGION.pooler.supabase.com`, port `5432`, username `postgres.YOUR-PROJECT-REF`.
>
> The linked Supabase project's non-system schemas were checked and no legacy MentorLink/Mentorship tables or `__EFMigrationsHistory` were present. The `InitialConsuCareCreate` EF migration was then applied successfully to that remote project through its IPv4 session pooler. The separate local PostgreSQL instance was not updated. Always inspect an existing target database and back it up before applying this initial migration there.

## Supabase CLI and EF Core

The Supabase CLI is a project-local development dependency. Install it with `npm ci`, authenticate interactively with `npx supabase login`, then link this workspace with:

```bash
npx supabase link --project-ref gsxenkilmtxltlirasws
```

Linking associates the workspace with the remote project; it does not apply the EF Core schema. The EF migration is the source of truth for this backend: do not use `supabase db push` to apply the C# model. To apply EF migrations to the remote database, configure `ConnectionStrings__DefaultConnection` in the environment from the database connection details in the Supabase dashboard, then run:

```bash
dotnet ef database update --project src/ConsuCare.Api --startup-project src/ConsuCare.Api
```

Keep the remote connection string and database password out of tracked files and chat. The CLI cannot retrieve the remote database password. Review the destination schema and EF migration history, and back up production data before applying the regenerated initial migration; it may conflict with tables left by the predecessor schema. Local `supabase start` / `supabase status` require Docker Desktop, which is not currently installed on this machine.

## Local JWT signing key

The API requires `Jwt:Key` from .NET User Secrets or the `Jwt__Key` environment variable; no signing key is stored in `appsettings.json`. Generate and store a local key from the repository root with:

```powershell
$bytes = New-Object byte[] 64
$rng = [Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($bytes)
$key = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "Jwt:Key" $key --project src\ConsuCare.Api\ConsuCare.Api.csproj
Remove-Variable key, bytes
$rng.Dispose()
```

Configure a separate, strong key in the hosting environment for deployment. Never reuse the test-only key from the test project.

## The 14 screens

| # | Screen | Route |
|---|--------|-------|
| 1 | Landing Page | `/` |
| 2 | Sign Up | `/signup` |
| 3 | Login | `/login` |
| 4 | Patient Hub | `/dashboard` |
| 5 | Discover Supporters | `/discover` |
| 6 | Supporter Profile | `/supporter/{id}` |
| 7 | Support Request | `/request/{supporterId}` |
| 8 | Messages (SignalR) | `/messages` |
| 9 | Goal Tracker | `/goals` |
| 10 | Supporter Hub | `/supporter-dashboard` |
| 11 | Notifications | `/notifications` |
| 12 | Settings & Profile | `/settings` |
| 13 | Support Network | `/support-network` |
| 14 | Admin — Supporter Verification | `/admin/verifications` |

## Project contributor

| Name | Student ID | Role | Responsibility |
|------|------------|------|----------------|
| Daniel Kwabena Osei-Boateng | 22205128 | Sole contributor / Full-stack developer | Leads and implements the ConsuCare project across its application, database, testing and documentation. |

Paths starting with `Api/`, `Client/` or `Shared/` are under `src/ConsuCare.*`; `tests/` and `docs/` are at the repository root.

## Notes / known simplifications (fine for an MVP, fix before production)

- The chat hub trusts the user ID the client sends; it should use the signed-in user from the JWT instead.
- Some protected endpoints accept user IDs in the URL without checking that they belong to the signed-in user.
- Endpoints take user IDs in the URL and don't check they belong to the signed-in user.
- An empty database is seeded with demo accounts — turn this off before real users sign up.
- File attachments in chat and photo upload are placeholders.
