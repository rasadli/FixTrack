## What

<!-- What does this PR change? Keep it to one feature or fix. -->

## Why

<!-- Which ticket or requirement does it address? -->

## How tested

Commands run:

```powershell
dotnet build backend/FixTrack.slnx
dotnet test backend/FixTrack.slnx
```

Scenarios checked manually (request → expected status):

- 

## README §9 scenarios covered

<!-- Which demo scenarios from README §9 does this PR make possible or change? -->

- [ ] Successful CRUD operations
- [ ] Authentication failures (401)
- [ ] Authorization failures (cross-user access)
- [ ] Administrator access
- [ ] Validation errors (400)
- [ ] Nonexistent resource (404)
- [ ] HTML greeting page

## Screenshots

<!-- Optional: responses from the .http file / Postman, or the greeting page. -->

## Checklist

- [ ] `dotnet build backend/FixTrack.slnx` succeeds
- [ ] `dotnet test backend/FixTrack.slnx` passes
- [ ] Docs updated (README, `docs/API_CONTRACT.md`, `.http` file) if behavior changed
- [ ] No secrets, real credentials, or `.env` files committed
- [ ] No invented data (student IDs, contributions, results)
