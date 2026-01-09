''(c) Ashley Furniture Industries, Inc. 2007
''All Rights Reserved
''
'' Modification History
'' Author			Date		Description
'' Tessa Lockington  5/28/09     Added Invoice To date.
'' Tyler Kiel        08/20/2010  Added CustomerName and Territory
'' Tyler Kiel        08/20/2010  Added methods to load unconfirmed payments and epay users.
'' Tyler Kiel        04/14/2011  Changed LoadInvoice() to return a DataTable instead of a DataReader to support paging in a GridView

'Public Class Customer
'    Implements IDisposable


'#Region " Attribute Declarations "
'    Private m_bDisposed As Boolean = False
'    Private m_sCustomerNumber As String
'    Private m_sCustomerName As String
'    Private m_sShipToNumber As String
'    Private m_sCreditTerritory As String
'    Private m_sTermsCode As String
'    Private m_sBillToState As String
'    Private m_bAllShipTo As Boolean = False
'    Private m_sSecurityMHS As String
'    Private m_dInvoiceDate As String
'    Private m_dInvoiceToDate As String
'    Private m_bShowCredits As Boolean
'    Private m_bShowOldCredits As Boolean
'    Private m_sInvoiceNumber As String
'    Private m_sCreditNumber As String
'    Private m_sPONumber As String
'    Private m_sSortBy As String
'    Private m_bSortAscending As Boolean
'    Private m_iPageNumber As Int32
'    Private m_bQueryEPayStatus As Boolean

'#End Region

'#Region " Property Declarations "

'    Public Property CustomerNumber() As String
'        Get
'            Return m_sCustomerNumber
'        End Get
'        Set(ByVal value As String)
'            m_sCustomerNumber = value
'        End Set
'    End Property

'    Public Property CustomerName() As String
'        Get
'            Return m_sCustomerName
'        End Get
'        Set(ByVal value As String)
'            m_sCustomerName = value
'        End Set
'    End Property

'    Public Property ShipToNumber() As String
'        Get
'            Return m_sShipToNumber
'        End Get
'        Set(ByVal value As String)
'            m_sShipToNumber = value
'        End Set
'    End Property

'    Public Property CreditTerritory() As String
'        Get
'            Return m_sCreditTerritory
'        End Get
'        Set(ByVal value As String)
'            m_sCreditTerritory = value
'        End Set
'    End Property

'    Public Property TermsCode() As String
'        Get
'            Return m_sTermsCode
'        End Get
'        Set(ByVal value As String)
'            m_sTermsCode = value
'        End Set
'    End Property

'    Public Property BillToState() As String
'        Get
'            Return m_sBillToState
'        End Get
'        Set(ByVal value As String)
'            m_sBillToState = value
'        End Set
'    End Property

'    Public Property AllShipTos() As Boolean
'        Get
'            Return m_bAllShipTo
'        End Get
'        Set(ByVal value As Boolean)
'            m_bAllShipTo = value
'        End Set
'    End Property

'    Public Property SecurityMHS() As String
'        Get
'            Return m_sSecurityMHS
'        End Get
'        Set(ByVal value As String)
'            m_sSecurityMHS = value
'        End Set
'    End Property

'    Public Property InvoiceDate() As Date
'        Get
'            Return m_dInvoiceDate
'        End Get
'        Set(ByVal value As Date)
'            m_dInvoiceDate = value
'        End Set
'    End Property

'    Public Property InvoiceToDate() As Date
'        Get
'            Return m_dInvoiceToDate
'        End Get
'        Set(ByVal value As Date)
'            m_dInvoiceToDate = value
'        End Set
'    End Property

'    Public Property ShowCredits() As Boolean
'        Get
'            Return m_bShowCredits
'        End Get
'        Set(ByVal value As Boolean)
'            m_bShowCredits = value
'        End Set
'    End Property

'    Public Property ShowOldCredits() As Boolean
'        Get
'            Return m_bShowOldCredits
'        End Get
'        Set(ByVal value As Boolean)
'            m_bShowOldCredits = value
'        End Set
'    End Property

'    Public Property InvoiceNumber() As String
'        Get
'            Return m_sInvoiceNumber
'        End Get
'        Set(ByVal value As String)
'            m_sInvoiceNumber = value
'        End Set
'    End Property

'    Public Property CreditNumber() As String
'        Get
'            Return m_sCreditNumber
'        End Get
'        Set(ByVal value As String)
'            m_sCreditNumber = value
'        End Set
'    End Property

'    Public Property PONumber() As String
'        Get
'            Return m_sPONumber
'        End Get
'        Set(ByVal value As String)
'            m_sPONumber = value
'        End Set
'    End Property

'    Public Property SortBy() As String
'        Get
'            Return m_sSortBy
'        End Get
'        Set(ByVal value As String)
'            m_sSortBy = value
'        End Set
'    End Property

'    Public Property SortAscending() As Boolean
'        Get
'            Return m_bSortAscending
'        End Get
'        Set(ByVal value As Boolean)
'            m_bSortAscending = value
'        End Set
'    End Property

'    Public Property PageNumber() As Integer
'        Get
'            Return m_iPageNumber
'        End Get
'        Set(ByVal value As Integer)
'            m_iPageNumber = value
'        End Set
'    End Property

'    Public Property QueryEPayStatus() As Boolean
'        Get
'            Return m_bQueryEPayStatus
'        End Get
'        Set(ByVal value As Boolean)
'            m_bQueryEPayStatus = value
'        End Set
'    End Property

'#End Region

'#Region " Constructors Declarations "



'#End Region

'#Region " Private Methods "

'    Private Function GetNextEpayRefNo() As Integer

'        Dim dataReader As SqlClient.SqlDataReader = Nothing
'        Dim sbSQL As New System.Text.StringBuilder
'        Dim ePayRefNumb As Integer

'        Try
'            'Get Reference Number
'            sbSQL.Append("EXEC Ashley.dbo.usp_GetNextID 'EPayRefNum'")
'            dataReader = DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL.ToString)

'            If dataReader.HasRows = True Then
'                dataReader.Read()

'                ePayRefNumb = dataReader.Item("NextID")

'            Else
'                Throw New Exception("Can not get Next EPayRefNum")
'            End If

'            Return ePayRefNumb

'        Catch ex As Exception
'            Throw
'        Finally
'            If dataReader IsNot Nothing Then
'                dataReader.Close()
'            End If

'            dataReader = Nothing
'            sbSQL = Nothing
'        End Try

'    End Function

'#End Region

'#Region " Public Methods "

'    Public Function LoadInvoices() As DataTable

'        Dim sbSQL As New System.Text.StringBuilder

'        Try

'            With sbSQL

'                If Customer.DetermineIfToUseConsumerSearch(m_sCustomerNumber) = False Or m_sPONumber.Trim = "" Then
'                    If QueryEPayStatus Then
'                        .Append("EXEC Datawhse.dbo.usp_GetEpayInvoices '1', '" & m_dInvoiceToDate & "',")
'                    Else
'                        .Append("EXEC Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices1 ")
'                    End If

'                Else
'                    If QueryEPayStatus Then
'                        .Append("EXEC Datawhse.dbo.usp_GetEpayInvoices '0', '" & m_dInvoiceToDate & "',")
'                    Else
'                        .Append("EXEC Ashley.dbo.usp_GetOpenInvoicesFromNonAshleyPo2 ")

'                    End If
'                End If

'                .Append("'" & m_sCustomerNumber & "', ")
'                .Append("'" & m_sShipToNumber & "', ")

'                If m_bAllShipTo = True Then
'                    .Append("1, ")
'                Else
'                    .Append("0, ")
'                End If

'                .Append("'" & m_sSecurityMHS & "', ")
'                .Append("'" & m_dInvoiceDate & "', ")

'                If m_bShowCredits = True Then
'                    .Append("1, ")
'                Else
'                    .Append("0, ")
'                End If

'                If m_bShowOldCredits = True Then
'                    .Append("1, ")
'                Else
'                    .Append("0, ")
'                End If

'                .Append("'" & m_sInvoiceNumber & "', ")
'                .Append("'" & m_sCreditNumber & "', ")
'                .Append("'" & m_sPONumber & "', ")
'                .Append("'" & m_sSortBy & "', ")

'                If m_bSortAscending = True Then
'                    .Append("' ASC'")
'                Else
'                    .Append("' DESC'")
'                End If

'                If Customer.DetermineIfToUseConsumerSearch(m_sCustomerNumber) = False Or m_sPONumber.Trim = "" Then
'                    If Not QueryEPayStatus Then
'                        .Append(",'0', '0'")
'                    End If
'                End If

'            End With

'            System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

'            Return DataAccess.GetDataTable(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Function

'    Public Function LoadUnconfirmedPayments() As DataTable

'        Dim sbSQL As New System.Text.StringBuilder

'        Try

'            With sbSQL

'                .Append("EXEC Datawhse.dbo.usp_GetEpayUnconfirmedPayments ")

'                .Append("'" & m_sCustomerNumber & "', ")
'                .Append("'" & m_sShipToNumber & "', ")
'                .Append("'" & m_sSecurityMHS & "', ")
'                .Append("'" & m_sCreditTerritory & "', ")
'                .Append("'" & m_sSortBy & "', ")

'                If m_bSortAscending = True Then
'                    .Append("' ASC'")
'                Else
'                    .Append("' DESC'")
'                End If

'            End With

'            System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

'            Return DataAccess.GetDataTable(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Function

'    Public Function LoadEpayUsers(ByVal iPageNumber As Integer, ByVal iPageSize As Integer) As System.Data.SqlClient.SqlDataReader

'        Dim sbSQL As New System.Text.StringBuilder

'        Try

'            With sbSQL

'                .Append("EXEC Ashley.dbo.usp_GetEpayUsers ")

'                .Append("'" & m_sCustomerNumber & "', ")
'                .Append("'" & m_sCustomerName & "', ")
'                .Append("'" & m_sBillToState & "', ")
'                .Append("'" & m_sCreditTerritory & "', ")
'                .Append("'" & m_sTermsCode & "', ")

'                If Customer.DetermineIfToUseConsumerSearch(m_sCustomerNumber) = False Then
'                    .Append("'" & iPageNumber & "', '" & iPageSize & "'")
'                End If

'            End With

'            System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

'            Return DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL.ToString)

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Function


'    Public Shared Function DetermineIfToUseConsumerSearch(ByRef sCustomerNumber As String) As Boolean
'        ' Determine if this is a customer with consolidated POs where the 
'        ' originally entered PO#s are stored in extension tables
'        '(AAFES, for example).  In this case if a PO# is supplied
'        ' we must check for both the Ashley PO# and the customer's original PO#.

'        Dim AllowConsumerSearchDataReader As SqlClient.SqlDataReader = Nothing
'        Dim sbSQL As New System.Text.StringBuilder

'        Try

'            ' look up accounts that should check OPNORDEX and TSEXIN for non-Ashley PO#s
'            With sbSQL
'                .Append("SELECT ppeAllowConsumerSearch ")
'                .Append("FROM Ashley.dbo.tblPartnerProfileDetailExtended with (nolock) ")
'                .Append("WHERE ppePartnerNo = '" & sCustomerNumber & "' ")
'                .Append("AND ppeTransactionCode = '850'")

'            End With

'            AllowConsumerSearchDataReader = DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL.ToString)

'            If AllowConsumerSearchDataReader.HasRows = True Then
'                AllowConsumerSearchDataReader.Read()

'                If CType(AllowConsumerSearchDataReader.Item("ppeAllowConsumerSearch"), Boolean) = True Then
'                    DetermineIfToUseConsumerSearch = True

'                Else
'                    DetermineIfToUseConsumerSearch = False
'                End If
'            Else
'                DetermineIfToUseConsumerSearch = False
'            End If

'        Catch ex As Exception
'            Throw

'        Finally

'            If AllowConsumerSearchDataReader IsNot Nothing Then
'                AllowConsumerSearchDataReader.Close()
'            End If

'            AllowConsumerSearchDataReader = Nothing
'            sbSQL = Nothing

'        End Try

'    End Function

'    Public Function WriteInvoicesToEpay(ByRef xmlInvoices As String, ByRef refNo As Integer) As Decimal

'        Dim dataReader As SqlClient.SqlDataReader = Nothing
'        Dim sbSQL As New System.Text.StringBuilder
'        Dim totalInvAmount As Decimal = New Decimal(0)

'        Try
'            refNo = GetNextEpayRefNo()

'            ' call sproc to write invoices to EPay files
'            With sbSQL
'                .Append("EXEC Datawhse.dbo.usp_InsertInvoicesToEpay ")
'                .Append(refNo & ",'" & xmlInvoices & "'")
'            End With

'            'System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

'            dataReader = DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Batch, sbSQL.ToString)
'            If dataReader.HasRows = True Then
'                dataReader.Read()
'                totalInvAmount = dataReader.Item("total")
'            End If
'            'System.Web.HttpContext.Current.Trace.Warn(totalInvAmount.ToString)

'            Return totalInvAmount

'        Catch ex As Exception
'            Throw New Exception(ex.ToString & ":" & sbSQL.ToString)

'        Finally
'            If dataReader IsNot Nothing Then
'                dataReader.Close()
'            End If

'            dataReader = Nothing
'            sbSQL = Nothing
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

'        If disposing = True Then
'            ' Free managed resources 

'        End If

'        ' Free unmanaged resources 

'        ' Flag class as disposed 
'        m_bDisposed = True

'    End Sub

'    Protected Overrides Sub Finalize()
'        Me.Dispose(False)
'    End Sub

'#End Region



'End Class
