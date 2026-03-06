# "No Invoices to send!" Error - Complete Documentation

## Overview

The error message **"No Invoices to send!"** appears when a user clicks the **"Make Payment"** button without having any valid invoices selected in the grid.

---

## Error Location

**File:** `main.aspx.vb`  
**Method:** `cmdPayment_Click`  
**Line:** 212 (original), now with enhanced logging

---

## Scenarios That Trigger This Error

### ✅ **Scenario 1: No Checkboxes Selected** (Most Common)

**What Happens:**
- User searches for invoices and grid displays results
- User clicks "Make Payment" without checking any invoice checkboxes
- `AreInvoicesSelected()` returns `False`

**Log Output:**
```
2025-03-03 14:30:15.123 | [INFO   ] | Thread:8  | Source:main.aspx | User:DOMAIN\jsmith | Make Payment button clicked
2025-03-03 14:30:15.145 | [DEBUG  ] | Thread:8  | Source:main.aspx | User:DOMAIN\jsmith | Grid has 25 rows
2025-03-03 14:30:15.167 | [WARNING] | Thread:8  | Source:main.aspx | User:DOMAIN\jsmith | No invoices selected - TotalRows=25, Enabled=25, Disabled=0, Checked=0
```

**Diagnosis:**
- `TotalRows > 0` - Grid has data
- `Enabled > 0` - Invoices are available for selection
- `Checked = 0` - User didn't select any

**Solution:**
- User needs to check at least one invoice checkbox before clicking "Make Payment"

---

### ✅ **Scenario 2: All Invoices Already in EPay Status**

**What Happens:**
- All invoices in the grid have an EPay status ("Sent", "Verifying", "Confirmed")
- Checkboxes are automatically disabled for invoices with status
- User cannot select any invoices

**Code That Disables Checkboxes:**
```vb
'If Status has a value don't allow user to select this invoice
If sEpayStatus = String.Empty Then
    DirectCast(e.Row.FindControl(GRID_SELECT_ID), CheckBox).Enabled = True
Else
    DirectCast(e.Row.FindControl(GRID_SELECT_ID), CheckBox).Checked = False
    DirectCast(e.Row.FindControl(GRID_SELECT_ID), CheckBox).Enabled = False
```

**Log Output:**
```
2025-03-03 14:35:22.456 | [INFO   ] | Thread:12 | Source:main.aspx | User:DOMAIN\jdoe | Make Payment button clicked
2025-03-03 14:35:22.478 | [DEBUG  ] | Thread:12 | Source:main.aspx | User:DOMAIN\jdoe | Grid has 15 rows
2025-03-03 14:35:22.490 | [WARNING] | Thread:12 | Source:main.aspx | User:DOMAIN\jdoe | No invoices selected - TotalRows=15, Enabled=0, Disabled=15, Checked=0, DisabledStatuses=[Sent, Sent, Verifying, Confirmed, Sent]
```

**Diagnosis:**
- `TotalRows > 0` - Grid has data
- `Enabled = 0` - No invoices available for selection
- `Disabled > 0` - All invoices already in payment process
- `DisabledStatuses` shows which statuses are present

**Solution:**
- These invoices are already being processed
- User should wait for payment confirmation or check payment history
- Search for different invoices if needed

---

### ✅ **Scenario 3: Empty Grid (No Search Results)**

**What Happens:**
- User's search criteria returns no invoices
- Grid is empty (`gvInvoices.Rows.Count = 0`)
- User clicks "Make Payment" anyway

**Log Output:**
```
2025-03-03 14:40:10.789 | [INFO   ] | Thread:15 | Source:main.aspx | User:DOMAIN\jsmith | Make Payment button clicked
2025-03-03 14:40:10.801 | [DEBUG  ] | Thread:15 | Source:main.aspx | User:DOMAIN\jsmith | Grid has 0 rows
2025-03-03 14:40:10.812 | [WARNING] | Thread:15 | Source:main.aspx | User:DOMAIN\jsmith | No invoices selected - TotalRows=0, Enabled=0, Disabled=0, Checked=0
```

**Diagnosis:**
- `TotalRows = 0` - No data in grid
- All other counts are 0

**Solution:**
- Adjust search criteria (date range, invoice number, PO number, etc.)
- Verify customer has open invoices in the system
- Check if invoices are already paid

---

### ✅ **Scenario 4: User Unchecked All Invoices**

**What Happens:**
- User initially checks some invoice checkboxes
- User then unchecks all of them
- User clicks "Make Payment"

**Log Output:**
```
2025-03-03 14:45:30.234 | [INFO   ] | Thread:18 | Source:main.aspx | User:DOMAIN\jdoe | Make Payment button clicked
2025-03-03 14:45:30.256 | [DEBUG  ] | Thread:18 | Source:main.aspx | User:DOMAIN\jdoe | Grid has 30 rows
2025-03-03 14:45:30.278 | [WARNING] | Thread:18 | Source:main.aspx | User:DOMAIN\jdoe | No invoices selected - TotalRows=30, Enabled=30, Disabled=0, Checked=0
```

**Diagnosis:**
- Same as Scenario 1
- User changed their mind

**Solution:**
- Select invoices again before clicking "Make Payment"

---

### ✅ **Scenario 5: Mixed Status (Some Enabled, Some Disabled)**

**What Happens:**
- Grid has mix of available invoices and invoices with EPay status
- User doesn't select any of the available invoices
- User clicks "Make Payment"

**Log Output:**
```
2025-03-03 14:50:15.567 | [INFO   ] | Thread:22 | Source:main.aspx | User:DOMAIN\jsmith | Make Payment button clicked
2025-03-03 14:50:15.589 | [DEBUG  ] | Thread:22 | Source:main.aspx | User:DOMAIN\jsmith | Grid has 20 rows
2025-03-03 14:50:15.601 | [WARNING] | Thread:22 | Source:main.aspx | User:DOMAIN\jsmith | No invoices selected - TotalRows=20, Enabled=12, Disabled=8, Checked=0, DisabledStatuses=[Sent, Verifying, Sent]
```

**Diagnosis:**
- `TotalRows = 20` - Grid has data
- `Enabled = 12` - 12 invoices available for selection
- `Disabled = 8` - 8 invoices already in payment process
- `Checked = 0` - User didn't select any of the 12 available

**Solution:**
- User needs to check at least one of the 12 enabled invoices

---

## Enhanced Code Implementation

### **cmdPayment_Click Method (with Logging)**

```vb
Protected Sub cmdPayment_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdPayment.Click
    Dim totalAmt As Decimal
    Dim refNo As Integer = 0
    
    Try
        Logger.Info("Make Payment button clicked", "main.aspx")
        
        ' Log grid state
        Dim gridRowCount As Integer = Me.gvInvoices.Rows.Count
        Logger.Debug(String.Format("Grid has {0} rows", gridRowCount), "main.aspx")
        
        If Me.AreInvoicesSelected() Then
            Logger.Info("Invoices selected - proceeding with payment", "main.aspx")
            
            totalAmt = WritePaymentFiles(refNo)
            
            If totalAmt = 0 Then
                Logger.Warning(String.Format("Payment failed - Invoices already in EPay file. RefNo={0}", refNo), "main.aspx")
                lblErrorMsg.Text = "Invoices are already in EPay File!"
                
            ElseIf totalAmt < 0 Then
                Logger.Error(String.Format("Payment failed - Negative amount: {0:C}. RefNo={1}", totalAmt, refNo), "main.aspx")
                Common.DeleteEpayRecords(Me.SessionData("CUSTOMERNUMBER"), refNo, Me.SessionData("LOGON_USER"))
                lblErrorMsg.Text = "Amount is less then 0!"
                
            Else
                Logger.Info(String.Format("Payment submitted successfully - RefNo={0}, Amount={1:C}", refNo, totalAmt), "main.aspx")
                Response.Redirect(String.Format("Confirmation.aspx?RefNo={0}", refNo.ToString), False)
            End If
            
        Else
            ' Log detailed information about why no invoices were selected
            Dim enabledCount As Integer = 0
            Dim disabledCount As Integer = 0
            Dim checkedCount As Integer = 0
            Dim statusSummary As New System.Text.StringBuilder()
            
            For Each row As GridViewRow In Me.gvInvoices.Rows
                Dim chk As CheckBox = DirectCast(row.FindControl(GRID_SELECT_ID), CheckBox)
                If chk.Enabled Then
                    enabledCount += 1
                    If chk.Checked Then
                        checkedCount += 1
                    End If
                Else
                    disabledCount += 1
                    ' Get the status of disabled invoices
                    Dim statusLink As HyperLink = DirectCast(row.FindControl(GRID_STATUS_ID), HyperLink)
                    If statusLink IsNot Nothing AndAlso Not String.IsNullOrEmpty(statusLink.Text) Then
                        statusSummary.Append(statusLink.Text & ", ")
                    End If
                End If
            Next
            
            ' Build detailed warning message
            Dim logMessage As String = String.Format("No invoices selected - TotalRows={0}, Enabled={1}, Disabled={2}, Checked={3}", _
                                                    gridRowCount, enabledCount, disabledCount, checkedCount)
            
            If disabledCount > 0 AndAlso statusSummary.Length > 0 Then
                logMessage &= String.Format(", DisabledStatuses=[{0}]", statusSummary.ToString().TrimEnd(","c, " "c))
            End If
            
            Logger.Warning(logMessage, "main.aspx")
            
            lblErrorMsg.Text = "No Invoices to send!"
        End If
        
    Catch ex As Exception
        Logger.Error(String.Format("Payment processing error - RefNo={0}", refNo), ex, "main.aspx")
        
        'Remove Epay records if there is a failure
        If refNo > 0 Then
            Common.DeleteEpayRecords(Me.SessionData("CUSTOMERNUMBER"), refNo, Me.SessionData("LOGON_USER"))
            Logger.Info(String.Format("Cleaned up EPay records for RefNo={0}", refNo), "main.aspx")
        End If
        
        Me.RedirectToErrorProcessing(ex)
        
    Finally
        totalAmt = Nothing
        refNo = Nothing
    End Try
End Sub
```

---

## Diagnostic Summary Table

| Scenario | TotalRows | Enabled | Disabled | Checked | Root Cause |
|----------|-----------|---------|----------|---------|------------|
| No selection | > 0 | > 0 | Any | 0 | User didn't check boxes |
| All in EPay | > 0 | 0 | > 0 | 0 | All invoices have status |
| Empty grid | 0 | 0 | 0 | 0 | No search results |
| Unchecked all | > 0 | > 0 | Any | 0 | User changed mind |
| Mixed status | > 0 | > 0 | > 0 | 0 | User didn't select available ones |

---

## Copyright

Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.  
This software is proprietary and confidential.

