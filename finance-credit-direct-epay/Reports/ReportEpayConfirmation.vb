Imports GrapeCity.ActiveReports.Document

Imports GrapeCity.ActiveReports.Document.Section

Imports GrapeCity.ActiveReports.SectionReportModel

Imports GrapeCity.ActiveReports.Controls

Imports GrapeCity.ActiveReports




Public Class ReportEpayConfirmation

#Region " Attribute Declaration "
    'Private m_DataReader As SqlClient.SqlDataReader
    Private m_ReferenceNumber As Integer
    Private m_CustomerNumber As String
    Private m_AppPath As String


#End Region

#Region " Public Methods "

    Public Sub SendParameters(
        ByVal customerNumber As String,
        ByVal referenceNumber As Integer,
        ByVal appPath As String)

        Try
            m_ReferenceNumber = referenceNumber
            m_CustomerNumber = customerNumber
            m_AppPath = appPath

        Catch ex As Exception
            Throw

        End Try

    End Sub

#End Region

#Region " Private Methods "


    Private Sub rptEpayConfirmation_DataInitialize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DataInitialize
        Try
            Using dt As DataTable = Common.LoadEpayInvoicesByRefNumber(m_CustomerNumber, m_ReferenceNumber)
                Me.DataSource = dt
            End Using

        Catch ex As Exception
            Throw

        End Try

    End Sub

    'Private Sub rptEpayConfirmation_FetchData(ByVal sender As Object, ByVal eArgs As DataDynamics.ActiveReports.ActiveReport3.FetchEventArgs) Handles Me.FetchData
    '    ' Dim i As Integer

    '    Try
    '        If m_DataReader.HasRows = True Then

    '            If m_DataReader.Read = False Then
    '                'If the row counter has reached the end of the data then
    '                'set the eArgs.EOF flag to true and exit the procedure
    '                eArgs.EOF = True
    '                Return
    '            Else
    '                'Populate the fields collection from the DataReader
    '                'For i = 0 To (m_TeamCommissionDataReader.FieldCount) - 1
    '                '    Me.Fields(m_TeamCommissionDataReader.GetName(i)).Value = m_TeamCommissionDataReader.Item(i).ToString
    '                'Next i

    '                eArgs.EOF = False
    '            End If

    '            Me.lblInvoiceNo.Text = m_DataReader.Item("epyInvoiceNumber").ToString
    '            Me.lblOriginalAmt.Text = FormatNumber(m_DataReader.Item("epyGrossAmount"), 2)
    '            Me.lblDiscount.Text = FormatNumber(m_DataReader.Item("Discount"), 2)
    '            Me.lblAmount.Text = FormatNumber(m_DataReader.Item("epyAmountCharged"), 2)
    '            Me.lblInvDate.Text = m_DataReader.Item("opiDagedt").ToString

    '        End If

    '        If m_TotalDataReader.Read = True Then

    '            Me.lblOriginalAmt.Text = FormatNumber(m_TotalDataReader.Item("epyGrossAmount"), 2)
    '            Me.lblDiscount.Text = FormatNumber(m_TotalDataReader.Item("Discount"), 2)
    '            Me.lblAmount.Text = FormatNumber(m_TotalDataReader.Item("epyAmountCharged"), 2)


    '        End If

    '    Catch ex As Exception
    '        Throw

    '    Finally
    '        If m_DataReader IsNot Nothing Then
    '            m_DataReader.Close()
    '            m_DataReader = Nothing

    '        End If

    '        If m_TotalDataReader IsNot Nothing Then
    '            m_TotalDataReader.Close()
    '            m_TotalDataReader = Nothing

    '        End If
    '    End Try

    'End Sub

    Private Sub rptEpayConfirmation_ReportStart(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ReportStart
        'Sets a 1 inch top margin
        Me.Document.Printer.PrinterName = String.Empty

        Me.lblReferenceNumber.Text = m_ReferenceNumber
        Me.lblDate.Text = Date.Now
        'Me.lblTotalAmt.Text = FormatCurrency(m_TotalAmt, 2)

        Me.lblAshleyDirectHdr.Image = System.Drawing.Image.FromFile(m_AppPath & "\Images\ConfirmationHeader.jpg")

    End Sub

#End Region

End Class
