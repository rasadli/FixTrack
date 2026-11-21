# FixTrack

FixTrack is a repair, warranty, and return case management system built for **CSCI 3509 — Introduction to Software Engineering**. This repository contains the Assignment 2 deliverable: a REST API for **repair cases** with mock authentication, ownership-based authorization, validation, consistent JSON responses, and an HTML greeting page.

Assignment 2 deliberately uses **in-memory mock data** and **predefined bearer tokens**. There is no database and no real identity provider.

**Demo video:** TODO — add the YouTube link (max. 5 minutes).

---

## 1. Overview

- Resource: repair cases at `/api/v1/cases` (list, read, create, update, delete)
- Authentication: three predefined bearer tokens (two regular users and one administrator)
- Authorization: users manage only their own cases; the administrator manages all cases
- Responses: every JSON response uses the same success or error envelope
- Greeting page: `GET /api/v1/greeting` returns an HTML page about the project, team, and endpoints

## 2. Tech stack and project structure

- .NET 10, ASP.NET Core (controllers), xUnit + `Microsoft.AspNetCore.Mvc.Testing`

```
backend/
  FixTrack.slnx
  src/
    FixTrack.Api/             HTTP layer: controllers, auth handler, validation filter, error handling, greeting page
    FixTrack.Application/     Use cases: case service, request/response contracts, envelopes, validation rules
    FixTrack.Domain/          Domain model: RepairCase, CaseType, CaseStatus
    FixTrack.Infrastructure/  In-memory case repository and seed data
  tests/
    FixTrack.Tests/           Integration tests (WebApplicationFactory)
docs/
  API_CONTRACT.md             Agreed API contract
  DEMO_SCRIPT.md              Demo video script
```

`frontend/` and `ai-service/` contain early scaffolding for later assignments and are not part of Assignment 2.

## 3. Prerequisites and installation

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

```powershell
git clone https://github.com/rasadli/FixTrack.git
cd FixTrack
dotnet restore backend/FixTrack.slnx
```

## 4. Running and testing

Start the API from the repository root:

```powershell
dotnet run --project backend/src/FixTrack.Api --launch-profile http
```

| What | URL |
|---|---|
| API base URL | `http://localhost:5072/api/v1` |
| Greeting page | `http://localhost:5072/api/v1/greeting` |
| Swagger UI (Development only) | `http://localhost:5072/swagger` |
| OpenAPI document (Development only) | `http://localhost:5072/openapi/v1.json` |

In Swagger UI, click **Authorize**, enter one of the tokens from section 6 (for example `user-a-token`, without the `Bearer` prefix), then use **Try it out** on any endpoint.

Run the automated tests:

```powershell
dotnet test backend/FixTrack.slnx
```

Example requests for every scenario are in [`backend/src/FixTrack.Api/FixTrack.Api.http`](backend/src/FixTrack.Api/FixTrack.Api.http). They can be sent from Visual Studio or the VS Code REST Client extension.

## 5. In-memory data

No database is needed. Cases are stored in memory, so **all changes are lost and the data below is restored every time the application restarts.**

| ID | Tracking code | Owner | Device | Type | Status |
|---|---|---|---|---|---|
| `case-101` | `FT-84A2` | `user-a` | Laptop | Repair | Open |
| `case-102` | `FT-3K9D` | `user-a` | Smartphone | Warranty | InProgress |
| `case-103` | `FT-7Q2M` | `user-b` | Headphones | Return | Open |
| `case-104` | `FT-5T8X` | `user-b` | Tablet | Repair | Completed |

New cases get IDs from `case-105` upward and a generated tracking code. `case-999` never exists and is used to demonstrate 404 responses.

## 6. Authentication and authorization

Send a token in the `Authorization` header:

```
Authorization: Bearer user-a-token
```

| Token | User ID | Role | Access |
|---|---|---|---|
| `user-a-token` | `user-a` | User | Own cases only |
| `user-b-token` | `user-b` | User | Own cases only |
| `admin-token` | `admin` | Administrator | All cases |

- A missing token, an unknown token, or a non-Bearer scheme returns **401 `UNAUTHORIZED`**.
- The greeting page needs no token.
- A new case is always owned by the user whose token created it. The owner cannot be set or changed through the request body; sending `ownerId` (or any other server-managed field) returns 400.
- **Cross-user access returns 404, not 403.** When user A requests one of user B's cases, the API answers exactly as if the case did not exist (`404 NOT_FOUND`, "Case not found."). Returning 403 would confirm that the ID belongs to someone else; 404 reveals nothing about other users' data.

## 7. API reference

All endpoints except the greeting page require a bearer token. Request bodies must be JSON (`Content-Type: application/json`).

| Method | Path | Success | Purpose |
|---|---|---|---|
| GET | `/api/v1/cases` | 200 | List the requester's cases (admin: all), paginated |
| GET | `/api/v1/cases/{id}` | 200 | Get one case |
| POST | `/api/v1/cases` | 201 + `Location` | Create a case owned by the requester |
| PUT | `/api/v1/cases/{id}` | 200 | Replace a case's editable fields |
| DELETE | `/api/v1/cases/{id}` | 200 | Delete a case |
| GET | `/api/v1/greeting` | 200 (`text/html; charset=utf-8`) | Project and team page (no token) |

### Fields and validation

| Field | In | Rules |
|---|---|---|
| `customerName` | POST, PUT | required, 2–100 characters |
| `device` | POST, PUT | required, 2–100 characters |
| `issueDescription` | POST, PUT | required, 10–1000 characters |
| `caseType` | POST, PUT | required: `Repair`, `Warranty`, or `Return` |
| `status` | PUT only | required: `Open`, `InProgress`, or `Completed` (new cases always start as `Open`) |

- Strings are trimmed; empty or whitespace-only values count as missing.
- PUT is a **full replacement**: all five fields are required.
- Unknown fields, including server-managed `id`, `trackingCode`, `ownerId`, `createdAt`, and `updatedAt`, are rejected with 400.
- Malformed JSON, wrong value types, invalid enum values, and an empty body return 400.

Query parameters for `GET /api/v1/cases`:

| Parameter | Default | Allowed |
|---|---|---|
| `page` | 1 | integer ≥ 1 |
| `perPage` | 10 | integer 1–50 |

Results are ordered by `createdAt`, then `id`. Invalid values return 400.

The examples below are real responses from the running application.

### GET /api/v1/cases — list cases

```http
GET /api/v1/cases?page=1&perPage=10
Authorization: Bearer user-a-token
```

`200 OK`

```json
{
  "success": true,
  "message": "Cases retrieved.",
  "data": [
    {
      "id": "case-101",
      "trackingCode": "FT-84A2",
      "ownerId": "user-a",
      "customerName": "Aylin Mammadova",
      "device": "Laptop",
      "issueDescription": "Laptop does not power on after a liquid spill on the keyboard.",
      "caseType": "Repair",
      "status": "Open",
      "createdAt": "2026-11-02T09:15:00+00:00"
    },
    {
      "id": "case-102",
      "trackingCode": "FT-3K9D",
      "ownerId": "user-a",
      "customerName": "Murad Huseynov",
      "device": "Smartphone",
      "issueDescription": "Touchscreen stops responding intermittently; device is under warranty.",
      "caseType": "Warranty",
      "status": "InProgress",
      "createdAt": "2026-11-04T14:30:00+00:00",
      "updatedAt": "2026-11-06T10:00:00+00:00"
    }
  ],
  "meta": {
    "total": 2,
    "page": 1,
    "perPage": 10,
    "totalPages": 1
  }
}
```

### GET /api/v1/cases/{id} — get one case

```http
GET /api/v1/cases/case-101
Authorization: Bearer user-a-token
```

`200 OK`

```json
{
  "success": true,
  "message": "Case retrieved.",
  "data": {
    "id": "case-101",
    "trackingCode": "FT-84A2",
    "ownerId": "user-a",
    "customerName": "Aylin Mammadova",
    "device": "Laptop",
    "issueDescription": "Laptop does not power on after a liquid spill on the keyboard.",
    "caseType": "Repair",
    "status": "Open",
    "createdAt": "2026-11-02T09:15:00+00:00"
  }
}
```

### POST /api/v1/cases — create a case

```http
POST /api/v1/cases
Authorization: Bearer user-a-token
Content-Type: application/json

{
  "customerName": "Nigar Karimova",
  "device": "Smartwatch",
  "issueDescription": "Battery drains within a few hours after the last update.",
  "caseType": "Warranty"
}
```

`201 Created` with `Location: http://localhost:5072/api/v1/cases/case-105`

```json
{
  "success": true,
  "message": "Case created.",
  "data": {
    "id": "case-105",
    "trackingCode": "FT-BJJA",
    "ownerId": "user-a",
    "customerName": "Nigar Karimova",
    "device": "Smartwatch",
    "issueDescription": "Battery drains within a few hours after the last update.",
    "caseType": "Warranty",
    "status": "Open",
    "createdAt": "2026-11-21T17:21:09.3582185+00:00"
  }
}
```

### PUT /api/v1/cases/{id} — replace a case

```http
PUT /api/v1/cases/case-105
Authorization: Bearer user-a-token
Content-Type: application/json

{
  "customerName": "Nigar Karimova",
  "device": "Smartwatch",
  "issueDescription": "Battery replaced under warranty; awaiting final test.",
  "caseType": "Warranty",
  "status": "InProgress"
}
```

`200 OK`

```json
{
  "success": true,
  "message": "Case updated.",
  "data": {
    "id": "case-105",
    "trackingCode": "FT-BJJA",
    "ownerId": "user-a",
    "customerName": "Nigar Karimova",
    "device": "Smartwatch",
    "issueDescription": "Battery replaced under warranty; awaiting final test.",
    "caseType": "Warranty",
    "status": "InProgress",
    "createdAt": "2026-11-21T17:21:09.3582185+00:00",
    "updatedAt": "2026-11-21T17:21:10.0381685+00:00"
  }
}
```

### DELETE /api/v1/cases/{id} — delete a case

```http
DELETE /api/v1/cases/case-105
Authorization: Bearer user-a-token
```

`200 OK`

```json
{
  "success": true,
  "message": "Case deleted.",
  "data": null
}
```

## 8. Response format and error codes

Success (single item):

```json
{ "success": true, "message": "Case retrieved.", "data": { } }
```

Success (list) adds pagination metadata:

```json
{ "success": true, "message": "Cases retrieved.", "data": [ ], "meta": { "total": 2, "page": 1, "perPage": 10, "totalPages": 1 } }
```

Error. `details` is present only for validation errors. This is a real response to a POST with `"customerName": "A"` and `"issueDescription": "short"`:

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid request.",
    "details": [
      { "field": "customerName", "message": "customerName must be between 2 and 100 characters." },
      { "field": "issueDescription", "message": "issueDescription must be between 10 and 1000 characters." }
    ]
  }
}
```

| HTTP | Code | When |
|---|---|---|
| 400 | `VALIDATION_ERROR` | Invalid body or query parameters, unknown fields, malformed JSON |
| 401 | `UNAUTHORIZED` | Missing or invalid token |
| 404 | `NOT_FOUND` | Case does not exist or belongs to another user ("Case not found."); unknown route ("The requested resource does not exist.") |
| 405 | `METHOD_NOT_ALLOWED` | Unsupported method, e.g. `PATCH /api/v1/cases/case-101` |
| 415 | `UNSUPPORTED_MEDIA_TYPE` | POST/PUT body is not JSON |
| 500 | `INTERNAL_ERROR` | Unexpected error; no internal details are exposed |

- **403 `FORBIDDEN`** is reserved but not used, because cross-user access returns 404 (see section 6).
- **409 Conflict** is never returned: IDs and tracking codes are generated by the server and there are no unique client-supplied fields, so no request can conflict with existing data.

## 9. Demo scenarios

Every scenario below has a matching request in [`FixTrack.Api.http`](backend/src/FixTrack.Api/FixTrack.Api.http). Restart the API first so the seed data is fresh.

| # | Scenario | Request | Expected |
|---|---|---|---|
| 1 | Successful CRUD | List user A's cases → read `case-101` → create → replace → delete | 200, 200, 201 + `Location`, 200, 200 with `data: null` |
| 2 | Authentication failures | `GET /api/v1/cases` without a token; with `Bearer nope` | 401 `UNAUTHORIZED` |
| 3 | Authorization failures | User A reads, replaces, or deletes `case-103` (user B's) | 404 "Case not found." |
| 4 | Administrator access | `GET /api/v1/cases` and `GET /api/v1/cases/case-103` with `admin-token` | 200, all four cases |
| 5 | Validation errors | Missing fields; one-character `customerName`; unknown `ownerId`; `?perPage=0` | 400 `VALIDATION_ERROR` with `details` |
| 6 | Nonexistent resource | `GET /api/v1/cases/case-999` | 404 `NOT_FOUND` |
| 7 | HTML greeting page | `GET /api/v1/greeting` without a token | 200 `text/html; charset=utf-8` |

The full script for the video, with timings, is in [`docs/DEMO_SCRIPT.md`](docs/DEMO_SCRIPT.md).

## 10. Team and contributions

| Member | Student ID | Contribution |
|---|---|---|
| Rashad Asadli | 17960 | Designed the shared API contract that the team built on and implemented repair case CRUD: the in-memory repository with seed data, the case service with ownership rules, the cases controller, and the CRUD and ownership tests. Integrated and reviewed the feature branches, and wrote the README and CONTRIBUTORS file. |
| Khavar Jabbarli | 18167 | Implemented mock bearer-token authentication, request body validation, and the JSON error responses for 401, 404, 405, 415, and 500, with tests for each. |
| Faig Almuradli | 23168 | Built the HTML greeting page, wrote the example API requests and the demo video script, and added the greeting page tests. |

GitHub accounts and detailed responsibilities are listed in [CONTRIBUTORS.md](CONTRIBUTORS.md).

Every member developed on their own feature branch (`feature/api-contract`, `feature/cases-core`, `feature/mock-auth-validation`, `feature/greeting-docs-demo`) and merged it into `main` through a pull request.

Output of `git shortlog -sn --all`:

```
TODO — run `git shortlog -sn --all` after the final merge and paste the output here.
```
