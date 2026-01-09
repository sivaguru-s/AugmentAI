Imports IBM.Data.DB2.iSeries

#Region " Revision History "

'   Date            Programmer      Description
'   ============    ============    ========================================
'   03/28/2012      TKiel           Created from uniting classes from OpenInvoicesBLL
'   03/28/2012      TKiel           Changed to point to "AFI_Batch" instead of Ashley.Data.DataAccess.SqlConnections.SQL_Batch

#End Region

Public Class Common

#Region " Private Functions/Methods "

    Private Shared Function GetNextEpayRefNo() As Integer

        Dim dataReader As SqlClient.SqlDataReader = Nothing
        Dim sbSQL As New System.Text.StringBuilder
        Dim ePayRefNumb As Integer

        Try
            'Get Reference Number
            sbSQL.Append("EXEC Ashley.dbo.usp_GetNextID 'EPayRefNum'")
            dataReader = DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL.ToString)

            If dataReader.HasRows = True Then
                dataReader.Read()

                ePayRefNumb = dataReader.Item("NextID")

            Else
                WriteErrorToAuditLog(0, "Could not get EPay Reference Number")

            End If

            Return ePayRefNumb

        Catch ex As Exception
            Throw

        Finally
            If dataReader IsNot Nothing Then
                dataReader.Close()
            End If

            dataReader = Nothing
            sbSQL = Nothing

        End Try

    End Function

#End Region

#Region " Public Functions/Methods "

    Public Shared Function GetTotals(ByVal custNum As String, ByVal referenceNumber As Integer) As System.Data.SqlClient.SqlDataReader
        Dim sbSQL As String

        Try
            sbSQL = "EXEC Datawhse.dbo.usp_GetEpayTotal '" & custNum & "'," & referenceNumber

            System.Web.HttpContext.Current.Trace.Warn("Totals sbSQL:" & sbSQL.ToString)

            Return DataAccess.GetDataReader("AFI_Batch", sbSQL.ToString)

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

        End Try

    End Function

    Public Shared Sub UpdateEpayStatus(ByVal custNum As String, ByVal referenceNumber As Integer, ByVal status As String, ByVal sUserChanged As String)
        Dim sbSQL As New System.Text.StringBuilder

        Try
            With sbSQL
                .Append("EXEC Datawhse.dbo.usp_UpdateEpayStatus ")
                .Append("'" & custNum & "'," & referenceNumber & ",'" & status & "', '" & sUserChanged & "'")

            End With

            DataAccess.ExecuteCommand("AFI_Batch", sbSQL.ToString)

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

        End Try

    End Sub

    Public Shared Sub UpdateEpayConfirmationNumber(ByVal custNum As String, ByVal referenceNumber As Integer, ByVal confirmationNumber As String)
        Dim colParameters As Collection = Nothing

        Try
            colParameters = New Collection
            colParameters.Add(New SqlClient.SqlParameter("@customer", custNum))
            colParameters.Add(New SqlClient.SqlParameter("@RefNo", referenceNumber.ToString))
            colParameters.Add(New SqlClient.SqlParameter("@confirmationNumber", confirmationNumber))

            DataAccess.ExecuteStoredProcedure("AFI_Batch", "Datawhse.dbo.usp_UpdateEpayConfirmationNumber", DataAccess.StoredProcedureReturnType.RowsAffected, colParameters)

            'Dim timer As Stopwatch = Stopwatch.StartNew
            'timer.Stop()
            'Common.WriteErrorToAuditLog(0, "- START - " & timer.ElapsedMilliseconds.ToString)
            'timer.Start()
            UpdateEpayMC2Status(referenceNumber, confirmationNumber)
            'timer.Stop()
            'Common.WriteErrorToAuditLog(0, "- END - " & timer.ElapsedMilliseconds.ToString)
            'timer.Start()

        Catch ex As Exception
            Throw

        Finally
            colParameters = Nothing

        End Try

    End Sub

    'Piech - 09/09/2010 - RN613538 - Idea 11464 ---
    Public Shared Sub UpdateEpayMC2Status(ByVal iReferenceNumber As Integer, ByVal sConfirmationNumber As String)
        Dim colParameters As Collection = Nothing
        Dim dataReader As System.Data.SqlClient.SqlDataReader = Nothing

        Try
            ' call sproc to read the records for this reference/confirmation pair ---
            colParameters = New Collection
            colParameters.Add(New SqlClient.SqlParameter("@ReferenceNumber", iReferenceNumber))
            colParameters.Add(New SqlClient.SqlParameter("@ConfirmationNumber", sConfirmationNumber))

            'timer.Stop()
            'Common.WriteErrorToAuditLog(0, "- BEFORE SQL - " & timer.ElapsedMilliseconds.ToString)
            'timer.Start()
            dataReader = DataAccess.ExecuteStoredProcedure("AFI_Batch", "Datawhse.dbo.usp_EpayGetMC2BatchConfirmation", DataAccess.StoredProcedureReturnType.DataReader, colParameters)
            'timer.Stop()
            'Common.WriteErrorToAuditLog(0, "- AFTER SQL - " & timer.ElapsedMilliseconds.ToString)
            'timer.Start()

            If dataReader.HasRows Then
                Do While dataReader.Read
                    colParameters = New Collection
                    colParameters.Add(DB2DataAccess.SetSQLParameterProperties("@COMPANY", iDB2DbType.iDB2Decimal, dataReader.Item("CompanyNumber").ToString.TrimEnd))
                    colParameters.Add(DB2DataAccess.SetSQLParameterProperties("@CUSTNO", iDB2DbType.iDB2Decimal, dataReader.Item("CustomerNumber").ToString.TrimEnd))
                    colParameters.Add(DB2DataAccess.SetSQLParameterProperties("@REFNO", iDB2DbType.iDB2Integer, dataReader.Item("ReferenceNumber").ToString.TrimEnd))
                    colParameters.Add(DB2DataAccess.SetSQLParameterProperties("@INVNO", iDB2DbType.iDB2Char, dataReader.Item("InvoiceNumber").ToString.TrimEnd, 14, ParameterDirection.Input))
                    colParameters.Add(DB2DataAccess.SetSQLParameterProperties("@INVDAT", iDB2DbType.iDB2Decimal, dataReader.Item("InvoiceDate").ToString.TrimEnd))
                    colParameters.Add(DB2DataAccess.SetSQLParameterProperties("@INVAMT", iDB2DbType.iDB2Decimal, dataReader.Item("AmountCharged").ToString.TrimEnd))
                    colParameters.Add(DB2DataAccess.SetSQLParameterProperties("@SQLSTATE", iDB2DbType.iDB2Char, String.Empty, 5, ParameterDirection.InputOutput))

                    'timer.Stop()
                    'Common.WriteErrorToAuditLog(0, "- BEFORE 400 INSERT - " & timer.ElapsedMilliseconds.ToString)
                    'timer.Start()
                    DB2DataAccess.ExecuteStoredProcedure("DB2_AFI", "USP_MCEPAY_INSERT", DB2DataAccess.StoredProcedureReturnType.RowsAffected, colParameters)
                    'timer.Stop()
                    'Common.WriteErrorToAuditLog(0, "- AFTER 400 INSERT - " & timer.ElapsedMilliseconds.ToString)
                    'timer.Start()

                Loop

                colParameters = New Collection
                colParameters.Add(DB2DataAccess.SetSQLParameterProperties("@REFNO", iDB2DbType.iDB2Integer, iReferenceNumber))

                'timer.Stop()
                'Common.WriteErrorToAuditLog(0, "- BEFORE 400 PROC - " & timer.ElapsedMilliseconds.ToString)
                'timer.Start()
                DB2DataAccess.ExecuteStoredProcedure("DB2_AFI", "USP_CALL_CR048A", DB2DataAccess.StoredProcedureReturnType.RowsAffected, colParameters)
                'timer.Stop()
                'Common.WriteErrorToAuditLog(0, "- AFTER 400 PROC - " & timer.ElapsedMilliseconds.ToString)
                'timer.Start()

            End If

        Catch ex As Exception
            WriteErrorToAuditLog(iReferenceNumber, "EPAY MC2 BATCH CONFIRMATION ERROR: " & ex.ToString)
            Throw

        Finally
            If dataReader IsNot Nothing Then dataReader.Close()

        End Try

    End Sub

    Public Shared Sub DeleteEpayRecords(ByVal custNum As String, ByVal referenceNumber As Integer, ByVal sUserChanged As String)
        Dim sbSQL As String = Nothing

        Try
            sbSQL = "EXEC Datawhse.dbo.usp_DeleteEpay '" & custNum & "'," & referenceNumber & ", '" & sUserChanged & "'"

            DataAccess.ExecuteCommand("AFI_Batch", sbSQL)

        Catch ex As Exception
            Throw

        Finally

            sbSQL = Nothing
        End Try

    End Sub

    Public Shared Sub DeleteEpayUnconfirmedPayment(ByVal custNum As String, ByVal invoiceNumber As String, ByVal sUserChanged As String)
        Dim sbSQL As String = Nothing

        Try
            sbSQL = "EXEC Datawhse.dbo.usp_DeleteEpayUnconfirmedPayment '" & custNum & "', '" & invoiceNumber & "', '" & sUserChanged & "'"

            ' System.Web.HttpContext.Current.Trace.Warn("sbSQL:" & sbSQL.ToString)

            DataAccess.ExecuteCommand("AFI_Batch", sbSQL)

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

        End Try

    End Sub

    Public Shared Function LoadEpayInvoicesByRefNumber(ByVal custNum As String, ByVal referenceNumber As Integer) As DataTable
        Dim sbSQL As String

        Try
            sbSQL = "EXEC Datawhse.dbo.usp_GetEpayInvoicesByRefNumber '" & custNum & "'," & referenceNumber

            Return DataAccess.GetDataTable("AFI_Batch", sbSQL.ToString)

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

        End Try

    End Function

    'Public Shared Function LoadEpayInvoiceHistory(ByVal custNum As String, ByVal referenceNumber As String, ByVal InvoiceNo As String, ByVal paymentAdded As Date, _
    '                                              ByVal detail As Boolean, ByVal ConfirmationNo As String) As System.Data.DataSet
    '    Dim sbSQL As String
    '    Dim ds As New System.Data.DataSet

    '    Try

    '        sbSQL = "EXEC Datawhse.dbo.usp_GetEpayInvoiceHistory '" & custNum & "','" & referenceNumber & "','" _
    '             & InvoiceNo & "','" & ConfirmationNo & "','" & paymentAdded & "',0"

    '        ' System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

    '        DataAccess.FillDataSet("AFI_Batch", sbSQL.ToString, "Header", ds)

    '        If detail Then
    '            sbSQL = "EXEC Datawhse.dbo.usp_GetEpayInvoiceHistory '" & custNum & "','" & referenceNumber & "','" _
    '                 & InvoiceNo & "','" & ConfirmationNo & "','" & paymentAdded & "',1"

    '            ' System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

    '            DataAccess.FillDataSet("AFI_Batch", sbSQL.ToString, "Detail", ds)

    '            Dim dcDetailSummaryKey As DataColumn() = {ds.Tables("Detail").Columns("dtea"), _
    '                                                      ds.Tables("Detail").Columns("ConfirmationNumber"), _
    '                                                      ds.Tables("Detail").Columns("EpayStatus"), _
    '                                                      ds.Tables("Detail").Columns("ReferenceNumber"), _
    '                                                      ds.Tables("Detail").Columns("fileType")}
    '            Dim dcHeaderSummaryKey As DataColumn() = {ds.Tables("Header").Columns("dtea"), _
    '                                                      ds.Tables("Header").Columns("ConfirmationNumber"), _
    '                                                      ds.Tables("Header").Columns("EpayStatus"), _
    '                                                      ds.Tables("Header").Columns("ReferenceNumber"), _
    '                                                      ds.Tables("Header").Columns("fileType")}

    '            ds.Relations.Add("Join", dcHeaderSummaryKey, _
    '                                    dcDetailSummaryKey, False)

    '        End If

    '        Return ds

    '    Catch ex As Exception
    '        Throw

    '    Finally
    '        sbSQL = Nothing

    '    End Try

    'End Function

    Public Shared Function LoadEpayAnalystReport(ByVal customerNumber As String, ByVal referenceNumber As String, ByVal paymentAdded As Date, ByVal ConfirmationNo As String, _
                                              ByVal CreditTerritory As String, ByVal sortBy As String, ByVal sortAscending As Boolean) As System.Data.SqlClient.SqlDataReader
        Dim sbSQL As New System.Text.StringBuilder

        Try
            sbSQL.Append("EXEC Datawhse.dbo.usp_GetEpayAnalystReport '" & customerNumber & "','" & referenceNumber & "','" _
                 & ConfirmationNo & "','" & paymentAdded & "','" & CreditTerritory & "','" & sortBy & "', ")

            If sortAscending = True Then
                sbSQL.Append("' ASC'")
            Else
                sbSQL.Append("' DESC'")
            End If

            System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

            Return DataAccess.GetDataReader("AFI_Batch", sbSQL.ToString)

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

        End Try

    End Function

    Public Shared Function LoadInvoices(ByVal sCustomerNumber As String, ByVal sShipToNumber As String, ByVal bAllShipTos As Boolean, _
                                        ByVal sSecurityMHS As String, ByVal sPONumber As String, ByVal dInvoiceFromDate As Date, _
                                        ByVal dInvoiceToDate As Date, ByVal sInvoiceNumber As String, ByVal sCreditNumber As String, _
                                        ByVal sSortColumn As String, ByVal bSortAscending As Boolean, ByVal iPageNumber As Integer, _
                                        ByVal iPageSize As Integer) As DataTable
        Try
            If sInvoiceNumber Is Nothing Then sInvoiceNumber = String.Empty
            If sCreditNumber Is Nothing Then sCreditNumber = String.Empty
            If sPONumber Is Nothing Then sPONumber = String.Empty

            Dim bConsumerPOSearch As Boolean = False
            If DetermineIfToUseConsumerSearch(sCustomerNumber) = True _
                    AndAlso sPONumber <> String.Empty Then
                bConsumerPOSearch = True
            End If

            Dim parameters As New Collection
            parameters.Add(New SqlClient.SqlParameter("@customerNumber", sCustomerNumber))
            parameters.Add(New SqlClient.SqlParameter("@shiptoNumber", sShipToNumber))
            parameters.Add(New SqlClient.SqlParameter("@allShiptos", bAllShipTos))
            parameters.Add(New SqlClient.SqlParameter("@securityMHS", sSecurityMHS))
            parameters.Add(New SqlClient.SqlParameter("@FromDate", dInvoiceFromDate))
            parameters.Add(New SqlClient.SqlParameter("@ToDate", dInvoiceToDate))
            parameters.Add(New SqlClient.SqlParameter("@searchInvoice", sInvoiceNumber.Replace("'", String.Empty)))
            parameters.Add(New SqlClient.SqlParameter("@searchCredit", sCreditNumber.Replace("'", String.Empty)))
            parameters.Add(New SqlClient.SqlParameter("@searchPo", sPONumber.Replace("'", "''")))
            parameters.Add(New SqlClient.SqlParameter("@sortBy", sSortColumn))
            parameters.Add(New SqlClient.SqlParameter("@sortAscending", If(bSortAscending, " ASC", " DESC")))
            parameters.Add(New SqlClient.SqlParameter("@pageNum", iPageNumber))
            parameters.Add(New SqlClient.SqlParameter("@pageSize", iPageSize))
            parameters.Add(New SqlClient.SqlParameter("@ConsumerPOSearch", bConsumerPOSearch))
            parameters.Add(New SqlClient.SqlParameter("@Application", "EPAY"))

            Return DirectCast(DataAccess.ExecuteStoredProcedure("AFI_Batch",
                                                                "Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices3",
                                                                DataAccess.StoredProcedureReturnType.DataTable,
                                                                parameters), DataTable)

        Catch ex As Exception
            Throw

        End Try

    End Function

    Public Shared Function LoadUnconfirmedPayments(ByVal sCustomerNumber As String, ByVal sShipToNumber As String, ByVal sSecurityMHS As String, _
                                                   ByVal sCreditTerritory As String, ByVal sSortColumn As String, ByVal bSortAscending As Boolean) As DataTable

        Dim sbSQL As New System.Text.StringBuilder

        Try
            With sbSQL
                .Append("EXEC Datawhse.dbo.usp_GetEpayUnconfirmedPayments ")

                .Append("'" & sCustomerNumber & "', ")
                .Append("'" & sShipToNumber & "', ")
                .Append("'" & sSecurityMHS & "', ")
                .Append("'" & sCreditTerritory & "', ")
                .Append("'" & sSortColumn & "', ")

                If bSortAscending Then
                    .Append("' ASC'")

                Else
                    .Append("' DESC'")

                End If

            End With

            System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

            Return DataAccess.GetDataTable("AFI_Batch", sbSQL.ToString)

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

        End Try
    End Function

    Public Shared Function LoadEpayUsers(ByVal sCustomerNumber As String, ByVal sCustomerName As String, ByVal sBillToState As String, _
                                         ByVal sCreditTerritory As String, ByVal sTermsCode As String, ByVal iPageNumber As Integer, _
                                         ByVal iPageSize As Integer) As DataTable
        Dim sbSQL As New System.Text.StringBuilder

        Try

            With sbSQL

                .Append("EXEC Ashley.dbo.usp_GetEpayUsers ")

                .Append("'" & sCustomerNumber & "', ")
                .Append("'" & sCustomerName & "', ")
                .Append("'" & sBillToState & "', ")
                .Append("'" & sCreditTerritory & "', ")
                .Append("'" & sTermsCode & "', ")
                .Append("'" & iPageNumber & "', '" & iPageSize & "'")

            End With

            'System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

            Return DataAccess.GetDataTable(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL.ToString)

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

        End Try
    End Function


    Public Shared Function DetermineIfToUseConsumerSearch(ByRef sCustomerNumber As String) As Boolean
        ' Determine if this is a customer with consolidated POs where the 
        ' originally entered PO#s are stored in extension tables
        '(AAFES, for example).  In this case if a PO# is supplied
        ' we must check for both the Ashley PO# and the customer's original PO#.
        Dim dataTable As DataTable = Nothing
        Dim parameters As New Collection

        Try
            parameters.Add(New SqlClient.SqlParameter("@CustomerNumber", sCustomerNumber))

            ' look up accounts that should check OPNORDEX and TSEXIN for non-Ashley PO#s
            dataTable = DataAccess.ExecuteStoredProcedure("AFI_Dynamic", "Ashley.dbo.usp_OrdAndInvReportDetermineConsumerSearch", DataAccess.StoredProcedureReturnType.DataTable, parameters)

            If dataTable.Rows.Count > 0 Then
                If CBool(dataTable.Rows(0).Item("ppeAllowConsumerSearch")) Then
                    Return True

                Else
                    Return False

                End If

            Else
                Return False

            End If

        Catch ex As Exception
            Throw

        Finally
            If parameters IsNot Nothing Then
                parameters = Nothing

            End If

            If dataTable IsNot Nothing Then
                dataTable.Dispose()
                dataTable = Nothing

            End If

        End Try

    End Function

    Public Shared Function WriteInvoicesToEpay(ByRef xmlInvoices As String, ByRef refNo As Integer, ByVal sUserChanged As String) As Decimal

        Dim dataReader As SqlClient.SqlDataReader = Nothing
        Dim sbSQL As New System.Text.StringBuilder
        Dim totalInvAmount As Decimal = New Decimal(0)

        Try
            refNo = GetNextEpayRefNo()

            ' call sproc to write invoices to EPay files
            With sbSQL
                .Append("EXEC Datawhse.dbo.usp_InsertInvoicesToEpay ")
                .Append(refNo & ",'" & xmlInvoices & "', '" & sUserChanged & "'")
            End With

            'System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

            dataReader = DataAccess.GetDataReader("AFI_Batch", sbSQL.ToString)
            If dataReader.HasRows = True Then
                dataReader.Read()
                totalInvAmount = dataReader.Item("total")
            End If
            'System.Web.HttpContext.Current.Trace.Warn(totalInvAmount.ToString)

            Return totalInvAmount

        Finally
            If dataReader IsNot Nothing Then
                dataReader.Close()
            End If

            dataReader = Nothing
            sbSQL = Nothing
        End Try

    End Function

    Public Shared Function GetEpayStatus(ByVal custNum As String, ByVal referenceNumber As Integer) As String

        Dim sbSQL As New System.Text.StringBuilder
        Dim dataReader As System.Data.SqlClient.SqlDataReader = Nothing
        Dim status As String = Nothing

        Try

            ' call sproc to write invoices to EPay files
            With sbSQL
                .Append("EXEC Datawhse.dbo.usp_GetEpayStatus ")
                .Append("'" & custNum & "'," & referenceNumber)
            End With

            dataReader = DataAccess.GetDataReader("AFI_Batch", sbSQL.ToString)

            If dataReader.Read Then
                status = dataReader.GetValue(0)
            End If

            Return status

        Catch ex As Exception
            Throw

        Finally

            sbSQL = Nothing
            status = Nothing

            If dataReader IsNot Nothing Then
                dataReader.Close()
                dataReader = Nothing
            End If

        End Try

    End Function

    Public Shared Sub WriteErrorToAuditLog(ByVal referenceNumber As Integer, ByVal errorMsg As String)
        Dim sbSQL As String
        Dim sbError As New System.Text.StringBuilder
        Dim length As Integer

        Try
            sbError.Append("RefNo:" & referenceNumber.ToString & errorMsg)

            If sbError.Length > 8000 Then
                length = sbError.Length - 8000
                sbError = New System.Text.StringBuilder(sbError.Remove(8000, length).ToString)
            End If

            '@Machine, @ErrDesc, @Program, @Phase, @MhsName, @IsErr, @UserName 
            sbSQL = "EXEC ASHLEY.DBO.usp_InsertAuditRecord 'AD','" & sbError.ToString & _
                        "','EPay','RTPC','',1,'','',0,'',''"

            'Throw New Exception(sbSQL.ToString)

            DataAccess.ExecuteCommand(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL)

        Catch ex As Exception
            Throw

        Finally
            sbSQL = Nothing

        End Try

    End Sub

#End Region

End Class
