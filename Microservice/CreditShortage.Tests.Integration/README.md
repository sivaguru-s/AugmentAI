# Integration Tests

This project contains integration tests for the Credit Shortage Validation Microservice.

## Overview

Integration tests verify that multiple components work together correctly, including:
- Database access via repositories
- API endpoints
- External service integration

## Test Categories

### Repository Tests
- Test actual database operations
- Verify stored procedure integration
- Test connection handling and error scenarios

### API Tests
- Test end-to-end API endpoints
- Verify request/response handling
- Test authentication and authorization
- Validate error responses

### External Service Tests  
- Test IWS integration
- Verify resilience policies (retry, circuit breaker)
- Test timeout scenarios

## Running Tests

### All Integration Tests
```powershell
dotnet test CreditShortage.Tests.Integration
```

### With Coverage
```powershell
dotnet test CreditShortage.Tests.Integration /p:CollectCoverage=true
```

### Specific Test Class
```powershell
dotnet test --filter FullyQualifiedName~ShortageValidationRepositoryTests
```

## Database Requirements

Integration tests require access to test databases:
- **Ashley** database (for reference data)
- **Datawhse** database (for invoice/order data)
- **Archive** database (for historical data)

### Test Database Setup

Option 1: Use Testcontainers (Recommended for CI/CD)
- Tests automatically spin up SQL Server containers
- No manual database setup required
- Tests are isolated and repeatable

Option 2: Use Existing Test Database
- Configure connection strings in `testsettings.json`
- Ensure test data exists
- Run database migrations/scripts

## Configuration

Create `testsettings.json` (git-ignored):

```json
{
  "ConnectionStrings": {
    "AshleyDatabase": "Server=test-server;Database=Ashley_Test;Integrated Security=true;",
    "DatawhseDatabase": "Server=test-server;Database=Datawhse_Test;Integrated Security=true;"
  },
  "ShortageValidationSettings": {
    "DefaultDefectCode": "XP",
    "DefaultLocationCode": "WU",
    "DefaultEnvironment": "TEST"
  }
}
```

## Test Data

Integration tests use:
- **Fixed test data** - Known customer, item, invoice numbers
- **Cleanup** - Tests clean up after themselves
- **Isolation** - Each test is independent

### Test Data Constants
```csharp
public static class TestData
{
    public const string ValidCustomerNumber = "2067300";
    public const string ValidItemNumber = "D425-325";
    public const string ValidSerialNumber = "999999";
    public const long ValidInvoiceNumber = 12345678;
}
```

## Best Practices

1. **Test Isolation** - Each test should be independent
2. **Data Cleanup** - Clean up test data after tests
3. **Descriptive Names** - Use clear test method names
4. **Arrange-Act-Assert** - Follow AAA pattern
5. **Async Tests** - Use async/await for all database operations

## CI/CD Integration

These tests are designed to run in CI/CD pipelines:
- Use Testcontainers for database isolation
- No external dependencies
- Fast execution (< 2 minutes)

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**
