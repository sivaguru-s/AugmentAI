Imports System.Xml

Partial Public Class main
    Inherits EpayBasePage

#Region " Declarations "

    Private bIsExcelResponseTerminating As Boolean = False
    Private Const GRID_PAGER_MODULUS As Integer = 500
    Private Const PAY_INFO_DELIMITER As String = "|"

    Private Const GRID_STATUS_HEADER_ID = "lkbGridStatusHeader"
    Private Const GRID_INVOICENUMBER_HEADER_ID As String = "lkbGridInvoiceNumberHeader"
    Private Const GRID_CREDITNUMBER_HEADER_ID As String = "lkbGridCreditNumberHeader"
    Private Const GRID_SHIPTO_HEADER_ID As String = "lkbGridShipToHeader"
    Private Const GRID_INVOICEDATE_HEADER_ID As String = "lkbGridInvoiceDateHeader"
    Private Const GRID_ORDERNUMBER_HEADER_ID As String = "lkbGridOrderNumberHeader"
    Private Const GRID_TRIPNUMBER_HEADER_ID As String = "lkbGridTripNumberHeader"
    Private Const GRID_RPPNUMBER_HEADER_ID As String = "lkbGridRPPNumberHeader"
    Private Const GRID_PONUMBER_HEADER_ID As String = "lkbGridPONumberHeader"
    Private Const GRID_INVOICEAMOUNT_HEADER_ID As String = "lkbGridInvoiceAmountHeader"
    Private Const GRID_AMOUNTPAID_HEADER_ID As String = "lkbGridAmountPaidHeader"
    Private Const GRID_BALANCE_HEADER_ID As String = "lkbGridBalanceHeader"
    Private Const GRID_CODE_HEADER_ID As String = "lkbGridCodeHeader"
    Private Const GRID_DAYS_HEADER_ID As String = "lkbGridDaysHeader"

    Private Const GRID_SELECTALL_ID As String = "chkSelectAll"
    Private Const GRID_SELECT_ID As String = "chkSelect"
    Private Const GRID_STATUS_ID As String = "hplGridStatus"
    Private Const GRID_PAYINFO_ID As String = "hdnGridPayInfo"
    Private Const GRID_SHIPTO_ID As String = "lblGridShipTo"
    Private Const GRID_INVOICENUMBER_ID As String = "hplGridInvoiceNumber"
    Private Const GRID_CREDITNUMBER_ID As String = "hplGridCreditNumber"
    Private Const GRID_INVOICEDATE_ID As String = "lblGridInvDate"
    Private Const GRID_ORDERNUMBER_ID As String = "lblGridOrderNumber"
    Private Const GRID_TRIPNUMBER_ID As String = "lblGridTripNumber"
    Private Const GRID_RPPNUMBER_ID As String = "lblGridRPPNumber"
    Private Const GRID_PONUMBER_ID As String = "lblGridPONumber"
    Private Const GRID_INVOICEAMOUNT_ID As String = "lblGridInvoiceAmount"
    Private Const GRID_AMOUNTPAID_ID As String = "lblGridAmountPaid"
    Private Const GRID_BALANCE_ID As String = "lblGridBalance"
    Private Const GRID_CODE_ID As String = "lblGridCode"
    Private Const GRID_DAYS_ID As String = "lblGridDays"
    Private Const GRID_PAGECOUNT_ID As String = "lblGridPageCount"

    Private Enum GridColumnIndices
        [Select] = 0
        Status = 1
        InvoiceNum = 2
        CreditNum = 3
        ShipTo = 4
        InvoiceDate = 5
        OrderNumber = 6
        TripNumber = 7
        RPPNumber = 8
        PONumber = 9
        InvoiceAmt = 10
        AmtPaid = 11
        Balance = 12
        Code = 13
        Days = 14
    End Enum

#End Region

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strSortColumnName As String = String.Empty
        Dim blnSortAscending As Boolean = False

        Try
            Me.Navigation.AppAuthorization = Me.VerifyAppAuthorization("EPAYANLYST", False)

            'If Me.VerifyAppAuthorization("EPAYANLYST", False) Then
            '    Me.hlUserList.Visible = True
            '    Me.hlAdminMaintenance.Visible = True
            '    Me.hlReport.Visible = True

            'Else
            '    Me.hlUserList.Visible = False
            '    Me.hlAdminMaintenance.Visible = False
            '    Me.hlReport.Visible = False

            'End If

            If Me.SessionData("CUSTOMERNUMBER") <> String.Empty Then
                Me.lblNoAccountSelected.Visible = False

                'Me.hdnShowPricing.Value = Me.VerifyAppAuthorization("PRICING", False)
                'Me.ShowPricing = CType(Me.VerifyAppAuthorization("PRICING", False), Boolean)

                'Me.hdnShowCredits.Value = Me.VerifyAppAuthorization("NOCREDITS", False)
                'Me.hdnShowOldCredits.Value = Me.VerifyAppAuthorization("CRDEPT", False)

                Me.hdnCustomerNumber.Value = Me.SessionData("CUSTOMERNUMBER")
                Me.hdnShiptoNumber.Value = Me.SessionData("SHIPTONUMBER")
                Me.hdnAllShiptos.Value = Me.SessionData("ALLSHIPTOS")
                Me.hdnSecurityMHS.Value = Me.SessionData("SECURITY_MHS")

                If Not Me.IsPostBack Then
                    If Me.hdnSortColumn.Value.ToString <> String.Empty Then
                        strSortColumnName = CStr(Me.hdnSortColumn.Value)

                        If Me.hdnSortAscending.Value.ToString = String.Empty Then
                            blnSortAscending = False

                        Else
                            blnSortAscending = CType(Me.hdnSortAscending.Value, Boolean)

                        End If
                    Else
                        strSortColumnName = String.Empty
                        blnSortAscending = False

                    End If

                    Me.hdnSortAscending.Value = blnSortAscending
                    Me.hdnSortColumn.Value = strSortColumnName
                    Me.txtFromDate.Text = DateAdd(DateInterval.Day, (Me.GetDefaultDateSpanInDays() * -1), Today).ToString("MM/dd/yyyy")
                    Me.txtToDate.Text = Today
                    Me.LoadGrid(1)

                End If

                Me.lblInvoiceDate.Text = Me.txtFromDate.Text
                Me.lblToDate.Text = Me.txtToDate.Text

            Else
                Me.lblNoAccountSelected.Visible = True

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

    Private Sub chkShowAllInvoices_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkShowAllInvoices.CheckedChanged
        Try
            Me.LoadGrid(0)

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Private Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSearch.Click

        Try
            If txtFromDate.Text = Nothing OrElse txtToDate.Text = Nothing OrElse Not IsDate(txtFromDate.Text) = True OrElse Not IsDate(txtToDate.Text) = True OrElse txtFromDate.Text <= DateAdd(DateInterval.Year, -200, Today) OrElse txtToDate.Text <= DateAdd(DateInterval.Year, -200, Today) Then
                Me.txtFromDate.Text = DateAdd(DateInterval.Day, (Me.GetDefaultDateSpanInDays() * -1), Today)
                Me.txtToDate.Text = Today
                Me.lblInvoiceDate.Text = Me.txtFromDate.Text
                Me.lblToDate.Text = Me.txtToDate.Text
            End If

            Me.LoadGrid(IIf(Me.chkShowAllInvoices.Checked, 0, 1))

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Protected Sub cmdPayment_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdPayment.Click

        Dim totalAmt As Decimal
        Dim refNo As Integer = 0

        Try
            If Me.AreInvoicesSelected() Then

                totalAmt = WritePaymentFiles(refNo)

                If totalAmt = 0 Then
                    lblErrorMsg.Text = "Invoices are already in EPay File!"
                ElseIf totalAmt < 0 Then
                    Common.DeleteEpayRecords(Me.SessionData("CUSTOMERNUMBER"), refNo, Me.SessionData("LOGON_USER"))
                    lblErrorMsg.Text = "Amount is less then 0!"

                Else
                    Response.Redirect(String.Format("Confirmation.aspx?RefNo={0}", refNo.ToString), False)

                End If

            Else
                lblErrorMsg.Text = "No Invoices to send!"
            End If

        Catch ex As Exception
            'Remove Epay records if there is a failure
            If refNo > 0 Then
                Common.DeleteEpayRecords(Me.SessionData("CUSTOMERNUMBER"), refNo, Me.SessionData("LOGON_USER"))
            End If

            Me.RedirectToErrorProcessing(ex)

        Finally
            totalAmt = Nothing
            refNo = Nothing

        End Try

    End Sub

    Protected Sub cmdExportToExcel_Click(ByVal sender As System.Object, ByVal e As ImageClickEventArgs) Handles cmdExportToExcel.Click
        If gvInvoices.Rows.Count > 0 Then
            Dim gridView As GridView = Nothing
            Dim currencyColumnIndexes As List(Of Integer) = Nothing

            Try
                currencyColumnIndexes = New List(Of Integer)
                currencyColumnIndexes.Insert(0, GridColumnIndices.InvoiceAmt)
                currencyColumnIndexes.Insert(1, GridColumnIndices.AmtPaid)
                currencyColumnIndexes.Insert(2, GridColumnIndices.Balance)

                gridView = Me.gvInvoices
                gridView.AllowPaging = False
                gridView.DataSource = Me.LoadInvoicesDataTable(0)
                gridView.DataBind()

                Ashley.Web.UI.GridViewExportUtil.Export("EpaymentCustomerInvoices.xls", gridView, bIsExcelResponseTerminating, currencyColumnIndexes)

            Finally
                If gridView IsNot Nothing Then gridView.Dispose()

            End Try
        End If
    End Sub  

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId:="1")>
    Protected Sub gvInvoices_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvInvoices.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.Header Then
                Me.TranslateDataBoundControlHeaders(e.Row)

            ElseIf e.Row.RowType = DataControlRowType.DataRow Then
                Dim drv As DataRowView = DirectCast(e.Row.DataItem, DataRowView)
                Dim sCustomerNumber As String = drv.Item("opicusno").ToString.Trim,
                    sShipTo As String = drv.Item("opishpno").ToString.Trim,
                    sInvNo As String = drv.Item("opiinvno").ToString.Trim,
                    sCreditNo As String = drv.Item("opicrmnr").ToString.Trim,
                    sOrdNo As String = drv.Item("opiordno").ToString.Trim,
                    sPONo As String = drv.Item("opiponum").ToString.Trim,
                    sInvDate As String = CDate(drv.Item("opiDagedt")).ToString("MM/dd/yyyy"),
                    sOrdDate As String = If(drv.Item("inhdordda") IsNot DBNull.Value, CDate(drv.Item("inhdordda")).ToString("MM/dd/yyyy"), String.Empty),
                    sEpayStatus As String = String.Empty,
                    sEpayRefNumber As String = If(drv.Item("EpayRefNumber") IsNot DBNull.Value, drv.Item("EpayRefNumber").ToString, String.Empty)

                DirectCast(e.Row.FindControl(GRID_PAYINFO_ID), HiddenField).Value = String.Format("{1}{0}{2}{0}{3}",
                                                                                                  PAY_INFO_DELIMITER,
                                                                                                  sInvNo,
                                                                                                  sCustomerNumber,
                                                                                                  sShipTo)

                If drv.Item("EpayStatus") IsNot DBNull.Value Then
                    sEpayStatus = drv.Item("EpayStatus").ToString.Trim
                    DirectCast(e.Row.FindControl(GRID_STATUS_ID), HyperLink).Text = sEpayStatus
                End If

                'If Status has a value don't allow user to select this invoice
                If sEpayStatus = String.Empty Then
                    DirectCast(e.Row.FindControl(GRID_SELECT_ID), CheckBox).Enabled = True
                Else
                    DirectCast(e.Row.FindControl(GRID_SELECT_ID), CheckBox).Checked = False
                    DirectCast(e.Row.FindControl(GRID_SELECT_ID), CheckBox).Enabled = False
                    If String.Equals(sEpayStatus, "verifying", StringComparison.OrdinalIgnoreCase) _
                            OrElse String.Equals(sEpayStatus, "Sent", StringComparison.OrdinalIgnoreCase) Then
                        DirectCast(e.Row.FindControl(GRID_STATUS_ID), HyperLink).NavigateUrl = String.Format("Confirmation.aspx?RefNo={0}", sEpayRefNumber)
                    Else
                        DirectCast(e.Row.FindControl(GRID_STATUS_ID), HyperLink).ForeColor = Drawing.Color.Black
                    End If
                End If

                DirectCast(e.Row.FindControl(GRID_SHIPTO_ID), Label).Text = sShipTo
                DirectCast(e.Row.FindControl(GRID_INVOICEDATE_ID), Label).Text = sInvDate
                DirectCast(e.Row.FindControl(GRID_ORDERNUMBER_ID), Label).Text = sOrdNo

                If drv.Item("inhTripNo") IsNot DBNull.Value Then
                    Dim sTripNo As String = drv.Item("inhTripNo").ToString
                    If sTripNo <> "0" Then
                        DirectCast(e.Row.FindControl(GRID_TRIPNUMBER_ID), Label).Text = sTripNo
                    Else
                        DirectCast(e.Row.FindControl(GRID_TRIPNUMBER_ID), Label).Text = String.Empty
                    End If

                End If

                Dim sRPP As String = drv.Item("RPP #").ToString
                If sRPP = "0" Then
                    DirectCast(e.Row.FindControl(GRID_RPPNUMBER_ID), Label).Text = String.Empty
                Else
                    DirectCast(e.Row.FindControl(GRID_RPPNUMBER_ID), Label).Text = sRPP
                End If

                DirectCast(e.Row.FindControl(GRID_PONUMBER_ID), Label).Text = sPONo

                Dim dInvAmt As Decimal = 0
                If Decimal.TryParse(drv.Item("opiInvam").ToString.Trim, dInvAmt) Then
                    DirectCast(e.Row.FindControl(GRID_INVOICEAMOUNT_ID), Label).Text = dInvAmt.ToString("#,##0.00")
                End If

                Dim dAmtPaid As Decimal = 0
                If Decimal.TryParse(drv.Item("opiTtlcr").ToString.Trim, dAmtPaid) Then
                    DirectCast(e.Row.FindControl(GRID_AMOUNTPAID_ID), Label).Text = dAmtPaid.ToString("#,##0.00")
                End If

                Dim dBalance As Decimal = 0
                If Decimal.TryParse(drv.Item("opiOpamt").ToString.Trim, dBalance) Then
                    DirectCast(e.Row.FindControl(GRID_BALANCE_ID), Label).Text = dBalance.ToString("#,##0.00")
                End If

                DirectCast(e.Row.FindControl(GRID_CODE_ID), Label).Text = drv.Item("opicatcd").ToString
                DirectCast(e.Row.FindControl(GRID_DAYS_ID), Label).Text = drv.Item("Days").ToString

                Dim sInvoiceDtlLink As String = String.Empty,
                    bHideDetail As Boolean = BooleanParser(drv.Item("HideDetail"))

                If bHideDetail = False Then
                    sInvoiceDtlLink = String.Format("javascript:Invoice(""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"");",
                                                    sCustomerNumber,
                                                    sShipTo,
                                                    sInvNo,
                                                    sInvDate,
                                                    sPONo,
                                                    sOrdNo,
                                                    sOrdDate)
                End If

                If sCreditNo <> "0" _
                        AndAlso sCreditNo <> String.Empty Then
                    If bHideDetail = False Then
                        DirectCast(e.Row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).Text = sCreditNo
                        DirectCast(e.Row.FindControl(GRID_CREDITNUMBER_ID), HyperLink).NavigateUrl = sInvoiceDtlLink
                    Else
                        e.Row.Cells(GridColumnIndices.InvoiceNum).Text = String.Empty
                        e.Row.Cells(GridColumnIndices.CreditNum).Text = sCreditNo
                    End If

                Else
                    If bHideDetail = False Then
                        DirectCast(e.Row.FindControl(GRID_INVOICENUMBER_ID), HyperLink).Text = sInvNo
                        DirectCast(e.Row.FindControl(GRID_INVOICENUMBER_ID), HyperLink).NavigateUrl = sInvoiceDtlLink
                    Else
                        e.Row.Cells(GridColumnIndices.InvoiceNum).Text = sInvNo
                        e.Row.Cells(GridColumnIndices.CreditNum).Text = String.Empty
                    End If

                End If

            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId:="1")>
    Protected Sub gvInvoices_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvInvoices.Sorting
        Me.hdnSortColumn.Value = e.SortExpression

        If CType(Me.hdnSortAscending.Value, Boolean) = True Then
            Me.hdnSortAscending.Value = False
        Else
            Me.hdnSortAscending.Value = True
        End If

        Me.LoadGrid(IIf(Me.chkShowAllInvoices.Checked, 0, 1))

    End Sub

    Public Sub Page_Changed(ByVal sender As Object, ByVal e As EventArgs)
        Dim pageIndex As Integer = Integer.Parse(CType(sender, LinkButton).CommandArgument)
        Me.LoadGrid(pageIndex)

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

    Private Function LoadInvoicesDataTable(ByVal iPageNumber As Integer) As DataTable
        Dim dtDataTable As DataTable = Nothing

        Dim bShowAll As Boolean = False

        Try
            If Me.chkShowAllInvoices.Checked _
                    OrElse iPageNumber = 0 Then
                bShowAll = True

            End If

            dtDataTable = Common.LoadInvoices(Me.SessionData("CUSTOMERNUMBER"),
                                              Me.SessionData("SHIPTONUMBER"),
                                              Me.BooleanParser(Me.SessionData("ALLSHIPTOS")),
                                              Me.SessionData("SECURITY_MHS"),
                                              Me.txtPONumber.Text.Trim,
                                              Me.txtFromDate.Text,
                                              Me.txtToDate.Text,
                                              Me.txtInvoiceNumber.Text.Trim,
                                              Me.txtCreditNumber.Text.Trim,
                                              Me.hdnSortColumn.Value, _
                                              Me.BooleanParser(Me.hdnSortAscending.Value),
                                              iPageNumber,
                                              If(bShowAll, 0, GRID_PAGER_MODULUS))

            Return dtDataTable

        Catch ex As Exception
            Throw

        Finally
            If dtDataTable IsNot Nothing Then dtDataTable.Dispose()

        End Try

    End Function

    Private Sub LoadGrid(ByVal iPageNumber As Integer)
        Using dtDatatable As DataTable = Me.LoadInvoicesDataTable(iPageNumber)
            Me.gvInvoices.DataSource = dtDatatable            
            Me.gvInvoices.DataBind()

            If dtDatatable IsNot Nothing _
                AndAlso dtDatatable.Rows.Count > 0 _
                AndAlso dtDatatable.Columns.Contains("PageCount") Then

                Me.rptPager.DataSource = Ashley.Web.UI.GridPaging.LoadPagerItems(GRID_PAGER_MODULUS,
                                                                                 CInt(dtDatatable.Rows(0).Item("PageCount").ToString),
                                                                                 iPageNumber,
                                                                                 nextText:=gt("Next"),
                                                                                 prevText:=gt("Prev"),
                                                                                 allOption:=False)
                Me.rptPager.DataBind()

            Else
                Me.rptPager.DataSource = Nothing
                Me.rptPager.DataBind()

            End If

        End Using

    End Sub

    Private Function WritePaymentFiles(ByRef refNo As Integer) As Decimal
        Try
            'EXAMPLE
            '<invoices><invoice><number>123456</number><customer>8888300</customer><ShipTo>480</ShipTo></invoice></invoices>

            'Map to an object list in the future that the procedure can use instead of writing out XML document
            '   This code was written to match the existing xml format the procedure requires
            Using sw As New System.IO.StringWriter()
                Using xw As New XmlTextWriter(sw)
                    xw.WriteStartElement("invoices")

                    For Each row As GridViewRow In Me.gvInvoices.Rows
                        If DirectCast(row.FindControl(GRID_SELECT_ID), CheckBox).Checked Then
                            xw.WriteStartElement("invoice")

                            Dim i As Integer = 1
                            For Each str As String In DirectCast(row.FindControl(GRID_PAYINFO_ID), HiddenField).Value.Split(PAY_INFO_DELIMITER)
                                Select Case i
                                    Case 1
                                        xw.WriteElementString("number", str)
                                    Case 2
                                        xw.WriteElementString("customer", str)
                                    Case 3
                                        xw.WriteElementString("ShipTo", str)
                                    Case Else
                                        Exit For
                                End Select
                                i += 1
                            Next

                            xw.WriteEndElement() 'invoice

                        End If
                    Next

                    xw.WriteEndElement() 'invoices

                End Using

                Return Common.WriteInvoicesToEpay(sw.ToString, refNo, Me.SessionData("LOGON_USER"))

            End Using

        Catch ex As Exception
            Throw

        End Try

    End Function

    Private Function AreInvoicesSelected() As Boolean
        Try
            For Each row As GridViewRow In Me.gvInvoices.Rows
                If DirectCast(row.FindControl(GRID_SELECT_ID), CheckBox).Checked Then
                    Return True

                End If

            Next

            Return False

        Catch ex As Exception
            Throw

        End Try

    End Function


    Private Function GetDefaultDateSpanInDays() As Integer
        Dim sbSQL As New System.Text.StringBuilder
        Dim dataReader As System.Data.SqlClient.SqlDataReader = Nothing
        Dim defaultDateSpanInDays As Integer = 0

        Try
            sbSQL.Append("EXEC Ashley.dbo.usp_GetEpayDefaultDateSpanInDays ")

            dataReader = DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL.ToString)

            If dataReader.Read Then
                defaultDateSpanInDays = dataReader.GetValue(0)

            End If

            Return defaultDateSpanInDays

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

            If dataReader IsNot Nothing Then
                dataReader.Close()
                dataReader = Nothing

            End If

        End Try

    End Function

    Private Sub TranslateDataBoundControlHeaders(ByVal currentRow As GridViewRow)
        Try
            DirectCast(currentRow.FindControl(GRID_STATUS_HEADER_ID), LinkButton).Text = "Status"
            DirectCast(currentRow.FindControl(GRID_INVOICENUMBER_HEADER_ID), LinkButton).Text = "Invoice #"
            DirectCast(currentRow.FindControl(GRID_CREDITNUMBER_HEADER_ID), LinkButton).Text = "Credit #"
            DirectCast(currentRow.FindControl(GRID_SHIPTO_HEADER_ID), LinkButton).Text = "ShipTo"
            DirectCast(currentRow.FindControl(GRID_INVOICEDATE_HEADER_ID), LinkButton).Text = "Inv Date"
            DirectCast(currentRow.FindControl(GRID_ORDERNUMBER_HEADER_ID), LinkButton).Text = "Order #"
            DirectCast(currentRow.FindControl(GRID_TRIPNUMBER_HEADER_ID), LinkButton).Text = "Trip #"
            DirectCast(currentRow.FindControl(GRID_RPPNUMBER_HEADER_ID), LinkButton).Text = "RPP #"
            DirectCast(currentRow.FindControl(GRID_PONUMBER_HEADER_ID), LinkButton).Text = "PO #"
            DirectCast(currentRow.FindControl(GRID_INVOICEAMOUNT_HEADER_ID), LinkButton).Text = "Inv Amt"
            DirectCast(currentRow.FindControl(GRID_AMOUNTPAID_HEADER_ID), LinkButton).Text = "Amt Pd"
            DirectCast(currentRow.FindControl(GRID_BALANCE_HEADER_ID), LinkButton).Text = "Balance"
            DirectCast(currentRow.FindControl(GRID_CODE_HEADER_ID), LinkButton).Text = "Code"
            DirectCast(currentRow.FindControl(GRID_DAYS_HEADER_ID), LinkButton).Text = "Days"

        Catch ex As Exception
            Throw

        End Try

    End Sub

#End Region
    
End Class