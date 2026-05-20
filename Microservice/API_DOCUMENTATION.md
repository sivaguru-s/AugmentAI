# Credit Shortage Validation API - Documentation

## Base URL
```
Development: https://localhost:5001
Production: https://credit-shortage-api.ashley.com
```

## Authentication
Currently using API key authentication for IWS integration. Internal endpoints are open for authorized network access.

---

## API Endpoints

### 1. Validate Single Shortage Item

Validates a single shortage item against all business rules.

**Endpoint:** `POST /api/v1/ShortageValidation/validate`

**Request Headers:**
```
Content-Type: application/json
```

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
  "locationCode": "WU",
  "orderNumber": 789012,
  "orderItemSeq": 1,
  "environment": "AFI"
}
```

**Field Descriptions:**
- `customerNumber` (required): 8-character customer number
- `shipToNumber` (required): 4-character ship-to location
- `invoiceNumber` (required): Invoice number where shortage occurred
- `itemNumber` (required): Item/SKU number (max 15 chars)
- `serialNumber` (required): Serial number from IWS (max 10 chars)
- `shortageQuantity` (required): Quantity of items short (1-9999999)
- `defectCode` (optional): Defect code (defaults to 'XP' if null)
- `locationCode` (optional): Warehouse code (defaults to 'WU' if null)
- `orderNumber` (optional): Original order number
- `orderItemSeq` (optional): Order item sequence
- `environment` (optional): Environment code (defaults to 'AFI')

**Success Response (200 OK):**
```json
{
  "customerNumber": "12345678",
  "shipToNumber": "0001",
  "invoiceNumber": 123456,
  "itemNumber": "ITEM001",
  "serialNumber": "SER001",
  "shortageQuantity": 1,
  "defectCode": "XP",
  "locationCode": "WU",
  "orderNumber": 789012,
  "orderItemSeq": 1,
  "isValid": true,
  "validationErrors": "",
  "orderedQuantity": 2,
  "alreadyCreditedQuantity": 0,
  "remainingCreditableQuantity": 2,
  "flags": {
    "itemExists": true,
    "customerSerialItemValid": true,
    "defectCodeValid": true,
    "locationCodeValid": true
  },
  "validatedAt": "2026-05-20T10:30:00Z",
  "serialFromIWS": false
}
```

**Error Response (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "customerNumber": ["The customerNumber field is required."]
  }
}
```

---

### 2. Validate Batch Shortage Items

Validates multiple shortage items in a single request.

**Endpoint:** `POST /api/v1/ShortageValidation/validate-batch`

**Request Body:**
```json
{
  "items": [
    {
      "customerNumber": "12345678",
      "shipToNumber": "0001",
      "invoiceNumber": 123456,
      "itemNumber": "ITEM001",
      "serialNumber": "SER001",
      "shortageQuantity": 1
    },
    {
      "customerNumber": "12345678",
      "shipToNumber": "0001",
      "invoiceNumber": 123457,
      "itemNumber": "ITEM002",
      "serialNumber": "SER002",
      "shortageQuantity": 2
    }
  ],
  "environment": "AFI"
}
```

**Success Response (200 OK):**
```json
{
  "results": [
    { /* ShortageValidationResponse for item 1 */ },
    { /* ShortageValidationResponse for item 2 */ }
  ],
  "totalItemsCount": 2,
  "validItemsCount": 1,
  "invalidItemsCount": 1,
  "allValid": false,
  "validatedAt": "2026-05-20T10:30:00Z"
}
```

---

### 3. Validate with IWS Integration

Validates a shortage item and automatically obtains the serial number from IWS.

**Endpoint:** `POST /api/v1/ShortageValidation/validate-with-iws`

**Request Body:**
```json
{
  "customerNumber": "12345678",
  "shipToNumber": "0001",
  "invoiceNumber": 123456,
  "itemNumber": "ITEM001",
  "shortageQuantity": 1,
  "defectCode": "XP",
  "locationCode": "WU"
}
```

**Note:** No `serialNumber` required - it will be fetched from IWS.

**Success Response:** Same as single validation, with `serialFromIWS: true`

---

### 4. Get Validation Statistics

Returns statistics about recent validations.

**Endpoint:** `GET /api/v1/ShortageValidation/statistics`

**Success Response (200 OK):**
```json
{
  "totalValidationsToday": 150,
  "successfulValidationsToday": 120,
  "failedValidationsToday": 30,
  "successRate": 80.0,
  "topFailureReasons": {
    "Item quantity exceeds ordered quantity": 12,
    "Defect code is not valid or active": 8,
    "Item already fully credited": 10
  }
}
```

---

### 5. Get Defect Codes

Returns all active defect codes.

**Endpoint:** `GET /api/v1/ReferenceData/defect-codes`

**Success Response (200 OK):**
```json
[
  {
    "code": "XP",
    "description": "Customer Shortage",
    "isActive": true,
    "isDefault": true
  },
  {
    "code": "DF",
    "description": "Defective",
    "isActive": true,
    "isDefault": false
  }
]
```

---

### 6. Get Location Codes

Returns all active warehouse/location codes.

**Endpoint:** `GET /api/v1/ReferenceData/location-codes`

**Success Response (200 OK):**
```json
[
  {
    "code": "WU",
    "description": "Warehouse Unit",
    "isActive": true,
    "isDefault": true
  }
]
```

---

### 7. Get Default Settings

Returns default configuration settings.

**Endpoint:** `GET /api/v1/ReferenceData/defaults`

**Success Response (200 OK):**
```json
{
  "defaultDefectCode": "XP",
  "defaultLocationCode": "WU",
  "defaultEnvironment": "AFI",
  "maxBatchSize": 500
}
```

---

### 8. Health Check

Returns health status of the service and database connections.

**Endpoint:** `GET /health`

**Success Response (200 OK):**
```json
{
  "status": "Healthy",
  "checks": {
    "ashley-database": "Healthy",
    "datawhse-database": "Healthy"
  }
}
```

---

## Validation Rules

The API validates shortage items against these rules:

1. **Item Existence**: Item must exist in the item master table
2. **Customer/Serial/Item Combo**: Valid combination in order history
3. **Order Quantity**: Shortage quantity must not exceed ordered quantity
4. **Credit History**: Item must not have been fully credited already
5. **Remaining Quantity**: Calculates available creditable quantity
6. **Defect Code**: Must be valid and active (defaults to 'XP')
7. **Location Code**: Must be valid and active (defaults to 'WU')

---

## Error Handling

### Standard Error Response Format

```json
{
  "title": "Error Title",
  "detail": "Detailed error message",
  "status": 500
}
```

### HTTP Status Codes

- `200 OK` - Request successful
- `400 Bad Request` - Invalid request data
- `500 Internal Server Error` - Server error

---

## Rate Limiting

Currently no rate limiting implemented. Consider implementing in production.

---

## Swagger UI

Interactive API documentation available at: `https://localhost:5001/swagger`
