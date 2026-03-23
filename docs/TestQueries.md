# Test Queries for Onbase Invoice Chatbot

This document contains sample queries you can use to test the chatbot functionality.

## Vendor-Based Queries

```
Show me all invoices from vendor ABC Corp
Find invoices from supplier Microsoft
Get all invoices for vendor name "Ashley Furniture"
List invoices from company XYZ
```

## Invoice Number Queries

```
Find invoice number INV-12345
Show me invoice #2024-001
Get invoice INV-2024-03-001
Search for invoice number 12345
```

## Status-Based Queries

```
Show all pending invoices
Find approved invoices
Get paid invoices
List rejected invoices
Show me all invoices with status processing
```

## Amount-Based Queries

```
Show invoices greater than 5000
Find invoices between 1000 and 10000
Get invoices less than 500
Show me invoices above 25000
Find invoices under 100
List invoices with amount more than 1000
```

## Date-Based Queries

```
Show invoices from this month
Find invoices from last month
Get invoices from this year
Show invoices from last year
Find invoices from today
Get invoices from yesterday
Show invoices from this week
Find invoices between 01/01/2024 and 03/31/2024
Get invoices from 2024-01-01 to 2024-03-31
```

## PO Number Queries

```
Find invoices with PO number PO-12345
Show purchase order PO-2024-001
Get invoices for PO# 12345
```

## General Search Queries

```
Search for office supplies
Find all invoices
Show me recent invoices
List all invoices
Get invoices containing "furniture"
Search for "computer equipment"
```

## Complex Queries

```
Show me all pending invoices from vendor ABC Corp
Find approved invoices from this month
Get invoices greater than 5000 from last year
List all paid invoices from supplier XYZ
```

## Testing the API with cURL

### Basic Query
```bash
curl -X POST http://localhost:5000/api/chatbot/query \
  -H "Content-Type: application/json" \
  -d "{\"prompt\": \"Show me all invoices from vendor ABC Corp\"}"
```

### Status Query
```bash
curl -X POST http://localhost:5000/api/chatbot/query \
  -H "Content-Type: application/json" \
  -d "{\"prompt\": \"Find all pending invoices\"}"
```

### Amount Query
```bash
curl -X POST http://localhost:5000/api/chatbot/query \
  -H "Content-Type: application/json" \
  -d "{\"prompt\": \"Show invoices greater than 5000\"}"
```

### Date Query
```bash
curl -X POST http://localhost:5000/api/chatbot/query \
  -H "Content-Type: application/json" \
  -d "{\"prompt\": \"Get invoices from this month\"}"
```

## Testing with PowerShell

```powershell
$body = @{
    prompt = "Show me all invoices from vendor ABC Corp"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/chatbot/query" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

## Expected Response Format

```json
{
  "answer": "I found 15 invoice(s) matching your query:\n\nTotal amount: $75,000.00\nVendor: ABC Corp\n\nHere are the details:",
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

## Notes

- All queries are case-insensitive
- The chatbot uses pattern matching to understand intent
- Partial matches are supported (e.g., "ABC" will match "ABC Corp")
- Date queries support both relative ("this month") and absolute ("01/01/2024") formats
- Amount queries support comparison operators (greater than, less than, between)

