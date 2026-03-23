# Onbase Invoice Chatbot AI Agent

An intelligent chatbot that searches and retrieves invoice information from Onbase SQL database using natural language queries - **no API key authentication required**.

## Business Context

The Finance Accounting department at Ashley Furniture Industries uses a service to upload Accounts Payable (AP) invoices to the **Onbase system** (a comprehensive document management system). Vendor invoices are automatically uploaded to Onbase through various vendor systems, with **Kofax** being the primary integration platform.

This chatbot was developed as a **Proof of Concept (POC)** in response to an internal request from the Finance team for an intelligent search solution. The chatbot communicates directly with the Onbase database and processes natural language queries to fetch invoice analytics and information in real-time.

**📖 For detailed documentation, see [User Guide](docs/USER_GUIDE.md)**

## Features

- 🤖 **AI-Powered Natural Language Processing** - Uses pattern matching and intent detection (no external API keys needed)
- 🔍 **Smart Invoice Search** - Search by vendor, invoice number, PO number, status, amount, date range, and more
- 💬 **Conversational Interface** - Simple web-based chat interface
- 🚀 **Fast & Efficient** - Direct SQL queries to Onbase database
- 🔒 **Secure** - Uses Windows Integrated Security for database connection

## Technology Stack

- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: SQL Server (Onbase)
- **ORM**: Dapper
- **Frontend**: HTML, CSS, JavaScript (Vanilla)
- **AI/NLP**: Custom pattern-based query parser (no external dependencies)

## Prerequisites

- .NET 8.0 SDK
- Access to Onbase SQL Server database
- Windows environment (for Integrated Security)

## Configuration

The connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OnBaseConnection": "Server=aazeus-obdmsq01;Database=Onbase;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

## Installation & Running

1. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Access the application**:
   - Web Interface: `http://localhost:5000` or `https://localhost:5001`
   - Swagger API Documentation: `http://localhost:5000/swagger`

## Usage Examples

The chatbot understands natural language queries such as:

### Real-World Examples

**Example 1: Vendor Search with Year Range**
```
fetch invoices for the vendor "SUPREME GRAPHICS" to see 2025/2026 invoices
```
*Returns all invoices for SUPREME GRAPHICS from January 2025 through December 2026*

**Example 2: Vendor Search with Relative Date**
```
fetch invoices for the vendor "GOOGLE" to see last one month invoices
```
*Returns all GOOGLE invoices from the past 30 days*

### Search by Vendor
- "Show me all invoices from vendor ABC Corp"
- "Find invoices from supplier XYZ Company"
- "Get invoices for SUPREME GRAPHICS"

### Search by Invoice Number
- "Find invoice number INV-12345"
- "Show me invoice #INV-2024-001"

### Search by Status
- "Get all pending invoices"
- "Show approved invoices"
- "Find paid invoices"

### Search by Amount
- "Show invoices greater than 5000"
- "Find invoices between 1000 and 10000"
- "Get invoices less than 500"

### Search by Date
- "Show invoices from this month"
- "Find invoices from last year"
- "Get invoices between 01/01/2024 and 03/31/2024"

### Search by PO Number
- "Find invoices with PO number PO-12345"
- "Show purchase order PO-2024-001"

### General Search
- "Search for invoices containing 'office supplies'"
- "Find all invoices"

## API Endpoints

### POST `/api/chatbot/query`
Process a natural language query to search for invoices.

**Request Body**:
```json
{
  "prompt": "Show me all invoices from vendor ABC Corp"
}
```

**Response**:
```json
{
  "answer": "I found 15 invoice(s) matching your query...",
  "invoices": [
    {
      "invoiceId": 1,
      "invoiceNumber": "INV-001",
      "vendorName": "ABC Corp",
      "amount": 5000.00,
      "invoiceDate": "2024-03-01T00:00:00",
      "dueDate": "2024-03-31T00:00:00",
      "status": "Pending",
      "poNumber": "PO-001",
      "description": "Office supplies",
      "documentType": "Invoice",
      "createdDate": "2024-03-01T10:00:00"
    }
  ],
  "query": "Show me all invoices from vendor ABC Corp",
  "resultCount": 15
}
```

### GET `/api/chatbot/health`
Health check endpoint.

## Architecture

### Components

1. **ChatbotController** - REST API endpoints for chat interactions
2. **ChatbotService** - Main service orchestrating query processing
3. **InvoiceQueryParser** - NLP component for intent detection and parameter extraction
4. **OnbaseRepository** - Data access layer for Onbase database
5. **Models** - Data transfer objects (Invoice, ChatRequest, ChatResponse)

### How It Works

1. User enters a natural language query
2. `InvoiceQueryParser` analyzes the query and detects intent (vendor search, date range, etc.)
3. Parser extracts relevant parameters (vendor name, dates, amounts, etc.)
4. `ChatbotService` routes the request to appropriate repository method
5. `OnbaseRepository` executes SQL query against Onbase database
6. Results are formatted and returned to the user with a natural language response

## Database Schema

The application expects the following Onbase database structure:

- **hsi.ItemData** - Main invoice data table
- **hsi.ItemTypeGroup** - Document type information

Key fields:
- ItemNum (Invoice ID)
- InvoiceNumber
- VendorName
- Amount
- InvoiceDate
- DueDate
- Status
- PONumber
- Description
- CreatedDate

## Security Considerations

- Uses Windows Integrated Security (no credentials in code)
- CORS enabled for development (configure appropriately for production)
- Input validation on all API endpoints
- SQL injection protection via parameterized queries (Dapper)

## Future Enhancements

- Add more sophisticated NLP using ML.NET
- Implement caching for frequently accessed data
- Add export functionality (PDF, Excel)
- Implement user authentication and authorization
- Add invoice analytics and reporting
- Support for multiple languages

## License

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**

This software is proprietary and confidential. Unauthorized copying, distribution, or use of this software, via any medium, is strictly prohibited.
