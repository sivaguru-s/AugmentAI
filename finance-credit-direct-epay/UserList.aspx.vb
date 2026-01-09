#Region " Revision History "

'   Date            Programmer          Description
'   ============    ============        ========================================
'   08/20/2010      TKiel               Creation
'   03/28/2012      TKiel               Made changes for uniting OpenInvoicesBLL project into Epay

#End Region

Imports System.Xml
Imports System.Data.SqlClient



Partial Public Class UserList
    Inherits EpayBasePage
#Region " Variable/Constant Declarations "

    Dim bIsExcelResponseTerminating As Boolean = False    

#End Region

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strSortColumnName As String = String.Empty
        Dim blnSortAscending As Boolean = False

        Try
            Me.Navigation.AppAuthorization = Me.VerifyAppAuthorization("EPAYANLYST", True)

            If Me.Navigation.AppAuthorization Then
                If Not Me.IsPostBack Then

                    If Me.hdnSortColumn.Value.ToString <> String.Empty Then
                        strSortColumnName = CStr(Me.hdnSortColumn.Value)

                        If Me.hdnSortAscending.Value.ToString = String.Empty Then
                            blnSortAscending = True

                        Else
                            blnSortAscending = CType(Me.hdnSortAscending.Value, Boolean)

                        End If
                    Else
                        strSortColumnName = String.Empty
                        blnSortAscending = True

                    End If
                    Me.Hdnpageno.Value = 0
                    Me.hdnSortAscending.Value = blnSortAscending
                    Me.hdnSortColumn.Value = strSortColumnName
                    Me.lblDate.Text = DateTime.Today

                    Me.LoadBillToStateListbox()
                    Me.LoadTerritoryListbox()
                    Me.LoadTermsCodeListbox()

                    Me.GetReport(1, 500)

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
    Private Sub btnReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Try
            Me.txtAccountNumber.Text = String.Empty
            Me.txtAccountName.Text = String.Empty
            Me.lstBillToState.SelectedValue = "All"
            Me.lstTerritory.SelectedValue = "All"
            Me.lstTermsCode.SelectedValue = "All"

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Private Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSearch.Click
        Try
            Me.GetReport(1, 500)

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub



    Protected Sub btnPrintablePage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrintablePage.Click
        Try
            Response.Redirect("UserListPrintablePage.aspx?accountnumber=" & Me.txtAccountNumber.Text & "&accountname=" & Me.txtAccountName.Text & _
                        "&billtostate=" & Me.lstBillToState.SelectedValue.ToString & "&territory=" & Me.lstTerritory.SelectedValue.ToString & _
                        "&termscode=" & Me.lstTermsCode.SelectedValue.ToString & "")

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Protected Sub cmdExportToExcel_Click(ByVal sender As System.Object, ByVal e As ImageClickEventArgs) Handles cmdExportToExcel.Click
        If grvUsers.Rows.Count > 0 Then


            Dim currencyColumnIndexes As List(Of Integer) = Nothing
            Try

                Ashley.Web.UI.GridViewExportUtil.Export("EpaymentUserList.xls", Me.grvUsers, bIsExcelResponseTerminating, currencyColumnIndexes)
            Catch ex As Exception
                Throw ex
            Finally

            End Try
        End If
    End Sub

#End Region

#Region " Private Functions/Methods "

    Private Sub LoadBillToStateListbox()
        Dim drDataReader As SqlClient.SqlDataReader = Nothing

        Try
            drDataReader = BillToStateDataReader

            Me.lstBillToState.DataValueField = "Bill-To-State"
            Me.lstBillToState.DataTextField = "Bill-To-State"
            Me.lstBillToState.DataSource = drDataReader
            Me.lstBillToState.DataBind()

        Finally
            If drDataReader IsNot Nothing AndAlso Not drDataReader.IsClosed Then drDataReader.Close()

        End Try

    End Sub

    Private Sub LoadTerritoryListbox()
        Dim drDataReader As SqlClient.SqlDataReader = Nothing

        Try
            drDataReader = Me.CreditTerritoryDataReader

            Me.lstTerritory.DataValueField = "CreditTerritory"
            Me.lstTerritory.DataTextField = "CreditTerritory"
            Me.lstTerritory.DataSource = drDataReader
            Me.lstTerritory.DataBind()

        Finally
            If drDataReader IsNot Nothing AndAlso Not drDataReader.IsClosed Then drDataReader.Close()

        End Try

    End Sub

    Private Sub LoadTermsCodeListbox()
        Dim drDataReader As SqlClient.SqlDataReader = Nothing

        Try
            drDataReader = Me.TermsCodesDataReader

            Me.lstTermsCode.DataValueField = "Terms Code"
            Me.lstTermsCode.DataTextField = "Terms Code"
            Me.lstTermsCode.DataSource = drDataReader
            Me.lstTermsCode.DataBind()

        Finally
            If drDataReader IsNot Nothing AndAlso Not drDataReader.IsClosed Then drDataReader.Close()

        End Try

    End Sub
    Public Sub Page_Changed(ByVal sender As Object, ByVal e As EventArgs)
        Dim pageIndex As Integer = Integer.Parse(CType(sender, LinkButton).CommandArgument)    
        Me.GetReport(pageIndex, 500)

    End Sub
    Private Sub GetReport(ByVal iPageNumber As Integer, ByVal iPageSize As Integer)
        Dim dtDatatable As DataTable = Nothing
        Try
            dtDatatable = Common.LoadEpayUsers(Me.txtAccountNumber.Text, Me.txtAccountName.Text, Me.lstBillToState.SelectedValue.ToString, _
                                                         Me.lstTerritory.SelectedValue.ToString, Me.lstTermsCode.SelectedValue.ToString, iPageNumber, iPageSize)

            grvUsers.DataSource = dtDatatable            
            grvUsers.DataBind()

            If dtDatatable IsNot Nothing _
              AndAlso dtDatatable.Rows.Count > 0 _
              AndAlso dtDatatable.Columns.Contains("pageCount") Then
                rptPager.DataSource = Ashley.Web.UI.GridPaging.LoadPagerItems(iPageSize, CInt(dtDatatable.Rows(0).Item("pageCount").ToString), iPageNumber)
                Me.rptPager.DataBind()

            Else
                Me.rptPager.DataSource = Nothing
                Me.rptPager.DataBind()

            End If            

        Catch ex As Exception

        Finally

            If dtDatatable IsNot Nothing Then
                dtDatatable.Dispose()
                dtDatatable = Nothing
            End If
        End Try

    End Sub
#End Region
    Private Sub grvUsers_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles grvUsers.RowCreated
        Dim sortImage As Image = Nothing

        Try
            If e Is Nothing Then

                Exit Sub

            End If
            If e.Row.RowType = DataControlRowType.Header Then
                For Each field As DataControlField In grvUsers.Columns

                    If field.SortExpression = Me.hdnSortColumn.Value.ToString() Then
                        sortImage = New Image()
                        If Me.hdnSortAscending.Value = True Then
                            sortImage.ImageUrl = "images/asc.png"
                        Else
                            sortImage.ImageUrl = "images/desc.png"
                        End If
                        e.Row.Cells(grvUsers.Columns.IndexOf(field)).Controls.Add(sortImage)
                    End If
                Next
            End If          
        Catch ex As Exception

        Finally

            If sortImage IsNot Nothing Then
                sortImage.Dispose()
                sortImage = Nothing
            End If

        End Try
    End Sub

    Protected Sub grvUsers_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles grvUsers.Sorting
        Dim dvDataView As DataView = Nothing
        Dim dtDatatable As DataTable = Nothing
        Try


            If e Is Nothing Then
                Exit Sub

            End If

            Me.hdnSortColumn.Value = e.SortExpression


            If CType(Me.hdnSortAscending.Value, Boolean) = True Then
                Me.hdnSortAscending.Value = False

            Else
                Me.hdnSortAscending.Value = True

            End If
            dtDatatable = Common.LoadEpayUsers(Me.txtAccountNumber.Text, Me.txtAccountName.Text, Me.lstBillToState.SelectedValue.ToString, _
                                                             Me.lstTerritory.SelectedValue.ToString, Me.lstTermsCode.SelectedValue.ToString, Hdnpageno.Value, 500)

            dvDataView = New DataView(dtDatatable)

            If Me.hdnSortAscending.Value = True Then
                dvDataView.Sort = e.SortExpression & " ASC"
            Else
                dvDataView.Sort = e.SortExpression & " DESC"
            End If

            grvUsers.DataSource = dvDataView
            grvUsers.DataBind()

        Catch ex As Exception

        Finally
            If dvDataView IsNot Nothing Then dvDataView.Dispose()
            If dtDatatable IsNot Nothing Then dtDatatable.Dispose()

        End Try
    End Sub

End Class