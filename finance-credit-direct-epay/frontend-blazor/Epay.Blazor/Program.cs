using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Epay.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Use backend API as the default HttpClient base for simplicity in this POC
// Note: ensure the dev HTTPS certificate is trusted locally
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001/") });
builder.Services.AddScoped<Epay.Blazor.Services.InvoiceApiClient>();

await builder.Build().RunAsync();
