Imports System.Xml
Imports System.Net
Imports System.IO

'' Accepts Real-Time Payment Confirmation can be sent in XML format

Partial Public Class RTPConf
    Inherits EpayBasePage

#Region " Event Handlers "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub Sumbit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Sumbit.Click
        Dim x As String = String.Format("TransactionMode=verifying&ConfirmationId=ASHINV000{0}&ReferenceNumber={1}", txtConf.Text.ToString, txtRefNum.Text.ToString)
        'Dim x As String = "UnsuccessfulEvent=ASHINV000001007&ReferenceNumber=385"
        'Dim x As String = "TransactionMode=verifying&ConfirmationId=ASHINV000024795&ReferenceNumber=28596"
        Dim returnValue As String

        Try
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(x)

            Dim myRequest As HttpWebRequest '= WebRequest.Create("https://www.ashleydirect.com/EPay/RTPConf.ashx")
            'Dim myRequest As HttpWebRequest = WebRequest.Create("http://dev.ashleydirect.com/EPay/RTPConf.ashx")

            Select Case envRadio.SelectedValue
                Case "Prod"
                    myRequest = WebRequest.Create("https://www.ashleydirect.com/EPay/RTPConf.ashx")
                Case "Stage"
                    myRequest = WebRequest.Create("http://stage.ashleydirect.com/EPay/RTPConf.ashx")
                Case Else
                    myRequest = WebRequest.Create("http://dev.ashleydirect.com/EPay/RTPConf.ashx")
            End Select

            'Label1.Text = envRadio.SelectedValue.ToString & " --- " & x

            myRequest.Method = "POST"
            myRequest.ContentLength = bytes.Length

            'The encoding might have to be chaged based on requirement
            Dim encoder As New UTF8Encoding()
            Dim data As Byte() = encoder.GetBytes(x)
            Dim cr As New System.Net.NetworkCredential("test_ms", "Apple123")

            'postbody is plain string of xml

            Dim reqStream As Stream = myRequest.GetRequestStream()
            reqStream.Write(data, 0, data.Length)
            reqStream.Close()
            myRequest.Credentials = cr

            'Trace.Warn("test" & myRequest.)

            'Dim myResponse As HttpWebResponse = myRequest.GetResponse
            Dim response As HttpWebResponse = DirectCast(myRequest.GetResponse(), HttpWebResponse)
            If response.StatusCode <> HttpStatusCode.OK Then
                Dim message As String = [String].Format("POST failed. Received HTTP {0}", response.StatusCode)
                Throw New ArgumentNullException(response.StatusCode, message)

            End If

            Dim rs As HttpWebResponse = DirectCast(myRequest.GetResponse(), HttpWebResponse)
            Dim receiveStream As Stream = rs.GetResponseStream()
            Dim encode As Encoding = System.Text.Encoding.GetEncoding("utf-8")
            Dim readStream As New StreamReader(receiveStream, encode)
            Dim read(256) As [Char]
            Dim count As Integer = readStream.Read(read, 0, 256)
            While count > 0
                Dim str As New [String](read, 0, count)
                returnValue += str
                count = readStream.Read(read, 0, 256)
            End While
            Label1.Text = returnValue & x
            readStream.Close()
            rs.Close()

        Catch ex As Exception
            'Me.LogTo_tblAudit("ERROR: " & ex.ToString)
            Me.RedirectToErrorProcessing(ex)

        End Try

    End Sub

#End Region

End Class