#Region " Revision History "

'   Date            Programmer      Description
'   ============    ============    ========================================
'   03/28/2012      TKiel           Made changes for uniting OpenInvoicesBLL project into Epay

#End Region

Partial Public Class History
    Inherits EpayBasePage

#Region " Variable Declarations "

    Private m_bIsExcelResponseTerminating As Boolean = False

    'Private GRID_DATE_ADDED_HEADER_ID As String = "lkbDateAdded"
    'Private GRID_CONFIRMATION_NUMBER_HEADER_ID As String = "lkbConfirmationNumber"
    'Private GRID_STATUS_HEADER_ID As String = "lkbStatus"
    'Private GRID_REFERENCE_NUMBER_HEADER_ID As String = "lkbReferenceNumber"
    'Private GRID_TYPE_HEADER_ID As String = "lkbType"
    Private GRID_GROSS_AMOUNT_HEADER_ID As String = "lkbGrossAmount"
    Private GRID_DISCOUNT_HEADER_ID As String = "lkbDiscount"
    Private GRID_AMOUNT_CHARGED_HEADER_ID As String = "lkbAmountCharged"
    Private GRID_REFERENCE_NUMBER_ID As String = "hplReferenceNumber"
    Private GRID_TYPE_CODE_ID As String = "hdnTypeCode"
    Private GRID_TYPE_ID As String = "lblType"

    Private m_sHistoryType As String = String.Empty
    Private m_sReturnType As String = String.Empty
    Private m_sCurrentType As String = String.Empty

    Enum EpayHistoryColumnIndices
        DateAdded = 0
        ConfirmationNumber = 1
        Status = 2
        ReferenceNumber = 3
        Type = 4
        GrossAmount = 5
        Discount = 6
        AmountCharged = 7

    End Enum

#End Region

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Navigation.AppAuthorization = Me.VerifyAppAuthorization("EPAYANLYST", False)

        Try
            If Me.CustomerNumber <> String.Empty Then
                Me.lblErrorMsg.Visible = False
                Me.lblNoAccountSelected.Visible = False

                If Not Me.IsPostBack Then
                    Me.txtDate.Text = DateAdd(DateInterval.Day, -90, Today)
                    Me.LoadGrid(1)

                End If

            Else
                Me.lblNoAccountSelected.Visible = True

            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        If writer Is Nothing Then Exit Sub

        If Me.m_bIsExcelResponseTerminating = False Then
            MyBase.Render(writer)

        Else
            Me.m_bIsExcelResponseTerminating = False

        End If

    End Sub

    Private Sub btnExportToExcel_Click(sender As Object, e As EventArgs) Handles btnExportToExcel.Click
        If grvEpayHistory.Rows.Count > 0 Then
            Dim lstCurrencyColumnIndexes As New List(Of Integer)
            lstCurrencyColumnIndexes.Insert(0, Me.grvEpayHistory.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.grvEpayHistory.HeaderRow.FindControl(GRID_GROSS_AMOUNT_HEADER_ID), LinkButton).Parent, TableCell)))
            lstCurrencyColumnIndexes.Insert(1, Me.grvEpayHistory.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.grvEpayHistory.HeaderRow.FindControl(GRID_DISCOUNT_HEADER_ID), LinkButton).Parent, TableCell)))
            lstCurrencyColumnIndexes.Insert(2, Me.grvEpayHistory.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.grvEpayHistory.HeaderRow.FindControl(GRID_AMOUNT_CHARGED_HEADER_ID), LinkButton).Parent, TableCell)))

            Dim lstRightAlignedColumns As New List(Of Integer)
            lstRightAlignedColumns.Insert(0, Me.grvEpayHistory.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.grvEpayHistory.HeaderRow.FindControl(GRID_GROSS_AMOUNT_HEADER_ID), LinkButton).Parent, TableCell)))
            lstRightAlignedColumns.Insert(1, Me.grvEpayHistory.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.grvEpayHistory.HeaderRow.FindControl(GRID_DISCOUNT_HEADER_ID), LinkButton).Parent, TableCell)))
            lstRightAlignedColumns.Insert(2, Me.grvEpayHistory.HeaderRow.Cells.GetCellIndex(DirectCast(DirectCast(Me.grvEpayHistory.HeaderRow.FindControl(GRID_AMOUNT_CHARGED_HEADER_ID), LinkButton).Parent, TableCell)))

            Ashley.Web.UI.GridViewExportUtil.Export("EpayHistory.xls", Me.grvEpayHistory, Me.m_bIsExcelResponseTerminating, lstCurrencyColumnIndexes, lstRightAlignedColumns)
        End If
    End Sub

    Private Sub cmdSearch_Click(sender As Object, e As EventArgs) Handles cmdSearch.Click
        If txtDate.Text = Nothing Or Not IsDate(txtDate.Text) Then
            Me.txtDate.Text = DateAdd(DateInterval.Day, -90, Today)
        End If

        Me.LoadGrid(1)

    End Sub

    Private Sub grvEpayHistory_Init(sender As Object, e As EventArgs) Handles grvEpayHistory.Init
        Me.m_sHistoryType = gt("HISTORY")
        Me.m_sReturnType = gt("RETURN")
        Me.m_sCurrentType = gt("CURRENT")

    End Sub

    Private Sub grvEpayHistory_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles grvEpayHistory.RowCreated
        Dim imgImageSort As Image = Nothing
        Dim lkbSort As LinkButton = Nothing
        Dim sSortExpression As String = String.Empty
        Dim bSortAscending As Boolean = False

        Try
            If e.Row.RowType <> DataControlRowType.Header Then
                Return
            End If

            If Me.ViewState("SortExpression") IsNot Nothing _
                    AndAlso Me.ViewState("SortAscending") IsNot Nothing _
                    AndAlso Me.ViewState("SortExpression").ToString <> String.Empty _
                    AndAlso Me.ViewState("SortAscending").ToString <> String.Empty Then
                sSortExpression = Me.ViewState("SortExpression").ToString
                bSortAscending = CBool(Me.ViewState("SortAscending").ToString)

            End If

            If sSortExpression <> String.Empty Then
                For Each tableCell As TableCell In e.Row.Cells
                    If Not tableCell.HasControls() Then
                        Continue For
                    End If

                    lkbSort = TryCast(tableCell.Controls(1), LinkButton)

                    If lkbSort Is Nothing Then
                        Continue For
                    End If

                    If lkbSort.CommandArgument = sSortExpression Then
                        imgImageSort = New Image
                        imgImageSort.ImageAlign = ImageAlign.AbsMiddle
                        imgImageSort.Width = 20

                        If bSortAscending Then
                            imgImageSort.ImageUrl = "~/Images/asc.png"
                        Else
                            imgImageSort.ImageUrl = "~/Images/desc.png"
                        End If

                        tableCell.Controls.Add(imgImageSort)

                    End If

                Next

            End If

        Catch ex As Exception
            Throw

        Finally
            If imgImageSort IsNot Nothing Then imgImageSort.Dispose()
            If lkbSort IsNot Nothing Then lkbSort.Dispose()

        End Try

    End Sub

    Private Sub grvEpayHistory_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grvEpayHistory.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(EpayHistoryColumnIndices.DateAdded).Text = IIf(Date.TryParse(DirectCast(e.Row.DataItem, Data.DataRowView).Item("DateAdded").ToString, Nothing), _
                                                                        CDate(DirectCast(e.Row.DataItem, Data.DataRowView).Item("DateAdded").ToString).ToShortDateString, _
                                                                        String.Empty).ToString
            e.Row.Cells(EpayHistoryColumnIndices.ConfirmationNumber).Text = DirectCast(e.Row.DataItem, Data.DataRowView).Item("ConfirmationNumber").ToString
            e.Row.Cells(EpayHistoryColumnIndices.Status).Text = DirectCast(e.Row.DataItem, Data.DataRowView).Item("Status").ToString
            DirectCast(e.Row.FindControl(GRID_TYPE_CODE_ID), HiddenField).Value = DirectCast(e.Row.DataItem, Data.DataRowView).Item("Type").ToString

            Select Case DirectCast(e.Row.DataItem, Data.DataRowView).Item("Type").ToString
                Case "H"
                    DirectCast(e.Row.FindControl(GRID_TYPE_ID), Label).Text = Me.m_sHistoryType

                Case "R"
                    DirectCast(e.Row.FindControl(GRID_TYPE_ID), Label).Text = Me.m_sReturnType

                Case "C"
                    DirectCast(e.Row.FindControl(GRID_TYPE_ID), Label).Text = Me.m_sCurrentType

            End Select

            e.Row.Cells(EpayHistoryColumnIndices.GrossAmount).Text = IIf(Decimal.TryParse(DirectCast(e.Row.DataItem, Data.DataRowView).Item("GrossAmount").ToString, Nothing), _
                                                                            CDec(DirectCast(e.Row.DataItem, Data.DataRowView).Item("GrossAmount").ToString).ToString("##,##0.00"),
                                                                            String.Empty)
            e.Row.Cells(EpayHistoryColumnIndices.Discount).Text = IIf(Decimal.TryParse(DirectCast(e.Row.DataItem, Data.DataRowView).Item("Discount").ToString, Nothing), _
                                                                        CDec(DirectCast(e.Row.DataItem, Data.DataRowView).Item("Discount").ToString).ToString("##,##0.00"),
                                                                        String.Empty)
            e.Row.Cells(EpayHistoryColumnIndices.AmountCharged).Text = IIf(Decimal.TryParse(DirectCast(e.Row.DataItem, Data.DataRowView).Item("AmountCharged").ToString, Nothing), _
                                                                            CDec(DirectCast(e.Row.DataItem, Data.DataRowView).Item("AmountCharged").ToString).ToString("##,##0.00"),
                                                                            String.Empty)

            DirectCast(e.Row.FindControl(GRID_REFERENCE_NUMBER_ID), HyperLink).Text = DirectCast(e.Row.DataItem, Data.DataRowView).Item("ReferenceNumber").ToString
            DirectCast(e.Row.FindControl(GRID_REFERENCE_NUMBER_ID), HyperLink).NavigateUrl = String.Format("javascript:ViewEpayPayment('{0}', '{1}');",
                                                                                                           DirectCast(e.Row.FindControl(GRID_REFERENCE_NUMBER_ID), HyperLink).Text,
                                                                                                           DirectCast(e.Row.FindControl(GRID_TYPE_CODE_ID), HiddenField).Value)

        End If

    End Sub

    Private Sub grvEpayHistory_Sorting(sender As Object, e As GridViewSortEventArgs) Handles grvEpayHistory.Sorting
        Dim sSortExpression As String = String.Empty
        Dim bSortAscending As Boolean = False

        If Me.ViewState("SortExpression") IsNot Nothing _
                    AndAlso Me.ViewState("SortAscending") IsNot Nothing _
                    AndAlso Me.ViewState("SortExpression").ToString <> String.Empty _
                    AndAlso Me.ViewState("SortAscending").ToString <> String.Empty Then
            sSortExpression = Me.ViewState("SortExpression").ToString
            bSortAscending = CBool(Me.ViewState("SortAscending").ToString)

        End If

        If sSortExpression = e.SortExpression Then
            Me.ViewState("SortAscending") = If(bSortAscending, False, True)

        Else
            Me.ViewState("SortExpression") = e.SortExpression
            Me.ViewState("SortAscending") = True

        End If

        Me.LoadGrid(1)

    End Sub

    Protected Sub Page_Changed(ByVal sender As Object, ByVal e As EventArgs)
        Me.LoadGrid(Integer.Parse(CType(sender, LinkButton).CommandArgument))

    End Sub

#End Region

#Region " Private Functions/Methods "

    Private Sub LoadGrid(ByVal iCurrentPage As Integer)
        Dim colParameters As Collection = Nothing
        Dim lstListItems As List(Of ListItem) = Nothing
        Dim sSortExpression As String = String.Empty
        Dim bSortAscending As Boolean = False

        Try
            If Me.ViewState("SortExpression") IsNot Nothing _
                    AndAlso Me.ViewState("SortAscending") IsNot Nothing _
                    AndAlso Me.ViewState("SortExpression").ToString <> String.Empty _
                    AndAlso Me.ViewState("SortAscending").ToString <> String.Empty Then
                sSortExpression = Me.ViewState("SortExpression").ToString
                bSortAscending = CBool(Me.ViewState("SortAscending").ToString)

            End If

            colParameters = New Collection
            colParameters.Add(New SqlClient.SqlParameter("@CustomerNo", Me.CustomerNumber))
            colParameters.Add(New SqlClient.SqlParameter("@RefNo", Me.txtReferenceNo.Text.Trim))
            colParameters.Add(New SqlClient.SqlParameter("@InvoiceNo", Me.txtInvoiceNumber.Text.Trim))
            colParameters.Add(New SqlClient.SqlParameter("@ConfirmationNo", Me.txtConfirmationNo.Text.Trim))
            colParameters.Add(New SqlClient.SqlParameter("@PaymentDate", Me.txtDate.Text))
            colParameters.Add(New SqlClient.SqlParameter("@SortColumn", sSortExpression))
            colParameters.Add(New SqlClient.SqlParameter("@SortAscending", bSortAscending))
            colParameters.Add(New SqlClient.SqlParameter("@CurrentPage", iCurrentPage))
            colParameters.Add(New SqlClient.SqlParameter("@PageSize", 500))

            Using dtDataTable As DataTable = DataAccess.ExecuteStoredProcedure("AFI_Batch", "Datawhse.dbo.usp_GetEpayInvoiceHistory_1", DataAccess.StoredProcedureReturnType.DataTable, colParameters)
                Me.grvEpayHistory.DataSource = dtDataTable
                Me.grvEpayHistory.DataBind()

                If dtDataTable IsNot Nothing _
                        AndAlso dtDataTable.Rows.Count > 0 _
                        AndAlso dtDataTable.Columns.Contains("PageCount") Then
                    lstListItems = Ashley.Web.UI.GridPaging.LoadPagerItems(10, CInt(dtDataTable.Rows(0).Item("PageCount").ToString), iCurrentPage)

                    Me.rptPager.DataSource = lstListItems
                    Me.rptPager.DataBind()

                End If

            End Using

        Catch ex As Exception
            Throw

        End Try

    End Sub

#End Region

End Class