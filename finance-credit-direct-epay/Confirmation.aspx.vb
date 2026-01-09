
#Region " Revision History "

'   Date            Programmer          Description
'   ============    ============        ========================================
'   07/16/09        Tessa Lockington    Add confirm box to the cancel button.  
'   03/28/2012      TKiel               Made changes for uniting OpenInvoicesBLL project into Epay
'   06/22/2012      TKiel               Added GetPaymentContactEmailAddress to pass to US Bank
'   07/23/2012      TKiel               Changed disallowLogin query string from 'Y' to 'N'

#End Region

Imports GrapeCity.ActiveReports.Export.Pdf.Section

Partial Public Class Confirmation
    Inherits EpayBasePage

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not Me.IsPostBack Then
                Me.cmdCancel.Attributes.Add("onclick", _
                       "return confirm('Are you sure you want to cancel payment?')")
            End If

            If Request.Params("CusNo") <> Nothing Then
                cmdCancel.Enabled = False
                cmdOK.Enabled = False

            End If

            If Request.Params("RefNo") <> "" Then
                lblReferenceNumber.Text = Request.Params("RefNo")
                lblTotalAmt.Text = FormatCurrency(GetTotal(Request.Params("RefNo")), 2)

                GetReport()

            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Protected Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        Dim totalAmt As Decimal
        Dim url As StringBuilder
        Dim isTestEnvironment As Boolean = False
        Dim sCustomerNumber As String = String.Empty
        Dim sContactEmailAddress As String = String.Empty
        Dim status As String = Nothing
        Try
            url = New StringBuilder("https://epayment.epymtservice.com/epay.jhtml?")
            'Check to see if we are in the test environment.
            If Me.SessionData("SITENAME").Equals("DEV.ASHLEYDIRECT.COM") Or
                    Me.SessionData("SITENAME").Equals("STAGE.ASHLEYDIRECT.COM") Then

                isTestEnvironment = True

            End If
            If isTestEnvironment = True Then

                lblErrorMsg.Text = "Can not call USBank from Test!!:" & url.ToString
                Exit Sub

            End If

            status = Common.GetEpayStatus(Me.SessionData("CUSTOMERNUMBER"), Integer.Parse(lblReferenceNumber.Text))
            'Added the below if condition to stop resubmission as part of incident no :1314311 
            If status <> "Sent" Then
                totalAmt = Decimal.Parse(GetTotal(Request.Params("RefNo")))
                'Trace.Warn(lblTotalAmt.Text & ":" & lblReferenceNumber.Text & "totalAmt" & FormatCurrency(Request.Params("totalAmt"), 2))

                If (totalAmt > 0) Then
                    sCustomerNumber = Me.SessionData("CUSTOMERNUMBER")

                    Common.UpdateEpayStatus(sCustomerNumber, Integer.Parse(lblReferenceNumber.Text), "Sent", Me.SessionData("LOGON_USER"))

                    sContactEmailAddress = Me.GetPaymentContactEmailAddress(sCustomerNumber)


                    url.Append("productCode=OpenInvoices&billerId=INV&billerGroupId=ASH&")
                    url.Append("disallowLogin=N&paymentMethod=ACH&paymentType=Single")
                    url.Append("&amountDue=" & Format(totalAmt, "0.00"))
                    url.Append("&billerPayorId=" & sCustomerNumber)
                    url.Append("&ReferenceNumber=" & lblReferenceNumber.Text)
                    'url.Append("&firstName=" & Me.SessionData("CUSTOMERNAME"))
                    'url.Append("&lastName=" & Me.SessionData("ALLSHIPTOS"))
                    url.Append("&streetAddress1=" & Server.UrlEncode(Me.SessionData("CUSTOMERADDRESS1")))
                    url.Append("&streetAddress2=" & Server.UrlEncode(Me.SessionData("CUSTOMERADDRESS2")))
                    url.Append("&city=" & Server.UrlEncode(Me.SessionData("CUSTOMERCITY")))
                    url.Append("&stateRegion=" & Server.UrlEncode(Me.SessionData("CUSTOMERSTATE")))
                    url.Append("&companyName=" & Server.UrlEncode(Me.SessionData("CUSTOMERNAME")))
                    If Me.SessionData("CUSTOMERZIPCODE").Length > 5 Then
                        url.Append("&zipPostalcode=" & Me.SessionData("CUSTOMERZIPCODE").Remove(5))
                    Else
                        url.Append("&zipPostalcode=" & Me.SessionData("CUSTOMERZIPCODE"))
                    End If
                    url.Append("&countryCode=" & Me.SessionData("ALLSHIPTOS"))
                    url.Append("&emailAddress=" & Server.UrlEncode(sContactEmailAddress))
                    url.Append("&phoneNumber=" & Server.UrlEncode(Me.SessionData("CUSTOMERPHONE").Replace("-", "")))

                    Me.Auditor.AddAudit("EpayPayment", url.ToString, Diagnostics.EventLogEntryType.Information)

                    Response.Redirect(url.ToString, False)

                End If

            Else

                lblErrorMsg.Text = "The payment has already been sent to US Bank."

            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        Finally
            totalAmt = Nothing
            url = Nothing

        End Try

    End Sub

    Protected Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Dim status As String = Nothing

        Try
            status = Common.GetEpayStatus(Me.SessionData("CUSTOMERNUMBER"), Integer.Parse(lblReferenceNumber.Text))

            If status = "verifying" Then
                Common.DeleteEpayRecords(Me.SessionData("CUSTOMERNUMBER"), Integer.Parse(lblReferenceNumber.Text), Me.SessionData("LOGON_USER"))
                Response.Redirect("main.aspx", False)

            Else
                lblErrorMsg.Text = "Can not delete. It has already been sent to US Bank."

            End If

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

    Protected Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdPrint.Click
        Try
            Me.ExportPdf()

        Catch exThread As Threading.ThreadAbortException

        Catch ex As Exception
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

  

#End Region

#Region " Private Functions/Methods "

    Private Sub GetReport()
        Dim sCustomerNumber As String = String.Empty

        Try

            If Request.Params("CusNo") = Nothing Then
                sCustomerNumber = Me.SessionData("CUSTOMERNUMBER")

            Else
                sCustomerNumber = Request.QueryString("CusNo")

            End If

            Using dt As DataTable = Common.LoadEpayInvoicesByRefNumber(sCustomerNumber, Integer.Parse(lblReferenceNumber.Text))
                Me.grvEPayInvoices.DataSource = dt
                Me.grvEPayInvoices.DataBind()
            End Using

        Catch ex As Exception
            Throw

        Finally

        End Try

    End Sub

    'exports the report in PDF Format
    Private Sub ExportPdf()
        Dim oReport As ReportEpayConfirmation = Nothing

        Try
            oReport = New ReportEpayConfirmation

            Dim sCustNo As String = Request.Params("CusNo")
            If sCustNo Is Nothing Then
                sCustNo = Me.SessionData("CUSTOMERNUMBER")
            End If

            oReport.SendParameters(sCustNo, Integer.Parse(Request.Params("RefNo")), Server.MapPath(""))

            Try
                'g_oReportSettings.Halted = False
                oReport.Run(False)

            Catch eRunReport As GrapeCity.ActiveReports.ReportException
                ' Failure running report, just report the error to the user:
                Response.Clear()
                Response.Write("<h1>Error running report:</h1>")
                Response.Write(eRunReport.ToString())
                Return

            End Try

            Dim reportBytes As Byte() = Nothing
            Using pdfExport As New PdfExport
                Using oMemStream As New System.IO.MemoryStream
                    pdfExport.Export(oReport.Document, oMemStream)
                    reportBytes = oMemStream.ToArray
                End Using
            End Using

            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=MyPDF.pdf")
            Response.BinaryWrite(reportBytes)

            HttpContext.Current.ApplicationInstance.CompleteRequest()

        Catch ex As Exception
            Throw

        Finally
            If oReport IsNot Nothing Then
                oReport.Document.Dispose()
                oReport.Dispose()
            End If

        End Try

    End Sub

    Private Function GetTotal(ByVal refNo As String) As String
        Dim total As String = "0.00"
        Dim dataReader As System.Data.SqlClient.SqlDataReader = Nothing

        Try

            If Request.Params("CusNo") = Nothing Then
                dataReader = Common.GetTotals(Me.SessionData("CUSTOMERNUMBER"), refNo)

            Else
                dataReader = Common.GetTotals(Request.Params("CusNo"), refNo)

            End If

            If dataReader.Read Then
                If Not IsDBNull(dataReader.GetValue(2)) Then
                    total = dataReader.GetValue(2)

                End If

            End If

            Return total

        Catch ex As Exception
            Throw

        Finally
            If dataReader IsNot Nothing Then
                dataReader.Close()
                dataReader = Nothing
            End If

        End Try

    End Function

    Private Function GetPaymentContactEmailAddress(ByVal sCustomerNumber As String)
        Dim dtDataTable As DataTable = Nothing
        Dim colParameters As Collection = Nothing

        Try
            colParameters = New Collection
            colParameters.Add(New SqlClient.SqlParameter("@CustomerNumber", sCustomerNumber))

            dtDataTable = DataAccess.ExecuteStoredProcedure("AFI_Dynamic", "Ashley.dbo.usp_EpayGetPaymentContactEmailAddress", DataAccess.StoredProcedureReturnType.DataTable, colParameters)

            If dtDataTable.Rows.Count > 0 Then
                Return dtDataTable.Rows(0).Item("ContactEmail").ToString.Trim

            Else
                Return String.Empty

            End If

        Catch ex As Exception
            Throw

        Finally
            If dtDataTable IsNot Nothing Then
                dtDataTable.Dispose()
                dtDataTable = Nothing
            End If

            colParameters = Nothing

        End Try

    End Function

#End Region

End Class