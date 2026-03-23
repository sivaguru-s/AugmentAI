# Vendor Field Discovery - Analysis Results

**Date:** March 12, 2026  
**Analyst:** Augment AI  
**Database:** aazeus-obdmsq01 / Onbase

---

## 🎯 **Discovery Summary**

### **VENDOR FIELD FOUND!**

✅ **Table:** `hsi.keytable105`  
✅ **Column:** `keyvaluechar`  
✅ **Link Table:** `hsi.keyxitem105`  
✅ **Contains:** Vendor names including "CDW COMPUTER CENTERS INC"

---

## 📊 **Evidence**

### **Sample Vendor Names Found in keytable105:**

| keywordnum | Vendor Name |
|------------|-------------|
| 1486675 | CDW COMPUTER CENTERS,INC |
| 1372678 | CDW COMPUTER CENTERS, INC |
| 1343782 | CDW COMPUTER CENTERS,INC. |
| 1333118 | CDW COMPUTER CENTERS, INC. |
| 1179944 | CDW COMPUTER CENTERS INC. |
| 1031026 | CDW COMPUTER CENTERS |
| 964951 | CDW COMPUTER CENTERS INC |
| 1581262 | CDW |
| 1776975 | CDW COMP CENT. INC |
| 1332404 | CDW COMPUTER |

### **Additional Vendor Variations:**
- CDW COMPUTER CENTER
- CDW COMPUTER CENTER INC
- CDW COMPUTER CENTER, INC.
- CDW COMPUTER CENT. INC.
- CDW COMPUTER CENT.INC
- CDW COMPUTER CENETERS INC (typo in data)

---

## 🔗 **Table Structure**

### **keytable105 Schema:**
```sql
CREATE TABLE hsi.keytable105 (
    keywordnum INT PRIMARY KEY,
    keyvaluechar CHAR(100)  -- Vendor names stored here
)
```

### **keyxitem105 Schema (Link Table):**
```sql
CREATE TABLE hsi.keyxitem105 (
    itemnum INT,        -- Links to hsi.itemdata.itemnum
    keywordnum INT,     -- Links to hsi.keytable105.keywordnum
    keysetnum INT
)
```

---

## 🔧 **How to Join Vendor Data**

### **SQL Query Pattern:**

```sql
SELECT 
    i.itemnum,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    kt105.keyvaluechar as VendorName
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
    ON i.itemnum = ki106.itemnum
LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) 
    ON i.itemnum = kx105.itemnum
LEFT JOIN hsi.keytable105 kt105 WITH (NOLOCK) 
    ON kx105.keywordnum = kt105.keywordnum
WHERE i.itemtypenum = 102
```

---

## ⚠️ **Important Notes**

### **1. Data Availability**
- Vendor data exists in `keytable105`
- However, it may not be linked to all invoices (itemtypenum = 102)
- Some vendor records are linked to itemtype 1353 and 263 instead

### **2. Performance Considerations**
- Queries joining keytable105 to itemdata can be slow
- Use `WITH (NOLOCK)` hint to prevent locking
- Consider adding indexes if performance is critical

### **3. Data Quality**
- Vendor names have inconsistent formatting:
  - "CDW COMPUTER CENTERS INC"
  - "CDW COMPUTER CENTERS, INC."
  - "CDW COMPUTER CENTERS,INC"
- May need data normalization for accurate searches

---

## 📝 **Next Steps to Implement**

### **Step 1: Update Invoice Model**

Add VendorName property to `Models/Invoice.cs`:

```csharp
public class Invoice
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string PONumber { get; set; }
    public string VendorName { get; set; }  // ← ADD THIS
    public string Description { get; set; }
}
```

### **Step 2: Update Repository Queries**

Modify all queries in `Data/OnbaseRepository.cs` to include vendor JOIN:

```csharp
var query = @"
    SELECT 
        i.itemnum as InvoiceId,
        CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
        ki112.keyvaluedate as InvoiceDate,
        kt104.keyvaluechar as PONumber,
        RTRIM(kt105.keyvaluechar) as VendorName,  -- ← ADD THIS
        i.itemtypegroupnum as Description
    FROM hsi.itemdata i WITH (NOLOCK)
    LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
        ON i.itemnum = ki106.itemnum
    LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) 
        ON i.itemnum = ki112.itemnum
    LEFT JOIN hsi.keytable104 kt104 WITH (NOLOCK) 
        ON i.itemnum = kt104.itemnum
    LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK)   -- ← ADD THIS
        ON i.itemnum = kx105.itemnum
    LEFT JOIN hsi.keytable105 kt105 WITH (NOLOCK)   -- ← ADD THIS
        ON kx105.keywordnum = kt105.keywordnum
    WHERE i.itemtypenum = 102";
```

### **Step 3: Update Frontend**

Modify `wwwroot/index.html` to display vendor name in invoice cards.

### **Step 4: Add Vendor Search Intent**

Update `Services/InvoiceQueryParser.cs` to support vendor searches:

```csharp
if (ContainsAny(prompt, new[] { "vendor", "supplier", "company" }))
{
    intent.IntentType = "SearchByVendor";
    intent.Parameters["vendorName"] = ExtractVendorName(prompt);
}
```

### **Step 5: Add Repository Method**

Add new method to `Data/OnbaseRepository.cs`:

```csharp
public async Task<List<Invoice>> GetInvoicesByVendorAsync(string vendorName)
{
    // Implementation with vendor search
}
```

---

## 🧪 **Testing Queries**

### **Test 1: Find all CDW invoices**
```sql
SELECT TOP 10
    i.itemnum,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    RTRIM(kt105.keyvaluechar) as VendorName
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) 
    ON i.itemnum = kx105.itemnum
INNER JOIN hsi.keytable105 kt105 WITH (NOLOCK) 
    ON kx105.keywordnum = kt105.keywordnum
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
    ON i.itemnum = ki106.itemnum
WHERE kt105.keyvaluechar LIKE '%CDW%'
ORDER BY i.itemnum DESC
```

### **Test 2: Check vendor data coverage**
```sql
SELECT 
    COUNT(DISTINCT i.itemnum) as TotalInvoices,
    COUNT(DISTINCT kx105.itemnum) as InvoicesWithVendor,
    (COUNT(DISTINCT kx105.itemnum) * 100.0 / COUNT(DISTINCT i.itemnum)) as CoveragePercent
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) 
    ON i.itemnum = kx105.itemnum
WHERE i.itemtypenum = 102
```

---

## ✅ **Conclusion**

**Vendor field has been successfully identified:**
- ✅ Table: `hsi.keytable105`
- ✅ Column: `keyvaluechar`
- ✅ Link: `hsi.keyxitem105`
- ✅ Contains vendor names like "CDW COMPUTER CENTERS INC"

**Ready to implement vendor search functionality!**


