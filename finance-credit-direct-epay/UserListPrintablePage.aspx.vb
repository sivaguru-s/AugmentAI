
#Region " Revision History "

'   Date            Programmer          Description
'   ============    ============        ========================================
'   08/23/2010      TKiel               Creation
'   03/28/2012      TKiel               Made changes for uniting OpenInvoicesBLL project into Epay

#End Region

Imports Infragistics.WebUI.UltraWebGrid
Imports System.Xml

Public Class UserListPrintablePage
    Inherits EpayBasePage

#Region " Variable/Constant Declarations "

    Private AccountNumber As String = String.Empty
    Private AccountName As String = String.Empty
    Private BillToState As String = String.Empty
    Private Territory As String = String.Empty
    Private TermsCode As String = String.Empty

#End Region

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Me.PageSetup()
            Me.GetReport(1, 0)

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

#End Region

#Region " Private Functions/Methods "

    Private Sub PageSetup()
        Try
            Me.lblReportTitle.Text = "E-Payment User List Report"

            Me.AccountNumber = Request.QueryString("accountnumber")
            Me.AccountName = Request.QueryString("accountname")
            Me.BillToState = Request.QueryString("billtostate")
            Me.Territory = Request.QueryString("territory")
            Me.TermsCode = Request.QueryString("termscode")

            If Me.AccountNumber <> String.Empty Then
                Me.lblSearchedAccountNumber.Text = Me.AccountNumber

            Else
                Me.lblSearchedAccountNumber.Text = "All"

            End If

            If Me.AccountName <> String.Empty Then
                Me.lblSearchedAccountName.Text = Me.AccountName

            Else
                Me.lblSearchedAccountName.Text = "All"

            End If

            Me.lblSearchedBillToState.Text = Me.BillToState
            Me.lblSearchedTerritory.Text = Me.Territory
            Me.lblSearchedTermsCode.Text = Me.TermsCode

        Catch ex As Exception
            Throw

        End Try

    End Sub

    Private Sub GetReport(ByVal iPageNumber As Integer, ByVal iPageSize As Integer)
        Try
            Using dt As DataTable = Common.LoadEpayUsers(Me.AccountNumber, Me.AccountName, Me.BillToState, Me.Territory, Me.TermsCode, iPageNumber, iPageSize)
                Me.grdUsers.DataSource = dt
                Me.grdUsers.DataBind()
            End Using

        Catch ex As Exception
            Throw

        End Try

    End Sub

#End Region

End Class