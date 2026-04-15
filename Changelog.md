# Changelog

All notable changes to the Onbase Invoice Chatbot project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- 2026-04-15 - **Azure OpenAI Integration** - Replaced pattern-based NLP with Azure OpenAI for advanced natural language processing
  - Added Azure.AI.OpenAI NuGet package (v2.1.0)
  - Created AzureOpenAIService with function calling for structured parameter extraction
  - Created AzureOpenAISettings model for configuration management
  - Added comprehensive Azure OpenAI setup guide (docs/AZURE_OPENAI_SETUP.md)
  - Configured support for GPT-4 and GPT-3.5-Turbo deployments
  - Implemented secure API key management (User Secrets, Azure Key Vault, Environment Variables)
  - Added detailed cost analysis and pricing estimates
  - Created troubleshooting guide for common Azure OpenAI issues

### Changed
- 2026-04-15 - **ChatbotService** - Updated to use IAzureOpenAIService instead of IInvoiceQueryParser
  - Replaced synchronous ParseQuery with asynchronous ParseQueryAsync
  - Maintained backward compatibility with existing QueryIntent structure
  - Enhanced logging for Azure OpenAI API calls
- 2026-04-15 - **Program.cs** - Added Azure OpenAI configuration and service registration
  - Configured AzureOpenAISettings from appsettings.json
  - Registered IAzureOpenAIService with dependency injection
  - Added logging for Azure OpenAI configuration validation
  - Updated Swagger description to reflect Azure OpenAI integration
- 2026-04-15 - **README.md** - Updated documentation to reflect Azure OpenAI integration
  - Added Azure OpenAI to technology stack
  - Updated prerequisites to include Azure OpenAI resource
  - Added configuration section for Azure OpenAI settings
  - Included security warnings for API key management
  - Added link to Azure OpenAI Setup Guide
- 2026-04-15 - **appsettings.json** - Added Azure OpenAI configuration section
  - Endpoint URL for Azure OpenAI resource
  - API Key placeholder with security warning
  - Deployment name configuration
  - MaxTokens and Temperature settings
- 2026-03-23 - Multiple files - Replace production DB connection string with staging server (aazeus-obdmsq01)
  - Updated appsettings.json to use staging server: aazeus-obdmsq01
  - Removed production-specific documentation files
  - Updated all documentation references to staging server
  - Removed production server traces (AE1DCVPSQ23407)
  - Updated .gitignore to exclude production config files
  - Cleaned up SQL authentication code from Program.cs

### Added
- 2026-03-23 - docs/USER_GUIDE.html - Created interactive HTML version of user guide
  - Professional responsive design with gradient header and smooth navigation
  - Embedded Mermaid.js for interactive diagram rendering
  - Color-coded sections (business context, architecture, NLP, examples, API, security)
  - Styled code blocks with syntax highlighting
  - Interactive navigation with smooth scrolling
  - Print-friendly CSS for documentation export
  - Mobile-responsive layout
- 2026-03-23 - docs/USER_GUIDE.md - Created comprehensive user guide with business context and NLP flow diagrams
  - Added business context explaining Finance AP invoice upload process via Kofax to Onbase
  - Documented POC chatbot solution for Finance team's invoice analytics needs
  - Created detailed NLP processing flow diagrams using Mermaid
  - Added two real-world example use cases with sequence diagrams:
    * Example 1: Vendor search with year range (SUPREME GRAPHICS 2025/2026)
    * Example 2: Vendor search with relative date (GOOGLE last one month)
  - Documented supported query patterns (vendor, date, amount, status)
  - Added technical implementation details (NLP components, database schema)
  - Included API reference, security compliance, and performance metrics
- 2026-03-23 - README.md - Enhanced with business context and real-world examples
  - Added Finance department context and Kofax integration explanation
  - Included link to comprehensive User Guide
  - Added real-world query examples from Finance team use cases
- 2026-03-23 - Tests/ - Created comprehensive test infrastructure (`.augment` rules compliance)
  - Created xUnit test project targeting .NET 8.0
  - Added unit tests for InvoiceQueryParser (9 test cases covering NLP parsing logic)
  - Added unit tests for ChatbotService (7 test cases covering business logic)
  - Added integration tests for ChatbotController (8 test cases covering API endpoints)
  - Added test dependencies: xUnit 2.9.2, Moq 4.20.72, Microsoft.AspNetCore.Mvc.Testing 8.0.0
  - Created `/docs/TESTING_SETUP.md` documenting test infrastructure
- 2026-03-23 - docs/ - Organized documentation per `.augment` rules
  - Moved 28 markdown files from root to `/docs` folder
  - Added Ashley Furniture copyright notice to README.md
  - Created Changelog.md following "Keep a Changelog" format
- 2026-03-13 - wwwroot/index.html - Fix API URL path for subdirectory deployments (IIS virtual directory support)
  - Added automatic base path detection for IIS virtual directory deployments
  - Updates all API endpoints (query, health, dbtest) to use base path
- 2026-03-13 - Program.cs - Add comprehensive error logging and IIS deployment diagnostics
  - Added Serilog file-based logging system
  - Added startup configuration validation
  - Added detailed error messages for troubleshooting
- 2026-03-13 - Controllers/ChatbotController.cs - Add health check and database test endpoints
  - Added /api/chatbot/health endpoint for application health checks
  - Added /api/chatbot/dbtest endpoint for database connectivity testing
- 2026-03-13 - Services/InvoiceAnalyticsService.cs - Implement Advanced Invoice Analytics
  - Add analytics capabilities: cost summaries, averages, pricing trend detection for vendor invoices

### Fixed
- 2026-03-13 - wwwroot/index.html - Fixed 404 errors when deployed to IIS virtual directories
- 2026-03-13 - Services/InvoiceQueryParser.cs - Fixed date range parsing for ISO format dates
- 2026-03-13 - Data/OnbaseRepository.cs - Fixed vendor search to support both vendor codes and names
- 2026-03-13 - Controllers/ChatbotController.cs - Fixed error handling to return detailed error messages

### Security
- 2026-03-23 - .gitignore, appsettings.json - Removed production credentials and server information
  - Deleted production-specific configuration files
  - Added rules to prevent committing sensitive configuration files

### Known Issues
- 2026-03-23 - Tests/Chatbot-Onbase.Tests.csproj - MSBuild duplicate assembly attribute errors
  - Test project encounters CS0579 errors for duplicate assembly attributes
  - Main project builds successfully; issue only affects test project compilation
  - Multiple cleanup attempts made (obj/bin removal, target framework alignment)
  - Documented in `/docs/TESTING_SETUP.md` with recommended solutions
  - Tests are written but cannot be executed until build issue is resolved

---

## Version History

### [1.0.0] - 2026-03-13

#### Initial Release
- Natural language invoice search chatbot
- Pattern-based query parser (no external API dependencies)
- Support for vendor, invoice number, PO number, status, amount, and date range queries
- Web-based chat interface
- Direct SQL queries to Onbase database
- Windows Integrated Security authentication
- Comprehensive logging with Serilog
- IIS deployment support with virtual directory compatibility

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**
This software is proprietary and confidential. Unauthorized copying, distribution, or use of this software, via any medium, is strictly prohibited.

