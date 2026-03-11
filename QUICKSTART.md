# Quick Start Guide - Onbase Invoice Chatbot

Get your chatbot up and running in 5 minutes!

## Prerequisites Check

Before you begin, ensure you have:
- ✅ .NET 8.0 SDK installed
- ✅ Access to Onbase SQL Server (aazeus-obdmsq01)
- ✅ Windows environment (for Integrated Security)

## Step 1: Verify .NET Installation

```bash
dotnet --version
```

You should see version 8.0 or higher.

## Step 2: Restore Dependencies

```bash
dotnet restore
```

This will download all required NuGet packages:
- Microsoft.Data.SqlClient
- Dapper
- Microsoft.ML
- Swashbuckle.AspNetCore

## Step 3: Build the Project

```bash
dotnet build
```

Ensure there are no build errors.

## Step 4: Run the Application

```bash
dotnet run
```

You should see output like:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

## Step 5: Access the Chatbot

Open your web browser and navigate to:

**Web Interface**: http://localhost:5000

You should see a beautiful chat interface with example queries.

**API Documentation**: http://localhost:5000/swagger

Interactive API documentation powered by Swagger.

## Step 6: Try Your First Query

In the chat interface, try one of these queries:

1. **"Show me all invoices from this month"**
2. **"Find pending invoices"**
3. **"Get invoices greater than 5000"**

## Troubleshooting

### Database Connection Issues

If you get a database connection error:

1. Verify you're on the correct network
2. Check if you have access to the Onbase database
3. Verify the connection string in `appsettings.json`

### Port Already in Use

If port 5000 is already in use, you can specify a different port:

```bash
dotnet run --urls "http://localhost:5050"
```

### Build Errors

If you encounter build errors:

1. Clean the solution:
   ```bash
   dotnet clean
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Rebuild:
   ```bash
   dotnet build
   ```

## Testing the API with PowerShell

```powershell
$body = @{
    prompt = "Show me all pending invoices"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/chatbot/query" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

## Testing the API with cURL

```bash
curl -X POST http://localhost:5000/api/chatbot/query \
  -H "Content-Type: application/json" \
  -d "{\"prompt\": \"Show me all pending invoices\"}"
```

## Understanding the Response

The chatbot returns a JSON response with:

- **answer**: Natural language summary
- **invoices**: Array of matching invoices
- **query**: Your original query
- **resultCount**: Number of results found

Example:
```json
{
  "answer": "I found 15 invoice(s) matching your query...",
  "invoices": [...],
  "query": "Show me all pending invoices",
  "resultCount": 15
}
```

## Next Steps

1. **Explore More Queries**: Check `TestQueries.md` for more examples
2. **Customize**: Modify the query parser to understand your specific terminology
3. **Extend**: Add more search capabilities in the repository
4. **Deploy**: Deploy to IIS or Azure for production use

## Key Features

✨ **No API Keys Required** - Uses local pattern matching for NLP
🔍 **Smart Search** - Understands natural language queries
⚡ **Fast** - Direct SQL queries to Onbase
🎨 **Beautiful UI** - Modern chat interface
📚 **Well Documented** - Swagger API docs included

## Support

For issues or questions:
1. Check the README.md for detailed documentation
2. Review TestQueries.md for query examples
3. Check the Swagger documentation at /swagger

## Development Mode

The application runs in development mode by default, which includes:
- Detailed error messages
- Swagger UI enabled
- Debug logging
- CORS enabled for all origins

For production deployment, set the environment variable:
```bash
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```

---

**Happy Chatting! 🤖**

