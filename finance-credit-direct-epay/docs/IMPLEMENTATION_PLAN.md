# Finance Credit Direct EPay - Implementation Plan with Rollback Strategy

**Document Version:** 1.0
**Last Updated:** 2026-01-22
**Status:** Draft
**Migration Approach:** Strangler Fig Pattern with Blue-Green Deployment

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Implementation Strategy](#implementation-strategy)
3. [Rollback Strategy](#rollback-strategy)
4. [Phase-by-Phase Implementation](#phase-by-phase-implementation)
5. [Environment Setup](#environment-setup)
6. [Deployment Architecture](#deployment-architecture)
7. [Testing Strategy](#testing-strategy)
8. [Monitoring & Observability](#monitoring--observability)
9. [Risk Management](#risk-management)
10. [Appendices](#appendices)

---

## Executive Summary

### Purpose

This document provides a detailed implementation plan for migrating the Finance Credit Direct EPay application from ASP.NET Web Forms (.NET Framework 3.5, VB.NET) to ASP.NET Core 8.0 (C#) with Angular 17+ frontend.

**Key Principle:** Every deployment must support **instant rollback** to the previous stable version with zero data loss.

### Migration Approach

**Strangler Fig Pattern** - Gradually replace legacy system components while maintaining full backward compatibility.

**Blue-Green Deployment** - Maintain two identical production environments for instant rollback capability.

### Timeline

- **Total Duration:** 10 months
- **Phases:** 10 phases (9 migration phases + 1 logging phase integrated throughout)
- **Rollback Windows:** Defined for each phase
- **Go-Live Strategy:** Phased rollout with traffic shifting

### Success Criteria

✅ **Zero downtime** during migration
✅ **Instant rollback** capability at every phase
✅ **Zero data loss** during rollback
✅ **Parallel running** of old and new systems
✅ **Gradual traffic migration** (0% → 10% → 50% → 100%)

---

## Implementation Strategy

### 1. Strangler Fig Pattern

The Strangler Fig pattern allows us to incrementally replace the legacy system without a "big bang" migration.

```
┌─────────────────────────────────────────────────────────────┐
│                    Load Balancer / Reverse Proxy            │
│                    (IIS ARR or Azure App Gateway)           │
└────────────────────────┬────────────────────────────────────┘
                         │
         ┌───────────────┴───────────────┐
         │                               │
         ▼                               ▼
┌─────────────────┐             ┌─────────────────┐
│  Legacy System  │             │   New System    │
│  (Web Forms)    │             │  (Angular +     │
│  .NET 3.5       │             │   .NET Core 8)  │
│                 │             │                 │
│  - main.aspx    │◄───────────►│  - /api/invoice │
│  - History.aspx │             │  - /api/payment │
│  - Admin.aspx   │             │  - /dashboard   │
└────────┬────────┘             └────────┬────────┘
         │                               │
         └───────────────┬───────────────┘
                         │
                         ▼
              ┌──────────────────┐
              │  Shared Database │
              │  SQL Server +    │
              │  DB2 AS/400      │
              └──────────────────┘
```

**Key Components:**

1. **Routing Layer** - Directs traffic based on URL patterns
2. **Legacy System** - Existing Web Forms application (unchanged)
3. **New System** - Angular frontend + .NET Core API
4. **Shared Database** - Both systems access same data
5. **Feature Flags** - Control which features use new vs old system

### 2. Blue-Green Deployment

Maintain two identical production environments for instant rollback.

```
Production Environment Architecture:

┌─────────────────────────────────────────────────────────────┐
│                    Azure Traffic Manager                    │
│                    (DNS-based load balancing)               │
└────────────────────────┬────────────────────────────────────┘
                         │
         ┌───────────────┴───────────────┐
         │                               │
         ▼                               ▼
┌─────────────────┐             ┌─────────────────┐
│  BLUE (Active)  │             │ GREEN (Standby) │
│                 │             │                 │
│  Current Prod   │             │  New Version    │
│  Version 1.2    │             │  Version 1.3    │
│                 │             │                 │
│  - Web Forms    │             │  - Angular      │
│  - .NET 3.5     │             │  - .NET Core 8  │
└────────┬────────┘             └────────┬────────┘
         │                               │
         └───────────────┬───────────────┘
                         │
                         ▼
              ┌──────────────────┐
              │  Shared Database │
              │  (Read Replicas) │
              └──────────────────┘
```

**Deployment Process:**

1. **Deploy to GREEN** (standby environment)
2. **Test GREEN** thoroughly
3. **Switch Traffic** from BLUE to GREEN (DNS change)
4. **Monitor GREEN** for issues
5. **Rollback** - Switch traffic back to BLUE if issues detected
6. **Decommission BLUE** after stabilization period





### 3. Traffic Shifting Strategy

Gradually shift traffic from legacy to new system using percentage-based routing.

**Week 1: 0% (Testing)**
- New system deployed
- Internal testing only
- No production traffic

**Week 2: 10% (Canary)**
- 10% of users routed to new system
- Monitor metrics closely
- Rollback threshold: Any critical error

**Week 3: 25% (Early Adoption)**
- 25% of users on new system
- Collect user feedback
- Performance comparison

**Week 4: 50% (Majority Testing)**
- 50% traffic split
- Load testing validation
- Database performance monitoring

**Week 5: 75% (Pre-Full Rollout)**
- 75% on new system
- Legacy system on standby
- Final validation

**Week 6: 100% (Full Migration)**
- All traffic to new system
- Legacy system remains available for 30 days
- Decommission after stabilization

**Rollback at Any Stage:**
- Instant traffic shift back to legacy
- No data migration required
- Zero downtime

---

## Rollback Strategy

### Rollback Principles

1. **Instant Rollback** - DNS/routing change only (< 5 minutes)
2. **Zero Data Loss** - Database changes are backward compatible
3. **No Downtime** - Both systems run in parallel
4. **Automated Rollback** - Triggered by health checks
5. **Manual Override** - Operations team can force rollback

### Rollback Triggers

**Automatic Rollback Conditions:**

| Metric | Threshold | Action |
|--------|-----------|--------|
| **Error Rate** | > 5% | Immediate rollback |
| **Response Time** | > 3 seconds (p95) | Rollback after 5 minutes |
| **Availability** | < 99% | Immediate rollback |
| **Payment Failures** | > 1% | Immediate rollback |
| **Database Errors** | > 10 errors/minute | Immediate rollback |
| **Memory Usage** | > 90% | Rollback after 2 minutes |
| **CPU Usage** | > 95% | Rollback after 2 minutes |

**Manual Rollback Conditions:**

- User-reported critical bugs
- Data integrity issues
- Security vulnerabilities discovered
- Business decision to pause migration
- Regulatory compliance issues

### Rollback Procedures

#### 1. Immediate Rollback (< 5 minutes)

**Scenario:** Critical production issue detected

**Steps:**

```powershell
# 1. Switch Traffic Manager to BLUE (legacy)
az network traffic-manager endpoint update `
    --resource-group epay-prod-rg `
    --profile-name epay-traffic-manager `
    --name blue-endpoint `
    --type azureEndpoints `
    --endpoint-status Enabled `
    --priority 1

az network traffic-manager endpoint update `
    --resource-group epay-prod-rg `
    --profile-name epay-traffic-manager `
    --name green-endpoint `
    --type azureEndpoints `
    --endpoint-status Disabled

# 2. Verify traffic routing
Invoke-WebRequest -Uri "https://epay.ashleydirect.com/health" -Headers @{"X-Test"="rollback"}

# 3. Monitor legacy system
# Check Application Insights dashboard
# Verify error rates return to normal

# 4. Notify stakeholders
Send-MailMessage -To "epay-team@ashleyfurniture.com" `
    -Subject "EPay Rollback Executed" `
    -Body "Traffic rolled back to legacy system at $(Get-Date)"
```

**Expected Duration:** 2-5 minutes
**User Impact:** None (seamless transition)
**Data Impact:** None (shared database)

#### 2. Planned Rollback (30 minutes)

**Scenario:** Non-critical issues, planned rollback during maintenance window

**Steps:**

```powershell
# 1. Announce maintenance window
# Send notification to users 1 hour in advance

# 2. Gradually reduce traffic to GREEN
# 100% → 75% → 50% → 25% → 0%

# 3. Stop GREEN application pool
Stop-WebAppPool -Name "EPayCore-AppPool"

# 4. Switch traffic to BLUE
# (Same as immediate rollback)

# 5. Verify BLUE system health
Test-WebApplication -Url "https://epay.ashleydirect.com"

# 6. Analyze GREEN logs for root cause
Get-Content "C:\Logs\EPay-Core\*.log" | Select-String "ERROR"

# 7. Document rollback reason
# Update incident report
```

**Expected Duration:** 30 minutes
**User Impact:** Minimal (during maintenance window)
**Data Impact:** None

#### 3. Database Rollback (1-2 hours)

**Scenario:** Database schema changes need to be reverted

**Important:** All database changes MUST be backward compatible to avoid this scenario.

**Steps:**

```sql
-- 1. Verify current schema version
SELECT TOP 1 * FROM __MigrationHistory ORDER BY MigrationId DESC;

-- 2. Run rollback migration script
-- Example: Rollback from v1.3 to v1.2
BEGIN TRANSACTION;

-- Drop new columns (if any)
ALTER TABLE Invoices DROP COLUMN IF EXISTS NewColumn;

-- Restore old stored procedures
EXEC sp_executesql @RollbackScript;

-- Update migration history
DELETE FROM __MigrationHistory WHERE MigrationId = '20260122_v1.3';

COMMIT TRANSACTION;

-- 3. Verify data integrity
EXEC sp_ValidateDataIntegrity;

-- 4. Restart applications
```

**Expected Duration:** 1-2 hours
**User Impact:** Application downtime during rollback
**Data Impact:** Potential data loss if not backward compatible

**Prevention:** Use backward-compatible migrations only!

### Rollback Testing

**Pre-Deployment Rollback Drills:**

1. **Weekly Rollback Tests** - Practice rollback procedures in staging
2. **Automated Rollback Scripts** - Test automation weekly
3. **Team Training** - All team members trained on rollback procedures
4. **Runbook Validation** - Update runbooks after each test

**Rollback Test Checklist:**

- [ ] Traffic switch completes in < 5 minutes
- [ ] No data loss during rollback
- [ ] Legacy system handles full load
- [ ] Monitoring alerts trigger correctly
- [ ] Team can execute rollback without documentation
- [ ] Stakeholder notification process works
- [ ] Post-rollback validation passes

---

## Phase-by-Phase Implementation

### Phase 0: Planning & Setup (4 Weeks)

**Objectives:**
- Set up development, staging, and production environments
- Configure CI/CD pipelines
- Establish monitoring and alerting
- Train team on new technologies

**Deliverables:**
- ✅ Azure infrastructure provisioned
- ✅ CI/CD pipelines configured
- ✅ Monitoring dashboards created
- ✅ Team training completed
- ✅ Rollback procedures documented and tested

**Rollback Strategy:**
- N/A (no production changes)

**Environment Setup:**

```yaml
# Azure Resources
Resource Group: epay-prod-rg
Location: East US

# Blue Environment (Legacy)
- App Service: epay-blue-app
- App Service Plan: epay-blue-plan (Windows, .NET Framework 3.5)
- URL: https://epay-blue.azurewebsites.net

# Green Environment (New)
- App Service: epay-green-app
- App Service Plan: epay-green-plan (Linux, .NET 8)
- Angular Static Web App: epay-green-frontend
- URL: https://epay-green.azurewebsites.net

# Shared Resources
- SQL Server: epay-sql-server.database.windows.net
- Database: EPayDB
- Application Insights: epay-appinsights
- Traffic Manager: epay-traffic-manager
- Key Vault: epay-keyvault

# Staging Environment
- Resource Group: epay-staging-rg
- Same structure as production
```

**CI/CD Pipeline:**

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main
      - develop
      - feature/*

stages:
  - stage: Build
    jobs:
      - job: BuildBackend
        steps:
          - task: DotNetCoreCLI@2
            inputs:
              command: 'build'
              projects: '**/*.csproj'

      - job: BuildFrontend
        steps:
          - task: Npm@1
            inputs:
              command: 'install'
              workingDir: 'frontend'
          - task: Npm@1
            inputs:
              command: 'custom'
              customCommand: 'run build:prod'

  - stage: Test
    jobs:
      - job: UnitTests
      - job: IntegrationTests
      - job: E2ETests

  - stage: DeployStaging
    condition: eq(variables['Build.SourceBranch'], 'refs/heads/develop')
    jobs:
      - deployment: DeployToStaging
        environment: 'staging'

  - stage: DeployProduction
    condition: eq(variables['Build.SourceBranch'], 'refs/heads/main')
    jobs:
      - deployment: DeployToGreen
        environment: 'production-green'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: AzureWebApp@1
                  inputs:
                    appName: 'epay-green-app'
                    package: '$(Pipeline.Workspace)/**/*.zip'
```

---

### Phase 1: Backend API Foundation (4 Weeks)

**Objectives:**
- Create ASP.NET Core 8.0 Web API project
- Implement authentication and authorization
- Set up database connectivity (SQL Server + DB2)
- Create core API endpoints

**Deliverables:**
- ✅ ASP.NET Core 8.0 Web API project
- ✅ JWT authentication implemented
- ✅ SQL Server connectivity with EF Core
- ✅ DB2 connectivity preserved (IBM.Data.DB2.Core)
- ✅ Core API endpoints (Invoice, Payment)
- ✅ Swagger/OpenAPI documentation
- ✅ Unit tests for core services

**Deployment Strategy:**
- Deploy to GREEN environment only
- No production traffic
- Internal testing only

**Rollback Strategy:**
- N/A (no production traffic)
- Can delete GREEN environment if needed

**Database Changes:**
- None (read-only access to existing database)
- No schema changes in Phase 1

**Testing Checklist:**
- [ ] API endpoints return correct data
- [ ] Authentication works with existing AD users
- [ ] DB2 connectivity works
- [ ] SQL Server queries match legacy system
- [ ] Performance meets requirements (< 500ms response time)
- [ ] Swagger documentation is complete

---

### Phase 2: Frontend Foundation (6 Weeks)

**Objectives:**
- Create Angular 17+ application
- Set up routing and NgRx state management
- Create UI component library matching existing design
- Implement authentication flow

**Deliverables:**
- ✅ Angular 17+ application with TypeScript
- ✅ Angular Material theme matching Ashley Direct branding
- ✅ NgRx state management (Store + Effects)
- ✅ Angular Router with lazy loading
- ✅ HTTP client with interceptors
- ✅ Reusable component library
- ✅ Layout and navigation components

**Deployment Strategy:**
- Deploy to GREEN environment
- Accessible via direct URL only (not through Traffic Manager)
- Internal testing and UAT

**Rollback Strategy:**
- N/A (no production traffic)
- Can redeploy previous version if needed

**Database Changes:**
- None (read-only access)

**Testing Checklist:**
- [ ] UI matches existing design pixel-perfect
- [ ] Navigation works correctly
- [ ] Authentication flow works
- [ ] State management works correctly
- [ ] All components render correctly
- [ ] Responsive design works on all devices

---

### Phase 3-9: Feature Migration (Incremental Rollout)

**Strategy:** Each phase migrates one feature with independent rollback capability.

#### Phase 3: Invoice Search & Selection (8 Weeks)

**Feature:** Main invoice search page (main.aspx)

**Deployment Strategy:**
1. Deploy to GREEN environment
2. Enable feature flag `InvoiceSearch.UseNewSystem = true`
3. Route 10% of invoice search traffic to GREEN
4. Monitor for 1 week
5. Gradually increase to 100%

**Rollback Strategy:**
```csharp
// Feature flag rollback
if (FeatureFlags.InvoiceSearch.UseNewSystem)
{
    // Route to Angular/API
    return RedirectToAction("Index", "Invoice", new { area = "Angular" });
}
else
{
    // Route to legacy Web Forms
    return RedirectToAction("main.aspx");
}
```

**Rollback Trigger:**
- Error rate > 2% → Immediate rollback
- User complaints > 5 → Investigate and rollback if needed
- Performance degradation > 20% → Rollback

**Database Changes:**
- Add `InvoiceSearchAudit` table (backward compatible)
- Add indexes for performance (non-breaking)

**Testing Checklist:**
- [ ] Invoice search returns same results as legacy
- [ ] Pagination works correctly
- [ ] Sorting works correctly
- [ ] Export to Excel works
- [ ] Performance is equal or better than legacy

---

#### Phase 4: Payment Confirmation (6 Weeks)

**Feature:** Payment confirmation page (Confirmation.aspx)

**Deployment Strategy:**
1. Deploy to GREEN
2. Enable feature flag `PaymentConfirmation.UseNewSystem = true`
3. Route 10% → 25% → 50% → 100%

**Rollback Strategy:**
- Feature flag toggle (instant)
- Database transactions are backward compatible
- US Bank integration unchanged

**Critical:** Payment processing must be 100% reliable

**Rollback Trigger:**
- ANY payment failure → Immediate rollback
- US Bank integration error → Immediate rollback

**Database Changes:**
- Add `PaymentConfirmationLog` table (backward compatible)
- No changes to existing payment tables

---

#### Phase 5: Payment History (4 Weeks)

**Feature:** Payment history page (History.aspx)

**Deployment Strategy:**
- Low-risk feature (read-only)
- Can deploy to 100% immediately after testing

**Rollback Strategy:**
- Feature flag toggle
- No database changes

---

#### Phase 6: User Management (6 Weeks)

**Feature:** User list and management (UserList.aspx)

**Deployment Strategy:**
- Admin-only feature
- Deploy to admins first
- Then roll out to all users

**Rollback Strategy:**
- Feature flag toggle
- User data unchanged (read from existing tables)

---

#### Phase 7: Admin Maintenance (6 Weeks)

**Feature:** Admin maintenance page (AdminMaintenance.aspx)

**Deployment Strategy:**
- Admin-only feature
- Thorough testing required
- Gradual rollout to admin users

**Rollback Strategy:**
- Feature flag toggle
- Database changes are backward compatible

---

#### Phase 8: Analyst Reports (6 Weeks)

**Feature:** Analyst report page (AnalystReport.aspx)

**Deployment Strategy:**
- Analyst-only feature
- Deploy to analysts first
- Monitor report accuracy

**Rollback Strategy:**
- Feature flag toggle
- Report data unchanged

---

#### Phase 9: Final Migration & Decommission (4 Weeks)

**Objectives:**
- Migrate remaining features
- Decommission legacy system
- Final performance optimization

**Deployment Strategy:**
1. All features migrated
2. 100% traffic on GREEN
3. BLUE (legacy) on standby for 30 days
4. Decommission BLUE after stabilization

**Rollback Strategy:**
- Full system rollback to BLUE
- Available for 30 days post-migration
- After 30 days, rollback requires database restore

---

## Environment Setup

### Development Environment

**Purpose:** Developer workstations and shared dev server

**Configuration:**

```yaml
Environment: Development
URL: http://localhost:4200 (Angular), https://localhost:7001 (API)

Database:
  - SQL Server: localhost\SQLEXPRESS
  - DB2: Dev AS/400 system

Features:
  - Debug mode enabled
  - Hot reload enabled
  - Detailed error messages
  - US Bank integration mocked
```

**Setup Steps:**

```bash
# 1. Clone repository
git clone https://github.com/afi-internal/finance-credit-direct-epay.git
cd finance-credit-direct-epay

# 2. Set up backend
cd backend
dotnet restore
dotnet build
dotnet ef database update

# 3. Set up frontend
cd ../frontend
npm install
ng serve

# 4. Configure environment
cp .env.example .env
# Edit .env with local database connection strings
```

---

### Staging Environment

**Purpose:** Pre-production testing and UAT

**Configuration:**

```yaml
Environment: Staging
URL: https://epay-staging.ashleydirect.com

Azure Resources:
  - Resource Group: epay-staging-rg
  - App Service: epay-staging-app
  - SQL Server: epay-staging-sql.database.windows.net
  - Application Insights: epay-staging-appinsights

Database:
  - SQL Server: Staging database (refreshed from production weekly)
  - DB2: Staging AS/400 system

Features:
  - Production-like configuration
  - US Bank integration disabled (test mode)
  - Sanitized production data
  - Performance testing enabled
```

**Deployment:**

```bash
# Automated deployment via Azure DevOps
# Triggered on merge to 'develop' branch

# Manual deployment
az webapp deployment source config-zip \
  --resource-group epay-staging-rg \
  --name epay-staging-app \
  --src ./deployment-package.zip
```

---

### Production Environment

**Purpose:** Live production system

**Configuration:**

```yaml
Environment: Production
URL: https://epay.ashleydirect.com

Azure Resources:
  - Resource Group: epay-prod-rg
  - Traffic Manager: epay-traffic-manager
  - Blue App Service: epay-blue-app (Legacy)
  - Green App Service: epay-green-app (New)
  - SQL Server: epay-prod-sql.database.windows.net
  - Application Insights: epay-prod-appinsights
  - Key Vault: epay-keyvault

Database:
  - SQL Server: Production database
  - DB2: Production AS/400 system

Features:
  - Production mode
  - US Bank integration enabled
  - Detailed logging
  - Performance monitoring
  - Automated alerts
```

---

## Deployment Architecture

### Traffic Routing with Feature Flags

**Implementation:**

```csharp
// Feature flag configuration
public class FeatureFlags
{
    public bool InvoiceSearch { get; set; }
    public bool PaymentConfirmation { get; set; }
    public bool PaymentHistory { get; set; }
    public bool UserManagement { get; set; }
    public bool AdminMaintenance { get; set; }
    public bool AnalystReports { get; set; }
    public int TrafficPercentage { get; set; } // 0-100
}

// Routing middleware
public class FeatureFlagMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var featureFlags = context.RequestServices.GetService<FeatureFlags>();
        var path = context.Request.Path.Value;

        // Determine if request should go to new system
        bool useNewSystem = ShouldUseNewSystem(path, featureFlags);

        if (useNewSystem)
        {
            // Route to Angular/API (GREEN)
            context.Response.Redirect($"https://epay-green.azurewebsites.net{path}");
        }
        else
        {
            // Route to legacy Web Forms (BLUE)
            await _next(context);
        }
    }

    private bool ShouldUseNewSystem(string path, FeatureFlags flags)
    {
        // Check feature-specific flags
        if (path.Contains("/invoice") && !flags.InvoiceSearch) return false;
        if (path.Contains("/payment") && !flags.PaymentConfirmation) return false;

        // Check traffic percentage
        var random = new Random().Next(0, 100);
        return random < flags.TrafficPercentage;
    }
}
```

**Feature Flag Management:**

```json
// appsettings.Production.json
{
  "FeatureFlags": {
    "InvoiceSearch": true,
    "PaymentConfirmation": false,
    "PaymentHistory": false,
    "UserManagement": false,
    "AdminMaintenance": false,
    "AnalystReports": false,
    "TrafficPercentage": 10
  }
}
```

**Rollback via Feature Flag:**

```powershell
# Instant rollback - disable all feature flags
az webapp config appsettings set \
  --resource-group epay-prod-rg \
  --name epay-green-app \
  --settings FeatureFlags__TrafficPercentage=0

# Restart app to apply changes
az webapp restart \
  --resource-group epay-prod-rg \
  --name epay-green-app
```

---

## Testing Strategy

### Testing Pyramid

```
                    ┌─────────────┐
                    │   E2E Tests │  (10%)
                    │   Cypress   │
                ┌───┴─────────────┴───┐
                │  Integration Tests  │  (30%)
                │  API + Database     │
            ┌───┴─────────────────────┴───┐
            │      Unit Tests             │  (60%)
            │  Backend + Frontend         │
        ┌───┴─────────────────────────────┴───┐
```

### Unit Tests (60% of tests)

**Backend (C# + xUnit):**

```csharp
// Example: Invoice service unit test
public class InvoiceServiceTests
{
    [Fact]
    public async Task GetInvoices_ReturnsCorrectData()
    {
        // Arrange
        var mockRepo = new Mock<IInvoiceRepository>();
        mockRepo.Setup(r => r.GetInvoicesAsync(It.IsAny<InvoiceSearchCriteria>()))
                .ReturnsAsync(GetTestInvoices());
        var service = new InvoiceService(mockRepo.Object);

        // Act
        var result = await service.GetInvoicesAsync(new InvoiceSearchCriteria());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
    }
}
```

**Frontend (Angular + Jasmine):**

```typescript
// Example: Invoice component unit test
describe('InvoiceSearchComponent', () => {
  let component: InvoiceSearchComponent;
  let fixture: ComponentFixture<InvoiceSearchComponent>;
  let mockInvoiceService: jasmine.SpyObj<InvoiceService>;

  beforeEach(() => {
    mockInvoiceService = jasmine.createSpyObj('InvoiceService', ['searchInvoices']);

    TestBed.configureTestingModule({
      declarations: [InvoiceSearchComponent],
      providers: [
        { provide: InvoiceService, useValue: mockInvoiceService }
      ]
    });

    fixture = TestBed.createComponent(InvoiceSearchComponent);
    component = fixture.componentInstance;
  });

  it('should search invoices on submit', () => {
    mockInvoiceService.searchInvoices.and.returnValue(of(testInvoices));

    component.searchForm.setValue({ customerNumber: '12345' });
    component.onSubmit();

    expect(mockInvoiceService.searchInvoices).toHaveBeenCalled();
    expect(component.invoices.length).toBe(5);
  });
});
```

### Integration Tests (30% of tests)

**API Integration Tests:**

```csharp
public class InvoiceApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public InvoiceApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetInvoices_ReturnsOkResult()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/invoices?customerNumber=12345");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var invoices = JsonSerializer.Deserialize<List<Invoice>>(content);
        Assert.NotEmpty(invoices);
    }
}
```

### E2E Tests (10% of tests)

**Cypress E2E Tests:**

```typescript
// cypress/e2e/invoice-search.cy.ts
describe('Invoice Search', () => {
  beforeEach(() => {
    cy.login('testuser', 'password');
    cy.visit('/invoices');
  });

  it('should search and display invoices', () => {
    cy.get('[data-cy=customer-number]').type('12345');
    cy.get('[data-cy=search-button]').click();

    cy.get('[data-cy=invoice-table]').should('be.visible');
    cy.get('[data-cy=invoice-row]').should('have.length.greaterThan', 0);
  });

  it('should match legacy system results', () => {
    // Compare results with legacy system
    cy.request('GET', 'https://epay-blue.azurewebsites.net/api/invoices?customerNumber=12345')
      .then((legacyResponse) => {
        cy.request('GET', 'https://epay-green.azurewebsites.net/api/invoices?customerNumber=12345')
          .then((newResponse) => {
            expect(newResponse.body).to.deep.equal(legacyResponse.body);
          });
      });
  });
});
```

### Comparison Testing

**Automated Comparison Tests:**

```csharp
// Compare new system output with legacy system
public class ComparisonTests
{
    [Theory]
    [InlineData("12345")]
    [InlineData("67890")]
    public async Task InvoiceSearch_MatchesLegacySystem(string customerNumber)
    {
        // Get results from legacy system
        var legacyClient = new HttpClient { BaseAddress = new Uri("https://epay-blue.azurewebsites.net") };
        var legacyResponse = await legacyClient.GetAsync($"/api/invoices?customerNumber={customerNumber}");
        var legacyData = await legacyResponse.Content.ReadAsStringAsync();

        // Get results from new system
        var newClient = new HttpClient { BaseAddress = new Uri("https://epay-green.azurewebsites.net") };
        var newResponse = await newClient.GetAsync($"/api/invoices?customerNumber={customerNumber}");
        var newData = await newResponse.Content.ReadAsStringAsync();

        // Compare results
        Assert.Equal(legacyData, newData);
    }
}
```

---

## Monitoring & Observability

### Health Checks

**Backend Health Checks:**

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "sql-server")
    .AddCheck<Db2HealthCheck>("db2")
    .AddCheck<USBankHealthCheck>("us-bank")
    .AddApplicationInsightsPublisher();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

**Automated Health Monitoring:**

```powershell
# Health check script (runs every 30 seconds)
while ($true) {
    $health = Invoke-RestMethod -Uri "https://epay-green.azurewebsites.net/health"

    if ($health.status -ne "Healthy") {
        # Trigger rollback
        Write-Host "UNHEALTHY - Triggering rollback" -ForegroundColor Red
        & .\rollback.ps1
        break
    }

    Start-Sleep -Seconds 30
}
```


### Application Insights Dashboards

**Key Metrics Dashboard:**

```kusto
// KQL Query: Error Rate
requests
| where timestamp > ago(1h)
| summarize
    TotalRequests = count(),
    FailedRequests = countif(success == false),
    ErrorRate = (countif(success == false) * 100.0) / count()
| project ErrorRate, TotalRequests, FailedRequests

// KQL Query: Response Time (P95)
requests
| where timestamp > ago(1h)
| summarize P95ResponseTime = percentile(duration, 95)
| project P95ResponseTime

// KQL Query: Payment Success Rate
customEvents
| where name == "PaymentProcessed"
| where timestamp > ago(1h)
| summarize
    TotalPayments = count(),
    SuccessfulPayments = countif(customDimensions.Status == "Success"),
    SuccessRate = (countif(customDimensions.Status == "Success") * 100.0) / count()
| project SuccessRate, TotalPayments, SuccessfulPayments

// KQL Query: Database Performance
dependencies
| where type == "SQL"
| where timestamp > ago(1h)
| summarize
    AvgDuration = avg(duration),
    P95Duration = percentile(duration, 95),
    Count = count()
| project AvgDuration, P95Duration, Count
```

**Comparison Dashboard (Legacy vs New):**

```kusto
// Compare response times
let legacyData = requests
| where cloud_RoleName == "epay-blue-app"
| where timestamp > ago(1h)
| summarize AvgDuration = avg(duration) by bin(timestamp, 5m);

let newData = requests
| where cloud_RoleName == "epay-green-app"
| where timestamp > ago(1h)
| summarize AvgDuration = avg(duration) by bin(timestamp, 5m);

legacyData
| join kind=inner (newData) on timestamp
| project timestamp, LegacyAvg = AvgDuration, NewAvg = AvgDuration1, Difference = AvgDuration1 - AvgDuration
```

### Alert Rules

**Critical Alerts (Immediate Rollback):**

| Alert Name | Condition | Action |
|------------|-----------|--------|
| **High Error Rate** | Error rate > 5% for 2 minutes | Trigger automatic rollback |
| **Payment Failures** | Payment failure rate > 1% | Trigger automatic rollback |
| **Database Errors** | > 10 DB errors/minute | Trigger automatic rollback |
| **Service Unavailable** | Health check fails 3 times | Trigger automatic rollback |

**Warning Alerts (Manual Investigation):**

| Alert Name | Condition | Action |
|------------|-----------|--------|
| **Elevated Error Rate** | Error rate > 2% for 5 minutes | Notify on-call engineer |
| **Slow Response Time** | P95 > 2 seconds for 5 minutes | Notify on-call engineer |
| **High Memory Usage** | Memory > 80% for 10 minutes | Notify on-call engineer |
| **High CPU Usage** | CPU > 85% for 10 minutes | Notify on-call engineer |

**Alert Configuration:**

```yaml
# Azure Monitor Alert Rule
name: HighErrorRate
description: Trigger rollback when error rate exceeds 5%
severity: Critical
evaluationFrequency: PT1M
windowSize: PT2M
criteria:
  allOf:
    - metricName: requests/failed
      operator: GreaterThan
      threshold: 5
      timeAggregation: Average
actions:
  - actionGroupId: /subscriptions/.../actionGroups/epay-rollback-group
    webhookProperties:
      rollbackType: immediate
```

---

## Risk Management

### Risk Assessment Matrix

| Risk | Probability | Impact | Severity | Mitigation Strategy |
|------|-------------|--------|----------|---------------------|
| **Payment Processing Failure** | Low | Critical | **HIGH** | - Extensive testing<br>- Gradual rollout<br>- Instant rollback capability<br>- US Bank integration unchanged |
| **Data Loss During Migration** | Very Low | Critical | **MEDIUM** | - Backward-compatible DB changes<br>- Shared database<br>- No data migration required |
| **Performance Degradation** | Medium | High | **MEDIUM** | - Performance testing<br>- Load testing<br>- Comparison testing<br>- Monitoring dashboards |
| **User Adoption Issues** | Medium | Medium | **MEDIUM** | - UI preservation<br>- User training<br>- Gradual rollout<br>- Feedback collection |
| **Integration Failures (DB2/US Bank)** | Low | Critical | **HIGH** | - Integration testing<br>- Preserve existing flows<br>- Fallback mechanisms |
| **Security Vulnerabilities** | Low | High | **MEDIUM** | - Security audit<br>- Penetration testing<br>- Code review<br>- Dependency scanning |
| **Deployment Failures** | Medium | Medium | **MEDIUM** | - CI/CD automation<br>- Staging environment<br>- Rollback procedures<br>- Deployment checklist |
| **Team Knowledge Gap** | Medium | Medium | **MEDIUM** | - Training program<br>- Documentation<br>- Pair programming<br>- Knowledge transfer |

### Mitigation Strategies

#### 1. Payment Processing Failure

**Prevention:**
- Comprehensive unit and integration tests for payment flow
- US Bank integration remains unchanged (no code changes)
- Comparison testing with legacy system
- Gradual rollout starting with 10% traffic

**Detection:**
- Real-time monitoring of payment success rate
- Alert triggers at 1% failure rate
- Automated comparison with legacy system

**Response:**
- Immediate automatic rollback
- Incident response team activated
- Root cause analysis
- Fix and redeploy

#### 2. Data Loss During Migration

**Prevention:**
- All database changes are backward compatible
- Both systems share the same database
- No data migration required
- Database transactions are atomic

**Detection:**
- Data integrity checks
- Automated comparison queries
- Audit logging

**Response:**
- Rollback to legacy system
- Database restore from backup (if needed)
- Data reconciliation

#### 3. Performance Degradation

**Prevention:**
- Load testing in staging environment
- Performance benchmarking against legacy system
- Database query optimization
- Caching strategy

**Detection:**
- Real-time performance monitoring
- P95 response time alerts
- Database performance metrics

**Response:**
- Gradual rollback (reduce traffic percentage)
- Performance optimization
- Infrastructure scaling

---

## Communication Plan

### Stakeholder Communication

**Pre-Migration (2 weeks before):**

**Email Template:**

```
Subject: EPay System Migration - Phase [X] Starting [Date]

Dear EPay Users,

We are upgrading the EPay system to improve performance, security, and user experience.

What's Changing:
- Modern user interface (same design, better performance)
- Faster response times
- Enhanced security
- Improved reliability

What's NOT Changing:
- Your login credentials
- Payment processing flow
- US Bank integration
- Data and history

Timeline:
- Start Date: [Date]
- Gradual rollout over 6 weeks
- Full migration by [Date]

What You Need to Do:
- Nothing! The transition will be seamless
- Report any issues to epay-support@ashleyfurniture.com

Questions? Contact: [Project Manager Name] at [Email]

Thank you,
EPay Migration Team
```

**During Migration (Weekly Updates):**

```
Subject: EPay Migration Update - Week [X]

Current Status:
✅ [Feature] - Migrated successfully (100% traffic)
🔄 [Feature] - In progress (25% traffic)
⏳ [Feature] - Scheduled for next week

Metrics:
- Error Rate: 0.5% (Target: < 2%)
- Response Time: 450ms (Target: < 500ms)
- User Satisfaction: 95% (Target: > 90%)

Issues Resolved:
- [Issue 1] - Fixed on [Date]
- [Issue 2] - Fixed on [Date]

Next Week:
- [Feature] migration to 50% traffic
- [Feature] migration to 100% traffic

Report Issues: epay-support@ashleyfurniture.com
```

**Post-Migration (1 week after):**

```
Subject: EPay Migration Complete - Thank You!

Dear EPay Users,

The EPay system migration is now complete! All features have been successfully migrated to the new system.

Results:
✅ 100% of features migrated
✅ Zero downtime during migration
✅ Performance improved by 40%
✅ 98% user satisfaction

What's Next:
- Legacy system will remain available for 30 days as backup
- Continued monitoring and optimization
- New features coming soon!

Thank you for your patience and feedback during this migration.

EPay Migration Team
```

### Internal Communication

**Daily Standup (During Migration):**
- Current phase status
- Metrics review
- Issues and blockers
- Rollback decisions

**Incident Communication:**

```
SEVERITY: [Critical/High/Medium/Low]
INCIDENT: [Brief description]
STATUS: [Investigating/Mitigating/Resolved]
IMPACT: [Number of users affected]
ACTION: [Rollback initiated/Fix deployed/Monitoring]
ETA: [Expected resolution time]
```

---

## Success Criteria

### Technical Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| **Error Rate** | < 1% | Application Insights |
| **Response Time (P95)** | < 500ms | Application Insights |
| **Availability** | > 99.9% | Uptime monitoring |
| **Payment Success Rate** | > 99% | Payment logs |
| **Database Performance** | < 200ms (P95) | SQL Server metrics |
| **Code Coverage** | > 80% | Unit test reports |
| **Security Vulnerabilities** | 0 critical, 0 high | Security scan |

### Business Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| **User Satisfaction** | > 90% | User survey |
| **Training Completion** | 100% | Training records |
| **Deployment Success Rate** | > 95% | CI/CD metrics |
| **Rollback Incidents** | < 3 | Incident reports |
| **Time to Rollback** | < 5 minutes | Rollback drills |
| **Migration Timeline** | On schedule ± 2 weeks | Project tracking |

### Acceptance Criteria

**Phase Completion Criteria:**

- [ ] All features migrated and tested
- [ ] Error rate < 1% for 7 consecutive days
- [ ] Performance meets or exceeds legacy system
- [ ] No critical or high security vulnerabilities
- [ ] User acceptance testing passed
- [ ] Documentation complete
- [ ] Team training complete
- [ ] Rollback procedures tested and validated

**Final Migration Criteria:**

- [ ] 100% traffic on new system for 30 days
- [ ] Zero rollback incidents in last 14 days
- [ ] All success metrics met
- [ ] Stakeholder approval obtained
- [ ] Legacy system decommissioned
- [ ] Post-migration review completed


---

## Timeline and Milestones

### Overall Timeline: 10 Months

```
Month 1: Phase 0 - Planning & Setup
├─ Week 1-2: Azure infrastructure setup
├─ Week 3: CI/CD pipeline configuration
└─ Week 4: Team training

Month 2-3: Phase 1 - Backend API Foundation
├─ Week 1-2: ASP.NET Core project setup
├─ Week 3-4: Authentication & database connectivity
├─ Week 5-6: Core API endpoints
└─ Week 7-8: Testing and documentation

Month 3-5: Phase 2 - Frontend Foundation
├─ Week 1-2: Angular project setup
├─ Week 3-4: Angular Material theme
├─ Week 5-8: NgRx state management
├─ Week 9-12: Component library
└─ Week 13-14: Testing and UAT

Month 5-7: Phase 3 - Invoice Search Migration
├─ Week 1-4: Development and testing
├─ Week 5: Deploy to 10% traffic
├─ Week 6: Increase to 25% traffic
├─ Week 7: Increase to 50% traffic
└─ Week 8: Increase to 100% traffic

Month 7-8: Phase 4 - Payment Confirmation Migration
├─ Week 1-3: Development and testing
├─ Week 4: Deploy to 10% traffic
├─ Week 5: Increase to 50% traffic
└─ Week 6: Increase to 100% traffic

Month 8: Phase 5 - Payment History Migration
├─ Week 1-2: Development and testing
├─ Week 3: Deploy to 50% traffic
└─ Week 4: Deploy to 100% traffic

Month 8-9: Phase 6 - User Management Migration
├─ Week 1-3: Development and testing
├─ Week 4: Deploy to admins
├─ Week 5: Deploy to 50% users
└─ Week 6: Deploy to 100% users

Month 9: Phase 7 - Admin Maintenance Migration
├─ Week 1-3: Development and testing
├─ Week 4: Deploy to admins
├─ Week 5: Testing and validation
└─ Week 6: Full deployment

Month 9-10: Phase 8 - Analyst Reports Migration
├─ Week 1-3: Development and testing
├─ Week 4: Deploy to analysts
├─ Week 5: Testing and validation
└─ Week 6: Full deployment

Month 10: Phase 9 - Final Migration & Decommission
├─ Week 1-2: Final testing and optimization
├─ Week 3: 100% traffic on new system
└─ Week 4: Legacy system decommission planning
```

### Critical Milestones

| Milestone | Target Date | Dependencies | Success Criteria |
|-----------|-------------|--------------|------------------|
| **Azure Infrastructure Ready** | End of Month 1 | Budget approval | All resources provisioned |
| **Backend API Complete** | End of Month 3 | Infrastructure ready | All API endpoints tested |
| **Frontend Foundation Complete** | End of Month 5 | Backend API complete | UI matches legacy design |
| **First Feature Migrated (Invoice Search)** | End of Month 7 | Frontend complete | 100% traffic on new system |
| **Payment Processing Migrated** | End of Month 8 | Invoice search stable | 100% payment success rate |
| **All Features Migrated** | End of Month 10 | All phases complete | All features at 100% traffic |
| **Legacy System Decommissioned** | Month 11 | 30 days stability | BLUE environment removed |

---

## Appendices

### Appendix A: Rollback Scripts

#### A.1 Immediate Rollback Script

**File:** `rollback-immediate.ps1`

```powershell
<#
.SYNOPSIS
    Immediate rollback to legacy system (BLUE)
.DESCRIPTION
    Switches Traffic Manager to route all traffic to BLUE (legacy) environment
    Expected execution time: < 5 minutes
.PARAMETER Reason
    Reason for rollback (required for audit)
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$Reason
)

$ErrorActionPreference = "Stop"
$StartTime = Get-Date

Write-Host "========================================" -ForegroundColor Red
Write-Host "INITIATING IMMEDIATE ROLLBACK" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Red
Write-Host "Reason: $Reason"
Write-Host "Started: $StartTime"
Write-Host ""

# Step 1: Disable GREEN endpoint
Write-Host "[1/5] Disabling GREEN endpoint..." -ForegroundColor Yellow
az network traffic-manager endpoint update `
    --resource-group epay-prod-rg `
    --profile-name epay-traffic-manager `
    --name green-endpoint `
    --type azureEndpoints `
    --endpoint-status Disabled

Write-Host "✓ GREEN endpoint disabled" -ForegroundColor Green

# Step 2: Enable BLUE endpoint with priority 1
Write-Host "[2/5] Enabling BLUE endpoint..." -ForegroundColor Yellow
az network traffic-manager endpoint update `
    --resource-group epay-prod-rg `
    --profile-name epay-traffic-manager `
    --name blue-endpoint `
    --type azureEndpoints `
    --endpoint-status Enabled `
    --priority 1

Write-Host "✓ BLUE endpoint enabled" -ForegroundColor Green

# Step 3: Verify traffic routing
Write-Host "[3/5] Verifying traffic routing..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

$response = Invoke-WebRequest -Uri "https://epay.ashleydirect.com/health" -Headers @{"X-Test"="rollback"}
if ($response.StatusCode -eq 200) {
    Write-Host "✓ Traffic routing verified" -ForegroundColor Green
} else {
    Write-Host "✗ Traffic routing verification failed" -ForegroundColor Red
    exit 1
}

# Step 4: Update feature flags
Write-Host "[4/5] Updating feature flags..." -ForegroundColor Yellow
az webapp config appsettings set `
    --resource-group epay-prod-rg `
    --name epay-green-app `
    --settings FeatureFlags__TrafficPercentage=0

Write-Host "✓ Feature flags updated" -ForegroundColor Green

# Step 5: Send notifications
Write-Host "[5/5] Sending notifications..." -ForegroundColor Yellow

$EmailBody = @"
ROLLBACK EXECUTED

Time: $StartTime
Reason: $Reason
Duration: $((Get-Date) - $StartTime)
Status: SUCCESS

All traffic has been routed back to the legacy system (BLUE).
The new system (GREEN) is offline.

Next Steps:
1. Investigate root cause
2. Fix issues in GREEN environment
3. Test thoroughly in staging
4. Plan redeployment

EPay Operations Team
"@

Send-MailMessage `
    -To "epay-team@ashleyfurniture.com" `
    -Subject "URGENT: EPay Rollback Executed" `
    -Body $EmailBody `
    -SmtpServer "smtp.ashleyfurniture.com" `
    -From "epay-ops@ashleyfurniture.com"

Write-Host "✓ Notifications sent" -ForegroundColor Green

# Summary
$Duration = (Get-Date) - $StartTime
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "ROLLBACK COMPLETED SUCCESSFULLY" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Duration: $($Duration.TotalMinutes) minutes"
Write-Host "All traffic is now on BLUE (legacy) system"
Write-Host ""

# Log to file
$LogEntry = @{
    Timestamp = $StartTime
    Reason = $Reason
    Duration = $Duration.TotalMinutes
    Status = "Success"
} | ConvertTo-Json

Add-Content -Path "C:\Logs\EPay\rollback-log.json" -Value $LogEntry
```

#### A.2 Gradual Rollback Script

**File:** `rollback-gradual.ps1`

```powershell
<#
.SYNOPSIS
    Gradual rollback to legacy system
.DESCRIPTION
    Gradually reduces traffic to GREEN and increases traffic to BLUE
    100% → 75% → 50% → 25% → 0%
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$Reason
)

$Percentages = @(75, 50, 25, 0)

Write-Host "Starting gradual rollback..." -ForegroundColor Yellow
Write-Host "Reason: $Reason"

foreach ($Percentage in $Percentages) {
    Write-Host ""
    Write-Host "Reducing GREEN traffic to $Percentage%..." -ForegroundColor Yellow

    # Update feature flag
    az webapp config appsettings set `
        --resource-group epay-prod-rg `
        --name epay-green-app `
        --settings FeatureFlags__TrafficPercentage=$Percentage

    # Wait and monitor
    Write-Host "Monitoring for 5 minutes..."
    Start-Sleep -Seconds 300

    # Check health
    $health = Invoke-RestMethod -Uri "https://epay.ashleydirect.com/health"
    if ($health.status -eq "Healthy") {
        Write-Host "✓ System healthy at $Percentage%" -ForegroundColor Green
    } else {
        Write-Host "✗ System unhealthy - accelerating rollback" -ForegroundColor Red
        & .\rollback-immediate.ps1 -Reason "Health check failed during gradual rollback"
        exit 1
    }
}

Write-Host ""
Write-Host "✓ Gradual rollback complete" -ForegroundColor Green
```

### Appendix B: Deployment Checklist

#### B.1 Pre-Deployment Checklist

**Phase:** [Phase Number and Name]
**Date:** [Deployment Date]
**Deployed By:** [Name]

**Code Quality:**
- [ ] All unit tests passing (> 80% coverage)
- [ ] All integration tests passing
- [ ] E2E tests passing
- [ ] Code review completed and approved
- [ ] No critical or high security vulnerabilities
- [ ] Performance tests passed

**Infrastructure:**
- [ ] Azure resources provisioned
- [ ] Database migrations tested in staging
- [ ] Feature flags configured
- [ ] Monitoring dashboards configured
- [ ] Alert rules configured
- [ ] Rollback scripts tested

**Documentation:**
- [ ] API documentation updated
- [ ] User documentation updated
- [ ] Runbooks updated
- [ ] Deployment notes prepared

**Communication:**
- [ ] Stakeholders notified (2 weeks advance)
- [ ] User communication sent (1 week advance)
- [ ] On-call team briefed
- [ ] Rollback team on standby

**Approvals:**
- [ ] Technical lead approval
- [ ] Product owner approval
- [ ] Security team approval
- [ ] Operations team approval

#### B.2 Deployment Checklist

**During Deployment:**
- [ ] Backup current production database
- [ ] Deploy to GREEN environment
- [ ] Run database migrations
- [ ] Verify health checks passing
- [ ] Enable feature flags (start at 0%)
- [ ] Smoke tests passed
- [ ] Gradually increase traffic (10% → 25% → 50% → 75% → 100%)
- [ ] Monitor metrics at each stage
- [ ] Verify no errors or performance degradation

#### B.3 Post-Deployment Checklist

**After Deployment:**
- [ ] All features working as expected
- [ ] Error rate < 1%
- [ ] Response time < 500ms (P95)
- [ ] Payment success rate > 99%
- [ ] No user-reported critical issues
- [ ] Monitoring dashboards showing green
- [ ] Rollback procedures validated
- [ ] Post-deployment review scheduled
- [ ] Documentation updated
- [ ] Stakeholders notified of success

### Appendix C: Contact List

#### Escalation Matrix

| Role | Name | Email | Phone | Escalation Level |
|------|------|-------|-------|------------------|
| **Project Manager** | [Name] | [Email] | [Phone] | Level 1 |
| **Technical Lead** | [Name] | [Email] | [Phone] | Level 1 |
| **DevOps Engineer** | [Name] | [Email] | [Phone] | Level 1 |
| **Database Administrator** | [Name] | [Email] | [Phone] | Level 2 |
| **Security Lead** | [Name] | [Email] | [Phone] | Level 2 |
| **IT Director** | [Name] | [Email] | [Phone] | Level 3 |
| **CTO** | [Name] | [Email] | [Phone] | Level 4 |

#### On-Call Rotation

**Week 1-2:** [Name]
**Week 3-4:** [Name]
**Week 5-6:** [Name]
**Week 7-8:** [Name]

**On-Call Responsibilities:**
- Monitor system health 24/7
- Respond to alerts within 15 minutes
- Execute rollback if needed
- Escalate critical issues
- Document all incidents

### Appendix D: Glossary

| Term | Definition |
|------|------------|
| **BLUE Environment** | Legacy ASP.NET Web Forms system (.NET Framework 3.5) |
| **GREEN Environment** | New ASP.NET Core 8.0 + Angular 17+ system |
| **Blue-Green Deployment** | Deployment strategy with two identical production environments |
| **Feature Flag** | Configuration toggle to enable/disable features |
| **Rollback** | Process of reverting to previous system version |
| **Traffic Manager** | Azure service for routing traffic between environments |
| **Canary Deployment** | Gradual rollout starting with small percentage of users |
| **P95 Response Time** | 95th percentile response time (95% of requests faster than this) |
| **NgRx** | State management library for Angular applications |
| **EF Core** | Entity Framework Core - ORM for .NET |
| **Serilog** | Structured logging library for .NET |
| **Application Insights** | Azure monitoring and analytics service |

---

## Document Control

### Version History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-01-22 | EPay Migration Team | Initial implementation plan created with comprehensive rollback strategy |

### Approval Signatures

| Role | Name | Signature | Date |
|------|------|-----------|------|
| **Project Manager** | | | |
| **Technical Lead** | | | |
| **Product Owner** | | | |
| **IT Director** | | | |
| **Security Lead** | | | |

### Document Information

**Document Title:** Finance Credit Direct EPay - Implementation Plan with Rollback Strategy
**Document Owner:** EPay Migration Team
**Classification:** Internal Use Only
**Review Frequency:** Monthly during migration, Quarterly post-migration
**Next Review Date:** [Date]

---

**END OF DOCUMENT**



