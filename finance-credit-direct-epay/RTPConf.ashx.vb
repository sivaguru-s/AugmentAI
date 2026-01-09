'(c) Ashley Furniture Industries, Inc. 2007
'All Rights Reserved
'
' Modification History
' Author			Date		Description
' Tessa Lockington  7/09/09     Removed cancel for unsuccessfulEvent event. RN594884

Imports System.Web
Imports System.Web.Services
Imports System.Xml
Imports system.xml.Schema

Public Class RTPConf1
    Implements System.Web.IHttpHandler


    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim referenceNumber As Integer = 0
        Dim confirmationId As String = Nothing
        Dim transactionMode As String = Nothing
        Dim requestParms As String = ""
        Dim parms() As String
        Dim parameter() As String
        Dim name As String
        Dim value As String


        Try
            Common.WriteErrorToAuditLog(referenceNumber, "Start")

            If context Is Nothing Then
                Exit Sub

            End If

            requestParms = getInputStream(context.Request.InputStream)

            parms = requestParms.Split("&".ToCharArray())

            For index As Integer = 0 To parms.Length - 1

                parameter = parms(index).Split("=".ToCharArray())

                If parameter.Length > 1 Then
                    name = parameter(0)
                    value = parameter(1)

                    If name.Equals("ReferenceNumber") Then
                        referenceNumber = Integer.Parse(value)
                    ElseIf name.Equals("ConfirmationId") Then
                        If value.Length > 6 Then
                            confirmationId = value.Substring(value.Length - 6)
                        Else
                            confirmationId = value
                        End If
                    ElseIf name.Equals("TransactionMode") Then
                        transactionMode = value
                    ElseIf name.Equals("UnsuccessfulEvent") Then
                        'unsuccessfulEvent = value
                    End If
                End If

            Next

            If transactionMode IsNot Nothing Then
                UpdateStatus(transactionMode, referenceNumber)

                If confirmationId IsNot Nothing Then
                    UpdateConfirmationNumber(confirmationId, referenceNumber)
                End If

            End If

            context.Response.ContentType = "text/plain"
            context.Response.Write("EPAY_OK")
            Common.WriteErrorToAuditLog(referenceNumber, "End:" & requestParms)

        Catch ex As Exception
            If context IsNot Nothing Then
                context.Response.ContentType = "text/plain"
                context.Response.Write("Failed " & ex.ToString)

            End If

            Common.WriteErrorToAuditLog(referenceNumber, ex.ToString)
            Common.WriteErrorToAuditLog(referenceNumber, requestParms)

        Finally
            referenceNumber = Nothing
            confirmationId = Nothing
            transactionMode = Nothing
            'unsuccessfulEvent = Nothing
            requestParms = Nothing
            parms = Nothing
            parameter = Nothing
            name = Nothing
            value = Nothing

        End Try


    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

#Region " Private Methods "

    Private Sub UpdateStatus(ByVal status As String, ByVal refNo As Integer)
        Try
            Common.UpdateEpayStatus("", refNo, status, String.Empty)

        Catch ex As Exception
            Throw

        Finally

        End Try

    End Sub

    Private Sub UpdateConfirmationNumber(ByVal confNo As String, ByVal refNo As Integer)

        Try
            Common.UpdateEpayConfirmationNumber("", refNo, confNo)

        Catch ex As Exception
            Throw

        Finally

        End Try

    End Sub

    Private Sub DeleteEpayRecords(ByVal refNo As Integer)

        Try

            Common.DeleteEpayRecords("", refNo, String.Empty)

        Catch ex As Exception
            Throw

        Finally

        End Try

    End Sub

    Private Function getInputStream(ByVal stream As System.IO.Stream) As String

        Dim buffer As Byte()

        Try

            buffer = New Byte(stream.Length - 1) {}
            stream.Position = 0
            stream.Read(buffer, 0, CInt((stream.Length)))
            Return System.Text.UTF8Encoding.UTF8.GetString(buffer)

        Catch ex As Exception
            Throw

        Finally
            buffer = Nothing

        End Try

    End Function
#End Region


End Class