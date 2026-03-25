using FinanceBudget.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpClient for API integrations
builder.Services.AddHttpClient<IAFEDataService, AFEDataService>();
builder.Services.AddHttpClient<IJiraCostingService, JiraCostingService>();

// Register application services
builder.Services.AddScoped<IUnitBudgetService, UnitBudgetService>();
builder.Services.AddScoped<IAFEDataService, AFEDataService>();
builder.Services.AddScoped<IJiraCostingService, JiraCostingService>();

// Register new services for Planful and AFE Budget
builder.Services.AddScoped<IPlanfulService, PlanfulService>();
builder.Services.AddScoped<IAFEBudgetService, AFEBudgetService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
