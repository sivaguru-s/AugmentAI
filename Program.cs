using Chatbot_Onbase.Data;
using Chatbot_Onbase.Services;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    Log.Information("========================================");
    Log.Information("Starting Onbase Invoice Chatbot application");
    Log.Information("Environment: {Environment}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");
    Log.Information("Machine: {MachineName}", Environment.MachineName);
    Log.Information("OS: {OS}", Environment.OSVersion);
    Log.Information("Current Directory: {Directory}", Directory.GetCurrentDirectory());
    Log.Information("========================================");

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Log configuration
var connectionString = builder.Configuration.GetConnectionString("OnBaseConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Log.Error("Connection string 'OnBaseConnection' not found in configuration!");
}
else
{
    // Log connection string without password
    var safeConnectionString = System.Text.RegularExpressions.Regex.Replace(
        connectionString,
        @"(Password|Pwd)=[^;]*",
        "$1=***");
    Log.Information("Database Connection String: {ConnectionString}", safeConnectionString);
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Onbase Invoice Chatbot API", 
        Version = "v1",
        Description = "AI-powered chatbot for searching invoice information from Onbase database without API key authentication"
    });
});

// Register application services
builder.Services.AddScoped<IOnbaseRepository, OnbaseRepository>();
builder.Services.AddScoped<IInvoiceQueryParser, InvoiceQueryParser>();
builder.Services.AddScoped<IInvoiceAnalyticsService, InvoiceAnalyticsService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

Log.Information("Application built successfully");
Log.Information("Configuring HTTP request pipeline...");

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Onbase Invoice Chatbot API v1");
    c.RoutePrefix = "swagger"; // Swagger UI at /swagger
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

Log.Information("========================================");
Log.Information("Application configured successfully");
Log.Information("Listening on: {Urls}", string.Join(", ", app.Urls));
Log.Information("Swagger UI: /swagger");
Log.Information("API Endpoint: /api/chatbot/query");
Log.Information("Health Check: /api/chatbot/health");
Log.Information("DB Test: /api/chatbot/dbtest");
Log.Information("========================================");
Log.Information("Application is ready to accept requests");

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

