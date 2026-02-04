# Blazor WASM POC (E-Payment Invoices)

This adds a standalone Blazor WebAssembly client that calls the existing ASP.NET Core API.

## Paths
- Client: finance-credit-direct-epay/frontend-blazor/Epay.Blazor
- API: finance-credit-direct-epay/backend

## Prerequisites
- .NET 8 SDK
- Trust dev HTTPS cert so the browser can call https://localhost:5001
  - Windows: `dotnet dev-certs https --trust`

## Run
1) Start API (HTTPS):
   - `cd finance-credit-direct-epay/backend`
   - `dotnet run --launch-profile https`
2) Start Blazor client:
   - `cd finance-credit-direct-epay/frontend-blazor/Epay.Blazor`
   - `dotnet run`
3) Open the client URL printed by the dev server (e.g., http://localhost:5xxx) and navigate to "Invoices".

## What’s implemented
- Invoices page (/invoices)
  - Loads default date span from GET /api/Invoice/default-date-span
  - Submits search to POST /api/Invoice/search
  - Renders results table with key columns
- Ashley styling baseline
  - Open Sans font, base text color, primary button color (#DC6901)

## Notes
- API base address is configured in Program.cs to https://localhost:5001/
- If the browser blocks calls due to certificate trust, run the dev-certs command above and restart the browser.
- DTOs mirror backend: InvoiceDto, PagedResult<T>, InvoiceSearchResponse, InvoiceSearchRequest

## Next steps
- Export to Excel (POST /api/Invoice/export)
- Paging controls and sorting
- Shared DTOs project (optional) to avoid duplication
- Theming/components parity with the Ashley Direct Angular app

