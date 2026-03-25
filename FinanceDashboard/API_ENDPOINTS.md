# Finance Dashboard API Endpoints

## Dashboard Endpoints

### GET /Dashboard/Index
Main dashboard view with all financial data.

**Query Parameters:**
- `unitCode` (optional): Filter by unit code
- `natureId` (optional): Filter by nature ID

**Returns:** HTML view with complete dashboard

---

### GET /Dashboard/GetUnits
Get all available units.

**Returns:** JSON array of units

---

### GET /Dashboard/GetNatures
Get natures for a specific unit.

**Query Parameters:**
- `unitCode` (required): Unit code

**Returns:** JSON array of natures

---

### GET /Dashboard/ExportData
Export dashboard data in JSON format.

**Query Parameters:**
- `unitCode` (optional): Filter by unit code
- `format` (optional): Export format (default: "json")

**Returns:** JSON object with:
```json
{
  "UnitBudgets": [...],
  "AFEData": [...],
  "JiraCostings": [...],
  "PlanfulDetails": [...],
  "AFEBudgets": [...],
  "ExportDate": "2026-03-24T..."
}
```

---

## Planful Endpoints

### GET /Dashboard/GetPlanfulSummary
Get Planful summary data (nature-wise spend).

**Query Parameters:**
- `year` (optional): Filter by year
- `month` (optional): Filter by month (1-12)
- `nature` (optional): Filter by nature code

**Returns:** JSON array of PlanfulSummary objects
```json
[
  {
    "Nature": "5010",
    "NatureDescription": "Salaries",
    "Year": 2026,
    "Month": 3,
    "TotalBudget": 100000.00,
    "TotalActual": 85000.00,
    "TotalForecast": 95000.00
  }
]
```

---

### GET /Dashboard/GetPlanfulDetails
Get Planful detail data (unit-wise costs).

**Query Parameters:**
- `unitCode` (optional): Filter by unit code
- `natureId` (optional): Filter by nature ID
- `year` (optional): Filter by year
- `month` (optional): Filter by month (1-12)

**Returns:** JSON array of PlanfulDetail objects
```json
[
  {
    "Unit": "1000",
    "UnitDescription": "IT Department",
    "Nature": "5010",
    "NatureDescription": "Salaries",
    "Year": 2026,
    "Month": 3,
    "SpendType": "Salary",
    "BudgetAmount": 50000.00,
    "ActualAmount": 45000.00,
    "ForecastAmount": 48000.00
  }
]
```

---

## AFE Budget Endpoints

### GET /Dashboard/GetAFEBudgets
Get AFE budget data.

**Query Parameters:**
- `project` (optional): Filter by project name
- `afeNumber` (optional): Filter by AFE number
- `unit` (optional): Filter by unit code

**Returns:** JSON array of AFEBudget objects
```json
[
  {
    "Project": "Digital Transformation",
    "AFENumber": "AFE-2026-001",
    "ProjectDescription": "Cloud Migration Project",
    "Owner": "John Doe",
    "ApprovedAmount": 500000.00,
    "CommittedAmount": 350000.00,
    "Forecast": 480000.00,
    "PercentSpent": 70.0,
    "RemainingAmount": 150000.00,
    "Cap": 300000.00,
    "Opex": 200000.00,
    "Unit": "1000",
    "EstimatedCompletion": "2026-12-31"
  }
]
```

---

### GET /Dashboard/GetBusinessVerticals
Get all business verticals.

**Returns:** JSON array of BusinessVertical objects
```json
[
  {
    "Id": 1,
    "VerticalCode": "IT",
    "VerticalName": "Information Technology",
    "Description": "IT Department",
    "IsActive": true
  }
]
```

---

### POST /Dashboard/ImportAFEBudgets
Import AFE budgets from Excel data.

**Request Body:** JSON array of AFEBudget objects
```json
[
  {
    "Project": "Digital Transformation",
    "AFENumber": "AFE-2026-001",
    "ProjectDescription": "Cloud Migration Project",
    "Owner": "John Doe",
    "ApprovedAmount": 500000.00,
    "CommittedAmount": 350000.00,
    "Forecast": 480000.00,
    "PercentSpent": 70.0,
    "Cap": 300000.00,
    "Opex": 200000.00,
    "Unit": "1000"
  }
]
```

**Returns:** JSON object
```json
{
  "success": true,
  "count": 1,
  "message": "Imported 1 AFE budgets successfully"
}
```

---

### POST /Dashboard/LinkAFEToUnitNature
Link AFE budget to unit and nature.

**Request Body:** AFEUnitNatureMapping object
```json
{
  "AFENumber": "AFE-2026-001",
  "Project": "Digital Transformation",
  "Unit": "1000",
  "Nature": "5010",
  "AllocationPercentage": 100.0,
  "BusinessVertical": "IT"
}
```

**Returns:** JSON object
```json
{
  "success": true,
  "message": "Linked successfully"
}
```

---

## Notes

1. All monetary values are in decimal format with 2 decimal places
2. Dates are in ISO 8601 format (yyyy-MM-dd or yyyy-MM-ddTHH:mm:ss)
3. All endpoints return appropriate HTTP status codes:
   - 200 OK: Successful request
   - 400 Bad Request: Invalid parameters or error
   - 500 Internal Server Error: Server-side error

4. The AFE Budget service currently uses in-memory storage for demo purposes
5. In production, implement proper authentication and authorization for POST endpoints

