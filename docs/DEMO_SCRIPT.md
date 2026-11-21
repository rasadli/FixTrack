# FixTrack demo script

Target runtime is 4:40, leaving 20 seconds for transitions before the five-minute limit. Faig drives the browser and REST client throughout. The suggested speakers follow their project responsibilities.

## Before recording

- [ ] Confirm all assignment PRs have merged, including mock authentication and validation.
- [ ] From the repository root, run `dotnet test backend/FixTrack.slnx` and confirm the suite passes.
- [ ] Restart the API with `dotnet run --project backend/src/FixTrack.Api --launch-profile http` to restore the four seed cases. Keep it running throughout the take.
- [ ] Open the greeting page at `http://localhost:5072/api/v1/greeting` and the [example requests](../backend/src/FixTrack.Api/FixTrack.Api.http). Set `createdCaseId` to `case-105` for a fresh run.
- [ ] Close unrelated windows. Arrange the REST client so the request, response status, and body are visible together.

## Recording

Send each request below from the example file. The response cues describe the expected behavior on the merged application.

### 0:00–0:25 · Faig · Greeting page

Show the rendered greeting. Say “FixTrack tracks repairs, warranty claims, and returns. This public page shows the team, the API version, and the available endpoints.” Show the 200 HTML response without an authorization header.

```http
### View the anonymous greeting — 200 HTML
GET {{FixTrack.Api_HostAddress}}/api/v1/greeting
```

### 0:25–1:55 · Rashad · Successful CRUD

List user A’s cases and point out case-101 and case-102. Read case-101, then create the laptop repair. On the 201 response, show the Location header, the owner user-a, and the initial Open status. Use the ID from Location for the next two requests. Show the PUT returning 200 with InProgress, then the DELETE returning 200 with null data.

```http
### List user A’s cases — 200
GET {{FixTrack.Api_HostAddress}}/api/v1/cases
Authorization: Bearer {{userA}}

### Read user A’s own case — 200
GET {{FixTrack.Api_HostAddress}}/api/v1/cases/case-101
Authorization: Bearer {{userA}}

### Create a case as user A — 201, with the new case URL in Location
POST {{FixTrack.Api_HostAddress}}/api/v1/cases
Authorization: Bearer {{userA}}
Content-Type: application/json

{
  "customerName": "Alex Morgan",
  "device": "Laptop",
  "issueDescription": "The charging port stops working intermittently.",
  "caseType": "Repair"
}

### Replace the newly created case — 200
PUT {{FixTrack.Api_HostAddress}}/api/v1/cases/{{createdCaseId}}
Authorization: Bearer {{userA}}
Content-Type: application/json

{
  "customerName": "Alex Morgan",
  "device": "Laptop",
  "issueDescription": "The charging port stops working intermittently.",
  "caseType": "Repair",
  "status": "InProgress"
}

### Delete the newly created case — 200, with data set to null
DELETE {{FixTrack.Api_HostAddress}}/api/v1/cases/{{createdCaseId}}
Authorization: Bearer {{userA}}
```

### 1:55–2:25 · Khavar · Authentication failures

Send the list request without credentials and then with the unknown token. Show 401 and the UNAUTHORIZED code both times.

```http
### List cases without a token — 401
GET {{FixTrack.Api_HostAddress}}/api/v1/cases

### List cases with an invalid token — 401
GET {{FixTrack.Api_HostAddress}}/api/v1/cases
Authorization: Bearer nope
```

### 2:25–2:55 · Khavar · Cross-user access

User A requests case-103, which belongs to user B. Show the 404 and read the message “Case not found.” Leave this response available for comparison.

```http
### Read user B’s case as user A — 404
GET {{FixTrack.Api_HostAddress}}/api/v1/cases/case-103
Authorization: Bearer {{userA}}
```

### 2:55–3:20 · Khavar · Nonexistent case

Switch only the ID to case-999. Compare the two 404 responses. Say “Both give the same code and message, so user A cannot tell whether someone else owns that ID.”

```http
### Read a nonexistent case — 404
GET {{FixTrack.Api_HostAddress}}/api/v1/cases/case-999
Authorization: Bearer {{userA}}
```

### 3:20–3:55 · Rashad · Administrator access

List all cases with the admin token. Point out case-103, the two owner IDs, and a total of four. The case created earlier has already been deleted.

```http
### List all cases as admin — 200
GET {{FixTrack.Api_HostAddress}}/api/v1/cases
Authorization: Bearer {{admin}}
```

### 3:55–4:40 · Khavar, then Faig · Validation and close

Khavar sends the one-character customer name and shows 400, VALIDATION_ERROR, and the customerName entry in error.details. Faig closes with “The API keeps cases in memory. Restarting it restores the four cases we began with.” End the recording.

```http
### Create with a one-character customer name — 400
POST {{FixTrack.Api_HostAddress}}/api/v1/cases
Authorization: Bearer {{userA}}
Content-Type: application/json

{
  "customerName": "A",
  "device": "Laptop",
  "issueDescription": "The charging port stops working intermittently.",
  "caseType": "Repair"
}
```
