# Credit Shortage Validation Flow

## Process Flow Diagram

```mermaid
flowchart TD
    A[Start: Receive Shortage Items] --> B[Apply Default Codes]
    B --> C{Defect Code NULL?}
    C -->|Yes| D[Set DefectCode = 'XP']
    C -->|No| E[Keep Provided Code]
    D --> F{Location Code NULL?}
    E --> F
    F -->|Yes| G[Set LocationCode = 'WU']
    F -->|No| H[Keep Provided Code]
    G --> I[VALIDATION 1: Item Exists?]
    H --> I
    
    I -->|No| J[Error: Item Invalid]
    I -->|Yes| K[VALIDATION 2: Check Customer/Serial/Item]
    
    K --> L{Valid in Invoice History?}
    L -->|No - Check Archive| M{Valid in Archive?}
    M -->|No| N[Error: Invalid Combination]
    M -->|Yes| O[VALIDATION 3: Get Order Qty]
    L -->|Yes| O
    
    O --> P{Order Qty Found?}
    P -->|No| Q[Error: Cannot Determine Qty]
    P -->|Yes| R[VALIDATION 4: Check Existing Credits]
    
    R --> S[Calculate Already Credited Qty]
    S --> T[Calculate Remaining = Ordered - Credited]
    T --> U{Shortage Qty <= Remaining?}
    
    U -->|No| V[Error: Qty Exceeds Available]
    U -->|Yes| W[VALIDATION 5: Check Defect Code]
    
    W --> X{Defect Code Active?}
    X -->|No| Y[Error: Invalid Defect Code]
    X -->|Yes| Z[VALIDATION 6: Check Location]
    
    Z --> AA{Location Active?}
    AA -->|No| AB[Error: Invalid Location]
    AA -->|Yes| AC{All Validations Pass?}
    
    J --> AC
    N --> AC
    Q --> AC
    V --> AC
    Y --> AC
    AB --> AC
    
    AC -->|Yes| AD[IsValid = 1, SUCCESS]
    AC -->|No| AE[IsValid = 0, Return Errors]
    
    AD --> AF[Return Results]
    AE --> AF
    
    AF --> AG[End]
    
    style AD fill:#90EE90
    style AE fill:#FFB6C1
    style J fill:#FFB6C1
    style N fill:#FFB6C1
    style Q fill:#FFB6C1
    style V fill:#FFB6C1
    style Y fill:#FFB6C1
    style AB fill:#FFB6C1
```

## Validation Sequence

### Step-by-Step Breakdown

#### Phase 1: Initialization
1. **Input Receipt**: Receive shortage items in table-valued parameter
2. **Default Application**: Apply default defect code (XP) and location code (WU) where NULL

#### Phase 2: Item Validation
3. **Item Master Check**: 
   - Query `tblItemMaster`
   - Verify item exists
   - **Fail Fast**: If item doesn't exist, mark as invalid

#### Phase 3: Combination Validation
4. **Invoice History Check**:
   - First check `Datawhse.dbo.tblInvoiceDetail`
   - If not found, check `Archive.dbo.tblInvoiceDetail`
   - Validate Customer + Invoice + Serial + Item combination
   - **Fail Fast**: If combination invalid, mark as invalid

#### Phase 4: Quantity Analysis
5. **Order Quantity Retrieval**:
   - Get original ordered quantity from invoice
   - Check both current and archive tables
   - **Fail Fast**: If quantity cannot be determined, mark as invalid

6. **Credit History Check**:
   - Query `tblRetAllowHeader` and `tblRetAllowDetail`
   - Calculate sum of already credited quantities
   - Only count approved credits (`rahAprvDny = 'A'`)
   - Account for allowance percentages

7. **Remaining Calculation**:
   ```
   RemainingCreditableQuantity = OrderedQuantity - AlreadyCreditedQuantity
   ```

8. **Quantity Validation**:
   - Ensure: `ShortageQuantity <= RemainingCreditableQuantity`
   - **Fail Fast**: If exceeded, mark as invalid

#### Phase 5: Code Validation
9. **Defect Code Check**:
   - Query `tblDefectCodes`
   - Verify code exists and is active (`defActive = 'Y'`)
   - **Fail Fast**: If invalid, mark as invalid

10. **Location Code Check**:
    - Query `tblWarehouse`
    - Verify warehouse exists and is active (`whsActive = 'Y'`)
    - **Fail Fast**: If invalid, mark as invalid

#### Phase 6: Results
11. **Final Status**: 
    - `IsValid = 1` if ALL validations pass
    - `IsValid = 0` if ANY validation fails
12. **Error Aggregation**: Concatenate all error messages
13. **Return Results**: Ordered with failures first

---

## Database Interaction Map

```
Input: @ShortageItems
    ↓
┌─────────────────────────────────────────┐
│  #ValidationResults (Temp Table)        │
│  - Holds validation state               │
│  - Updated by each validation step      │
└─────────────────────────────────────────┘
    ↓
┌─────────────────────────────────────────┐
│  VALIDATION 1: tblItemMaster            │
│  - Check item exists                    │
│  - Update: ItemExists flag              │
└─────────────────────────────────────────┘
    ↓
┌─────────────────────────────────────────┐
│  VALIDATION 2: tblInvoiceDetail         │
│  - Current: Datawhse.dbo                │
│  - Archive: Archive.dbo                 │
│  - Update: CustomerSerialItemValid flag │
└─────────────────────────────────────────┘
    ↓
┌─────────────────────────────────────────┐
│  VALIDATION 3: tblInvoiceDetail         │
│  - Get original order quantity          │
│  - Update: OrderedQuantity              │
└─────────────────────────────────────────┘
    ↓
┌─────────────────────────────────────────┐
│  VALIDATION 4: tblRetAllowHeader/Detail │
│  - Calculate credited quantity          │
│  - Update: AlreadyCreditedQuantity      │
│  - Update: RemainingCreditableQuantity  │
└─────────────────────────────────────────┘
    ↓
┌─────────────────────────────────────────┐
│  VALIDATION 5: tblDefectCodes           │
│  - Check defect code active             │
│  - Update: DefectCodeValid flag         │
└─────────────────────────────────────────┘
    ↓
┌─────────────────────────────────────────┐
│  VALIDATION 6: tblWarehouse             │
│  - Check warehouse active               │
│  - Update: LocationCodeValid flag       │
└─────────────────────────────────────────┘
    ↓
Output: Validation Results
```

---

## Error Handling Strategy

### Fail-Fast Approach
- Each validation updates `IsValid` flag
- Errors are accumulated in `ValidationErrors` column
- All validations run (no short-circuit)
- Allows seeing ALL issues in one call

### Error Message Format
```
ERROR: Item [ITEMX] does not exist or is invalid. 
ERROR: Shortage quantity (10) exceeds remaining creditable quantity (5). Already credited: 5. 
ERROR: Defect code [XX] is invalid or inactive.
```

### Success Message Format
```
SUCCESS: All validations passed.
```

---

## Integration Points

### Upstream: IWS Integration
```
IWS API Call
    ↓
Extract Serial#
    ↓
Build Shortage Item
    ↓
Call Validation SP
```

### Downstream: Credit Submission
```
Validation SP Returns Results
    ↓
Filter: IsValid = 1
    ↓
Call usp_CEImportSubmittedCredits
    ↓
Credit Created
```

---

## Performance Optimization

### Index Usage
1. **tblItemMaster**: PK lookup on itmItemnumber
2. **tblInvoiceDetail**: Composite index on (Customer, Invoice, Serial, Item)
3. **tblRetAllowHeader**: Index on (Customer, ApprovalStatus, EnterDate, EnterTime)
4. **tblRetAllowDetail**: Index on (Invoice, Order, Item, EnterDate, EnterTime)

### NOLOCK Strategy
- All SELECT queries use `WITH (NOLOCK)`
- Prevents blocking on high-transaction tables
- Acceptable for validation scenario (eventual consistency)

### Archive Fallback
- Only queries Archive if not found in primary
- Uses `LEFT JOIN` with `NULL` check pattern
- Minimizes unnecessary Archive queries

