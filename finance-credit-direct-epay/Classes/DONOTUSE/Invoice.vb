''(c) Ashley Furniture Industries, Inc. 2007
''All Rights Reserved
''
'' Modification History
'' Author			Date		Description
'' Tessa Lockington  5/28/09     Added date and detail option to History method.
'' Tessa Lockington  6/2/09      Added method to get totals for epay payment.    
'' Tessa Lockington  7/14/09     Added method to get customers that have made payments.
'' Tyler Kiel        8/20/10     Added methods to update and delete unconfirmed payments.
'' Tyler Kiel        11/30/2010  Added method to get the default date span in days for the main page to search based on.

'Public Class Invoice
'    Implements IDisposable


'#Region " Attribute Declarations "

'    Private m_bDisposed As Boolean = False
'    Private m_sInvoiceNumber As String = Nothing


'#End Region

'#Region " Property Declarations "


'    Public Property InvoiceNumber() As String
'        Get
'            Return m_sInvoiceNumber
'        End Get
'        Set(ByVal value As String)
'            m_sInvoiceNumber = value
'        End Set
'    End Property

'    'Public Property EPayStatus() As String
'    '    Get
'    '        Return m_sEPayStatus
'    '    End Get
'    '    Set(ByVal value As String)
'    '        m_sEPayStatus = value
'    '    End Set
'    'End Property

'    'Public Property EPayRefNo() As Integer
'    '    Get
'    '        Return m_iEPayRefNo
'    '    End Get
'    '    Set(ByVal value As Integer)
'    '        m_iEPayRefNo = value
'    '    End Set
'    'End Property


'#End Region

'#Region " Constructors Declarations "




'#End Region

'#Region " Private Methods "


'#End Region

'#Region " Public Methods "

'    Public Shared Function GetTotals(ByVal custNum As String, ByVal referenceNumber As Integer) As System.Data.SqlClient.SqlDataReader

'        Dim sbSQL As String

'        Try

'            sbSQL = "EXEC Datawhse.dbo.usp_GetEpayTotal '" & custNum & "'," & referenceNumber

'            System.Web.HttpContext.Current.Trace.Warn("Totals sbSQL:" & sbSQL.ToString)

'            Return DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Function

'    Public Shared Function GetEpayStatus(ByVal custNum As String, ByVal referenceNumber As Integer) As String

'        Dim sbSQL As New System.Text.StringBuilder
'        Dim dataReader As System.Data.SqlClient.SqlDataReader = Nothing
'        Dim status As String = Nothing

'        Try

'            ' call sproc to write invoices to EPay files
'            With sbSQL
'                .Append("EXEC Datawhse.dbo.usp_GetEpayStatus ")
'                .Append("'" & custNum & "'," & referenceNumber)
'            End With

'            dataReader = DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'            If dataReader.Read Then
'                status = dataReader.GetValue(0)
'            End If

'            Return status

'        Catch ex As Exception
'            Throw

'        Finally

'            sbSQL = Nothing
'            status = Nothing

'            If dataReader IsNot Nothing Then
'                dataReader.Close()
'                dataReader = Nothing
'            End If

'        End Try

'    End Function

'    Public Shared Sub UpdateEpayStatus(ByVal custNum As String, ByVal referenceNumber As Integer, ByVal status As String)

'        Dim sbSQL As New System.Text.StringBuilder

'        Try

'            ' call sproc to write invoices to EPay files
'            With sbSQL
'                .Append("EXEC Datawhse.dbo.usp_UpdateEpayStatus ")
'                .Append("'" & custNum & "'," & referenceNumber & ",'" & status & "'")
'            End With

'            DataAccess.ExecuteCommand(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'        Catch ex As Exception
'            Throw

'        Finally

'            sbSQL = Nothing
'        End Try

'    End Sub

'    Public Shared Sub UpdateEpayConfirmationNumber(ByVal custNum As String, ByVal referenceNumber As Integer, ByVal confirmationNumber As String)

'        Dim sbSQL As New System.Text.StringBuilder

'        Try

'            ' call sproc to write invoices to EPay files
'            With sbSQL
'                .Append("EXEC Datawhse.dbo.usp_UpdateEpayConfirmationNumber '")
'                .Append(custNum & "'," & referenceNumber & ",'" & confirmationNumber & "'")
'            End With

'            DataAccess.ExecuteCommand(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'            'Piech - 09/09/2010 - RN613538 - Idea 11464 ---
'            'Invoice.UpdateEpayMC2Status(referenceNumber, confirmationNumber)

'        Catch ex As Exception
'            Throw

'        Finally

'            sbSQL = Nothing

'        End Try

'    End Sub

'    'Piech - 09/09/2010 - RN613538 - Idea 11464 ---
'    Public Shared Sub UpdateEpayMC2Status(ByVal referenceNumber As Integer, ByVal confirmationNumber As String)

'        Dim sbSQL As New System.Text.StringBuilder
'        Dim dataReader As System.Data.SqlClient.SqlDataReader = Nothing
'        Dim AS400Transaction As Odbc.OdbcTransaction = Nothing

'        Try

'            ' call sproc to read the records for this reference/confirmation pair ---
'            With sbSQL
'                .Append("EXEC Datawhse.dbo.usp_GetEpayMC2BatchConfirmation ")
'                .Append(referenceNumber & ",'" & confirmationNumber & "'")
'            End With

'            dataReader = DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'            If dataReader.HasRows Then
'                Do While dataReader.Read
'                    sbSQL = New System.Text.StringBuilder

'                    With sbSQL
'                        .Append("INSERT INTO ASHLEY.DWMCEP ")
'                        .Append("(DMCCOMPNY,DMCCUSNBR,DMCINVIFM,DMCINVDTE,DMCINVAMT) ")
'                        .Append("VALUES (")
'                        .Append("'").Append(dataReader.GetValue(0)).Append("'").Append(",")
'                        .Append("'").Append(dataReader.GetValue(1)).Append("'").Append(",")
'                        .Append("'").Append(dataReader.GetValue(2)).Append("'").Append(",")
'                        .Append("'").Append(dataReader.GetValue(3)).Append("'").Append(",")
'                        .Append("'").Append(dataReader.GetValue(4)).Append("'")
'                        .Append(")")
'                    End With

'                    'System.Web.HttpContext.Current.Trace.Warn("sbSQL:" & sbSQL.ToString)

'                    DB2DataAccess.ExecuteCommand(Ashley.Data.DataAccess.Db2Connections.DB2_AF02, sbSQL.ToString)

'                Loop

'                AS400Transaction.Commit()

'                sbSQL = New System.Text.StringBuilder
'                AS400Transaction = Nothing

'                With sbSQL
'                    .Append("CALL ACTLIBDB.USP_MC2_CUSTOMER_INVOICE_EXT_UPDATE")
'                End With

'                Try
'                    DB2DataAccess.ExecuteCommand(Ashley.Data.DataAccess.Db2Connections.DB2_AF02, sbSQL.ToString)

'                    AS400Transaction.Commit()
'                Catch ex As Exception
'                    WriteErrorToAuditLog(referenceNumber, "EPAY MC2 BATCH CONFIRMATION UPDATE FAILED")
'                    WriteErrorToAuditLog(referenceNumber, "EPAY MC2 BATCH CONFIRMATION UPDATE FAILED: " & ex.ToString)
'                    Throw

'                End Try

'            End If

'        Catch ex As Exception
'            WriteErrorToAuditLog(referenceNumber, "EPAY MC2 BATCH CONFIRMATION ERROR")
'            WriteErrorToAuditLog(referenceNumber, "EPAY MC2 BATCH CONFIRMATION ERROR: " & ex.ToString)
'            Throw

'        Finally

'            sbSQL = Nothing

'            If dataReader IsNot Nothing Then
'                dataReader.Close()
'                dataReader = Nothing
'            End If

'            If Not IsNothing(AS400Transaction) Then
'                If Not IsNothing(AS400Transaction.Connection) Then
'                    AS400Transaction.Connection.Close()
'                End If

'                AS400Transaction.Dispose()
'                AS400Transaction = Nothing
'            End If

'        End Try

'    End Sub


'    Public Shared Sub DeleteEpayRecords(ByVal custNum As String, ByVal referenceNumber As Integer)

'        Dim sbSQL As String = Nothing

'        Try

'            ' call sproc to write invoices to EPay files
'            sbSQL = "EXEC Datawhse.dbo.usp_DeleteEpay '" & custNum & "'," & referenceNumber

'            ' System.Web.HttpContext.Current.Trace.Warn("sbSQL:" & sbSQL.ToString)

'            DataAccess.ExecuteCommand(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL)

'        Catch ex As Exception
'            Throw

'        Finally

'            sbSQL = Nothing
'        End Try

'    End Sub

'    Public Shared Sub DeleteEpayUnconfirmedPayment(ByVal custNum As String, ByVal invoiceNumber As String)

'        Dim sbSQL As String = Nothing

'        Try

'            sbSQL = "EXEC Datawhse.dbo.usp_DeleteEpayUnconfirmedPayment '" & custNum & "', '" & invoiceNumber & "'"

'            ' System.Web.HttpContext.Current.Trace.Warn("sbSQL:" & sbSQL.ToString)

'            DataAccess.ExecuteCommand(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL)

'        Catch ex As Exception
'            Throw

'        Finally

'            sbSQL = Nothing
'        End Try

'    End Sub

'    Public Shared Function LoadEpayInvoices(ByVal custNum As String, ByVal referenceNumber As Integer) As System.Data.SqlClient.SqlDataReader

'        Dim sbSQL As String

'        Try

'            sbSQL = "EXEC Datawhse.dbo.usp_GetEpayInvoicesByRefNumber '" & custNum & "'," & referenceNumber

'            Return DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Function

'    Public Shared Function LoadEpayInvoiceHistory(ByVal custNum As String, ByVal referenceNumber As String, _
'                                                  ByVal InvoiceNo As String, ByVal paymentAdded As Date, _
'                                                  ByVal detail As Boolean, _
'                                                  ByVal ConfirmationNo As String) As System.Data.DataSet

'        Dim sbSQL As String
'        Dim ds As New System.Data.DataSet
'        Try

'            sbSQL = "EXEC Datawhse.dbo.usp_GetEpayInvoiceHistory '" & custNum & "','" & referenceNumber & "','" _
'                 & InvoiceNo & "','" & ConfirmationNo & "','" & paymentAdded & "',0"

'            ' System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

'            DataAccess.FillDataSet(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString, "Header", ds)

'            If detail Then

'                sbSQL = "EXEC Datawhse.dbo.usp_GetEpayInvoiceHistory '" & custNum & "','" & referenceNumber & "','" _
'                     & InvoiceNo & "','" & ConfirmationNo & "','" & paymentAdded & "',1"

'                ' System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

'                DataAccess.FillDataSet(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString, "Detail", ds)

'                Dim dcDetailSummaryKey As DataColumn() = {ds.Tables("Detail").Columns("dtea"), _
'                                                          ds.Tables("Detail").Columns("ConfirmationNumber"), _
'                                                          ds.Tables("Detail").Columns("EpayStatus"), _
'                                                          ds.Tables("Detail").Columns("ReferenceNumber"), _
'                                                          ds.Tables("Detail").Columns("fileType")}
'                Dim dcHeaderSummaryKey As DataColumn() = {ds.Tables("Header").Columns("dtea"), _
'                                                          ds.Tables("Header").Columns("ConfirmationNumber"), _
'                                                          ds.Tables("Header").Columns("EpayStatus"), _
'                                                          ds.Tables("Header").Columns("ReferenceNumber"), _
'                                                          ds.Tables("Header").Columns("fileType")}

'                ds.Relations.Add("Join", dcHeaderSummaryKey, _
'                                        dcDetailSummaryKey, False)

'            End If

'            Return ds

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Function

'    Public Shared Function LoadEpayAnalystReport(ByVal customerNumber As String, _
'                                              ByVal referenceNumber As String, _
'                                              ByVal paymentAdded As Date, _
'                                              ByVal ConfirmationNo As String, _
'                                              ByVal CreditTerritory As String, _
'                                              ByVal sortBy As String, _
'                                              ByVal sortAscending As Boolean) As System.Data.SqlClient.SqlDataReader

'        Dim sbSQL As New System.Text.StringBuilder

'        Try

'            sbSQL.Append("EXEC Datawhse.dbo.usp_GetEpayAnalystReport '" & customerNumber & "','" & referenceNumber & "','" _
'                 & ConfirmationNo & "','" & paymentAdded & "','" & CreditTerritory & "','" & sortBy & "', ")

'            If sortAscending = True Then
'                sbSQL.Append("' ASC'")
'            Else
'                sbSQL.Append("' DESC'")
'            End If

'            System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

'            Return DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Function

'    Public Shared Sub WriteErrorToAuditLog(ByVal referenceNumber As Integer, ByVal errorMsg As String)

'        Dim sbSQL As String
'        Dim sbError As New System.Text.StringBuilder
'        Dim length As Integer

'        Try
'            sbError.Append("RefNo:" & referenceNumber.ToString & errorMsg)

'            If sbError.Length > 8000 Then
'                length = sbError.Length - 8000
'                sbError = New System.Text.StringBuilder(sbError.Remove(8000, length).ToString)
'            End If

'            '@Machine, @ErrDesc, @Program, @Phase, @MhsName, @IsErr, @UserName 
'            sbSQL = "EXEC ASHLEY.DBO.usp_InsertAuditRecord 'AD','" & sbError.ToString & _
'                        "','EPay','RTPC','',1,'','',0,'',''"

'            'Throw New Exception(sbSQL.ToString)

'            DataAccess.ExecuteCommand(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL)

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Sub

'    Public Shared Function GetDefaultDateSpanInDays() As Integer

'        Dim sbSQL As New System.Text.StringBuilder
'        Dim dataReader As System.Data.SqlClient.SqlDataReader = Nothing
'        Dim defaultDateSpanInDays As Integer = 0

'        Try

'            'Call stored procedure to get default date span in days
'            With sbSQL
'                .Append("EXEC Ashley.dbo.usp_GetEpayDefaultDateSpanInDays ")
'            End With

'            dataReader = DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL.ToString)

'            If dataReader.Read Then
'                defaultDateSpanInDays = dataReader.GetValue(0)
'            End If

'            Return defaultDateSpanInDays


'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing
'            defaultDateSpanInDays = Nothing

'            If dataReader IsNot Nothing Then
'                dataReader.Close()
'                dataReader = Nothing
'            End If
'        End Try

'    End Function

'#End Region

'#Region " Dispose Methods "

'    Public Overridable Overloads Sub Dispose() Implements IDisposable.Dispose

'        If Not m_bDisposed Then
'            ' Call the dispose method
'            Me.Dispose(True)

'            ' Tell the garbage collector that the object doesn't require cleanup
'            GC.SuppressFinalize(Me)
'        End If

'    End Sub

'    Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)


'        If Not Me.m_bDisposed Then
'            If disposing Then

'                ' Free managed resources              


'            End If

'            ' TODO: free shared unmanaged resources
'        End If
'        Me.m_bDisposed = True

'    End Sub

'    Protected Overrides Sub Finalize()
'        Me.Dispose(False)
'    End Sub

'#End Region



'End Class


