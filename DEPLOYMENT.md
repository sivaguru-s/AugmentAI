# Deployment Guide - Onbase Invoice Chatbot

This guide covers deploying the chatbot to various environments.

## Table of Contents
1. [IIS Deployment](#iis-deployment)
2. [Azure App Service](#azure-app-service)
3. [Docker Deployment](#docker-deployment)
4. [Production Checklist](#production-checklist)

---

## IIS Deployment

### Prerequisites
- Windows Server with IIS installed
- .NET 8.0 Hosting Bundle
- Access to Onbase SQL Server from the server

### Step 1: Install .NET Hosting Bundle

Download and install the ASP.NET Core 8.0 Hosting Bundle:
https://dotnet.microsoft.com/download/dotnet/8.0

### Step 2: Publish the Application

```bash
dotnet publish -c Release -o ./publish
```

### Step 3: Create IIS Application Pool

1. Open IIS Manager
2. Create new Application Pool:
   - Name: `OnbaseChatbot`
   - .NET CLR Version: `No Managed Code`
   - Managed Pipeline Mode: `Integrated`

### Step 4: Create IIS Website

1. Right-click Sites → Add Website
2. Configure:
   - Site name: `Onbase Chatbot`
   - Application pool: `OnbaseChatbot`
   - Physical path: `C:\inetpub\wwwroot\onbase-chatbot`
   - Binding: HTTP, Port 80 (or your preferred port)

### Step 5: Copy Published Files

Copy all files from `./publish` to `C:\inetpub\wwwroot\onbase-chatbot`

### Step 6: Configure Application Pool Identity

1. Select the Application Pool
2. Advanced Settings → Identity
3. Set to appropriate account with database access

### Step 7: Test

Navigate to `http://your-server-name` to verify deployment.

---

## Azure App Service

### Step 1: Create Azure App Service

```bash
az webapp create \
  --resource-group YourResourceGroup \
  --plan YourAppServicePlan \
  --name onbase-chatbot \
  --runtime "DOTNET|8.0"
```

### Step 2: Configure Connection String

In Azure Portal:
1. Go to your App Service
2. Configuration → Connection strings
3. Add new connection string:
   - Name: `OnBaseConnection`
   - Value: Your connection string (update for Azure)
   - Type: `SQLServer`

### Step 3: Deploy from Visual Studio

1. Right-click project → Publish
2. Select Azure → Azure App Service
3. Select your subscription and app service
4. Click Publish

### Step 4: Deploy from CLI

```bash
dotnet publish -c Release
cd bin/Release/net8.0/publish
az webapp deployment source config-zip \
  --resource-group YourResourceGroup \
  --name onbase-chatbot \
  --src publish.zip
```

---

## Docker Deployment

### Step 1: Create Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Chatbot-Onbase.csproj", "./"]
RUN dotnet restore "Chatbot-Onbase.csproj"
COPY . .
RUN dotnet build "Chatbot-Onbase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chatbot-Onbase.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chatbot-Onbase.dll"]
```

### Step 2: Build Docker Image

```bash
docker build -t onbase-chatbot:latest .
```

### Step 3: Run Container

```bash
docker run -d \
  -p 8080:80 \
  -e ConnectionStrings__OnBaseConnection="Your-Connection-String" \
  --name onbase-chatbot \
  onbase-chatbot:latest
```

---

## Production Checklist

### Security

- [ ] Update CORS policy to specific origins
- [ ] Enable HTTPS/SSL
- [ ] Implement authentication/authorization
- [ ] Use secure connection strings (Azure Key Vault, etc.)
- [ ] Enable request rate limiting
- [ ] Implement input validation and sanitization
- [ ] Review and minimize logging of sensitive data

### Performance

- [ ] Enable response caching
- [ ] Implement connection pooling
- [ ] Add database query optimization
- [ ] Configure appropriate timeout values
- [ ] Enable compression
- [ ] Set up CDN for static files

### Monitoring

- [ ] Configure Application Insights or similar
- [ ] Set up health check endpoints
- [ ] Enable detailed error logging
- [ ] Configure alerts for errors/downtime
- [ ] Set up performance monitoring

### Configuration

- [ ] Update `appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "your-domain.com"
}
```

- [ ] Update CORS policy in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://your-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Database

- [ ] Verify database connection from production server
- [ ] Test with production database
- [ ] Ensure proper indexes on frequently queried columns
- [ ] Set up database backup strategy
- [ ] Configure connection string encryption

### Testing

- [ ] Perform load testing
- [ ] Test all query types
- [ ] Verify error handling
- [ ] Test database failover scenarios
- [ ] Validate response times

---

## Environment Variables

For production, use environment variables instead of appsettings.json:

### Windows
```powershell
$env:ConnectionStrings__OnBaseConnection="Your-Connection-String"
$env:ASPNETCORE_ENVIRONMENT="Production"
```

### Linux
```bash
export ConnectionStrings__OnBaseConnection="Your-Connection-String"
export ASPNETCORE_ENVIRONMENT="Production"
```

---

## Troubleshooting Production Issues

### Application Won't Start

1. Check Event Viewer (Windows) or logs
2. Verify .NET Hosting Bundle is installed
3. Check Application Pool identity has proper permissions
4. Verify connection string is correct

### Database Connection Fails

1. Test connection from server using SQL Management Studio
2. Verify firewall rules
3. Check SQL Server allows remote connections
4. Verify Application Pool identity has database access

### Performance Issues

1. Check database query performance
2. Review application logs for slow queries
3. Monitor server resources (CPU, Memory)
4. Consider implementing caching

---

## Support & Maintenance

### Regular Maintenance Tasks

- Monitor application logs weekly
- Review and optimize slow queries monthly
- Update dependencies quarterly
- Review security patches monthly
- Backup configuration files

### Updating the Application

1. Test updates in staging environment
2. Create backup of current deployment
3. Deploy during maintenance window
4. Verify functionality post-deployment
5. Monitor for errors

---

**For additional support, refer to README.md and QUICKSTART.md**

