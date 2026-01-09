
#Region " Revision History "

'   Date            Programmer      Description
'   ============    ============    ========================================
'   03/28/2012      TKiel           Made changes for uniting OpenInvoicesBLL project into Epay
'  
#End Region

Imports System.Data.SqlClient
Imports Ashley45.CorpSalesSys.Base.Common

Partial Public Class AnalystReport
    Inherits EpayBasePage

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strSortColumnName As String = String.Empty
        Dim blnSortAscending As Boolean = True

        Me.Navigation.AppAuthorization = Me.VerifyAppAuthorization("EPAYANLYST", True)

        Try

            Me.lblErrorMsg.Visible = False

            If Me.IsPostBack = False Then
                Me.txtDate.Text = DateAdd(DateInterval.Day, -90, Today)

                If Me.hdnSortColumn.Value.ToString <> "" Then
                    strSortColumnName = CStr(Me.hdnSortColumn.Value)

                    If Me.hdnSortAscending.Value.ToString = "" Then
                        blnSortAscending = True
                    Else
                        blnSortAscending = CType(Me.hdnSortAscending.Value, Boolean)
                    End If
                Else
                    strSortColumnName = "dtea"
                    blnSortAscending = True
                End If

                'RN647523 - Piech - 3/4/10 - Load the territory listbox ---
                Me.LoadTerritoryListbox()

                Me.hdnSortAscending.Value = blnSortAscending
                Me.hdnSortColumn.Value = strSortColumnName

            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Private Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSearch.Click
        Try
            'RN647523 - Piech - 3/4/10 - Only Execute report when search is clicked.

            If txtDate.Text = Nothing Or Not IsDate(txtDate.Text) = True Then
                Me.txtDate.Text = DateAdd(DateInterval.Day, -90, Today)
            End If

            Me.GetReport(0)

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

#End Region

#Region " Private Functions/Methods "

    Private Sub GetReport(ByVal iPageNumber As Integer)
        Dim drDataReader As SqlDataReader = Nothing
        Dim dtDatatable As DataTable = Nothing
        Try

            drDataReader = Common.LoadEpayAnalystReport(txtCustomerNumber.Text, _
                                            txtReferenceNo.Text, _
                                            txtDate.Text, txtConfirmationNo.Text, lstTerritory.SelectedValue.ToString, _
                                            Me.hdnSortColumn.Value, CType(Me.hdnSortAscending.Value, Boolean))
            If drDataReader.HasRows Then


                dtDatatable = New DataTable()

                dtDatatable.Load(drDataReader)

                grvReport.DataSource = dtDatatable
                grvReport.PageIndex = iPageNumber
                Me.grvReport.DataBind()

            Else

                grvReport.DataSource = Nothing
                grvReport.DataBind()

            End If

        Catch ex As Exception
            Throw ex
        Finally
            If dtDatatable IsNot Nothing Then dtDatatable.Dispose()
            If drDataReader IsNot Nothing AndAlso Not drDataReader.IsClosed Then drDataReader.Close()
        End Try

    End Sub

    'RN647523 - Piech - 3/4/10 - Populate the Territory list box ---
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

#End Region


    Protected Sub grvReport_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grvReport.PageIndexChanging

        Try

            If e Is Nothing Then
                Exit Sub
            End If

            Me.GetReport(e.NewPageIndex)

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub grvReport_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles grvReport.RowCreated
        Ashley.Web.UI.GridViewExportUtil.GridView_RowCreated(Me.grvReport, e)
    End Sub

    Protected Sub grvReport_Sorting(sender As Object, e As GridViewSortEventArgs) Handles grvReport.Sorting
        Dim drDataReader As SqlDataReader = Nothing
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
            drDataReader = Common.LoadEpayAnalystReport(txtCustomerNumber.Text, _
                                            txtReferenceNo.Text, _
                                            txtDate.Text, txtConfirmationNo.Text, lstTerritory.SelectedValue.ToString, _
                                            Me.hdnSortColumn.Value, CType(Me.hdnSortAscending.Value, Boolean))

            dtDatatable = New DataTable()

            dtDatatable.Load(drDataReader)
            dvDataView = New DataView(dtDatatable)

            If Me.hdnSortAscending.Value = True Then
                dvDataView.Sort = e.SortExpression & " ASC"
            Else
                dvDataView.Sort = e.SortExpression & " DESC"
            End If

            grvReport.DataSource = dvDataView
            grvReport.DataBind()

        Catch ex As Exception
            Throw ex
        Finally
            If dvDataView IsNot Nothing Then dvDataView.Dispose()
            If dtDatatable IsNot Nothing Then dtDatatable.Dispose()
            If drDataReader IsNot Nothing AndAlso Not drDataReader.IsClosed Then drDataReader.Close()
        End Try

    End Sub
End Class

