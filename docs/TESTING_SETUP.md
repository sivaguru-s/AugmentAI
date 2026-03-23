# Testing Infrastructure Setup

## Overview

This document describes the testing infrastructure created for the Chatbot-Onbase project as part of the codebase cleanup to comply with `.augment` rules.

## Test Project Structure

### Location
- **Test Project**: `Tests/Chatbot-Onbase.Tests.csproj`
- **Target Framework**: .NET 8.0
- **Test Framework**: xUnit 2.9.2

### Dependencies
```xml
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageReference Include="coverlet.collector" Version="6.0.2" />
```

## Test Files Created

### 1. Unit Tests

#### `Tests/Services/InvoiceQueryParserTests.cs`
Tests for the NLP query parsing logic:
- ✅ Vendor name extraction
- ✅ Status keyword detection
- ✅ Amount range parsing
- ✅ Date range parsing
- ✅ Invoice number extraction
- ✅ Edge cases and error handling

#### `Tests/Services/ChatbotServiceTests.cs`
Tests for the main chatbot service:
- ✅ Query processing workflow
- ✅ Repository interaction (mocked)
- ✅ Response formatting
- ✅ Error handling
- ✅ Empty result handling
- ✅ Multiple result handling

### 2. Integration Tests

#### `Tests/Controllers/ChatbotControllerIntegrationTests.cs`
End-to-end API tests:
- ✅ Health check endpoint
- ✅ Query endpoint with valid requests
- ✅ Query endpoint with invalid requests
- ✅ Vendor search scenarios
- ✅ Status search scenarios
- ✅ Amount search scenarios
- ✅ Date search scenarios

## Known Issues

### Build Error: Duplicate Assembly Attributes

**Status**: ⚠️ **UNRESOLVED**

**Error**:
```
error CS0579: Duplicate 'global::System.Runtime.Versioning.TargetFrameworkAttribute' attribute
error CS0579: Duplicate 'System.Reflection.AssemblyCompanyAttribute' attribute
... (and other assembly attributes)
```

**Root Cause**:
MSBuild is generating assembly attributes multiple times when building the test project that references the main project. This is a known MSBuild issue that can occur when:
1. Project references cause transitive compilation
2. Multiple build configurations exist
3. Solution-level builds trigger cascading compilations

**Attempted Fixes**:
- ✅ Set `<GenerateAssemblyInfo>true</GenerateAssemblyInfo>` in both projects
- ✅ Cleaned obj/bin folders multiple times
- ✅ Used `git clean -fdx` to remove all build artifacts
- ✅ Changed test project target framework from net9.0 to net8.0
- ✅ Added `<IsTestProject>true</IsTestProject>` property
- ❌ Issue persists when building test project

**Recommended Solutions** (to be tried):
1. **Option A**: Add `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` and manually create AssemblyInfo.cs files
2. **Option B**: Investigate if there's a conflicting MSBuild SDK or target
3. **Option C**: Use a different test project structure (separate solution)
4. **Option D**: Check for environment-specific MSBuild configuration issues

**Workaround**:
The main project builds successfully on its own:
```powershell
dotnet build Chatbot-Onbase.csproj  # ✅ Works
```

The issue only occurs when building the test project or solution.

## Running Tests (Once Build Issue is Resolved)

### Run All Tests
```powershell
dotnet test
```

### Run Specific Test Class
```powershell
dotnet test --filter "FullyQualifiedName~InvoiceQueryParserTests"
```

### Run with Coverage
```powershell
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

## Test Coverage Goals

According to `.augment` rules, all code should have:
- ✅ Unit tests
- ✅ Integration tests
- ✅ End-to-end tests

**Current Status**: Test files created, awaiting build fix to execute.

## Next Steps

1. **Resolve build issue** - Priority: HIGH
2. **Run tests and verify they pass**
3. **Add additional test cases** for edge scenarios
4. **Integrate with CI/CD pipeline**
5. **Set up code coverage reporting**
6. **Add performance tests** for database queries

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**  
This software is proprietary and confidential. Unauthorized copying, distribution, or use of this software, via any medium, is strictly prohibited.

