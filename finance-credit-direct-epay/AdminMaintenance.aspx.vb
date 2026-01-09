
#Region " Revision History "

'   Date            Programmer      Description
'   ============    ============    ========================================
'   08/10/2010      TKiel           Creation
'   01/26/2011      TKiel           Added code to get the Credit Number column if the Invoice Number 
'                                       column is blank for use when deleting an unconfirmed payment.
'                                   Note: When Invoice Amount is less than $0, the Invoice Number is 
'                                       displayed in the Credit Number column instead to signify
'                                       to the user that it is a credit.
'   04/21/2011      TKiel           Converted the Infragistics UltraWebGrid control to a GridView and changed the logic accordingly.
'   03/28/2012      TKiel           Made changes for uniting OpenInvoicesBLL project into Epay

#End Region


Imports System.Xml

Partial Public Class AdminMaintenance
    Inherits EpayBasePage

#Region " Variable/Constant Declarations "

    Private Const GRID_INVOICEAMOUNT_HEADER_ID As String = "lbnGridInvoiceAmountHeader"
    Private Const GRID_AMOUNTPAID_HEADER_ID As String = "lbnGridAmountPaidHeader"
    Private Const GRID_BALANCE_HEADER_ID As String = "lbnGridBalanceHeader"

    Private Const GRID_SELECTALL_ID As String = "chkSelectAll"
    Private Const GRID_SELECT_ID As String = "chkSelect"
    Private Const GRID_STATUS_ID As String = "hplGridStatus"
    Private Const GRID_REFERENCENUMBER_ID As String = "lblGridRefNo"
    Private Const GRID_CUSTOMERNUMBER_ID As String = "lblGridCusNo"
    Private Const GRID_INVOICENUMBER_ID As String = "hplGridInvoiceNumber"
    Private Const GRID_CREDITNUMBER_ID As String = "hplGridCreditNumber"
    Private Const GRID_SHIPTO_ID As String = "lblGridShipTo"
    Private Const GRID_INVOICEDATE_ID As String = "lblGridInvDate"
    Private Const GRID_ORDERNUMBER_ID As String = "lblGridOrderNumber"
    Private Const GRID_ORDERDATE_ID As String = "lblGridOrderDate"
    Private Const GRID_TRIPNUMBER_ID As String = "lblGridTripNumber"
    Private Const GRID_RPPNUMBER_ID As String = "lblGridRPPNumber"
    Private Const GRID_PONUMBER_ID As String = "lblGridPONumber"
    Private Const GRID_INVOICEAMOUNT_ID As String = "lblGridInvoiceAmount"
    Private Const GRID_AMOUNTPAID_ID As String = "lblGridAmountPaid"
    Private Const GRID_BALANCE_ID As String = "lblGridBalance"
    Private Const GRID_CODE_ID As String = "lblGridCode"
    Private Const GRID_DAYS_ID As String = "lblGridDays"

    Private bIsExcelResponseTerminating As Boolean = False

#End Region

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strSortColumnName As String = ""
        Dim blnSortAscending As Boolean = True
        Dim bHasCustomer As Boolean = False

        Try
            Me.Navigation.AppAuthorization = Me.VerifyAppAuthorization("EPAYANLYST", True)

            If Me.Navigation.AppAuthorization Then

                If Me.SessionData("CUSTOMERNUMBER") <> "" Then
                    bHasCustomer = True
                End If

                Me.hdnCustomerNumber.Value = Me.SessionData("CUSTOMERNUMBER")
                Me.hdnShiptoNumber.Value = Me.SessionData("SHIPTONUMBER")
                Me.hdnAllShiptos.Value = Me.SessionData("ALLSHIPTOS")
                Me.hdnSecurityMHS.Value = Me.SessionData("SECURITY_MHS")

                If Me.IsPostBack = False Then
                    If bHasCustomer Then
                        Me.rdbSelectedCustomer.Checked = True
                        Me.lblPaymentsForSpecifiedCustomer.Text = "Unconfirmed payments for Acct #: " & Me.SessionData("CUSTOMERNUMBER")
                    Else
                        Me.rdbAllCustomers.Checked = True
                        Me.rdbSelectedCustomer.Enabled = False
                        Me.lblPaymentsForSpecifiedCustomer.Text = "Unconfirmed payments for ALL accounts"
                    End If

                    If Me.hdnSortColumn.Value.ToString <> "" Then
                        strSortColumnName = CStr(Me.hdnSortColumn.Value)

                        If Me.hdnSortAscending.Value.ToString = "" Then
                            blnSortAscending = True
                        Else
                            blnSortAscending = CType(Me.hdnSortAscending.Value, Boolean)
                        End If
                    Else
                        strSortColumnName = ""
                        blnSortAscending = True
                    End If

                    Me.hdnSortAscending.Value = blnSortAscending
                    Me.hdnSortColumn.Value = strSortColumnName
                    Me.LoadCreditTerritoryListbox()

                    Me.GetReport()

                End If

            Else
                Me.RedirectToAccessDenied()
            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
        Try

            If bIsExcelResponseTerminating = False Then
                MyBase.Render(writer)
            Else
                bIsExcelResponseTerminating = False
            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Private Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSearch.Click

        Try

            Me.GetReport()

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Protected Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdDelete.Click

        Dim invoiceNumber As String = ""
        Dim customerNumber As String = ""

        Try
            If Me.AreInvoicesSelected() Then

                For Each row As GridViewRow In Me.gvInvoices.Rows
                    If DirectCast(row.FindControl(GRID_SELECT_ID), CheckBox).Checked Then

                        If DirectCast(row.FindControl(GRID_INVOICENUMBER_ID), HyperLink).Text <> String.Empty Then
                            'If Invoice column is NOT blank, use invoice number
                            invoiceNumber = DirectCast(row.FindControl(GRID_INVOICENUMBER_ID), HyperLink).Text
                        Else
                            If DirectCast(row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text.StartsWith("0") Then
                                'If Invoice column IS blank, use the credit number (Note: Credit Number MAY start with an Extra 0.)
                                invoiceNumber = DirectCast(row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text.Substring(1, _
                                          DirectCast(row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text.Length - 1)
                            Else
                                'If Invoice column IS blank, use the credit number
                                invoiceNumber = DirectCast(row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text
                            End If
                        End If

                        customerNumber = DirectCast(row.FindControl(GRID_CUSTOMERNUMBER_ID), Label).Text
                        Common.DeleteEpayUnconfirmedPayment(customerNumber, invoiceNumber, Me.SessionData("LOGON_USER"))
                    End If
                Next

                'Clear selected invoices and reload the report
                Me.GetReport()

            Else
                lblErrorMsg.Text = "No unconfirmed payments to delete!"
            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        Finally
            invoiceNumber = Nothing
            customerNumber = Nothing
        End Try

    End Sub

    Protected Sub cmdExportToExcel_Click(ByVal sender As System.Object, ByVal e As ImageClickEventArgs) Handles cmdExportToExcel.Click
        If gvInvoices.Rows.Count > 0 Then

            Dim currencyColumnIndexes As List(Of Integer) = Nothing

            Try
                currencyColumnIndexes = New List(Of Integer)
                currencyColumnIndexes.Insert(0, Me.gvInvoices.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.gvInvoices.HeaderRow.FindControl(GRID_INVOICEAMOUNT_HEADER_ID), LinkButton).Parent, TableCell)))
                currencyColumnIndexes.Insert(1, Me.gvInvoices.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.gvInvoices.HeaderRow.FindControl(GRID_AMOUNTPAID_HEADER_ID), LinkButton).Parent, TableCell)))
                currencyColumnIndexes.Insert(2, Me.gvInvoices.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.gvInvoices.HeaderRow.FindControl(GRID_BALANCE_HEADER_ID), LinkButton).Parent, TableCell)))

                Ashley.Web.UI.GridViewExportUtil.Export("EpaymentAdminMaintenanceInvoices.xls", Me.gvInvoices, bIsExcelResponseTerminating, currencyColumnIndexes)
            Catch ex As Exception
                Throw ex
            Finally


            End Try
        End If
    End Sub

    Protected Sub gvInvoices_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvInvoices.RowCreated
        Ashley.Web.UI.GridViewExportUtil.GridView_RowCreated(Me.gvInvoices, e)

    End Sub

    Protected Sub gvInvoices_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvInvoices.RowDataBound

        Try
            If e Is Nothing Then
                Exit Sub

            End If

            If e.Row.RowType = DataControlRowType.DataRow Then

                DirectCast(e.Row.FindControl(GRID_STATUS_ID), HyperLink).NavigateUrl = String.Format("Confirmation.aspx?RefNo={0}&CusNo={1}",
                                                                                                     DirectCast(e.Row.FindControl(GRID_REFERENCENUMBER_ID), Label).Text,
                                                                                                     DirectCast(e.Row.FindControl(GRID_CUSTOMERNUMBER_ID), Label).Text)

                DirectCast(e.Row.FindControl(GRID_INVOICENUMBER_ID), HyperLink).NavigateUrl = "javascript:Invoice(""" _
                        & DirectCast(e.Row.FindControl(GRID_CUSTOMERNUMBER_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_SHIPTO_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_INVOICENUMBER_ID), HyperLink).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_INVOICEDATE_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_PONUMBER_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_ORDERNUMBER_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_ORDERDATE_ID), Label).Text & """)"

                If DirectCast(e.Row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text <> "0" And _
                        DirectCast(e.Row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text <> String.Empty Then
                    DirectCast(e.Row.FindControl(GRID_INVOICENUMBER_ID), HyperLink).Text = ""

                    DirectCast(e.Row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).NavigateUrl = "javascript:Invoice(""" _
                        & DirectCast(e.Row.FindControl(GRID_CUSTOMERNUMBER_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_SHIPTO_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_INVOICEDATE_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_PONUMBER_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_ORDERNUMBER_ID), Label).Text & """, """ _
                        & DirectCast(e.Row.FindControl(GRID_ORDERDATE_ID), Label).Text & """)"

                    DirectCast(e.Row.FindControl(GRID_AMOUNTPAID_ID), Label).Text = String.Empty
                Else
                    DirectCast(e.Row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text = String.Empty

                    DirectCast(e.Row.FindControl(GRID_AMOUNTPAID_ID), Label).Text = FormatCurrency(DirectCast(e.Row.FindControl(GRID_AMOUNTPAID_ID), Label).Text, 2)
                End If

                DirectCast(e.Row.FindControl(GRID_INVOICEAMOUNT_ID), Label).Text = FormatCurrency(DirectCast(e.Row.FindControl(GRID_INVOICEAMOUNT_ID), Label).Text, 2)
                DirectCast(e.Row.FindControl(GRID_BALANCE_ID), Label).Text = FormatCurrency(DirectCast(e.Row.FindControl(GRID_BALANCE_ID), Label).Text, 2)

                If DirectCast(e.Row.FindControl(GRID_RPPNUMBER_ID), Label).Text = "0" Then
                    DirectCast(e.Row.FindControl(GRID_RPPNUMBER_ID), Label).Text = String.Empty
                End If

                If DirectCast(e.Row.FindControl(GRID_TRIPNUMBER_ID), Label).Text = "0" Then
                    DirectCast(e.Row.FindControl(GRID_TRIPNUMBER_ID), Label).Text = String.Empty
                End If

                'If DirectCast(e.Row.FindControl(GRID_ORDERDATE_ID), Label).Text = "01/01/1900" Then
                '    DirectCast(e.Row.FindControl(GRID_ORDERDATE_ID), Label).Text = String.Empty
                'End If

            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Protected Sub gvInvoices_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles gvInvoices.PageIndexChanging
        If e Is Nothing Then
            Exit Sub

        End If

        Me.gvInvoices.PageIndex = e.NewPageIndex
        Me.GetReport()

    End Sub

    Protected Sub gvInvoices_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvInvoices.Sorting
        If e Is Nothing Then
            Exit Sub

        End If

        Me.hdnSortColumn.Value = e.SortExpression

        If CType(Me.hdnSortAscending.Value, Boolean) = True Then
            Me.hdnSortAscending.Value = False
        Else
            Me.hdnSortAscending.Value = True
        End If

        Me.GetReport()

    End Sub

    Protected Sub SelectAllCheckboxes(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim chkCheckBox As CheckBox
        Try
            chkCheckBox = CType(sender, CheckBox)

            Dim gridView As GridView = CType(chkCheckBox.NamingContainer.NamingContainer, GridView)

            For Each row As GridViewRow In gridView.Rows
                If DirectCast(row.FindControl(GRID_SELECT_ID), CheckBox).Enabled = True Then
                    DirectCast(row.FindControl(GRID_SELECT_ID), CheckBox).Checked = chkCheckBox.Checked
                End If
            Next

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

#End Region

#Region " Private Functions/Methods "

    Private Sub GetReport()
        Dim sCustomerNumber As String = String.Empty
        Dim sShipToNumber As String = String.Empty
        Dim sSecurityMHS As String = String.Empty
        Dim sCreditTerritory As String = String.Empty

        Try
            If rdbAllCustomers.Checked = True Then
                Me.lblPaymentsForSpecifiedCustomer.Text = "Unconfirmed payments for ALL accounts"

            ElseIf rdbSelectedCustomer.Checked = True Then
                sCustomerNumber = Me.SessionData("CUSTOMERNUMBER")
                sShipToNumber = Me.SessionData("SHIPTONUMBER")
                sSecurityMHS = Me.SessionData("SECURITY_MHS")

                Me.lblPaymentsForSpecifiedCustomer.Text = "Unconfirmed payments for Acct #: " & Me.SessionData("CUSTOMERNUMBER")
            End If

            If lstCreditTerritory.SelectedValue.ToString <> "All" Then
                sCreditTerritory = lstCreditTerritory.SelectedValue.ToString

            End If

            Me.gvInvoices.DataSource = Common.LoadUnconfirmedPayments(sCustomerNumber, sShipToNumber, sSecurityMHS, sCreditTerritory, Me.hdnSortColumn.Value, CType(Me.hdnSortAscending.Value, Boolean))

            If Me.gvInvoices.DataSource IsNot Nothing Then
                Me.gvInvoices.DataBind()

            End If

        Catch ex As Exception
            Throw

        End Try

    End Sub

    Private Sub LoadCreditTerritoryListbox()
        Dim drDataReader As SqlClient.SqlDataReader = Nothing

        Try
            drDataReader = Me.CreditTerritoryDataReader

            Me.lstCreditTerritory.DataValueField = "CreditTerritory"
            Me.lstCreditTerritory.DataTextField = "CreditTerritory"
            Me.lstCreditTerritory.DataSource = drDataReader
            Me.lstCreditTerritory.DataBind()

        Finally
            If drDataReader IsNot Nothing AndAlso Not drDataReader.IsClosed Then drDataReader.Close()

        End Try

    End Sub

    Private Function AreInvoicesSelected() As Boolean
        Try
            For Each row As GridViewRow In Me.gvInvoices.Rows
                If DirectCast(row.FindControl(GRID_SELECT_ID), CheckBox).Checked Then
                    Return True
                    Exit Function
                End If
            Next

            Return False

        Catch ex As Exception
            Throw
        End Try
    End Function

#End Region

End Class