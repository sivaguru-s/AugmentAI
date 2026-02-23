using IncidentResolution.API.Services;
using IncidentResolution.Core.Configuration;
using IncidentResolution.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure settings
builder.Services.Configure<ServiceNowSettings>(builder.Configuration.GetSection(ServiceNowSettings.SectionName));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));

// Register services
builder.Services.AddHttpClient<IServiceNowService, ServiceNowService>();
builder.Services.AddSingleton<IAIRecommendationService, KeywordMatchingService>(); // Local keyword matching - no API key required
builder.Services.AddSingleton<IEmailService, EmailService>();

// Configure CORS for Blazor WASM client
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:5002", "http://localhost:5003")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("BlazorClient");
app.UseAuthorization();
app.MapControllers();

app.Run();

