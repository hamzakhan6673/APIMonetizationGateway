# API Monetization Gateway

## Overview
Gateway enforces per-tier rate limits and monthly quotas, logs usage, and produces monthly billing summaries.

## Run locally
1. Update `appsettings.json` connection string.
2. Run `dotnet ef database update` to apply migrations.
3. Start: `dotnet run --project src/APIMonetizationGateway.API`
4. (Optional) Start Angular client: `npm install && ng serve`

## Tests
`dotnet test src/APIMonetizationGateway.Tests`

## Notes
- Use Redis in production by registering `RedisRateCounterStore`.
- Use token-based auth to identify customers in headers/claims.
