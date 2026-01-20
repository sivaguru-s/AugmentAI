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

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-01-20 | Migration Team | Initial version |

---

**END OF MIGRATION PLAN**


