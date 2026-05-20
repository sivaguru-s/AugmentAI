# Credit Shortage Validation Microservice - Architecture

## 🏗️ Solution Structure

```
CreditShortage.Solution/
│
├── CreditShortage.Api/                    # Presentation Layer (REST API)
│   ├── Controllers/
│   │   ├── ShortageValidationController.cs  # Main validation endpoints
│   │   └── ReferenceDataController.cs       # Reference data endpoints
│   ├── Program.cs                           # Application entry point
│   ├── appsettings.json                     # Configuration
│   └── CreditShortage.Api.csproj
│
├── CreditShortage.Application/            # Application Layer (Business Logic)
│   ├── DTOs/
│   │   ├── ShortageItemRequest.cs          # Request DTOs
│   │   ├── ShortageValidationResponse.cs   # Response DTOs (CREATED)
│   │   └── ReferenceDataDtos.cs            # Reference data DTOs
│   ├── Interfaces/
│   │   ├── IShortageValidationService.cs   # Validation service interface
│   │   ├── IReferenceDataService.cs        # Reference data interface
│   │   ├── IIWSIntegrationService.cs       # IWS integration interface
│   │   └── IShortageValidationRepository.cs # Repository interface (TO CREATE)
│   ├── Services/
│   │   ├── ShortageValidationService.cs    # Main validation logic
│   │   └── ReferenceDataService.cs         # Reference data logic (TO CREATE)
│   ├── DependencyInjection.cs              # Service registration (TO CREATE)
│   └── CreditShortage.Application.csproj
│
├── CreditShortage.Domain/                 # Domain Layer (Entities & Models)
│   ├── Entities/
│   │   ├── ShortageValidationInput.cs      # Input entity (TO CREATE)
│   │   ├── ShortageValidationResult.cs     # Result entity (TO CREATE)
│   │   ├── DefectCode.cs                   # Defect code entity (TO CREATE)
│   │   └── LocationCode.cs                 # Location code entity (TO CREATE)
│   └── CreditShortage.Domain.csproj        # (TO CREATE)
│
├── CreditShortage.Infrastructure/         # Infrastructure Layer (Data Access)
│   ├── Data/
│   │   ├── ShortageValidationRepository.cs # SQL data access (TO CREATE)
│   │   └── SqlConnectionFactory.cs         # Connection management (TO CREATE)
│   ├── ExternalServices/
│   │   └── IWSIntegrationService.cs        # IWS API client (TO CREATE)
│   ├── DependencyInjection.cs              # Infrastructure registration (TO CREATE)
│   └── CreditShortage.Infrastructure.csproj # (TO CREATE)
│
└── CreditShortage.Tests/                  # Test Projects (TO CREATE)
    ├── CreditShortage.Api.Tests/
    ├── CreditShortage.Application.Tests/
    └── CreditShortage.Integration.Tests/
```

---

## 📊 Architecture Pattern: Clean Architecture

### Layer Responsibilities

#### 1. **API Layer** (CreditShortage.Api) ✅ CREATED
- **Purpose**: HTTP endpoints, request/response handling
- **Responsibilities**:
  - Route management
  - Request validation (FluentValidation)
  - Response formatting
  - Swagger/OpenAPI documentation
  - Error handling middleware
  - Logging

**Key Files Created**:
- ✅ `ShortageValidationController.cs` - Main validation endpoints
- ✅ `ReferenceDataController.cs` - Reference data endpoints
- ✅ `Program.cs` - Startup configuration
- ✅ `appsettings.json` - Configuration settings

#### 2. **Application Layer** (CreditShortage.Application) ⚠️ PARTIAL
- **Purpose**: Business logic orchestration
- **Responsibilities**:
  - Service implementations
  - DTO mappings
  - Business rule enforcement
  - Workflow coordination

**Key Files Created**:
- ✅ `ShortageValidationService.cs` - Main service
- ✅ DTOs (Request/Response models)
- ✅ Service interfaces

**To Create**:
- ⏳ `ReferenceDataService.cs`
- ⏳ `DependencyInjection.cs`

#### 3. **Domain Layer** (CreditShortage.Domain) ⏳ TO CREATE
- **Purpose**: Core domain entities
- **Responsibilities**:
  - Entity definitions
  - Domain models
  - No dependencies on other layers

**Files to Create**:
- `ShortageValidationInput.cs`
- `ShortageValidationResult.cs`
- `DefectCode.cs`
- `LocationCode.cs`

#### 4. **Infrastructure Layer** (CreditShortage.Infrastructure) ⏳ TO CREATE
- **Purpose**: External concerns (DB, APIs)
- **Responsibilities**:
  - Database access (SQL Server)
  - External API calls (IWS)
  - File system access
  - Caching

**Files to Create**:
- `ShortageValidationRepository.cs` - DB access
- `IWSIntegrationService.cs` - IWS integration
- `DependencyInjection.cs`

---

## 🔄 Request Flow

```
┌─────────────────────────────────────────────────────────────────────┐
│                           REQUEST FLOW                               │
└─────────────────────────────────────────────────────────────────────┘

1. HTTP Request
   ↓
2. ShortageValidationController
   ├── Validates request (FluentValidation)
   ├── Logs request
   └── Calls IShortageValidationService
   ↓
3. ShortageValidationService
   ├── Maps DTO → Domain Entity
   ├── Applies business rules
   ├── Calls IShortageValidationRepository
   └── (Optional) Calls IIWSIntegrationService
   ↓
4. ShortageValidationRepository
   ├── Builds SQL parameters (Table-Valued Parameter)
   ├── Executes usp_CE_ValidateShortageItems
   ├── Maps SQL result → Domain Entity
   └── Returns to Service
   ↓
5. ShortageValidationService
   ├── Maps Domain Entity → Response DTO
   └── Returns to Controller
   ↓
6. ShortageValidationController
   ├── Logs response
   └── Returns HTTP 200 with JSON
   ↓
7. HTTP Response
```

---

## 🔌 API Endpoints

### Validation Endpoints

#### POST /api/v1/ShortageValidation/validate
- **Purpose**: Validate single shortage item
- **Request**: `ShortageItemRequest`
- **Response**: `ShortageValidationResponse`
- **Status Codes**: 200 (OK), 400 (Bad Request), 500 (Error)

#### POST /api/v1/ShortageValidation/validate-batch
- **Purpose**: Validate multiple items in batch
- **Request**: `BatchShortageValidationRequest`
- **Response**: `BatchShortageValidationResponse`
- **Status Codes**: 200 (OK), 400 (Bad Request), 500 (Error)

#### POST /api/v1/ShortageValidation/validate-with-iws
- **Purpose**: Validate with IWS serial number lookup
- **Request**: `ShortageItemIWSRequest` (no serial required)
- **Response**: `ShortageValidationResponse` (serial from IWS)
- **Status Codes**: 200 (OK), 400 (Bad Request), 500 (Error)

#### GET /api/v1/ShortageValidation/statistics
- **Purpose**: Get validation statistics
- **Response**: `ValidationStatistics`

### Reference Data Endpoints

#### GET /api/v1/ReferenceData/defect-codes
- **Purpose**: Get all active defect codes
- **Response**: `List<DefectCodeDto>`

#### GET /api/v1/ReferenceData/location-codes
- **Purpose**: Get all active location codes
- **Response**: `List<LocationCodeDto>`

#### GET /api/v1/ReferenceData/defaults
- **Purpose**: Get default settings
- **Response**: `DefaultSettingsDto`

### Health Check

#### GET /health
- **Purpose**: Health check endpoint
- **Response**: Health status with DB connectivity

---

## 🔐 Security & Configuration

### Configuration Settings (appsettings.json)

```json
{
  "ConnectionStrings": {
    "AshleyDatabase": "...",
    "DatawhseDatabase": "...",
    "ArchiveDatabase": "..."
  },
  "ShortageValidationSettings": {
    "DefaultDefectCode": "XP",
    "DefaultLocationCode": "WU",
    "DefaultEnvironment": "AFI",
    "MaxBatchSize": 500,
    "CommandTimeout": 60
  },
  "IWSIntegration": {
    "BaseUrl": "https://iws-api.ashley.com",
    "ApiKey": "...",
    "Timeout": 30,
    "RetryCount": 3
  }
}
```

---

## 📦 NuGet Packages

### API Layer
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore (Swagger)
- Serilog.AspNetCore (Logging)
- AspNetCore.HealthChecks.SqlServer
- FluentValidation.AspNetCore

### Application Layer
- FluentValidation
- Microsoft.Extensions.DependencyInjection.Abstractions
- Microsoft.Extensions.Logging.Abstractions

### Infrastructure Layer (TO ADD)
- Microsoft.Data.SqlClient (SQL Server)
- Polly (Resilience & Retry)
- Dapper (Micro-ORM)
- Microsoft.Extensions.Http

---

## ✅ Current Status

| Component | Status | Files Created |
|-----------|--------|---------------|
| **API Layer** | ✅ Complete | 5 files |
| **Application Layer** | ⚠️ Partial | 8 files |
| **Domain Layer** | ⏳ Pending | 0 files |
| **Infrastructure Layer** | ⏳ Pending | 0 files |
| **Tests** | ⏳ Pending | 0 files |

**Next Steps**:
1. Create Domain entities
2. Create Infrastructure repositories
3. Wire up dependency injection
4. Create unit tests
5. Create integration tests

