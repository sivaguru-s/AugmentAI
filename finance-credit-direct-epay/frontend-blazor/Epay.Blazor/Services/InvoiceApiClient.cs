using System.Net.Http.Json;
using Epay.Blazor.Models;

namespace Epay.Blazor.Services;

public class InvoiceApiClient(HttpClient http)
{
    private readonly HttpClient _http = http;

    public async Task<int> GetDefaultDateSpanAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<int>("api/Invoice/default-date-span", ct);

    public async Task<InvoiceSearchResponse?> SearchInvoicesAsync(InvoiceSearchRequest request, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("api/Invoice/search", request, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<InvoiceSearchResponse>(cancellationToken: ct);
    }
}
