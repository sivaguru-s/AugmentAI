# Credit Shortage Validation Microservice

## 📋 Overview

A .NET 8 microservice for validating shortage items before credit entry. This service integrates with the consolidated stored procedure `usp_CE_ValidateShortageItems` and provides REST API endpoints for shortage validation.

## 🏗️ Architecture

This microservice follows **Clean Architecture** principles with clear separation of concerns:

- **API Layer**: REST endpoints, request/response handling, Swagger documentation
- **Application Layer**: Business logic, service orchestration, DTOs
- **Domain Layer**: Core entities and business models
- **Infrastructure Layer**: Database access, external API integration

See [MICROSERVICE_ARCHITECTURE.md](MICROSERVICE_ARCHITECTURE.md) for detailed architecture documentation.

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (access to Ashley/Datawhse/Archive databases)
- Visual Studio 2022 or VS Code

### Build & Run

```bash
# Navigate to the Microservice directory
cd Microservice

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
cd CreditShortage.Api
dotnet run
```

The API will start at `https://localhost:5001` (or as configured). Swagger UI is available at the root URL.

## 🔧 Configuration

Update `appsettings.json` with your environment settings:

```json
{
  "ConnectionStrings": {
    "AshleyDatabase": "Server=YOUR_SERVER;Database=Ashley;Integrated Security=true;TrustServerCertificate=true;",
    "DatawhseDatabase": "Server=YOUR_SERVER;Database=Datawhse;Integrated Security=true;TrustServerCertificate=true;",
    "ArchiveDatabase": "Server=YOUR_SERVER;Database=Archive;Integrated Security=true;TrustServerCertificate=true;"
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
    "ApiKey": "YOUR_IWS_API_KEY",
    "Timeout": 30,
    "RetryCount": 3,
    "RetryDelaySeconds": 2
  }
}
```

## 📡 API Endpoints

### Validation Endpoints

#### `POST /api/v1/ShortageValidation/validate`
Validates a single shortage item.

**Request Body:**
```json
{
  "customerNumber": "12345678",
  "shipToNumber": "0001",
  "invoiceNumber": 123456,
  "itemNumber": "ITEM001",
  "serialNumber": "SER001",
  "shortageQuantity": 1,
  "defectCode": "XP",
  "locationCode": "WU"
}
```

#### `POST /api/v1/ShortageValidation/validate-batch`
Validates multiple shortage items in a batch.

#### `POST /api/v1/ShortageValidation/validate-with-iws`
Validates a shortage item with IWS integration to automatically obtain the serial number.

### Reference Data Endpoints

#### `GET /api/v1/ReferenceData/defect-codes`
Returns all active defect codes.

#### `GET /api/v1/ReferenceData/location-codes`
Returns all active location/warehouse codes.

#### `GET /api/v1/ReferenceData/defaults`
Returns default configuration settings.

### Health Check

#### `GET /health`
Returns health status including database connectivity.

## 📊 Database Integration

### Stored Procedure
The microservice calls `usp_CE_ValidateShortageItems` which performs:

1. **Item Validation** - Verifies item exists in item master
2. **Customer/Serial/Item Validation** - Validates the combination
3. **Order Quantity Validation** - Checks ordered vs. shortage quantities
4. **Credit History Validation** - Verifies no duplicate credits
5. **Remaining Creditable Quantity** - Calculates available credit amount
6. **Defect Code Validation** - Validates defect code (defaults to 'XP')
7. **Location Code Validation** - Validates warehouse code (defaults to 'WU')

### Table-Valued Parameter
Uses `typCEShortageItemValidation` for batch processing.

## 🔌 External Integrations

### IWS (Inventory Warehouse System)
- Fetches serial numbers for shortage items
- Implements retry logic with Polly
- Circuit breaker pattern for resilience

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📦 Project Structure

```
CreditShortage.Solution/
├── CreditShortage.Api/              # REST API Layer
├── CreditShortage.Application/      # Business Logic Layer
├── CreditShortage.Domain/           # Domain Entities
├── CreditShortage.Infrastructure/   # Data Access & External Services
└── CreditShortage.Tests/            # Unit & Integration Tests
```

## 🛠️ Technologies

- **.NET 8.0** - Framework
- **ASP.NET Core** - Web API
- **Dapper** - Micro-ORM for database access
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **Polly** - Resilience and retry policies
- **FluentValidation** - Request validation

## 📝 Logging

Logs are written to:
- Console (structured JSON)
- File: `logs/credit-shortage-{Date}.txt`

## 🔐 Security

- API Key authentication for IWS integration
- SQL parameter binding to prevent injection
- HTTPS enforcement
- CORS configuration

## 👥 Contributors

- **Sivaguru Sampanthamoorthy** (SSampanthamoorthy@ashleyfurnitureindia.com)

## 📄 License

Internal use only - Ashley Furniture Industries, Inc.
