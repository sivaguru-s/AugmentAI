# .NET Core + Blazor WASM vs .NET Core + Angular (for E-Pay)

## Executive summary
- Blazor WASM lets us build the whole stack in C#, sharing DTOs and validation across client/server. For an authenticated, data-heavy line-of-business app like E-Pay (grids, forms, exports), this increases consistency and developer velocity.
- Angular is mature with a massive ecosystem and proven rendering performance, but needs TypeScript/JS and duplicates model/validation logic across client/server.
- For E-Pay’s needs (no SEO, authenticated users, heavy API/data flows, shared models), Blazor WASM is a strong fit. We recommend a staged migration starting with the Invoices page.

## Strengths of Blazor WASM
- Single language end-to-end (C#) → shared DTOs/business rules, fewer mismatches
- Strong typing and tooling in one ecosystem (Visual Studio/.NET)
- Reuse: validation (DataAnnotations/FluentValidation) on both sides
- AOT in .NET 8 for faster runtime perf (trade-off: larger build size/time)
- Native DI, HttpClient, minimal JS interop needed
- Easy sharing of constants, enums, API contracts

## Strengths of Angular
- Massive UI/component ecosystem (Angular Material, PrimeNG, etc.)
- Excellent docs/community and established patterns (NgRx, RxJS)
- Mature SSR/SSG options (not needed for E-Pay)
- Many engineers already familiar with TS/JS

## Considerations/Risks with Blazor WASM
- Initial load size can be larger; AOT reduces runtime cost but increases build time
- Some complex UI scenarios may still need JS interop
- Third-party component ecosystem smaller than Angular’s (but growing)
- Local dev needs trusted HTTPS cert for API calls from browser (dotnet dev-certs https --trust)

## Fit for E-Pay
- Authenticated tool; SEO irrelevant → WASM fine
- Data grids, filtering, exports → straightforward in Blazor with HttpClient
- Existing .NET backend and DTOs → high reusability
- Current UI parity (Ashley theme) can be replicated in CSS

## Recommended approach
1) Keep ASP.NET Core API unchanged.
2) Add a standalone Blazor WASM client that calls the existing API.
3) Share DTOs gradually (start by duplicating DTOs in client for speed; optionally create a Shared project later).
4) Migrate screen-by-screen (start with Invoices), validate parity, then expand.

## Migration plan (phase 1)
- Scaffold Blazor WASM client at finance-credit-direct-epay/frontend-blazor/Epay.Blazor
- Configure HttpClient BaseAddress = https://localhost:5001/
- Implement Invoices page:
  - Fetch default date span
  - Search POST /api/Invoice/search
  - Render results grid; add Export to Excel
- Apply Ashley base styles (Open Sans, brand orange)
- Document setup (trust dev cert), build, and run steps

## Decision
Proceed with a Blazor WASM client alongside the current Angular app, migrate Invoices first, then iterate.

