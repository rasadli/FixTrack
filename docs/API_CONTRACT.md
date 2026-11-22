# FixTrack API Contract (Assignment 2)

This document defines the FixTrack API: routes, validation, authentication, response formats, and error codes. The types it describes live in:

- `backend/src/FixTrack.Domain/Cases/`: `RepairCase`, `CaseType`, `CaseStatus`
- `backend/src/FixTrack.Application/Cases/Contracts/`: request and response DTOs
- `backend/src/FixTrack.Application/Common/`: envelopes, `ErrorCodes`, `Messages`, factory helpers
- `backend/src/FixTrack.Application/Security/`: `Requester`, `ICurrentRequester`, `FixTrackClaims`

Any change to the API should update this document in the same pull request.

---

## 1. Routes

Base URL when running locally: `http://localhost:5072`.

| Method | Route | Auth | Success | Purpose |
|---|---|---|---|---|
| `GET` | `/api/v1/cases` | Bearer token | `200` | List the requester's cases (admin: all cases), paginated |
| `GET` | `/api/v1/cases/{id}` | Bearer token | `200` | Get one case |
| `POST` | `/api/v1/cases` | Bearer token | `201` + `Location: /api/v1/cases/{id}` | Create a case owned by the requester |
| `PUT` | `/api/v1/cases/{id}` | Bearer token | `200` | Replace a case's editable fields |
| `DELETE` | `/api/v1/cases/{id}` | Bearer token | `200` with `"data": null` | Delete a case |
| `GET` | `/api/v1/greeting` | **Anonymous** | `200`, `text/html; charset=utf-8` | HTML project and team page |

All JSON uses camelCase property names, enums are serialized as strings (`"Repair"`, `"InProgress"`), and `null` properties are omitted.

---

## 2. The case resource

| Field | Type | Managed by | Notes |
|---|---|---|---|
| `id` | string | Server, immutable | Format `case-101`. New IDs continue from `case-105`. |
| `trackingCode` | string | Server, immutable | Format like `FT-84A2`. |
| `ownerId` | string | Server, immutable | User ID of the creator (`user-a`, `user-b`, `admin`). |
| `customerName` | string | Client | 2–100 characters |
| `device` | string | Client | 2–100 characters |
| `issueDescription` | string | Client | 10–1000 characters |
| `caseType` | enum | Client | `Repair`, `Warranty`, `Return` |
| `status` | enum | Client (PUT only) | `Open`, `InProgress`, `Completed`. New cases are always `Open`. |
| `createdAt` | ISO 8601 date-time | Server, immutable | Set on creation |
| `updatedAt` | ISO 8601 date-time | Server | Set on each update; omitted until the first update |

### Request DTOs

| DTO | Fields |
|---|---|
| `CreateCaseRequest` (POST) | `customerName`, `device`, `issueDescription`, `caseType` |
| `UpdateCaseRequest` (PUT) | `customerName`, `device`, `issueDescription`, `caseType`, `status` |

Server-managed fields (`id`, `trackingCode`, `ownerId`, `createdAt`, `updatedAt`) are **not** part of any request DTO. Sending them counts as sending an unknown field (see §3), so ownership can't be changed through a request body.

---

## 3. Validation rules

- **PUT is a full replacement:** all five fields of `UpdateCaseRequest` are required. There are no partial updates.
- **Unknown fields → 400** `VALIDATION_ERROR` (including server-managed fields).
- **Strings are trimmed** before validation and storage. A string that is empty or only whitespace counts as **missing**.
- **Length limits** (after trimming): `customerName` 2–100, `device` 2–100, `issueDescription` 10–1000.
- **Enums:** `caseType` and `status` must be one of the listed values. Any other value, or a wrong JSON type, → 400.
- **Malformed JSON** → 400 `VALIDATION_ERROR`.
- **Non-JSON body** on POST/PUT → 415 `UNSUPPORTED_MEDIA_TYPE`.
- In `details`, `field` uses the camelCase JSON or query name, for example `customerName` or `perPage`.

### Pagination (`GET /api/v1/cases`)

| Query parameter | Default | Allowed | Invalid value |
|---|---|---|---|
| `page` | `1` | integer ≥ 1 | 400 `VALIDATION_ERROR` |
| `perPage` | `10` | integer 1–50 | 400 `VALIDATION_ERROR` |

Results are ordered by `createdAt` ascending, then `id`. A page past the end returns `200` with an empty `data` array.

---

## 4. Authentication

Requests send `Authorization: Bearer <token>`. Assignment 2 uses predefined mock tokens instead of real authentication (scheme name `MockBearer`, see `FixTrackClaims`).

| Token | User ID (`sub` claim) | Role | Can access |
|---|---|---|---|
| `user-a-token` | `user-a` | — | Own cases only |
| `user-b-token` | `user-b` | — | Own cases only |
| `admin-token` | `admin` | `Admin` | All cases |

- Missing `Authorization` header → **401** `UNAUTHORIZED`
- Unknown token or a non-`Bearer` scheme → **401** `UNAUTHORIZED`
- The greeting page needs no token.

## 5. Ownership rule

- A regular user can list, read, update and delete **only cases where `ownerId` equals their user ID**.
- The admin can list, read, update and delete **all** cases.
- `POST` always sets `ownerId` to the requester's user ID.
- **Cross-user access returns `404 NOT_FOUND`** with the message `"Case not found."`, the same response as for a case that doesn't exist.
  **Rationale:** returning 403 would confirm that another user's case with that ID exists. Returning 404 reveals nothing about other users' data.

---

## 6. Response envelopes

### Single item (`ApiResponse<T>`)

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

### List (`PagedApiResponse<T>`)

```json
{
  "success": true,
  "message": "Cases retrieved.",
  "data": [ { "id": "case-101", "…": "…" } ],
  "meta": { "total": 2, "page": 1, "perPage": 10, "totalPages": 1 }
}
```

`totalPages` = ceiling(`total` / `perPage`), or `0` when `total` is `0`.

### Delete

```json
{ "success": true, "message": "Case deleted.", "data": null }
```

`data` is written explicitly as `null` here (the envelope always contains `success`, `message` and `data`).

### Error (`ApiErrorResponse`)

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid request.",
    "details": [
      { "field": "customerName", "message": "customerName must be between 2 and 100 characters." }
    ]
  }
}
```

`details` appears **only** for `VALIDATION_ERROR`. For every other error it's omitted:

```json
{ "success": false, "error": { "code": "NOT_FOUND", "message": "Case not found." } }
```

Error responses never contain stack traces or exception messages.

### Success messages

Use these so that all endpoints read consistently:

| Operation | `message` |
|---|---|
| List | `Cases retrieved.` |
| Get by ID | `Case retrieved.` |
| Create | `Case created.` |
| Update | `Case updated.` |
| Delete | `Case deleted.` |

---

## 7. Error codes

| Code | HTTP | When | Factory |
|---|---|---|---|
| `VALIDATION_ERROR` | 400 | Invalid body, unknown field, malformed JSON, invalid query parameter | `ApiErrors.Validation(details)` |
| `UNAUTHORIZED` | 401 | Missing or invalid token | `ApiErrors.Unauthorized(message)` |
| `FORBIDDEN` | 403 | **Reserved, not used.** Cross-user access returns 404 (see §5). | — |
| `NOT_FOUND` | 404 | Case doesn't exist or belongs to another user; unknown route | `ApiErrors.NotFound(message)` |
| `METHOD_NOT_ALLOWED` | 405 | Wrong method on an existing route, for example `PATCH /api/v1/cases/case-101` | `ApiErrors.MethodNotAllowed()` |
| `UNSUPPORTED_MEDIA_TYPE` | 415 | POST/PUT body isn't `application/json` | `ApiErrors.UnsupportedMediaType()` |
| `INTERNAL_ERROR` | 500 | Unhandled exception | `ApiErrors.Internal()` |

**No 409:** no operation in this API has a genuine conflict (IDs and tracking codes are generated by the server, and there are no unique client-supplied fields), so 409 is never returned.

### The two 404 messages (`Messages`)

| Situation | Message |
|---|---|
| Case ID not found, or not visible to the requester | `Case not found.` (`Messages.CaseNotFound`) |
| No route matches the request | `The requested resource does not exist.` (`Messages.ResourceNotFound`) |

---

## 8. Seed data

Data is held **in memory only**. No database is used, and **all data resets to this seed when the application restarts**.

| ID | Tracking code | Owner | Device | Type | Status |
|---|---|---|---|---|---|
| `case-101` | `FT-84A2` | `user-a` | Laptop | Repair | Open |
| `case-102` | `FT-3K9D` | `user-a` | Smartphone | Warranty | InProgress |
| `case-103` | `FT-7Q2M` | `user-b` | Headphones | Return | Open |
| `case-104` | `FT-5T8X` | `user-b` | Tablet | Repair | Completed |

- Documented non-existent ID for demos and tests: **`case-999`**.
- New cases get IDs starting at **`case-105`**.
