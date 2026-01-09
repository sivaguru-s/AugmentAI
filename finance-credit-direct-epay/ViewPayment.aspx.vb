Public Class ViewPayment
    Inherits EpayBasePage

#Region " Variable Declarations "

    Private m_bIsExcelResponseTerminating As Boolean = False
    Private m_iReferenceNumber As Integer = 0
    Private m_sType As String = String.Empty

    Enum EpayPaymentColumnIndices
        InvoiceNumber = 0
        GrossAmount = 1
        Discount = 2
        AmountCharged = 3

    End Enum

#End Region

#Region " Private Properties "

    Private ReadOnly Property ReferenceNumber As Integer
        Get
            If Not String.IsNullOrEmpty(Me.Request("RefNo")) _
                    And Integer.TryParse(Me.Request("RefNo"), Nothing) Then
                Me.m_iReferenceNumber = CInt(Me.Request("RefNo").Trim)
            End If

            Return Me.m_iReferenceNumber
        End Get
    End Property

    Private ReadOnly Property Type As String
        Get
            If Not String.IsNullOrEmpty(Me.Request("Type")) Then
                Me.m_sType = Me.Request("Type").Trim
            End If

            Return Me.m_sType
        End Get
    End Property

#End Region

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Me.lblReferenceNumber.Text = Me.ReferenceNumber

            Select Case Me.Type
                Case "H"
                    Me.lblType.Text = "HISTORY"

                Case "R"
                    Me.lblType.Text = "RETURN"

                Case "C"
                    Me.lblType.Text = "CURRENT"

            End Select

            Me.LoadGrid()

        End If

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
        Dim lstCurrencyColumnIndexes As New List(Of Integer)
        lstCurrencyColumnIndexes.Insert(0, EpayPaymentColumnIndices.GrossAmount)
        lstCurrencyColumnIndexes.Insert(1, EpayPaymentColumnIndices.Discount)
        lstCurrencyColumnIndexes.Insert(2, EpayPaymentColumnIndices.AmountCharged)

        Dim lstRightAlignedColumns As New List(Of Integer)
        lstRightAlignedColumns.Insert(0, EpayPaymentColumnIndices.GrossAmount)
        lstRightAlignedColumns.Insert(1, EpayPaymentColumnIndices.Discount)
        lstRightAlignedColumns.Insert(2, EpayPaymentColumnIndices.AmountCharged)

        Ashley.Web.UI.GridViewExportUtil.Export(String.Format("PaymentRefNo{0}{1}.xls", Me.lblReferenceNumber.Text, Me.lblType.Text), Me.grvViewPayment, Me.m_bIsExcelResponseTerminating, lstCurrencyColumnIndexes, lstRightAlignedColumns)

    End Sub

    Private Sub grvViewPayment_PreRender(sender As Object, e As EventArgs) Handles grvViewPayment.PreRender
        If Me.grvViewPayment.Rows.Count > 0 Then
            'This replaces <td> with <th> and adds the scope attribute
            Me.grvViewPayment.UseAccessibleHeader = True

            'This will add the <thead> and <tbody> elements
            Me.grvViewPayment.HeaderRow.TableSection = TableRowSection.TableHeader

        End If

    End Sub

    Private Sub grvViewPayment_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grvViewPayment.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(EpayPaymentColumnIndices.GrossAmount).Text = IIf(Decimal.TryParse(e.Row.DataItem("GrossAmount").ToString, Nothing), _
                                                                            CDec(e.Row.DataItem("GrossAmount").ToString).ToString("##,##0.00"),
                                                                            String.Empty)
            e.Row.Cells(EpayPaymentColumnIndices.Discount).Text = IIf(Decimal.TryParse(e.Row.DataItem("Discount").ToString, Nothing), _
                                                                        CDec(e.Row.DataItem("Discount").ToString).ToString("##,##0.00"),
                                                                        String.Empty)
            e.Row.Cells(EpayPaymentColumnIndices.AmountCharged).Text = IIf(Decimal.TryParse(e.Row.DataItem("AmountCharged").ToString, Nothing), _
                                                                            CDec(e.Row.DataItem("AmountCharged").ToString).ToString("##,##0.00"),
                                                                            String.Empty)

        End If

    End Sub

    Private Sub grvViewPayment_Sorting(sender As Object, e As GridViewSortEventArgs) Handles grvViewPayment.Sorting
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

        Me.LoadGrid()

    End Sub

#End Region

#Region " Private Functions/Methods "

    Private Sub LoadGrid()
        Dim sSortExpression As String = String.Empty
        Dim bSortAscending As Boolean = False

        If Me.ViewState("SortExpression") IsNot Nothing _
                    AndAlso Me.ViewState("SortAscending") IsNot Nothing _
                    AndAlso Me.ViewState("SortExpression").ToString <> String.Empty _
                    AndAlso Me.ViewState("SortAscending").ToString <> String.Empty Then
            sSortExpression = Me.ViewState("SortExpression").ToString
            bSortAscending = CBool(Me.ViewState("SortAscending").ToString)

        End If

        Dim colParameters As New Collection
        colParameters.Add(New SqlClient.SqlParameter("@CustomerNumber", Me.CustomerNumber))
        colParameters.Add(New SqlClient.SqlParameter("@ReferenceNumber", Me.ReferenceNumber))
        colParameters.Add(New SqlClient.SqlParameter("@Type", Me.Type))

        Using dtDataTable As DataTable = DataAccess.ExecuteStoredProcedure("AFI_Batch", "Datawhse.dbo.usp_GetEpayPayment", DataAccess.StoredProcedureReturnType.DataTable, colParameters)
            If sSortExpression <> String.Empty Then
                dtDataTable.DefaultView.Sort = sSortExpression & IIf(bSortAscending, " ASC", " DESC")

            End If

            Me.grvViewPayment.DataSource = dtDataTable
            Me.grvViewPayment.DataBind()

        End Using

    End Sub

#End Region

End Class