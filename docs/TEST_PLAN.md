# ConsuCare — Test Plan

## Automated tests

`tests/ConsuCare.Api.Tests` holds xUnit integration tests. Each test class starts the real API in memory with its own fresh copy of the demo data, so no database or network is needed.

Run them from the repository root:

```bash
dotnet test
```

| File | What it covers |
|------|----------------|
| `PasswordHasherTests.cs` | Correct / wrong passwords, unique salts, the plain password never stored, malformed hashes rejected |
| `AuthTests.cs` | Demo logins for all three roles, email case-insensitivity, wrong password and unknown email, sign-up then login, duplicate email (`409`), supporters remain hidden unless approved and sharing is enabled |
| `AuthorizationTests.cs` | Protected endpoints return `401` without a token or with a forged one; patients and supporters get `403` on admin endpoints; admins get `200` |
| `SupportFlowTests.cs` | Sending a request notifies the supporter; accepting creates a support connection and notifies the patient; a request can't be accepted twice; declining creates no connection |
| `GoalsAndNotificationsTests.cs` | Blank milestones ignored; progress goes 0 → 50 → 100% and the goal completes; unticking reopens it; milestone notifications; unknown milestone gives `404`; Mark all read |

## Manual test checklist

Use the demo accounts in the user guide (password `demo1234`). Repeat the ★ rows at phone width (about 390px) and tablet width (about 820px) — in Chrome or Edge, press F12 and use the device toolbar.

| # | Screen / flow | Steps | Expected |
|---|---------------|-------|----------|
| 1 ★ | Landing | Open `/` | Hero, features, how-it-works and testimonials; nothing overflows sideways |
| 2 | Sign up | Create a patient, then a supporter with sharing off | Patient lands on Dashboard; supporter is Pending in Admin and remains hidden from Discover |
| 3 | Login | Wrong password, then the right one | Error message, then role-based landing page |
| 4 ★ | Navigation | On a phone, open ☰ and pick each item | Menu opens and closes, and each screen loads |
| 5 | Refresh | Refresh on `/goals`, `/messages`, `/supporter/2` | Same page reloads — no 404, still signed in |
| 6 ★ | Discover | Search for a condition such as "Crohn's" | Only approved supporters who opted in are listed |
| 7 | Supporter profile | Open a public supporter profile and add a review | Shared lived-experience details appear; review and rating update |
| 8 | Request | Send a support request as Amara | Appears under Pending; supporter gets a notification |
| 9 | Accept / decline | As Nnamdi, accept one request and decline another | Accepted → patient's Active tab and a notification |
| 10 ★ | Messages | Chat between patient and supporter in two windows | Messages arrive instantly; on a phone the back arrow returns to the list |
| 11 ★ | Goals | Add a goal with 3 milestones; tick all | Progress 33 → 67 → 100%, card turns dark (Completed) |
| 12 | Notifications | Mark all read | Unread dots disappear |
| 13 | Settings | Edit profile, toggle preferences, change password | Saved; new password works at next login |
| 14 ★ | Admin | Approve and reject applications | Status badges update; an approved supporter appears in Discover only when sharing is enabled |
| 15 | Sign out | Sign out, then press Back | Redirected to login |

## Reporting bugs

Open a GitHub issue with the screen, steps to reproduce, expected and actual result, device and browser, and a screenshot.
