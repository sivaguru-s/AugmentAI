# Migration Plan: VB.NET to .NET Core with Modern Frontend

**Project:** Finance Credit Direct EPay System Migration  
**Version:** 1.0  
**Date:** January 20, 2026  
**Status:** Planning Phase  

---

## Executive Summary

This document outlines the comprehensive migration plan for transforming the Finance Credit Direct EPay system from:
- **Backend:** ASP.NET Web Forms (VB.NET, .NET Framework 3.5) → **ASP.NET Core 8.0 (C#)**
- **Frontend:** ASP.NET Web Forms → **React 18 with TypeScript**
- **Preserving:** Existing UI/UX design, DB2 connectivity, US Bank EPay gateway integration

### Migration Objectives

✅ **Modernize Technology Stack** - Move to .NET Core 8.0 and React 18  
✅ **Preserve Design** - Maintain existing UI/UX look and feel  
✅ **Maintain Integrations** - Keep DB2 and US Bank gateway flows unchanged  
✅ **Improve Security** - Address all critical vulnerabilities  
✅ **Enhance Performance** - Leverage modern framework capabilities  
✅ **Enable Scalability** - Cloud-ready architecture  

### Key Constraints

🔒 **Design Preservation** - UI must match existing screenshots pixel-perfect  
🔒 **DB2 Integration** - Maintain existing DB2 connectivity and stored procedures  
🔒 **US Bank Gateway** - Preserve US Bank EPay gateway integration flow  
🔒 **Zero Downtime** - Parallel deployment strategy required  
🔒 **Data Integrity** - No data loss during migration  

---

## Current State Analysis

### Technology Stack (Current)

| Component | Technology | Version |
|-----------|-----------|---------|
| **Backend Framework** | ASP.NET Web Forms | .NET Framework 3.5 |
| **Language** | Visual Basic .NET | VB.NET 9.0 |
| **Frontend** | ASP.NET Web Forms | Server-side rendering |
| **UI Components** | ASP.NET Controls | GridView, Repeater, etc. |
| **JavaScript** | jQuery | 1.x |
| **Database (Primary)** | SQL Server | 2008+ |
| **Database (Legacy)** | IBM DB2 (iSeries) | AS/400 |
| **Reporting** | GrapeCity ActiveReports | 7.1 |
| **Authentication** | Windows Authentication | Active Directory |
| **Payment Gateway** | US Bank EPay | Custom integration |
| **Web Server** | IIS | 7.5+ |

### Application Architecture (Current)

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  (ASP.NET Web Forms - VB.NET)                               │
│  - main.aspx, Confirmation.aspx, History.aspx, etc.         │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    Business Logic Layer                      │
│  (VB.NET Classes)                                           │
│  - Common.vb, EpayBasePage.vb                               │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    Data Access Layer                         │
│  - DataAccess.vb (SQL Server)                               │
│  - Db2DataAccess.vb (DB2/AS400)                             │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────┬──────────────────┬──────────────────────┐
│   SQL Server     │    DB2/AS400     │   US Bank Gateway    │
│   (Datawhse)     │   (Legacy Data)  │   (Payment)          │
└──────────────────┴──────────────────┴──────────────────────┘
```

### Key Pages/Features

| Page | Purpose | Complexity |
|------|---------|------------|
| **main.aspx** | Invoice search and selection | High |
| **Confirmation.aspx** | Payment confirmation | Medium |
| **History.aspx** | Payment history | Medium |
| **AnalystReport.aspx** | Analyst reporting | High |
| **UserList.aspx** | User management | Low |
| **AdminMaintenance.aspx** | Admin functions | Medium |
| **ViewPayment.aspx** | Payment details | Low |

### External Dependencies

1. **Ashley.Web.Responsive** - Custom UI framework
2. **Ashley.Data.DataAccess** - Data access library
3. **GrapeCity ActiveReports** - Reporting engine
4. **IBM.Data.DB2.iSeries** - DB2 connector
5. **US Bank EPay Gateway** - Payment processing

---

## Target State Architecture

### Technology Stack (Target)

| Component | Technology | Version | Rationale |
|-----------|-----------|---------|-----------|
| **Backend Framework** | ASP.NET Core Web API | 8.0 LTS | Modern, cross-platform, high performance |
| **Language** | C# | 12.0 | Industry standard, better tooling |
| **Frontend Framework** | React | 18.2+ | Component-based, virtual DOM, large ecosystem |
| **UI Language** | TypeScript | 5.0+ | Type safety, better IDE support |
| **UI Component Library** | Material-UI (MUI) | 5.x | Customizable, matches existing design |
| **State Management** | Redux Toolkit | 2.0+ | Predictable state, dev tools |
| **HTTP Client** | Axios | 1.6+ | Promise-based, interceptors |
| **Database (Primary)** | SQL Server | 2019+ | Unchanged |
| **Database (Legacy)** | IBM DB2 (iSeries) | AS/400 | Unchanged |
| **DB2 Connector** | IBM.Data.DB2.Core | Latest | .NET Core compatible |
| **Reporting** | Telerik Reporting | Latest | .NET Core compatible alternative |
| **Authentication** | Azure AD / SAML 2.0 | Latest | Modern auth, SSO capable |
| **API Documentation** | Swagger/OpenAPI | 3.0 | Auto-generated docs |
| **Payment Gateway** | US Bank EPay | Existing | Unchanged integration |
| **Containerization** | Docker | Latest | Cloud-ready deployment |
| **Orchestration** | Kubernetes (optional) | Latest | Scalability |

### Target Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Frontend (React + TypeScript)                │
│  ┌──────────────┬──────────────┬──────────────┬──────────────────┐  │
│  │ Invoice Page │ Confirmation │ History Page │ Admin/Reports    │  │
│  │ (main)       │ Page         │              │                  │  │
│  └──────────────┴──────────────┴──────────────┴──────────────────┘  │
│                                                                       │
│  State Management: Redux Toolkit                                     │
│  UI Components: Material-UI (Customized to match existing design)    │
│  HTTP Client: Axios with Interceptors                                │
└─────────────────────────────────────────────────────────────────────┘
                                    ↓ HTTPS/REST API
┌─────────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core 8.0 Web API                          │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │                      API Controllers                          │   │
│  │  - InvoiceController, PaymentController, ReportController    │   │
│  └──────────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │                    Business Logic Layer                       │   │
│  │  - Services (InvoiceService, PaymentService, etc.)           │   │
│  │  - DTOs, Validators, Mappers                                 │   │
│  └──────────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │                    Data Access Layer                          │   │
│  │  - Repositories (SQL Server, DB2)                            │   │
│  │  - Entity Framework Core (SQL Server)                        │   │
│  │  - IBM.Data.DB2.Core (DB2/AS400)                             │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                       │
│  Cross-Cutting Concerns:                                             │
│  - Authentication/Authorization (JWT + Azure AD)                     │
│  - Logging (Serilog)                                                 │
│  - Exception Handling (Middleware)                                   │
│  - Validation (FluentValidation)                                     │
└─────────────────────────────────────────────────────────────────────┘
                                    ↓
┌──────────────────┬──────────────────┬──────────────────────────────┐
│   SQL Server     │    DB2/AS400     │   US Bank EPay Gateway       │
│   (Datawhse)     │   (Legacy Data)  │   (Unchanged Integration)    │
│   - EF Core      │   - IBM.Data.DB2 │   - HTTP POST to Gateway     │
│   - Stored Procs │   - Native Calls │   - Response Handling        │
└──────────────────┴──────────────────┴──────────────────────────────┘
```

---

## Migration Strategy

### Approach: **Strangler Fig Pattern**

We will use the **Strangler Fig Pattern** to gradually replace the legacy system:

1. **Build new functionality alongside old system**
2. **Route traffic incrementally to new system**
3. **Decommission old system once fully replaced**

### Benefits

✅ **Low Risk** - Gradual migration with rollback capability
✅ **Continuous Delivery** - Deploy features incrementally
✅ **Business Continuity** - No disruption to users
✅ **Parallel Testing** - Compare old vs new side-by-side

### Migration Phases Overview

| Phase | Duration | Description | Deliverables |
|-------|----------|-------------|--------------|
| **Phase 0** | 2 weeks | Planning & Setup | Environment, tools, team training |
| **Phase 1** | 4 weeks | Backend API Foundation | Core API, authentication, DB access |
| **Phase 2** | 6 weeks | Frontend Foundation | React app, routing, state management |
| **Phase 3** | 8 weeks | Feature Migration (Invoices) | Invoice search, selection, display |
| **Phase 4** | 6 weeks | Feature Migration (Payments) | Payment flow, US Bank integration |
| **Phase 5** | 4 weeks | Feature Migration (History) | Payment history, reporting |
| **Phase 6** | 4 weeks | Feature Migration (Admin) | Admin functions, user management |
| **Phase 7** | 4 weeks | Testing & Security | Security audit, penetration testing |
| **Phase 8** | 2 weeks | Deployment & Cutover | Production deployment, monitoring |
| **Phase 9** | 2 weeks | Stabilization & Support | Bug fixes, performance tuning |

**Total Duration:** ~42 weeks (~10 months)

---

## Phase 0: Planning & Setup (2 Weeks)

### Objectives

- Set up development environment
- Establish CI/CD pipeline
- Train development team
- Create project structure
- Set up version control strategy

### Tasks

#### Week 1: Environment Setup

**1.1 Development Environment**
- [ ] Install .NET 8.0 SDK
- [ ] Install Node.js 20 LTS
- [ ] Install Visual Studio 2022 / VS Code
- [ ] Install SQL Server Management Studio
- [ ] Install Docker Desktop
- [ ] Install Git and configure repositories
- [ ] Set up DB2 client libraries

**1.2 Project Structure**
```
finance-credit-direct-epay-v2/
├── backend/
│   ├── src/
│   │   ├── EPay.API/                 # Web API project
│   │   ├── EPay.Core/                # Business logic
│   │   ├── EPay.Infrastructure/      # Data access
│   │   └── EPay.Shared/              # DTOs, Constants
│   ├── tests/
│   │   ├── EPay.API.Tests/
│   │   ├── EPay.Core.Tests/
│   │   └── EPay.Infrastructure.Tests/
│   └── EPay.sln
├── frontend/
│   ├── public/
│   ├── src/
│   │   ├── components/               # Reusable components
│   │   ├── pages/                    # Page components
│   │   ├── services/                 # API services
│   │   ├── store/                    # Redux store
│   │   ├── styles/                   # CSS/SCSS
│   │   ├── types/                    # TypeScript types
│   │   └── utils/                    # Utilities
│   ├── package.json
│   └── tsconfig.json
├── docs/
│   ├── api/                          # API documentation
│   ├── architecture/                 # Architecture docs
│   └── migration/                    # Migration guides
├── scripts/
│   ├── database/                     # DB migration scripts
│   └── deployment/                   # Deployment scripts
└── docker-compose.yml
```

**1.3 Version Control Strategy**
- [ ] Create new repository: `finance-credit-direct-epay-v2`
- [ ] Set up branching strategy (GitFlow)
  - `main` - Production-ready code
  - `develop` - Integration branch
  - `feature/*` - Feature branches
  - `release/*` - Release branches
  - `hotfix/*` - Hotfix branches
- [ ] Configure branch protection rules
- [ ] Set up code review requirements

#### Week 2: CI/CD & Team Training

**2.1 CI/CD Pipeline**
- [ ] Set up Azure DevOps / GitHub Actions
- [ ] Configure build pipelines
  - Backend: .NET build, test, publish
  - Frontend: npm build, test, bundle
- [ ] Configure deployment pipelines
  - Dev environment (auto-deploy)
  - Staging environment (manual approval)
  - Production environment (manual approval)
- [ ] Set up automated testing
- [ ] Configure code quality gates (SonarQube)

**2.2 Team Training**
- [ ] .NET Core 8.0 training (for VB.NET developers)
- [ ] C# language features training
- [ ] React & TypeScript training
- [ ] RESTful API design principles
- [ ] Docker & containerization basics
- [ ] Security best practices

**2.3 Documentation**
- [ ] Create coding standards document
- [ ] Create API design guidelines
- [ ] Create UI component library guidelines
- [ ] Create database migration procedures

### Deliverables

✅ Development environment ready for all team members
✅ Project structure created and committed to repository
✅ CI/CD pipeline operational
✅ Team trained on new technologies
✅ Documentation framework established

---

## Phase 1: Backend API Foundation (4 Weeks)

### Objectives

- Create ASP.NET Core 8.0 Web API project
- Implement authentication and authorization
- Set up database connectivity (SQL Server & DB2)
- Create core infrastructure (logging, error handling)
- Implement first API endpoints

### Week 1: Project Setup & Infrastructure

**1.1 Create ASP.NET Core Web API Project**

```bash
dotnet new webapi -n EPay.API -o backend/src/EPay.API
dotnet new classlib -n EPay.Core -o backend/src/EPay.Core
dotnet new classlib -n EPay.Infrastructure -o backend/src/EPay.Infrastructure
dotnet new classlib -n EPay.Shared -o backend/src/EPay.Shared
dotnet new sln -n EPay -o backend
dotnet sln backend/EPay.sln add backend/src/**/*.csproj
```

**1.2 Install NuGet Packages**

```xml
<!-- EPay.API -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.*" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.*" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.*" />

<!-- EPay.Infrastructure -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.*" />
<PackageReference Include="IBM.Data.DB2.Core" Version="3.1.*" />
<PackageReference Include="Dapper" Version="2.1.*" />

<!-- EPay.Core -->
<PackageReference Include="AutoMapper" Version="12.0.*" />
<PackageReference Include="FluentValidation" Version="11.9.*" />
```

**1.3 Configure Dependency Injection (Program.cs)**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://epay.ashleyfurniture.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure logging
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Configure authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["AzureAd:Authority"];
        options.Audience = builder.Configuration["AzureAd:Audience"];
    });

// Register application services
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Configure database contexts
builder.Services.AddDbContext<EPayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// Configure DB2 connection
builder.Services.AddSingleton<IDb2ConnectionFactory, Db2ConnectionFactory>();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

**1.4 Implement Logging with Serilog**

```json
// appsettings.json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/epay-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

### Week 2: Database Connectivity

**2.1 SQL Server - Entity Framework Core**

```csharp
// EPay.Infrastructure/Data/EPayDbContext.cs
public class EPayDbContext : DbContext
{
    public EPayDbContext(DbContextOptions<EPayDbContext> options) : base(options) { }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentDetail> PaymentDetails { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EPayDbContext).Assembly);
    }
}

// EPay.Infrastructure/Data/Configurations/PaymentConfiguration.cs
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("EPay_Payments", "Datawhse");
        builder.HasKey(p => p.ReferenceNumber);
        builder.Property(p => p.CustomerNumber).HasMaxLength(10).IsRequired();
        builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
        // ... more configuration
    }
}
```

**2.2 DB2 Connectivity (Preserve Existing Pattern)**

```csharp
// EPay.Infrastructure/Data/Db2/Db2ConnectionFactory.cs
public interface IDb2ConnectionFactory
{
    iDB2Connection CreateConnection();
}

public class Db2ConnectionFactory : IDb2ConnectionFactory
{
    private readonly string _connectionString;

    public Db2ConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DB2");
    }

    public iDB2Connection CreateConnection()
    {
        var connection = new iDB2Connection(_connectionString);
        connection.Open();
        return connection;
    }
}

// EPay.Infrastructure/Repositories/Db2InvoiceRepository.cs
public class Db2InvoiceRepository : IDb2InvoiceRepository
{
    private readonly IDb2ConnectionFactory _connectionFactory;

    public Db2InvoiceRepository(IDb2ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Invoice>> GetOpenInvoicesAsync(string customerNumber)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new iDB2Command("CALL GETOPNINV(?)", connection);

        command.Parameters.Add(new iDB2Parameter("@CustomerNumber", customerNumber));

        using var reader = await command.ExecuteReaderAsync();
        var invoices = new List<Invoice>();

        while (await reader.ReadAsync())
        {
            invoices.Add(MapToInvoice(reader));
        }

        return invoices;
    }

    private Invoice MapToInvoice(iDB2DataReader reader)
    {
        return new Invoice
        {
            InvoiceNumber = reader.GetString(reader.GetOrdinal("INVNUM")),
            InvoiceDate = reader.GetDateTime(reader.GetOrdinal("INVDTE")),
            Amount = reader.GetDecimal(reader.GetOrdinal("INVAMT")),
            // ... map other fields
        };
    }
}
```

**2.3 Repository Pattern Implementation**

```csharp
// EPay.Core/Interfaces/IInvoiceRepository.cs
public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetOpenInvoicesAsync(string customerNumber, string shipToNumber);
    Task<Invoice> GetInvoiceByNumberAsync(string invoiceNumber);
    Task<decimal> GetInvoiceTotalAsync(string customerNumber, int referenceNumber);
}

// EPay.Infrastructure/Repositories/InvoiceRepository.cs
public class InvoiceRepository : IInvoiceRepository
{
    private readonly EPayDbContext _context;
    private readonly IDb2InvoiceRepository _db2Repository;

    public InvoiceRepository(EPayDbContext context, IDb2InvoiceRepository db2Repository)
    {
        _context = context;
        _db2Repository = db2Repository;
    }

    public async Task<IEnumerable<Invoice>> GetOpenInvoicesAsync(string customerNumber, string shipToNumber)
    {
        // Get invoices from DB2 (legacy system)
        return await _db2Repository.GetOpenInvoicesAsync(customerNumber);
    }

    public async Task<decimal> GetInvoiceTotalAsync(string customerNumber, int referenceNumber)
    {
        // Call stored procedure (preserve existing logic)
        var total = await _context.Database
            .SqlQueryRaw<decimal>("EXEC Datawhse.dbo.usp_GetEpayTotal @CustomerNumber, @ReferenceNumber",
                new SqlParameter("@CustomerNumber", customerNumber),
                new SqlParameter("@ReferenceNumber", referenceNumber))
            .FirstOrDefaultAsync();

        return total;
    }
}
```

### Week 3: Authentication & Authorization

**3.1 JWT Authentication Setup**

```csharp
// EPay.API/Controllers/AuthController.cs
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.AuthenticateAsync(request.Username, request.Password);

        if (!result.Success)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(new
        {
            token = result.Token,
            user = result.User,
            expiresAt = result.ExpiresAt
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (!result.Success)
            return Unauthorized();

        return Ok(new { token = result.Token });
    }
}
```

**3.2 Windows Authentication Integration (Preserve AD)**

```csharp
// EPay.Core/Services/AuthService.cs
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public async Task<AuthResult> AuthenticateAsync(string username, string password)
    {
        // Validate against Active Directory (preserve existing logic)
        using var context = new PrincipalContext(ContextType.Domain, "ASHLEYFURNITURE");

        if (!context.ValidateCredentials(username, password))
            return AuthResult.Failed("Invalid credentials");

        // Get user from database
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null)
            return AuthResult.Failed("User not found");

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return AuthResult.Success(token, user);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("CustomerNumber", user.CustomerNumber),
                new Claim("SecurityMHS", user.SecurityMHS)
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
```

### Week 4: Core API Endpoints

**4.1 Invoice API Controller**

```csharp
// EPay.API/Controllers/InvoiceController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<InvoiceController> _logger;

    public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    [HttpGet("open")]
    public async Task<IActionResult> GetOpenInvoices(
        [FromQuery] string customerNumber,
        [FromQuery] string? shipToNumber = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var invoices = await _invoiceService.GetOpenInvoicesAsync(
                customerNumber, shipToNumber, fromDate, toDate);

            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving open invoices for customer {CustomerNumber}", customerNumber);
            return StatusCode(500, new { message = "An error occurred while retrieving invoices" });
        }
    }

    [HttpGet("{invoiceNumber}")]
    public async Task<IActionResult> GetInvoice(string invoiceNumber)
    {
        var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);

        if (invoice == null)
            return NotFound();

        return Ok(invoice);
    }

    [HttpPost("select")]
    public async Task<IActionResult> SelectInvoices([FromBody] SelectInvoicesRequest request)
    {
        var result = await _invoiceService.SelectInvoicesForPaymentAsync(
            request.CustomerNumber, request.InvoiceNumbers);

        return Ok(result);
    }
}
```

**4.2 Payment API Controller**

```csharp
// EPay.API/Controllers/PaymentController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(request);
            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            return StatusCode(500, new { message = "An error occurred while creating payment" });
        }
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
    {
        // This will integrate with US Bank gateway (preserve existing flow)
        var result = await _paymentService.ConfirmPaymentAsync(request);

        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetPaymentHistory(
        [FromQuery] string customerNumber,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var history = await _paymentService.GetPaymentHistoryAsync(customerNumber, fromDate, toDate);
        return Ok(history);
    }
}
```

**4.3 DTOs and Models**

```csharp
// EPay.Shared/DTOs/InvoiceDto.cs
public class InvoiceDto
{
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal InvoiceAmount { get; set; }
    public decimal AmountDue { get; set; }
    public string CustomerNumber { get; set; }
    public string ShipToNumber { get; set; }
    public string PONumber { get; set; }
    public string Status { get; set; }
}

// EPay.Shared/DTOs/CreatePaymentRequest.cs
public class CreatePaymentRequest
{
    public string CustomerNumber { get; set; }
    public List<string> InvoiceNumbers { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string BankAccountNumber { get; set; }
    public string RoutingNumber { get; set; }
}
```

### Deliverables - Phase 1

✅ ASP.NET Core 8.0 Web API project structure
✅ Authentication & authorization implemented
✅ SQL Server connectivity with EF Core
✅ DB2 connectivity preserved (IBM.Data.DB2.Core)
✅ Core API endpoints (Invoice, Payment)
✅ Logging and error handling infrastructure
✅ Swagger/OpenAPI documentation
✅ Unit tests for core services

---

## Phase 2: Frontend Foundation (6 Weeks)

### Objectives

- Create React 18 application with TypeScript
- Set up routing and state management
- Create UI component library matching existing design
- Implement authentication flow
- Create layout and navigation components

### Week 1: React Project Setup

**1.1 Create React App with TypeScript**

```bash
npx create-react-app frontend --template typescript
cd frontend
npm install @mui/material @emotion/react @emotion/styled
npm install @reduxjs/toolkit react-redux
npm install react-router-dom
npm install axios
npm install @types/react-router-dom
```

**1.2 Project Structure**

```
frontend/src/
├── components/
│   ├── common/
│   │   ├── Button/
│   │   ├── Input/
│   │   ├── Table/
│   │   ├── DatePicker/
│   │   └── Modal/
│   ├── layout/
│   │   ├── Header/
│   │   ├── Footer/
│   │   ├── Navigation/
│   │   └── Sidebar/
│   └── features/
│       ├── invoices/
│       ├── payments/
│       └── history/
├── pages/
│   ├── InvoicePage/
│   ├── ConfirmationPage/
│   ├── HistoryPage/
│   └── AdminPage/
├── services/
│   ├── api/
│   │   ├── invoiceApi.ts
│   │   ├── paymentApi.ts
│   │   └── authApi.ts
│   └── http/
│       └── httpClient.ts
├── store/
│   ├── slices/
│   │   ├── authSlice.ts
│   │   ├── invoiceSlice.ts
│   │   └── paymentSlice.ts
│   └── store.ts
├── types/
│   ├── invoice.types.ts
│   ├── payment.types.ts
│   └── user.types.ts
├── utils/
│   ├── formatters.ts
│   ├── validators.ts
│   └── constants.ts
├── styles/
│   ├── theme.ts
│   └── global.css
├── App.tsx
└── index.tsx
```

**1.3 Configure TypeScript**

```json
// tsconfig.json
{
  "compilerOptions": {
    "target": "ES2020",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "jsx": "react-jsx",
    "module": "ESNext",
    "moduleResolution": "node",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true,
    "forceConsistentCasingInFileNames": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "baseUrl": "src",
    "paths": {
      "@components/*": ["components/*"],
      "@pages/*": ["pages/*"],
      "@services/*": ["services/*"],
      "@store/*": ["store/*"],
      "@types/*": ["types/*"],
      "@utils/*": ["utils/*"]
    }
  },
  "include": ["src"]
}
```

### Week 2: Theme & Design System

**2.1 Create Custom Theme (Match Existing Design)**

```typescript
// src/styles/theme.ts
import { createTheme } from '@mui/material/styles';

// Colors extracted from existing Ashley Furniture design
export const theme = createTheme({
  palette: {
    primary: {
      main: '#8B4513', // Ashley brown
      light: '#A0522D',
      dark: '#654321',
      contrastText: '#FFFFFF',
    },
    secondary: {
      main: '#D2691E', // Chocolate
      light: '#E6A85C',
      dark: '#8B4513',
      contrastText: '#FFFFFF',
    },
    background: {
      default: '#F5F5F5',
      paper: '#FFFFFF',
    },
    text: {
      primary: '#333333',
      secondary: '#666666',
    },
  },
  typography: {
    fontFamily: '"Arial", "Helvetica", sans-serif',
    h1: {
      fontSize: '2rem',
      fontWeight: 600,
    },
    h2: {
      fontSize: '1.5rem',
      fontWeight: 600,
    },
    body1: {
      fontSize: '0.875rem',
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          borderRadius: 4,
        },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          backgroundColor: '#8B4513',
          color: '#FFFFFF',
          fontWeight: 600,
        },
      },
    },
  },
});
```

**2.2 Create Reusable Components**

```typescript
// src/components/common/Table/DataTable.tsx
import { DataGrid, GridColDef } from '@mui/x-data-grid';

interface DataTableProps {
  columns: GridColDef[];
  rows: any[];
  loading?: boolean;
  onRowClick?: (row: any) => void;
}

export const DataTable: React.FC<DataTableProps> = ({ columns, rows, loading, onRowClick }) => {
  return (
    <DataGrid
      rows={rows}
      columns={columns}
      loading={loading}
      onRowClick={(params) => onRowClick?.(params.row)}
      pageSizeOptions={[10, 25, 50, 100]}
      initialState={{
        pagination: { paginationModel: { pageSize: 25 } },
      }}
      checkboxSelection
      disableRowSelectionOnClick
    />
  );
};
```

### Week 3-4: State Management & API Integration

**3.1 Redux Store Setup**

```typescript
// src/store/store.ts
import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import invoiceReducer from './slices/invoiceSlice';
import paymentReducer from './slices/paymentSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    invoice: invoiceReducer,
    payment: paymentReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
```

**3.2 Auth Slice**

```typescript
// src/store/slices/authSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { authApi } from '@services/api/authApi';

export const login = createAsyncThunk(
  'auth/login',
  async (credentials: { username: string; password: string }) => {
    const response = await authApi.login(credentials);
    localStorage.setItem('token', response.token);
    return response;
  }
);

const authSlice = createSlice({
  name: 'auth',
  initialState: {
    user: null,
    token: localStorage.getItem('token'),
    isAuthenticated: false,
    loading: false,
  },
  reducers: {
    logout: (state) => {
      state.user = null;
      state.token = null;
      state.isAuthenticated = false;
      localStorage.removeItem('token');
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.loading = true;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.user = action.payload.user;
        state.token = action.payload.token;
        state.isAuthenticated = true;
        state.loading = false;
      })
      .addCase(login.rejected, (state) => {
        state.loading = false;
      });
  },
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;
```

**3.3 HTTP Client with Interceptors**

```typescript
// src/services/http/httpClient.ts
import axios from 'axios';

const httpClient = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7001/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - add auth token
httpClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor - handle errors
httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default httpClient;
```

### Week 5-6: Layout & Navigation

**5.1 Header Component (Match Existing Design)**

```typescript
// src/components/layout/Header/Header.tsx
import { AppBar, Toolbar, Typography, Button, Box } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '@store/hooks';
import { logout } from '@store/slices/authSlice';

export const Header: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const user = useAppSelector((state) => state.auth.user);

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  return (
    <AppBar position="static" sx={{ backgroundColor: '#8B4513' }}>
      <Toolbar>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          Ashley Furniture - EPay
        </Typography>
        <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
          <Typography variant="body2">
            Welcome, {user?.username}
          </Typography>
          <Typography variant="body2">
            Customer: {user?.customerNumber}
          </Typography>
          <Button color="inherit" onClick={handleLogout}>
            Logout
          </Button>
        </Box>
      </Toolbar>
    </AppBar>
  );
};
```

**5.2 Navigation Component**

```typescript
// src/components/layout/Navigation/Navigation.tsx
import { Tabs, Tab, Box } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';

export const Navigation: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const tabs = [
    { label: 'Invoices', path: '/invoices' },
    { label: 'Payment History', path: '/history' },
    { label: 'Reports', path: '/reports' },
    { label: 'Admin', path: '/admin' },
  ];

  return (
    <Box sx={{ borderBottom: 1, borderColor: 'divider', backgroundColor: '#F5F5F5' }}>
      <Tabs value={location.pathname} onChange={(_, value) => navigate(value)}>
        {tabs.map((tab) => (
          <Tab key={tab.path} label={tab.label} value={tab.path} />
        ))}
      </Tabs>
    </Box>
  );
};
```

### Deliverables - Phase 2

✅ React 18 application with TypeScript
✅ Material-UI theme matching existing design
✅ Redux Toolkit state management
✅ Routing with React Router
✅ HTTP client with authentication
✅ Reusable component library
✅ Layout and navigation components
✅ Authentication flow (login/logout)

---

## Phase 3: Feature Migration - Invoices (8 Weeks)

### Objectives

- Migrate invoice search functionality
- Migrate invoice selection and display
- Implement Excel export
- Preserve all existing business logic

### Week 1-2: Invoice Search Page

**Invoice Search Component (Matching main.aspx)**

```typescript
// src/pages/InvoicePage/InvoiceSearch.tsx
import { useState } from 'react';
import { Box, TextField, Button, Grid, Paper } from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers';
import { DataTable } from '@components/common/Table/DataTable';
import { useAppDispatch, useAppSelector } from '@store/hooks';
import { searchInvoices } from '@store/slices/invoiceSlice';

export const InvoiceSearch: React.FC = () => {
  const dispatch = useAppDispatch();
  const { invoices, loading } = useAppSelector((state) => state.invoice);

  const [filters, setFilters] = useState({
    invoiceNumber: '',
    poNumber: '',
    fromDate: null,
    toDate: null,
  });

  const handleSearch = () => {
    dispatch(searchInvoices(filters));
  };

  const columns = [
    { field: 'invoiceNumber', headerName: 'Invoice #', width: 150 },
    { field: 'invoiceDate', headerName: 'Invoice Date', width: 120 },
    { field: 'dueDate', headerName: 'Due Date', width: 120 },
    { field: 'poNumber', headerName: 'PO #', width: 150 },
    { field: 'invoiceAmount', headerName: 'Invoice Amount', width: 150, type: 'number' },
    { field: 'amountDue', headerName: 'Amount Due', width: 150, type: 'number' },
  ];

  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ p: 3, mb: 3 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} md={3}>
            <TextField
              fullWidth
              label="Invoice Number"
              value={filters.invoiceNumber}
              onChange={(e) => setFilters({ ...filters, invoiceNumber: e.target.value })}
            />
          </Grid>
          <Grid item xs={12} md={3}>
            <TextField
              fullWidth
              label="PO Number"
              value={filters.poNumber}
              onChange={(e) => setFilters({ ...filters, poNumber: e.target.value })}
            />
          </Grid>
          <Grid item xs={12} md={2}>
            <DatePicker
              label="From Date"
              value={filters.fromDate}
              onChange={(date) => setFilters({ ...filters, fromDate: date })}
            />
          </Grid>
          <Grid item xs={12} md={2}>
            <DatePicker
              label="To Date"
              value={filters.toDate}
              onChange={(date) => setFilters({ ...filters, toDate: date })}
            />
          </Grid>
          <Grid item xs={12} md={2}>
            <Button
              fullWidth
              variant="contained"
              onClick={handleSearch}
              sx={{ height: '56px' }}
            >
              Search
            </Button>
          </Grid>
        </Grid>
      </Paper>

      <Paper sx={{ height: 600 }}>
        <DataTable columns={columns} rows={invoices} loading={loading} />
      </Paper>
    </Box>
  );
};
```

### Week 3-4: Payment Selection & Confirmation

**Payment Confirmation Page (Matching Confirmation.aspx)**

```typescript
// src/pages/ConfirmationPage/PaymentConfirmation.tsx
import { useState, useEffect } from 'react';
import { Box, Paper, Typography, Button, Alert } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '@store/hooks';
import { confirmPayment } from '@store/slices/paymentSlice';

export const PaymentConfirmation: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { selectedInvoices, totalAmount } = useAppSelector((state) => state.invoice);
  const [bankInfo, setBankInfo] = useState({
    accountNumber: '',
    routingNumber: '',
  });

  const handleConfirm = async () => {
    const result = await dispatch(confirmPayment({
      invoices: selectedInvoices,
      totalAmount,
      bankInfo,
    }));

    if (result.meta.requestStatus === 'fulfilled') {
      navigate('/confirmation-success');
    }
  };

  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h5" gutterBottom>
          Payment Confirmation
        </Typography>

        <Alert severity="info" sx={{ mb: 3 }}>
          Please review your payment details before confirming.
        </Alert>

        {/* Payment details UI matching existing design */}

        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
          <Button variant="outlined" onClick={() => navigate('/invoices')}>
            Cancel
          </Button>
          <Button variant="contained" onClick={handleConfirm}>
            Confirm Payment
          </Button>
        </Box>
      </Paper>
    </Box>
  );
};
```

### Deliverables - Phase 3

✅ Invoice search page (matching main.aspx)
✅ Invoice selection functionality
✅ Payment confirmation page
✅ Excel export functionality
✅ All business logic preserved

---

## Phase 4: Feature Migration - Payments & US Bank Integration (6 Weeks)

### Objectives

- Migrate payment processing flow
- **Preserve US Bank EPay gateway integration (UNCHANGED)**
- Implement payment confirmation
- Migrate payment status tracking

### US Bank EPay Gateway Integration (PRESERVE EXISTING FLOW)

**Current Flow (from RTPConf.ashx and Confirmation.aspx.vb):**

```
1. User confirms payment in UI
2. System creates EPay record in database
3. System posts to US Bank gateway (HTTPS POST)
4. US Bank processes ACH transaction
5. US Bank returns confirmation number
6. System updates EPay record with confirmation
7. User sees confirmation page
```

**Migration Strategy: Wrapper Pattern**

```csharp
// EPay.Infrastructure/PaymentGateway/USBankGatewayService.cs
public class USBankGatewayService : IPaymentGatewayService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<USBankGatewayService> _logger;
    private readonly HttpClient _httpClient;

    public async Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        // PRESERVE EXISTING LOGIC - DO NOT CHANGE
        // This is the exact same flow as RTPConf.ashx

        var gatewayUrl = _configuration["USBank:GatewayUrl"];
        var merchantId = _configuration["USBank:MerchantId"];

        // Build request exactly as existing system does
        var formData = new Dictionary<string, string>
        {
            { "MerchantID", merchantId },
            { "CustomerNumber", request.CustomerNumber },
            { "Amount", request.TotalAmount.ToString("F2") },
            { "AccountNumber", request.BankAccountNumber },
            { "RoutingNumber", request.RoutingNumber },
            { "ReferenceNumber", request.ReferenceNumber.ToString() },
            // ... other fields matching existing implementation
        };

        try
        {
            // Post to US Bank gateway (UNCHANGED)
            var response = await _httpClient.PostAsync(gatewayUrl, new FormUrlEncodedContent(formData));
            var responseContent = await response.Content.ReadAsStringAsync();

            // Parse response (UNCHANGED)
            var confirmationNumber = ParseConfirmationNumber(responseContent);

            _logger.LogInformation("Payment processed successfully. Confirmation: {ConfirmationNumber}",
                confirmationNumber);

            return new PaymentGatewayResponse
            {
                Success = true,
                ConfirmationNumber = confirmationNumber,
                TransactionDate = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment through US Bank gateway");
            return new PaymentGatewayResponse
            {
                Success = false,
                ErrorMessage = "Payment processing failed. Please try again."
            };
        }
    }

    private string ParseConfirmationNumber(string responseContent)
    {
        // PRESERVE EXISTING PARSING LOGIC
        // Extract confirmation number from US Bank response
        // (Keep exact same logic as existing system)
        return responseContent; // Simplified - use actual parsing logic
    }
}
```

### Week 1-2: Payment Service Implementation

```csharp
// EPay.Core/Services/PaymentService.cs
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGatewayService _gatewayService;
    private readonly ILogger<PaymentService> _logger;

    public async Task<PaymentResult> CreatePaymentAsync(CreatePaymentRequest request)
    {
        // Validate request
        var validationResult = await ValidatePaymentRequest(request);
        if (!validationResult.IsValid)
            return PaymentResult.Failed(validationResult.Errors);

        // Create payment record
        var payment = new Payment
        {
            ReferenceNumber = await GetNextReferenceNumber(),
            CustomerNumber = request.CustomerNumber,
            TotalAmount = request.TotalAmount,
            PaymentDate = request.PaymentDate,
            Status = "Pending",
            CreatedBy = request.Username,
            CreatedDate = DateTime.Now
        };

        await _paymentRepository.CreateAsync(payment);

        // Create payment details (invoices)
        foreach (var invoice in request.Invoices)
        {
            var detail = new PaymentDetail
            {
                ReferenceNumber = payment.ReferenceNumber,
                InvoiceNumber = invoice.InvoiceNumber,
                Amount = invoice.Amount
            };
            await _paymentRepository.AddDetailAsync(detail);
        }

        return PaymentResult.Success(payment);
    }

    public async Task<PaymentConfirmationResult> ConfirmPaymentAsync(ConfirmPaymentRequest request)
    {
        // Get payment
        var payment = await _paymentRepository.GetByReferenceNumberAsync(request.ReferenceNumber);
        if (payment == null)
            return PaymentConfirmationResult.Failed("Payment not found");

        // Process through US Bank gateway (UNCHANGED FLOW)
        var gatewayRequest = new PaymentGatewayRequest
        {
            CustomerNumber = payment.CustomerNumber,
            TotalAmount = payment.TotalAmount,
            BankAccountNumber = request.BankAccountNumber,
            RoutingNumber = request.RoutingNumber,
            ReferenceNumber = payment.ReferenceNumber
        };

        var gatewayResponse = await _gatewayService.ProcessPaymentAsync(gatewayRequest);

        if (!gatewayResponse.Success)
        {
            payment.Status = "Failed";
            payment.ErrorMessage = gatewayResponse.ErrorMessage;
            await _paymentRepository.UpdateAsync(payment);
            return PaymentConfirmationResult.Failed(gatewayResponse.ErrorMessage);
        }

        // Update payment with confirmation
        payment.Status = "Confirmed";
        payment.ConfirmationNumber = gatewayResponse.ConfirmationNumber;
        payment.ConfirmedDate = DateTime.Now;
        await _paymentRepository.UpdateAsync(payment);

        return PaymentConfirmationResult.Success(payment, gatewayResponse.ConfirmationNumber);
    }
}
```

### Week 3-4: Frontend Payment Flow

```typescript
// src/services/api/paymentApi.ts
import httpClient from '@services/http/httpClient';

export const paymentApi = {
  createPayment: async (request: CreatePaymentRequest) => {
    const response = await httpClient.post('/payment/create', request);
    return response.data;
  },

  confirmPayment: async (request: ConfirmPaymentRequest) => {
    const response = await httpClient.post('/payment/confirm', request);
    return response.data;
  },

  getPaymentHistory: async (customerNumber: string, fromDate?: Date, toDate?: Date) => {
    const response = await httpClient.get('/payment/history', {
      params: { customerNumber, fromDate, toDate },
    });
    return response.data;
  },
};
```

### Deliverables - Phase 4

✅ Payment processing service
✅ US Bank gateway integration (UNCHANGED)
✅ Payment confirmation flow
✅ Payment status tracking
✅ Error handling and logging

---

## Phase 5: Feature Migration - History & Reporting (4 Weeks)

### Objectives

- Migrate payment history page
- Migrate analyst reporting
- Replace ActiveReports with Telerik Reporting
- Implement Excel/PDF export

### Week 1-2: Payment History

```typescript
// src/pages/HistoryPage/PaymentHistory.tsx
export const PaymentHistory: React.FC = () => {
  const dispatch = useAppDispatch();
  const { payments, loading } = useAppSelector((state) => state.payment);

  const columns = [
    { field: 'referenceNumber', headerName: 'Reference #', width: 120 },
    { field: 'paymentDate', headerName: 'Payment Date', width: 120 },
    { field: 'confirmationNumber', headerName: 'Confirmation #', width: 150 },
    { field: 'totalAmount', headerName: 'Amount', width: 120, type: 'number' },
    { field: 'status', headerName: 'Status', width: 100 },
  ];

  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ height: 600 }}>
        <DataTable columns={columns} rows={payments} loading={loading} />
      </Paper>
    </Box>
  );
};
```

### Week 3-4: Reporting (Replace ActiveReports)

```csharp
// EPay.API/Controllers/ReportController.cs
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    [HttpGet("analyst")]
    public async Task<IActionResult> GetAnalystReport(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string format = "pdf")
    {
        var report = await _reportService.GenerateAnalystReportAsync(fromDate, toDate);

        if (format == "excel")
        {
            var excelBytes = await _reportService.ExportToExcelAsync(report);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AnalystReport.xlsx");
        }

        var pdfBytes = await _reportService.ExportToPdfAsync(report);
        return File(pdfBytes, "application/pdf", "AnalystReport.pdf");
    }
}
```

### Deliverables - Phase 5

✅ Payment history page
✅ Analyst reporting (Telerik Reporting)
✅ Excel/PDF export
✅ Report scheduling (optional)

---

## Phase 6: Feature Migration - Admin & User Management (4 Weeks)

### Objectives

- Migrate admin maintenance page
- Migrate user management
- Implement role-based access control

### Admin Features

```typescript
// src/pages/AdminPage/UserManagement.tsx
export const UserManagement: React.FC = () => {
  // User CRUD operations
  // Role assignment
  // Permission management
};
```

### Deliverables - Phase 6

✅ Admin maintenance page
✅ User management
✅ Role-based access control
✅ Audit logging

---

## Phase 7: Testing & Security (4 Weeks)

### Objectives

- Comprehensive testing (unit, integration, E2E)
- Security audit and remediation
- Performance testing
- User acceptance testing (UAT)

### Week 1: Unit & Integration Testing

**Backend Tests**

```csharp
// EPay.Core.Tests/Services/PaymentServiceTests.cs
public class PaymentServiceTests
{
    [Fact]
    public async Task CreatePayment_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var mockRepository = new Mock<IPaymentRepository>();
        var service = new PaymentService(mockRepository.Object, ...);

        // Act
        var result = await service.CreatePaymentAsync(validRequest);

        // Assert
        Assert.True(result.Success);
    }
}
```

**Frontend Tests**

```typescript
// src/pages/InvoicePage/InvoiceSearch.test.tsx
describe('InvoiceSearch', () => {
  it('should search invoices when search button clicked', async () => {
    render(<InvoiceSearch />);

    const searchButton = screen.getByText('Search');
    fireEvent.click(searchButton);

    await waitFor(() => {
      expect(screen.getByText('Invoice #')).toBeInTheDocument();
    });
  });
});
```

### Week 2: Security Remediation

**Address Critical Vulnerabilities from Security Audit:**

1. **SQL Injection** - ✅ Fixed with parameterized queries and EF Core
2. **Hardcoded Credentials** - ✅ Moved to Azure Key Vault
3. **Weak Authentication** - ✅ Implemented JWT with Azure AD
4. **Input Validation** - ✅ Implemented FluentValidation

**Security Checklist:**

- [ ] All SQL queries parameterized
- [ ] No hardcoded credentials
- [ ] HTTPS enforced
- [ ] JWT tokens with expiration
- [ ] Input validation on all endpoints
- [ ] CORS properly configured
- [ ] Rate limiting implemented
- [ ] Logging of security events

### Week 3: Performance Testing

- Load testing (JMeter/k6)
- Database query optimization
- API response time optimization
- Frontend bundle size optimization

### Week 4: User Acceptance Testing (UAT)

- Test with actual users
- Verify UI matches existing design
- Validate all business flows
- Collect feedback

### Deliverables - Phase 7

✅ 80%+ code coverage
✅ All security vulnerabilities remediated
✅ Performance benchmarks met
✅ UAT sign-off

---

## Phase 8: Deployment & Cutover (2 Weeks)

### Deployment Strategy

**Blue-Green Deployment:**

```
┌─────────────────┐
│   Load Balancer │
└────────┬────────┘
         │
    ┌────┴────┐
    │         │
┌───▼──┐  ┌──▼───┐
│ Blue │  │Green │
│(Old) │  │(New) │
└──────┘  └──────┘
```

### Week 1: Staging Deployment

1. Deploy to staging environment
2. Run smoke tests
3. Run full regression tests
4. Performance validation
5. Security scan

### Week 2: Production Deployment

**Day 1-2: Pre-Deployment**
- [ ] Backup production database
- [ ] Notify users of maintenance window
- [ ] Deploy to production (Blue environment stays active)
- [ ] Run smoke tests on Green environment

**Day 3-4: Gradual Cutover**
- [ ] Route 10% traffic to new system
- [ ] Monitor for errors
- [ ] Route 50% traffic to new system
- [ ] Monitor for errors
- [ ] Route 100% traffic to new system

**Day 5: Post-Deployment**
- [ ] Monitor system health
- [ ] Address any issues
- [ ] Decommission old system (Blue)

### Rollback Plan

If critical issues occur:
1. Route 100% traffic back to Blue (old system)
2. Investigate and fix issues
3. Retry deployment

### Deliverables - Phase 8

✅ Production deployment successful
✅ Zero downtime achieved
✅ All users migrated
✅ Old system decommissioned

---

## Phase 9: Stabilization & Support (2 Weeks)

### Objectives

- Monitor system performance
- Fix any post-deployment bugs
- Optimize based on real usage
- Knowledge transfer to support team

### Activities

- 24/7 monitoring for first week
- Daily stand-ups to address issues
- Performance tuning
- Documentation updates
- Support team training

### Deliverables - Phase 9

✅ System stable and performing well
✅ All critical bugs fixed
✅ Support team trained
✅ Documentation complete

---

## Risk Management

### High-Risk Items

| Risk | Impact | Mitigation |
|------|--------|------------|
| **US Bank Gateway Integration Breaks** | Critical | Extensive testing, preserve exact flow, parallel testing |
| **DB2 Connectivity Issues** | High | Test early, use same IBM libraries, validate queries |
| **Data Migration Errors** | Critical | Comprehensive testing, rollback plan, data validation |
| **Performance Degradation** | High | Load testing, performance benchmarks, optimization |
| **Security Vulnerabilities** | Critical | Security audit, penetration testing, code review |
| **User Adoption Issues** | Medium | Training, documentation, UI matches existing design |

### Mitigation Strategies

1. **Parallel Running** - Run old and new systems in parallel for 2 weeks
2. **Comprehensive Testing** - Unit, integration, E2E, UAT
3. **Gradual Rollout** - Route traffic incrementally
4. **Rollback Plan** - Quick rollback to old system if needed
5. **Monitoring** - Real-time monitoring and alerting

---

## Success Criteria

### Technical Metrics

- ✅ 99.9% uptime
- ✅ API response time < 500ms (95th percentile)
- ✅ Page load time < 2 seconds
- ✅ Zero critical security vulnerabilities
- ✅ 80%+ code coverage

### Business Metrics

- ✅ Zero data loss
- ✅ All features migrated
- ✅ UI matches existing design
- ✅ User satisfaction > 80%
- ✅ Zero payment processing errors

---

## Resource Requirements

### Team Composition

| Role | Count | Allocation |
|------|-------|------------|
| **Backend Developer (.NET Core)** | 2 | Full-time |
| **Frontend Developer (React)** | 2 | Full-time |
| **Full-Stack Developer** | 1 | Full-time |
| **QA Engineer** | 2 | Full-time |
| **DevOps Engineer** | 1 | Part-time (50%) |
| **UI/UX Designer** | 1 | Part-time (25%) |
| **Security Specialist** | 1 | Part-time (25%) |
| **Project Manager** | 1 | Full-time |
| **Business Analyst** | 1 | Part-time (50%) |

### Infrastructure

- Development environment (Azure/AWS)
- Staging environment (Azure/AWS)
- Production environment (Azure/AWS)
- CI/CD pipeline (Azure DevOps/GitHub Actions)
- Monitoring tools (Application Insights/Datadog)

---

## Budget Estimate

| Category | Cost (USD) |
|----------|------------|
| **Personnel (10 months)** | $800,000 |
| **Infrastructure** | $50,000 |
| **Software Licenses** | $30,000 |
| **Training** | $20,000 |
| **Contingency (20%)** | $180,000 |
| **Total** | **$1,080,000** |

---

## Timeline Summary

```
Month 1-2:   Phase 0-1 (Planning, Backend Foundation)
Month 3-4:   Phase 2-3 (Frontend Foundation, Invoice Migration)
Month 5-6:   Phase 4 (Payment Migration, US Bank Integration)
Month 7:     Phase 5 (History & Reporting)
Month 8:     Phase 6 (Admin & User Management)
Month 9:     Phase 7 (Testing & Security)
Month 10:    Phase 8-9 (Deployment & Stabilization)
```

---

## Next Steps

### Immediate Actions (Week 1)

1. **Get Approval** - Present plan to stakeholders
2. **Assemble Team** - Hire/assign team members
3. **Set Up Environment** - Development tools and infrastructure
4. **Collect UI Screenshots** - Document existing design (AWAITING FROM USER)
5. **Review US Bank Integration** - Document exact flow
6. **Review DB2 Queries** - Document all DB2 stored procedures

### Awaiting from User

📸 **UI Screenshots Required** - Please provide screenshots of:
- main.aspx (Invoice search page)
- Confirmation.aspx (Payment confirmation)
- History.aspx (Payment history)
- AnalystReport.aspx (Reporting page)
- AdminMaintenance.aspx (Admin page)
- UserList.aspx (User management)

These screenshots will be used to ensure pixel-perfect design preservation in the React migration.

---

## Appendix

### A. Technology Comparison

| Aspect | Current (.NET Framework) | Target (.NET Core) |
|--------|-------------------------|-------------------|
| **Performance** | Baseline | 2-3x faster |
| **Cross-Platform** | Windows only | Windows, Linux, macOS |
| **Cloud-Ready** | Limited | Native support |
| **Modern Features** | Limited | Latest C# features |
| **Community Support** | Declining | Growing |
| **Long-term Support** | End of life | Active development |

### B. Database Migration Scripts

All database schema changes will be managed through EF Core migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### C. Configuration Management

All configuration will be externalized:
- Development: appsettings.Development.json
- Staging: appsettings.Staging.json
- Production: Azure Key Vault

### D. Monitoring & Logging

- **Application Insights** - Performance monitoring
- **Serilog** - Structured logging
- **Azure Monitor** - Infrastructure monitoring
- **PagerDuty** - Alerting

---

---

## Phase 10: Structured Logging & Observability (Integrated Across All Phases)

### Current Logging Issues

**Problems Identified in Existing Codebase:**

1. **Minimal Logging** - Only basic `System.Web.HttpContext.Current.Trace.Warn()` statements
2. **No Structured Logging** - Plain text messages, hard to query
3. **Inconsistent Error Handling** - Many `Catch ex As Exception; Throw` blocks with no logging
4. **Limited Audit Trail** - Only `WriteErrorToAuditLog()` for errors, no business event tracking
5. **No Performance Metrics** - Commented-out Stopwatch code indicates performance concerns
6. **No Correlation IDs** - Cannot trace requests across layers
7. **No Log Levels** - Everything is either traced or not logged
8. **Database-Only Audit Log** - `usp_InsertAuditRecord` creates database bottleneck

**Current Logging Examples Found:**

```vb
' Minimal trace logging
System.Web.HttpContext.Current.Trace.Warn("Totals sbSQL:" & sbSQL.ToString)

' Basic error logging to database
WriteErrorToAuditLog(0, "Could not get EPay Reference Number")

' Commented-out performance tracking
'Dim timer As Stopwatch = Stopwatch.StartNew
'timer.Stop()
'Common.WriteErrorToAuditLog(0, "- START - " & timer.ElapsedMilliseconds.ToString)

' Exception handling with no context
Catch ex As Exception
    Throw
```

### Objectives

✅ **Structured Logging** - JSON-formatted logs with rich context
✅ **Correlation Tracking** - Trace requests across all layers
✅ **Performance Monitoring** - Automatic timing and metrics
✅ **Security Auditing** - Track all security-relevant events
✅ **Business Event Tracking** - Log payment lifecycle events
✅ **Centralized Log Management** - Aggregate logs from all sources
✅ **Real-time Alerting** - Proactive issue detection
✅ **Compliance** - Meet audit and regulatory requirements

---

### Structured Logging Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    Application Layers                            │
│  ┌──────────────┬──────────────┬──────────────┬──────────────┐  │
│  │ Controllers  │  Services    │ Repositories │  Middleware  │  │
│  └──────┬───────┴──────┬───────┴──────┬───────┴──────┬───────┘  │
│         │              │              │              │           │
│         └──────────────┴──────────────┴──────────────┘           │
│                         │                                         │
│                    Serilog Core                                   │
│                         │                                         │
│         ┌───────────────┼───────────────┬──────────────┐         │
│         │               │               │              │         │
│    ┌────▼────┐    ┌────▼────┐    ┌────▼────┐   ┌────▼────┐    │
│    │ Console │    │  File   │    │App Ins. │   │  Seq    │    │
│    │  Sink   │    │  Sink   │    │  Sink   │   │  Sink   │    │
│    └─────────┘    └─────────┘    └─────────┘   └─────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              │
                    ┌─────────┴─────────┐
                    │                   │
            ┌───────▼────────┐  ┌──────▼──────┐
            │ Azure Monitor  │  │  Seq/ELK    │
            │ (Production)   │  │  (Dev/QA)   │
            └────────────────┘  └─────────────┘
```

---

### Implementation Strategy

#### **1. Backend Structured Logging (.NET Core)**

**1.1 Install Serilog Packages**

```xml
<!-- EPay.API -->
<PackageReference Include="Serilog.AspNetCore" Version="8.0.*" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.*" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.*" />
<PackageReference Include="Serilog.Sinks.ApplicationInsights" Version="4.0.*" />
<PackageReference Include="Serilog.Sinks.Seq" Version="7.0.*" />
<PackageReference Include="Serilog.Enrichers.Environment" Version="2.3.*" />
<PackageReference Include="Serilog.Enrichers.Thread" Version="3.1.*" />
<PackageReference Include="Serilog.Enrichers.CorrelationId" Version="3.0.*" />
<PackageReference Include="Serilog.Exceptions" Version="8.4.*" />
```

**1.2 Configure Serilog (Program.cs)**

```csharp
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithCorrelationId()
    .Enrich.WithExceptionDetails()
    .Enrich.WithProperty("Application", "EPay")
    .Enrich.WithProperty("Version", "2.0")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/epay-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.ApplicationInsights(
        builder.Configuration["ApplicationInsights:InstrumentationKey"],
        TelemetryConverter.Traces)
    .WriteTo.Seq(
        serverUrl: builder.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341")
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting EPay API application");

    // ... rest of application setup

    var app = builder.Build();

    // Add request logging middleware
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name);
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

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
```

**1.3 Detailed Configuration (appsettings.json)**

```json
{
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File",
      "Serilog.Sinks.ApplicationInsights",
      "Serilog.Sinks.Seq"
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "System": "Warning",
        "EPay": "Debug"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/epay-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "fileSizeLimitBytes": 104857600,
          "rollOnFileSizeLimit": true,
          "shared": true,
          "flushToDiskInterval": "00:00:01"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/epay-errors-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 90,
          "restrictedToMinimumLevel": "Error"
        }
      },
      {
        "Name": "ApplicationInsights",
        "Args": {
          "restrictedToMinimumLevel": "Information",
          "telemetryConverter": "Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter, Serilog.Sinks.ApplicationInsights"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341",
          "apiKey": "your-api-key-here"
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithEnvironmentName",
      "WithThreadId",
      "WithExceptionDetails",
      "WithCorrelationId"
    ],
    "Properties": {
      "Application": "EPay",
      "Environment": "Development"
    }
  },
  "ApplicationInsights": {
    "InstrumentationKey": "your-instrumentation-key",
    "EnableAdaptiveSampling": true,
    "EnablePerformanceCounterCollectionModule": true
  },
  "Seq": {
    "ServerUrl": "http://localhost:5341",
    "ApiKey": ""
  }
}
```

**1.4 Correlation ID Middleware**

```csharp
// EPay.API/Middleware/CorrelationIdMiddleware.cs
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Items[CorrelationIdHeader] = correlationId;
        context.Response.Headers.Add(CorrelationIdHeader, correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}

// Register in Program.cs
app.UseMiddleware<CorrelationIdMiddleware>();
```

**1.5 Logging in Controllers**

```csharp
// EPay.API/Controllers/PaymentController.cs
[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentService paymentService,
        ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CustomerNumber"] = request.CustomerNumber,
            ["InvoiceCount"] = request.InvoiceNumbers.Count,
            ["TotalAmount"] = request.TotalAmount
        }))
        {
            _logger.LogInformation(
                "Creating payment for customer {CustomerNumber} with {InvoiceCount} invoices totaling {TotalAmount:C}",
                request.CustomerNumber,
                request.InvoiceNumbers.Count,
                request.TotalAmount);

            try
            {
                var payment = await _paymentService.CreatePaymentAsync(request);

                _logger.LogInformation(
                    "Payment created successfully with reference number {ReferenceNumber}",
                    payment.ReferenceNumber);

                return Ok(payment);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex,
                    "Payment validation failed for customer {CustomerNumber}: {ValidationErrors}",
                    request.CustomerNumber,
                    string.Join(", ", ex.Errors));

                return BadRequest(new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating payment for customer {CustomerNumber}",
                    request.CustomerNumber);

                return StatusCode(500, new { message = "An error occurred while creating payment" });
            }
        }
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["ReferenceNumber"] = request.ReferenceNumber,
            ["PaymentMethod"] = "ACH"
        }))
        {
            _logger.LogInformation(
                "Confirming payment {ReferenceNumber} through US Bank gateway",
                request.ReferenceNumber);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = await _paymentService.ConfirmPaymentAsync(request);
                stopwatch.Stop();

                if (result.Success)
                {
                    _logger.LogInformation(
                        "Payment {ReferenceNumber} confirmed successfully with confirmation number {ConfirmationNumber} in {ElapsedMs}ms",
                        request.ReferenceNumber,
                        result.ConfirmationNumber,
                        stopwatch.ElapsedMilliseconds);

                    // Log business event for audit
                    _logger.LogInformation(
                        "AUDIT: Payment confirmed - Reference: {ReferenceNumber}, Confirmation: {ConfirmationNumber}, Amount: {Amount:C}, User: {UserName}",
                        request.ReferenceNumber,
                        result.ConfirmationNumber,
                        result.TotalAmount,
                        User.Identity?.Name);

                    return Ok(result);
                }
                else
                {
                    stopwatch.Stop();

                    _logger.LogWarning(
                        "Payment {ReferenceNumber} confirmation failed: {ErrorMessage} (took {ElapsedMs}ms)",
                        request.ReferenceNumber,
                        result.ErrorMessage,
                        stopwatch.ElapsedMilliseconds);

                    return BadRequest(new { message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "Error confirming payment {ReferenceNumber} after {ElapsedMs}ms",
                    request.ReferenceNumber,
                    stopwatch.ElapsedMilliseconds);

                return StatusCode(500, new { message = "Payment processing failed" });
            }
        }
    }
}
```

**1.6 Logging in Services**

```csharp
// EPay.Core/Services/PaymentService.cs
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGatewayService _gatewayService;
    private readonly ILogger<PaymentService> _logger;

    public async Task<PaymentConfirmationResult> ConfirmPaymentAsync(ConfirmPaymentRequest request)
    {
        _logger.LogDebug(
            "Starting payment confirmation for reference {ReferenceNumber}",
            request.ReferenceNumber);

        // Get payment
        var payment = await _paymentRepository.GetByReferenceNumberAsync(request.ReferenceNumber);

        if (payment == null)
        {
            _logger.LogWarning(
                "Payment not found for reference number {ReferenceNumber}",
                request.ReferenceNumber);

            return PaymentConfirmationResult.Failed("Payment not found");
        }

        _logger.LogInformation(
            "Processing payment {ReferenceNumber} for customer {CustomerNumber}, amount {Amount:C}",
            payment.ReferenceNumber,
            payment.CustomerNumber,
            payment.TotalAmount);

        // Process through US Bank gateway
        var gatewayRequest = new PaymentGatewayRequest
        {
            CustomerNumber = payment.CustomerNumber,
            TotalAmount = payment.TotalAmount,
            BankAccountNumber = request.BankAccountNumber,
            RoutingNumber = request.RoutingNumber,
            ReferenceNumber = payment.ReferenceNumber
        };

        var stopwatch = Stopwatch.StartNew();
        var gatewayResponse = await _gatewayService.ProcessPaymentAsync(gatewayRequest);
        stopwatch.Stop();

        _logger.LogInformation(
            "US Bank gateway response received in {ElapsedMs}ms for reference {ReferenceNumber}: Success={Success}",
            stopwatch.ElapsedMilliseconds,
            payment.ReferenceNumber,
            gatewayResponse.Success);

        if (!gatewayResponse.Success)
        {
            payment.Status = "Failed";
            payment.ErrorMessage = gatewayResponse.ErrorMessage;
            await _paymentRepository.UpdateAsync(payment);

            _logger.LogError(
                "Payment {ReferenceNumber} failed at gateway: {ErrorMessage}",
                payment.ReferenceNumber,
                gatewayResponse.ErrorMessage);

            return PaymentConfirmationResult.Failed(gatewayResponse.ErrorMessage);
        }

        // Update payment with confirmation
        payment.Status = "Confirmed";
        payment.ConfirmationNumber = gatewayResponse.ConfirmationNumber;
        payment.ConfirmedDate = DateTime.Now;
        await _paymentRepository.UpdateAsync(payment);

        _logger.LogInformation(
            "Payment {ReferenceNumber} confirmed successfully with confirmation number {ConfirmationNumber}",
            payment.ReferenceNumber,
            gatewayResponse.ConfirmationNumber);

        return PaymentConfirmationResult.Success(payment, gatewayResponse.ConfirmationNumber);
    }
}
```

**1.7 Logging in Repositories (Data Access Layer)**

```csharp
// EPay.Infrastructure/Repositories/PaymentRepository.cs
public class PaymentRepository : IPaymentRepository
{
    private readonly EPayDbContext _context;
    private readonly ILogger<PaymentRepository> _logger;

    public async Task<Payment> CreateAsync(Payment payment)
    {
        _logger.LogDebug(
            "Creating payment record for customer {CustomerNumber}, reference {ReferenceNumber}",
            payment.CustomerNumber,
            payment.ReferenceNumber);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            stopwatch.Stop();

            _logger.LogInformation(
                "Payment record created in {ElapsedMs}ms: Reference={ReferenceNumber}, Customer={CustomerNumber}, Amount={Amount:C}",
                stopwatch.ElapsedMilliseconds,
                payment.ReferenceNumber,
                payment.CustomerNumber,
                payment.TotalAmount);

            return payment;
        }
        catch (DbUpdateException ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "Database error creating payment for customer {CustomerNumber} after {ElapsedMs}ms",
                payment.CustomerNumber,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }

    public async Task<decimal> GetInvoiceTotalAsync(string customerNumber, int referenceNumber)
    {
        _logger.LogDebug(
            "Calling stored procedure usp_GetEpayTotal for customer {CustomerNumber}, reference {ReferenceNumber}",
            customerNumber,
            referenceNumber);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var total = await _context.Database
                .SqlQueryRaw<decimal>(
                    "EXEC Datawhse.dbo.usp_GetEpayTotal @CustomerNumber, @ReferenceNumber",
                    new SqlParameter("@CustomerNumber", customerNumber),
                    new SqlParameter("@ReferenceNumber", referenceNumber))
                .FirstOrDefaultAsync();

            stopwatch.Stop();

            _logger.LogInformation(
                "Retrieved invoice total {Total:C} in {ElapsedMs}ms for customer {CustomerNumber}, reference {ReferenceNumber}",
                total,
                stopwatch.ElapsedMilliseconds,
                customerNumber,
                referenceNumber);

            return total;
        }
        catch (SqlException ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "SQL error retrieving invoice total for customer {CustomerNumber}, reference {ReferenceNumber} after {ElapsedMs}ms. SQL Error: {SqlErrorNumber}",
                customerNumber,
                referenceNumber,
                stopwatch.ElapsedMilliseconds,
                ex.Number);

            throw;
        }
    }
}
```

**1.8 US Bank Gateway Logging (Critical for Payment Processing)**

```csharp
// EPay.Infrastructure/PaymentGateway/USBankGatewayService.cs
public class USBankGatewayService : IPaymentGatewayService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<USBankGatewayService> _logger;
    private readonly HttpClient _httpClient;

    public async Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        var gatewayUrl = _configuration["USBank:GatewayUrl"];
        var merchantId = _configuration["USBank:MerchantId"];

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["GatewayUrl"] = gatewayUrl,
            ["MerchantId"] = merchantId,
            ["ReferenceNumber"] = request.ReferenceNumber,
            ["CustomerNumber"] = request.CustomerNumber
        }))
        {
            _logger.LogInformation(
                "Initiating US Bank gateway payment for reference {ReferenceNumber}, amount {Amount:C}",
                request.ReferenceNumber,
                request.TotalAmount);

            // Build request (DO NOT LOG SENSITIVE DATA)
            var formData = new Dictionary<string, string>
            {
                { "MerchantID", merchantId },
                { "CustomerNumber", request.CustomerNumber },
                { "Amount", request.TotalAmount.ToString("F2") },
                { "AccountNumber", MaskAccountNumber(request.BankAccountNumber) }, // Masked for logging
                { "RoutingNumber", request.RoutingNumber },
                { "ReferenceNumber", request.ReferenceNumber.ToString() }
            };

            _logger.LogDebug(
                "Gateway request prepared: MerchantID={MerchantId}, Customer={CustomerNumber}, Amount={Amount}, Reference={ReferenceNumber}",
                merchantId,
                request.CustomerNumber,
                request.TotalAmount,
                request.ReferenceNumber);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Post to US Bank gateway
                var response = await _httpClient.PostAsync(gatewayUrl, new FormUrlEncodedContent(formData));
                stopwatch.Stop();

                _logger.LogInformation(
                    "US Bank gateway responded with status {StatusCode} in {ElapsedMs}ms for reference {ReferenceNumber}",
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    request.ReferenceNumber);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "US Bank gateway returned error status {StatusCode} for reference {ReferenceNumber}. Response: {ResponseContent}",
                        (int)response.StatusCode,
                        request.ReferenceNumber,
                        responseContent);

                    return new PaymentGatewayResponse
                    {
                        Success = false,
                        ErrorMessage = $"Gateway error: {response.StatusCode}"
                    };
                }

                // Parse response
                var confirmationNumber = ParseConfirmationNumber(responseContent);

                _logger.LogInformation(
                    "Payment processed successfully through US Bank gateway. Reference: {ReferenceNumber}, Confirmation: {ConfirmationNumber}, Duration: {ElapsedMs}ms",
                    request.ReferenceNumber,
                    confirmationNumber,
                    stopwatch.ElapsedMilliseconds);

                // AUDIT LOG - Critical business event
                _logger.LogInformation(
                    "AUDIT: US Bank payment processed - Reference: {ReferenceNumber}, Confirmation: {ConfirmationNumber}, Customer: {CustomerNumber}, Amount: {Amount:C}, Duration: {ElapsedMs}ms",
                    request.ReferenceNumber,
                    confirmationNumber,
                    request.CustomerNumber,
                    request.TotalAmount,
                    stopwatch.ElapsedMilliseconds);

                return new PaymentGatewayResponse
                {
                    Success = true,
                    ConfirmationNumber = confirmationNumber,
                    TransactionDate = DateTime.Now
                };
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "HTTP error communicating with US Bank gateway for reference {ReferenceNumber} after {ElapsedMs}ms. Gateway URL: {GatewayUrl}",
                    request.ReferenceNumber,
                    stopwatch.ElapsedMilliseconds,
                    gatewayUrl);

                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Payment gateway communication error"
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogCritical(ex,
                    "CRITICAL: Unexpected error processing payment through US Bank gateway for reference {ReferenceNumber} after {ElapsedMs}ms",
                    request.ReferenceNumber,
                    stopwatch.ElapsedMilliseconds);

                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Payment processing failed"
                };
            }
        }
    }

    private string MaskAccountNumber(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length < 4)
            return "****";

        return "****" + accountNumber.Substring(accountNumber.Length - 4);
    }
}
```

---

#### **2. Frontend Logging (React/TypeScript)**

**2.1 Install Logging Packages**

```bash
npm install winston
npm install @microsoft/applicationinsights-web
npm install @sentry/react @sentry/tracing
```

**2.2 Configure Winston Logger**

```typescript
// src/utils/logger.ts
import winston from 'winston';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';

// Initialize Application Insights
const appInsights = new ApplicationInsights({
  config: {
    instrumentationKey: process.env.REACT_APP_APPINSIGHTS_KEY,
    enableAutoRouteTracking: true,
    enableCorsCorrelation: true,
    enableRequestHeaderTracking: true,
    enableResponseHeaderTracking: true,
  }
});
appInsights.loadAppInsights();

// Create Winston logger
const logger = winston.createLogger({
  level: process.env.NODE_ENV === 'production' ? 'info' : 'debug',
  format: winston.format.combine(
    winston.format.timestamp(),
    winston.format.errors({ stack: true }),
    winston.format.json()
  ),
  defaultMeta: {
    service: 'epay-frontend',
    environment: process.env.NODE_ENV,
  },
  transports: [
    new winston.transports.Console({
      format: winston.format.combine(
        winston.format.colorize(),
        winston.format.simple()
      ),
    }),
  ],
});

// Wrapper to send logs to Application Insights
export const log = {
  debug: (message: string, meta?: any) => {
    logger.debug(message, meta);
  },

  info: (message: string, meta?: any) => {
    logger.info(message, meta);
    appInsights.trackTrace({ message, severityLevel: 1, properties: meta });
  },

  warn: (message: string, meta?: any) => {
    logger.warn(message, meta);
    appInsights.trackTrace({ message, severityLevel: 2, properties: meta });
  },

  error: (message: string, error?: Error, meta?: any) => {
    logger.error(message, { error, ...meta });
    appInsights.trackException({
      exception: error || new Error(message),
      properties: meta
    });
  },

  // Track page views
  pageView: (name: string, url?: string, properties?: any) => {
    appInsights.trackPageView({ name, uri: url, properties });
  },

  // Track custom events
  event: (name: string, properties?: any, measurements?: any) => {
    appInsights.trackEvent({ name, properties, measurements });
  },

  // Track metrics
  metric: (name: string, average: number, properties?: any) => {
    appInsights.trackMetric({ name, average, properties });
  },
};

export { appInsights };
```

**2.3 API Client with Logging**

```typescript
// src/services/http/httpClient.ts
import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
import { log } from '@utils/logger';

const httpClient = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7001/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
httpClient.interceptors.request.use(
  (config: AxiosRequestConfig) => {
    const token = localStorage.getItem('token');
    const correlationId = generateCorrelationId();

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    config.headers['X-Correlation-ID'] = correlationId;

    log.debug('API Request', {
      method: config.method?.toUpperCase(),
      url: config.url,
      correlationId,
    });

    // Track API call start time
    config.metadata = { startTime: new Date() };

    return config;
  },
  (error: AxiosError) => {
    log.error('API Request Error', error);
    return Promise.reject(error);
  }
);

// Response interceptor
httpClient.interceptors.response.use(
  (response: AxiosResponse) => {
    const duration = new Date().getTime() - response.config.metadata.startTime.getTime();
    const correlationId = response.config.headers['X-Correlation-ID'];

    log.info('API Response', {
      method: response.config.method?.toUpperCase(),
      url: response.config.url,
      status: response.status,
      duration,
      correlationId,
    });

    // Track API performance
    log.metric('api_response_time', duration, {
      endpoint: response.config.url,
      method: response.config.method,
      status: response.status,
    });

    return response;
  },
  (error: AxiosError) => {
    const duration = error.config?.metadata
      ? new Date().getTime() - error.config.metadata.startTime.getTime()
      : 0;

    if (error.response) {
      // Server responded with error status
      log.error('API Error Response', error, {
        method: error.config?.method?.toUpperCase(),
        url: error.config?.url,
        status: error.response.status,
        statusText: error.response.statusText,
        duration,
        correlationId: error.config?.headers['X-Correlation-ID'],
        data: error.response.data,
      });

      if (error.response.status === 401) {
        log.warn('Unauthorized - redirecting to login');
        localStorage.removeItem('token');
        window.location.href = '/login';
      }
    } else if (error.request) {
      // Request made but no response
      log.error('API No Response', error, {
        method: error.config?.method?.toUpperCase(),
        url: error.config?.url,
        duration,
      });
    } else {
      // Error setting up request
      log.error('API Request Setup Error', error);
    }

    return Promise.reject(error);
  }
);

function generateCorrelationId(): string {
  return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
}

export default httpClient;
```

**2.4 Component-Level Logging**

```typescript
// src/pages/InvoicePage/InvoiceSearch.tsx
import { useEffect } from 'react';
import { log } from '@utils/logger';

export const InvoiceSearch: React.FC = () => {
  const dispatch = useAppDispatch();
  const { invoices, loading, error } = useAppSelector((state) => state.invoice);

  useEffect(() => {
    log.pageView('Invoice Search', window.location.pathname);
  }, []);

  const handleSearch = async () => {
    const startTime = performance.now();

    log.info('Invoice search initiated', {
      filters: {
        invoiceNumber: filters.invoiceNumber,
        poNumber: filters.poNumber,
        dateRange: { from: filters.fromDate, to: filters.toDate },
      },
    });

    try {
      await dispatch(searchInvoices(filters)).unwrap();

      const duration = performance.now() - startTime;

      log.info('Invoice search completed', {
        resultCount: invoices.length,
        duration,
      });

      log.metric('invoice_search_duration', duration);

      log.event('invoice_search_success', {
        resultCount: invoices.length,
        hasFilters: !!(filters.invoiceNumber || filters.poNumber),
      });
    } catch (error) {
      const duration = performance.now() - startTime;

      log.error('Invoice search failed', error as Error, {
        filters,
        duration,
      });

      log.event('invoice_search_failure', {
        errorMessage: (error as Error).message,
      });
    }
  };

  return (
    // ... component JSX
  );
};
```

**2.5 Error Boundary with Logging**

```typescript
// src/components/common/ErrorBoundary.tsx
import React, { Component, ErrorInfo, ReactNode } from 'react';
import { log } from '@utils/logger';

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
  error?: Error;
}

export class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    log.error('React Error Boundary caught error', error, {
      componentStack: errorInfo.componentStack,
      errorBoundary: true,
    });

    log.event('react_error_boundary_triggered', {
      errorMessage: error.message,
      errorStack: error.stack,
    });
  }

  render() {
    if (this.state.hasError) {
      return (
        <div style={{ padding: '20px', textAlign: 'center' }}>
          <h1>Something went wrong</h1>
          <p>We've logged the error and will investigate.</p>
          <button onClick={() => window.location.reload()}>
            Reload Page
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}
```

**2.6 Redux Action Logging**

```typescript
// src/store/middleware/loggingMiddleware.ts
import { Middleware } from '@reduxjs/toolkit';
import { log } from '@utils/logger';

export const loggingMiddleware: Middleware = (store) => (next) => (action) => {
  const startTime = performance.now();

  log.debug('Redux Action Dispatched', {
    type: action.type,
    payload: action.payload,
  });

  const result = next(action);

  const duration = performance.now() - startTime;

  log.debug('Redux Action Completed', {
    type: action.type,
    duration,
  });

  // Track slow actions
  if (duration > 100) {
    log.warn('Slow Redux Action', {
      type: action.type,
      duration,
    });
  }

  return result;
};

// Add to store configuration
export const store = configureStore({
  reducer: {
    auth: authReducer,
    invoice: invoiceReducer,
    payment: paymentReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(loggingMiddleware),
});
```

---

#### **3. Security & Audit Logging**

**3.1 Security Event Logging**

```csharp
// EPay.API/Middleware/SecurityAuditMiddleware.cs
public class SecurityAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityAuditMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // Log authentication attempts
        if (context.Request.Path.StartsWithSegments("/api/auth/login"))
        {
            var username = await GetUsernameFromRequest(context.Request);

            _logger.LogInformation(
                "SECURITY: Login attempt for user {Username} from IP {ClientIP}",
                username,
                context.Connection.RemoteIpAddress);
        }

        await _next(context);

        // Log successful authentication
        if (context.Response.StatusCode == 200 &&
            context.Request.Path.StartsWithSegments("/api/auth/login"))
        {
            _logger.LogInformation(
                "SECURITY: Successful login for user {Username} from IP {ClientIP}",
                context.User?.Identity?.Name,
                context.Connection.RemoteIpAddress);
        }

        // Log failed authentication
        if (context.Response.StatusCode == 401)
        {
            _logger.LogWarning(
                "SECURITY: Unauthorized access attempt to {Path} from IP {ClientIP}, User: {Username}",
                context.Request.Path,
                context.Connection.RemoteIpAddress,
                context.User?.Identity?.Name ?? "Anonymous");
        }

        // Log forbidden access
        if (context.Response.StatusCode == 403)
        {
            _logger.LogWarning(
                "SECURITY: Forbidden access attempt to {Path} by user {Username} from IP {ClientIP}",
                context.Request.Path,
                context.User?.Identity?.Name,
                context.Connection.RemoteIpAddress);
        }
    }
}
```

**3.2 Business Audit Logging**

```csharp
// EPay.Core/Services/AuditService.cs
public interface IAuditService
{
    Task LogPaymentCreatedAsync(Payment payment, string username);
    Task LogPaymentConfirmedAsync(Payment payment, string confirmationNumber, string username);
    Task LogPaymentFailedAsync(int referenceNumber, string errorMessage, string username);
    Task LogInvoiceAccessAsync(string customerNumber, string username);
}

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;
    private readonly IAuditRepository _auditRepository;

    public async Task LogPaymentCreatedAsync(Payment payment, string username)
    {
        _logger.LogInformation(
            "AUDIT: Payment created - Reference: {ReferenceNumber}, Customer: {CustomerNumber}, Amount: {Amount:C}, User: {Username}",
            payment.ReferenceNumber,
            payment.CustomerNumber,
            payment.TotalAmount,
            username);

        await _auditRepository.CreateAuditRecordAsync(new AuditRecord
        {
            EventType = "PaymentCreated",
            ReferenceNumber = payment.ReferenceNumber,
            CustomerNumber = payment.CustomerNumber,
            Amount = payment.TotalAmount,
            Username = username,
            Timestamp = DateTime.Now,
            Details = $"Payment created with {payment.Details.Count} invoices"
        });
    }

    public async Task LogPaymentConfirmedAsync(Payment payment, string confirmationNumber, string username)
    {
        _logger.LogInformation(
            "AUDIT: Payment confirmed - Reference: {ReferenceNumber}, Confirmation: {ConfirmationNumber}, Customer: {CustomerNumber}, Amount: {Amount:C}, User: {Username}",
            payment.ReferenceNumber,
            confirmationNumber,
            payment.CustomerNumber,
            payment.TotalAmount,
            username);

        await _auditRepository.CreateAuditRecordAsync(new AuditRecord
        {
            EventType = "PaymentConfirmed",
            ReferenceNumber = payment.ReferenceNumber,
            CustomerNumber = payment.CustomerNumber,
            Amount = payment.TotalAmount,
            Username = username,
            Timestamp = DateTime.Now,
            Details = $"Payment confirmed with confirmation number {confirmationNumber}"
        });
    }
}
```

---

#### **4. Performance Monitoring & Metrics**

**4.1 Custom Metrics**

```csharp
// EPay.Infrastructure/Monitoring/MetricsService.cs
public interface IMetricsService
{
    void TrackPaymentProcessingTime(int referenceNumber, long milliseconds);
    void TrackDatabaseQueryTime(string queryName, long milliseconds);
    void TrackGatewayResponseTime(long milliseconds, bool success);
    void IncrementPaymentCounter(string status);
}

public class MetricsService : IMetricsService
{
    private readonly ILogger<MetricsService> _logger;

    public void TrackPaymentProcessingTime(int referenceNumber, long milliseconds)
    {
        _logger.LogInformation(
            "METRIC: Payment processing time - Reference: {ReferenceNumber}, Duration: {Duration}ms",
            referenceNumber,
            milliseconds);

        // Send to Application Insights
        var telemetry = new MetricTelemetry("PaymentProcessingTime", milliseconds);
        telemetry.Properties.Add("ReferenceNumber", referenceNumber.ToString());
        // ... send telemetry
    }

    public void TrackDatabaseQueryTime(string queryName, long milliseconds)
    {
        _logger.LogDebug(
            "METRIC: Database query time - Query: {QueryName}, Duration: {Duration}ms",
            queryName,
            milliseconds);

        if (milliseconds > 1000) // Slow query threshold
        {
            _logger.LogWarning(
                "PERFORMANCE: Slow database query detected - Query: {QueryName}, Duration: {Duration}ms",
                queryName,
                milliseconds);
        }
    }

    public void TrackGatewayResponseTime(long milliseconds, bool success)
    {
        _logger.LogInformation(
            "METRIC: US Bank gateway response time - Duration: {Duration}ms, Success: {Success}",
            milliseconds,
            success);

        if (milliseconds > 5000) // Gateway timeout threshold
        {
            _logger.LogWarning(
                "PERFORMANCE: Slow gateway response - Duration: {Duration}ms",
                milliseconds);
        }
    }
}
```

**4.2 Health Checks with Logging**

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("SqlServer"),
        name: "sql-server",
        tags: new[] { "db", "sql" })
    .AddCheck<Db2HealthCheck>("db2", tags: new[] { "db", "db2" })
    .AddCheck<USBankGatewayHealthCheck>("usbank-gateway", tags: new[] { "external", "payment" });

// EPay.Infrastructure/HealthChecks/Db2HealthCheck.cs
public class Db2HealthCheck : IHealthCheck
{
    private readonly IDb2ConnectionFactory _connectionFactory;
    private readonly ILogger<Db2HealthCheck> _logger;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new iDB2Command("SELECT 1 FROM SYSIBM.SYSDUMMY1", connection);
            await command.ExecuteScalarAsync(cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation(
                "HEALTH: DB2 health check passed in {Duration}ms",
                stopwatch.ElapsedMilliseconds);

            return HealthCheckResult.Healthy($"DB2 connection successful ({stopwatch.ElapsedMilliseconds}ms)");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "HEALTH: DB2 health check failed after {Duration}ms",
                stopwatch.ElapsedMilliseconds);

            return HealthCheckResult.Unhealthy("DB2 connection failed", ex);
        }
    }
}
```

---

#### **5. Log Aggregation & Monitoring Setup**

**5.1 Development Environment - Seq**

```bash
# Run Seq in Docker
docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```

Access Seq at: http://localhost:5341

**Benefits:**
- Real-time log viewing
- Structured query language
- Correlation ID tracking
- Performance analysis
- Free for development

**5.2 Production Environment - Azure Monitor / Application Insights**

```csharp
// appsettings.Production.json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-production-key",
    "EnableAdaptiveSampling": true,
    "EnablePerformanceCounterCollectionModule": true,
    "EnableDependencyTrackingTelemetryModule": true,
    "EnableEventCounterCollectionModule": true
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "ApplicationInsights",
        "Args": {
          "restrictedToMinimumLevel": "Information"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/epay/epay-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

**5.3 Alerting Rules**

```yaml
# Azure Monitor Alert Rules
alerts:
  - name: "High Error Rate"
    condition: "traces | where severityLevel >= 3 | count > 10"
    window: "5 minutes"
    action: "Send email to ops team"

  - name: "Payment Gateway Failures"
    condition: "customEvents | where name == 'PaymentGatewayError' | count > 5"
    window: "10 minutes"
    action: "Page on-call engineer"

  - name: "Slow API Response"
    condition: "requests | where duration > 5000 | count > 10"
    window: "5 minutes"
    action: "Send Slack notification"

  - name: "DB2 Connection Failures"
    condition: "exceptions | where type contains 'DB2' | count > 3"
    window: "5 minutes"
    action: "Page database team"

  - name: "Authentication Failures"
    condition: "traces | where message contains 'SECURITY: Unauthorized' | count > 20"
    window: "5 minutes"
    action: "Send security alert"
```

---

#### **6. Logging Best Practices & Standards**

**6.1 Log Levels**

| Level | When to Use | Examples |
|-------|-------------|----------|
| **Trace** | Very detailed diagnostic info | Method entry/exit, variable values |
| **Debug** | Diagnostic info for developers | SQL queries, API requests, business logic flow |
| **Information** | General application flow | User actions, business events, successful operations |
| **Warning** | Unexpected but recoverable | Validation failures, slow queries, deprecated API usage |
| **Error** | Errors that need attention | Exceptions, failed operations, data errors |
| **Critical** | System failures | Database down, gateway unavailable, data corruption |

**6.2 Structured Logging Format**

```csharp
// ✅ GOOD - Structured with properties
_logger.LogInformation(
    "Payment {ReferenceNumber} created for customer {CustomerNumber} with amount {Amount:C}",
    payment.ReferenceNumber,
    payment.CustomerNumber,
    payment.TotalAmount);

// ❌ BAD - String concatenation
_logger.LogInformation(
    "Payment " + payment.ReferenceNumber + " created for customer " + payment.CustomerNumber);
```

**6.3 Sensitive Data Handling**

```csharp
// ✅ GOOD - Mask sensitive data
_logger.LogInformation(
    "Processing payment with account ****{LastFour}",
    accountNumber.Substring(accountNumber.Length - 4));

// ❌ BAD - Logging sensitive data
_logger.LogInformation(
    "Processing payment with account {AccountNumber}",
    accountNumber); // NEVER LOG FULL ACCOUNT NUMBERS
```

**Sensitive Data to NEVER Log:**
- Full credit card numbers
- Full bank account numbers
- Social Security Numbers
- Passwords or tokens
- Personal health information
- Full routing numbers (mask all but last 4)

**6.4 Correlation ID Pattern**

Every request should have a correlation ID that flows through all layers:

```
Frontend Request → API Controller → Service → Repository → Database
     [CORR-123]      [CORR-123]     [CORR-123]  [CORR-123]   [CORR-123]
```

This allows tracing a single request across all systems.

**6.5 Performance Logging Pattern**

```csharp
// Always log performance for critical operations
var stopwatch = Stopwatch.StartNew();
try
{
    var result = await _service.ProcessPaymentAsync(request);
    stopwatch.Stop();

    _logger.LogInformation(
        "Payment processed successfully in {ElapsedMs}ms",
        stopwatch.ElapsedMilliseconds);

    return result;
}
catch (Exception ex)
{
    stopwatch.Stop();

    _logger.LogError(ex,
        "Payment processing failed after {ElapsedMs}ms",
        stopwatch.ElapsedMilliseconds);

    throw;
}
```

---

#### **7. Migration Strategy for Logging**

**Phase 1 (Weeks 1-2): Infrastructure Setup**
- [ ] Install Serilog packages
- [ ] Configure logging sinks (Console, File, Application Insights, Seq)
- [ ] Set up correlation ID middleware
- [ ] Configure log levels per environment
- [ ] Set up Seq for development
- [ ] Set up Application Insights for production

**Phase 2 (Weeks 3-4): Backend Logging Implementation**
- [ ] Add logging to all controllers
- [ ] Add logging to all services
- [ ] Add logging to all repositories
- [ ] Add logging to US Bank gateway integration
- [ ] Add security audit logging
- [ ] Add performance metrics

**Phase 3 (Weeks 5-6): Frontend Logging Implementation**
- [ ] Configure Winston logger
- [ ] Add Application Insights to React app
- [ ] Add logging to API client
- [ ] Add logging to Redux actions
- [ ] Add error boundary with logging
- [ ] Add page view tracking

**Phase 4 (Weeks 7-8): Monitoring & Alerting**
- [ ] Configure Azure Monitor dashboards
- [ ] Set up alert rules
- [ ] Configure PagerDuty integration
- [ ] Create runbooks for common alerts
- [ ] Train operations team

**Phase 5 (Ongoing): Optimization**
- [ ] Review log volume and costs
- [ ] Optimize log levels
- [ ] Tune sampling rates
- [ ] Archive old logs
- [ ] Regular log analysis

---

#### **8. Logging Metrics & KPIs**

**Track These Metrics:**

| Metric | Target | Alert Threshold |
|--------|--------|-----------------|
| **Log Volume** | < 10 GB/day | > 15 GB/day |
| **Error Rate** | < 0.1% | > 1% |
| **API Response Time (P95)** | < 500ms | > 1000ms |
| **Payment Processing Time** | < 3 seconds | > 10 seconds |
| **Gateway Response Time** | < 2 seconds | > 5 seconds |
| **Database Query Time (P95)** | < 100ms | > 500ms |
| **Failed Logins** | < 5/hour | > 20/hour |
| **Payment Failures** | < 1% | > 5% |

---

#### **9. Sample Queries for Log Analysis**

**Seq Queries:**

```sql
-- Find all payment processing errors
select * from stream
where @Level = 'Error'
  and @Message like '%payment%'
  and @Timestamp > Now() - 1h

-- Track payment processing times
select avg(ElapsedMs) as AvgDuration, max(ElapsedMs) as MaxDuration
from stream
where @Message like '%Payment processed%'
  and @Timestamp > Now() - 1h

-- Find slow database queries
select QueryName, Duration
from stream
where @Message like '%METRIC: Database query%'
  and Duration > 1000
  and @Timestamp > Now() - 1h
order by Duration desc

-- Track correlation across layers
select * from stream
where CorrelationId = 'abc-123-def'
order by @Timestamp
```

**Application Insights (KQL) Queries:**

```kusto
// Payment processing funnel
traces
| where timestamp > ago(1h)
| where message contains "Payment"
| summarize count() by message
| order by count_ desc

// Error rate by endpoint
requests
| where timestamp > ago(1h)
| summarize ErrorRate = countif(success == false) * 100.0 / count() by name
| where ErrorRate > 1
| order by ErrorRate desc

// Slow API calls
requests
| where timestamp > ago(1h)
| where duration > 1000
| project timestamp, name, duration, resultCode
| order by duration desc

// US Bank gateway performance
dependencies
| where timestamp > ago(1h)
| where name contains "USBank"
| summarize avg(duration), max(duration), count() by bin(timestamp, 5m)
| render timechart
```

---

#### **10. Deliverables - Structured Logging**

✅ **Serilog configured** with multiple sinks (Console, File, Application Insights, Seq)
✅ **Correlation ID tracking** across all layers
✅ **Structured logging** in all controllers, services, repositories
✅ **Security audit logging** for authentication and authorization events
✅ **Business event logging** for payment lifecycle
✅ **Performance metrics** for critical operations
✅ **Frontend logging** with Winston and Application Insights
✅ **Error tracking** with detailed context
✅ **Health checks** with logging
✅ **Alerting rules** configured
✅ **Log analysis dashboards** in Azure Monitor
✅ **Documentation** for logging standards and best practices

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-01-20 | Migration Team | Initial version |
| 1.1 | 2026-01-20 | Migration Team | Added comprehensive structured logging strategy (Phase 10) |

---

**END OF MIGRATION PLAN**




