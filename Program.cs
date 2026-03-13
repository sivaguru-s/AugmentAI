using Chatbot_Onbase.Data;
using Chatbot_Onbase.Services;

var builder = WebApplication.CreateBuilder(args);

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

app.Run();

