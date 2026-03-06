# EPay Logging Integration Examples

This document provides practical examples of integrating the Logger class into existing EPay pages.

---

## Example 1: main.aspx.vb - Invoice Search Page

### Add Logger Import and Initialization

```vb
Imports System.Xml

Partial Public Class main
    Inherits EpayBasePage

    ' Initialize logger
    Private Shared ReadOnly log As Logger = New Logger()
```

### Page_Load Event with Logging

```vb
Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    Try
        Logger.Info("Page_Load started", "main.aspx")
        
        If Not IsPostBack Then
            Logger.Debug("First page load - initializing default values", "main.aspx")
            
            ' Initialize date fields
            Me.txtFromDate.Text = DateTime.Now.AddDays(-30).ToString("MM/dd/yyyy")
            Me.txtToDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
            
            Logger.Info("Default date range set: " & Me.txtFromDate.Text & " to " & Me.txtToDate.Text, "main.aspx")
        End If
        
        ' Check for customer selection
        If String.IsNullOrEmpty(Me.SessionData("CUSTOMERNUMBER")) Then
            Logger.Warning("No customer selected - showing error message", "main.aspx")
            Me.lblNoAccountSelected.Visible = True
            Me.grview.Visible = False
        Else
            Logger.Debug("Customer selected: " & Me.SessionData("CUSTOMERNUMBER"), "main.aspx")
        End If
        
    Catch ex As Exception
        Logger.Error("Error in Page_Load", ex, "main.aspx")
        Me.lblErrorMsg.Text = "An error occurred loading the page. Please try again."
    End Try
End Sub
```

### Search Button Click with Logging

```vb
Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
    Try
        Logger.Info("Search button clicked", "main.aspx")
        
        ' Log search parameters
        Dim searchParams As String = String.Format("CustomerNo={0}, ShipTo={1}, PONumber={2}, FromDate={3}, ToDate={4}, InvoiceNo={5}", _
                                                   Me.SessionData("CUSTOMERNUMBER"), _
                                                   Me.SessionData("SHIPTONUMBER"), _
                                                   Me.txtPONumber.Text, _
                                                   Me.txtFromDate.Text, _
                                                   Me.txtToDate.Text, _
                                                   Me.txtInvoiceNumber.Text)
        Logger.Debug("Search parameters: " & searchParams, "main.aspx")
        
        ' Perform search
        Dim startTime As DateTime = DateTime.Now
        LoadGrid(0)
        Dim duration As TimeSpan = DateTime.Now.Subtract(startTime)
        
        ' Log results
        Dim rowCount As Integer = If(gvInvoices.DataSource IsNot Nothing, DirectCast(gvInvoices.DataSource, DataTable).Rows.Count, 0)
        Logger.Info(String.Format("Search completed: {0} results in {1:F2} seconds", rowCount, duration.TotalSeconds), "main.aspx")
        
        If duration.TotalSeconds > 5 Then
            Logger.Warning(String.Format("Slow search detected: {0:F2} seconds", duration.TotalSeconds), "main.aspx")
        End If
        
    Catch ex As Exception
        Logger.Error("Error during invoice search", ex, "main.aspx")
        Me.lblErrorMsg.Text = "Search failed. Please try again."
    End Try
End Sub
```

### Submit Payment with Logging

```vb
Protected Sub btnSubmitPayment_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmitPayment.Click
    Try
        Logger.Info("Submit payment button clicked", "main.aspx")
        
        ' Get selected invoices
        Dim selectedInvoices As New List(Of String)
        Dim totalAmount As Decimal = 0
        
        For Each row As GridViewRow In gvInvoices.Rows
            Dim chkSelect As CheckBox = DirectCast(row.FindControl(GRID_SELECT_ID), CheckBox)
            If chkSelect IsNot Nothing AndAlso chkSelect.Checked Then
                Dim invoiceNo As String = DirectCast(row.FindControl(GRID_INVOICENUMBER_ID), HyperLink).Text
                Dim amount As Decimal = Decimal.Parse(DirectCast(row.FindControl(GRID_BALANCE_ID), Label).Text, Globalization.NumberStyles.Currency)
                
                selectedInvoices.Add(invoiceNo)
                totalAmount += amount
            End If
        Next
        
        If selectedInvoices.Count = 0 Then
            Logger.Warning("No invoices selected for payment", "main.aspx")
            Me.lblErrorMsg.Text = "Please select at least one invoice."
            Return
        End If
        
        Logger.Info(String.Format("Submitting payment: {0} invoices, Total={1:C}", selectedInvoices.Count, totalAmount), "main.aspx")
        Logger.Debug("Selected invoices: " & String.Join(", ", selectedInvoices), "main.aspx")
        
        ' Submit to US Bank
        Dim referenceNumber As Integer = SubmitToUSBank(selectedInvoices, totalAmount)
        
        Logger.Info(String.Format("Payment submitted successfully: RefNo={0}, Amount={1:C}", referenceNumber, totalAmount), "main.aspx")
        
        ' Redirect to confirmation
        Response.Redirect(String.Format("Confirmation.aspx?RefNo={0}", referenceNumber))
        
    Catch ex As Exception
        Logger.Error("Payment submission failed", ex, "main.aspx")
        Me.lblErrorMsg.Text = "Payment submission failed. Please try again or contact support."
    End Try
End Sub
```

---

## Example 2: History.aspx.vb - Payment History Page

```vb
Imports System.Data

Partial Public Class History
    Inherits EpayBasePage

Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Logger.Info("Payment history page loaded", "History.aspx")
            
            If Not IsPostBack Then
                ' Set default date
                Me.txtDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                Logger.Debug("Default date set: " & Me.txtDate.Text, "History.aspx")
                
                ' Load payment history
                LoadPaymentHistory()
            End If
            
        Catch ex As Exception
            Logger.Error("Error loading payment history page", ex, "History.aspx")
            Me.lblErrorMsg.Text = "An error occurred. Please try again."
        End Try
    End Sub
    
    Private Sub LoadPaymentHistory()
        Try
            Logger.Info("Loading payment history", "History.aspx")
            
            Dim customerNo As String = Me.SessionData("CUSTOMERNUMBER")
            Dim selectedDate As DateTime = DateTime.Parse(Me.txtDate.Text)
            
            Logger.Debug(String.Format("Loading history for CustomerNo={0}, Date={1:yyyy-MM-dd}", customerNo, selectedDate), "History.aspx")
            
            Dim startTime As DateTime = DateTime.Now
            Dim historyData As DataTable = Common.LoadPaymentHistory(customerNo, selectedDate)
            Dim duration As TimeSpan = DateTime.Now.Subtract(startTime)
            
            grvEpayHistory.DataSource = historyData
            grvEpayHistory.DataBind()
            
            Logger.Info(String.Format("Payment history loaded: {0} records in {1:F2} seconds", historyData.Rows.Count, duration.TotalSeconds), "History.aspx")
            
        Catch ex As Exception
            Logger.Error("Error loading payment history data", ex, "History.aspx")
            Throw
        End Try
    End Sub
End Class
```

---

## Example 3: Common.vb - Shared Utility Class

```vb
Public Class Common

    Public Shared Function LoadInvoices(ByVal customerNo As String, _
                                       ByVal shipTo As String, _
                                       ByVal allShipTos As Boolean, _
                                       ByVal securityMHS As String, _
                                       ByVal poNumber As String, _
                                       ByVal fromDate As String, _
                                       ByVal toDate As String, _
                                       ByVal invoiceNo As String, _
                                       ByVal creditNo As String, _
                                       ByVal sortColumn As String, _
                                       ByVal sortAscending As Boolean, _
                                       ByVal pageNumber As Integer, _
                                       ByVal pageSize As Integer) As DataTable
        Try
            Logger.Debug(String.Format("LoadInvoices called: CustomerNo={0}, PageNumber={1}, PageSize={2}", customerNo, pageNumber, pageSize), "Common.LoadInvoices")
            
            Dim startTime As DateTime = DateTime.Now
            
            ' Execute stored procedure
            Dim dt As DataTable = DataAccess.ExecuteStoredProcedure("usp_OrderAndInvoiceReportingOpenInvoices3", _
                                                                   customerNo, shipTo, allShipTos, securityMHS, _
                                                                   poNumber, fromDate, toDate, invoiceNo, creditNo, _
                                                                   sortColumn, sortAscending, pageNumber, pageSize)
            
            Dim duration As TimeSpan = DateTime.Now.Subtract(startTime)
            
            Logger.Info(String.Format("LoadInvoices completed: {0} rows in {1:F2} seconds", dt.Rows.Count, duration.TotalSeconds), "Common.LoadInvoices")
            
            If duration.TotalSeconds > 10 Then
                Logger.Warning(String.Format("Slow database query detected: {0:F2} seconds", duration.TotalSeconds), "Common.LoadInvoices")
            End If
            
            Return dt
            
        Catch ex As SqlException
            Logger.Error(String.Format("Database error in LoadInvoices: CustomerNo={0}", customerNo), ex, "Common.LoadInvoices")
            Throw
            
        Catch ex As Exception
            Logger.Error("Unexpected error in LoadInvoices", ex, "Common.LoadInvoices")
            Throw
        End Try
    End Function
    
    Public Shared Sub WriteErrorToAuditLog(ByVal referenceNumber As Integer, ByVal errorMsg As String)
        Try
            Logger.Error(String.Format("Audit log entry: RefNo={0}, Error={1}", referenceNumber, errorMsg), "Common.WriteErrorToAuditLog")
            
            ' Existing audit log code...
            Dim sbSQL As String = "EXEC ASHLEY.DBO.usp_InsertAuditRecord..."
            DataAccess.ExecuteCommand(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL)
            
            Logger.Info("Audit log entry written successfully: RefNo=" & referenceNumber, "Common.WriteErrorToAuditLog")
            
        Catch ex As Exception
            Logger.Fatal("Failed to write audit log entry: RefNo=" & referenceNumber, ex, "Common.WriteErrorToAuditLog")
            Throw
        End Try
    End Sub

End Class
```

---

## Copyright

Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.  
This software is proprietary and confidential.

