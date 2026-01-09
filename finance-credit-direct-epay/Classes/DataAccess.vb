Option Explicit On
Option Strict On

''' <summary>
''' Common Data Access Methods
''' </summary>
''' <remarks>
''' v1.0.0.0 - 00/00/0000 - Unknown - Created
''' v1.1.0.0 - 12/11/2012 - Ghunt - Resolve CA Error - Dispose of Objects before losing scope.
''' </remarks>
Public Class DataAccess

#Region " Attribute Declarations "
    Private Const COMMAND_TIMEOUT As Integer = 60

    ' Enumeration of valid return types that can be specidied for a stored procedure 
    Public Enum StoredProcedureReturnType
        DataReader = 0
        DataTable = 1
        RowsAffected = 2
        Scalar = 3
        XMLDataReader = 4
    End Enum

#End Region

#Region " Constructor "

    Private Sub New()
        ' constructor is marked as private so this class 
        ' cannot be instantiated                     
    End Sub

#End Region

#Region " Public Methods "

    Public Shared Function SetSQLParameterProperties( _
        ByVal parameterName As String, _
        ByVal parameterDbType As System.Data.DbType, _
        ByVal parameterValue As Object) As System.Data.SqlClient.SqlParameter

        Dim Parameter As New SqlClient.SqlParameter
        Parameter.ParameterName = parameterName
        Parameter.DbType = parameterDbType
        Parameter.Value = parameterValue

        Return Parameter

    End Function

    ' Returns a Sql Parameter object OVERLOADED ... parameterDirection
    ' --------------------------------------------------------------------------
    ' commandText: The text of the query to run.
    Public Shared Function SetSQLParameterProperties( _
        ByVal parameterName As String, _
        ByVal parameterDbType As System.Data.DbType, _
        ByVal parameterValue As Object, _
        ByVal parameterSize As Integer, _
        ByVal parameterDirection As System.Data.ParameterDirection) As System.Data.SqlClient.SqlParameter

        Dim Parameter As New SqlClient.SqlParameter
        Parameter.ParameterName = parameterName
        Parameter.DbType = parameterDbType
        Parameter.Value = parameterValue
        Parameter.Size = parameterSize
        Parameter.Direction = parameterDirection

        Return Parameter

    End Function

    ' Returns a SQLDataReader object that conforms to the IDataReader interface.
    ' --------------------------------------------------------------------------
    ' CommandText: The text of the query to run.
    Public Shared Function GetDataReader( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal commandText As String) As System.Data.SqlClient.SqlDataReader

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        ' call the common ashley Data component to return an open connection 
        Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
            AshleyData.SQLConn(MySqlConnection, sqlDatabase)

            ' instantiate the command object 
            Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                ' NOTE: the command behavior is set to close connection which means 
                ' the Database connection will be closed when the Data reader is closed 
                Return MySqlCommand.ExecuteReader(CommandBehavior.CloseConnection)
            End Using 'MySqlCommand

        End Using 'AshleyData

    End Function

    Public Shared Function GetDataReader( _
        ByVal sqlDatabase As String, _
        ByVal commandText As String) As System.Data.SqlClient.SqlDataReader

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        ' call the common ashley Data component to return an open connection 
        Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
            AshleyData.SQLConn(MySqlConnection, sqlDatabase)

            ' instantiate the command object 
            Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                ' NOTE: the command behavior is set to close connection which means 
                ' the Database connection will be closed when the Data reader is closed 
                Return MySqlCommand.ExecuteReader(CommandBehavior.CloseConnection)
            End Using 'MySqlCommand

        End Using 'AshleyData

    End Function

    ' Returns a Data table object. 
    ' --------------------------------------------------------------------------
    ' commandText: The text of the query to run.
    Public Shared Function GetDataTable( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal commandText As String) As DataTable

        Dim TempDataTable As DataTable = Nothing
        Dim ReturnDataTable As DataTable = Nothing
        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        Try
            TempDataTable = New DataTable

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' instantiate the command object 
                Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                    MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                    ' instantiate the Data aMySqlDataAdapterpter object 
                    Using MySqlDataAdapter As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter(MySqlCommand)
                        ' fill the Datatable with the query results 
                        MySqlDataAdapter.Fill(TempDataTable)

                    End Using 'MySqlDataAdapter

                End Using 'MySqlCommand

            End Using 'AshleyData

            ReturnDataTable = TempDataTable
            TempDataTable = Nothing

        Finally
            If TempDataTable IsNot Nothing Then TempDataTable.Dispose()

            If MySqlConnection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                MySqlConnection.Dispose()
            End If

        End Try

        ' return the filled Datatable 
        Return ReturnDataTable

    End Function

    Public Shared Function GetDataTable( _
        ByVal sqlDatabase As String, _
        ByVal commandText As String) As DataTable

        Dim TempDataTable As DataTable = Nothing
        Dim ReturnDataTable As DataTable = Nothing
        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        Try
            TempDataTable = New DataTable

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' instantiate the command object 
                Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                    MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                    ' instantiate the Data aMySqlDataAdapterpter object 
                    Using MySqlDataAdapter As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter(MySqlCommand)
                        ' fill the Datatable with the query results 
                        MySqlDataAdapter.Fill(TempDataTable)

                    End Using 'MySqlDataAdapter

                End Using 'MySqlCommand

            End Using 'AshleyData

            ReturnDataTable = TempDataTable
            TempDataTable = Nothing

        Finally
            If TempDataTable IsNot Nothing Then TempDataTable.Dispose()

            If MySqlConnection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                MySqlConnection.Dispose()
            End If

        End Try

        ' return the filled Datatable 
        Return ReturnDataTable

    End Function

    ' Queries the specified Database and return a scalar value.
    ' --------------------------------------------------------------------------
    ' commandText: The text of the query to run.
    Public Shared Function GetScalarValue( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal commandText As String) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        Try
            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' instantiate the command object 
                Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                    MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                    ' execute the command text and return the results 
                    Return MySqlCommand.ExecuteScalar

                End Using 'MySqlCommand

            End Using 'AshleyData

        Finally
            If MySqlConnection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                MySqlConnection.Dispose()
            End If

        End Try

    End Function

    Public Shared Function GetScalarValue( _
        ByVal sqlDatabase As String, _
        ByVal commandText As String) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        Try
            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' instantiate the command object 
                Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                    MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                    ' execute the command text and return the results 
                    Return MySqlCommand.ExecuteScalar

                End Using 'MySqlCommand

            End Using 'AshleyData

        Finally
            If MySqlConnection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                MySqlConnection.Dispose()
            End If

        End Try

    End Function

    ' Executes a command against the specified Database and returns the 
    ' number of rows affected. 
    ' --------------------------------------------------------------------------
    ' commandText: The text of the query to run.
    Public Overloads Shared Function ExecuteCommand( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal commandText As String) As Integer

        Dim rVal As Integer = 0
        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        Try
            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' instantiate the command object 
                Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                    MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                    ' execute the command text and return the number of rows affected 
                    rVal = MySqlCommand.ExecuteNonQuery

                End Using 'MySqlCommand

            End Using 'AshleyData

        Finally
            If MySqlConnection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                MySqlConnection.Dispose()
            End If

        End Try

        Return rVal

    End Function

    Public Overloads Shared Function ExecuteCommand( _
        ByVal sqlDatabase As String, _
        ByVal commandText As String) As Integer

        Dim rVal As Integer = 0
        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        Try
            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' instantiate the command object 
                Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                    MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                    ' execute the command text and return the number of rows affected 
                    rVal = MySqlCommand.ExecuteNonQuery

                End Using 'MySqlCommand

            End Using 'AshleyData

        Finally
            If MySqlConnection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                MySqlConnection.Dispose()
            End If

        End Try

        Return rVal

    End Function

    ' Executes a command against the specified Database and returns the 
    ' number of rows affected. 
    ' --------------------------------------------------------------------------
    ' commandText: The text of the query to run.
    ' Transaction: A transaction object which applies to the command.
    Public Overloads Shared Function ExecuteCommand( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal commandText As String, _
        ByRef transaction As System.Data.SqlClient.SqlTransaction) As Integer

        Dim rVal As Integer = 0
        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        Try
            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess

                If transaction Is Nothing Then
                    AshleyData.SQLConn(MySqlConnection, sqlDatabase)
                    transaction = MySqlConnection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MySqlConnection = transaction.Connection
                End If

                ' instantiate the command object 
                Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                    MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                    ' associate the transaction with the command
                    MySqlCommand.Transaction = transaction

                    ' execute the command text and return the number of rows affected 
                    rVal = MySqlCommand.ExecuteNonQuery

                End Using 'MySqlCommand

            End Using 'AshleyData

        Finally
            If MySqlConnection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                MySqlConnection.Dispose()
            End If

        End Try

        Return rVal

    End Function

    Public Overloads Shared Function ExecuteCommand( _
        ByVal sqlDatabase As String, _
        ByVal commandText As String, _
        ByRef transaction As System.Data.SqlClient.SqlTransaction) As Integer

        Dim rVal As Integer = 0
        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing

        Try
            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess

                If transaction Is Nothing Then
                    AshleyData.SQLConn(MySqlConnection, sqlDatabase)
                    transaction = MySqlConnection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MySqlConnection = transaction.Connection
                End If

                ' instantiate the command object 
                Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(commandText, MySqlConnection)
                    MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

                    ' associate the transaction with the command
                    MySqlCommand.Transaction = transaction

                    ' execute the command text and return the number of rows affected 
                    rVal = MySqlCommand.ExecuteNonQuery

                End Using 'MySqlCommand

            End Using 'AshleyData

        Finally
            If MySqlConnection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                MySqlConnection.Dispose()
            End If

        End Try

        Return rVal

    End Function

    ' Executes a stored procedure in the specified Database and 
    ' returns an object containing the return from the stored procedure 
    ' --------------------------------------------------------------------------
    ' StoredProcedureName: Name of the stored procedure to execute.
    ' ReturnType: Describes the output type expected.
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing
        Dim TempObject As Object = Nothing
        Dim ReturnObject As Object = Nothing

        Try
            TempObject = New Object

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' execute the stored procedure and return the appropriate type 
                TempObject = DataAccess.ExecuteStoredProcedureCommon( _
                    MySqlConnection, _
                    storedProcedureName, _
                    returnType, _
                    Nothing, _
                    Nothing)

            End Using 'AshleyData

            ReturnObject = TempObject
            TempObject = Nothing

        Finally
            If returnType <> StoredProcedureReturnType.DataReader OrElse TempObject IsNot Nothing Then
                'If TempObject IsNot Nothing then something failed before end of Try.  If that happens
                'or if the return type isnot a data reader then we need to dispose objects.

                If MySqlConnection IsNot Nothing Then
                    'Verify SQL connection is closed
                    If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()

                    'Dispose of SQL Connection
                    MySqlConnection.Dispose()

                End If 'MySqlConnection IsNot Nothing

            End If 'TempDataReader IsNot Nothing

        End Try

        Return ReturnObject

    End Function

    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal sqlDatabase As String, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing
        Dim TempObject As Object = Nothing
        Dim ReturnObject As Object = Nothing

        Try
            TempObject = New Object

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' execute the stored procedure and return the appropriate type 
                TempObject = DataAccess.ExecuteStoredProcedureCommon( _
                    MySqlConnection, _
                    storedProcedureName, _
                    returnType, _
                    Nothing, _
                    Nothing)

            End Using 'AshleyData

            ReturnObject = TempObject
            TempObject = Nothing

        Finally
            If returnType <> StoredProcedureReturnType.DataReader OrElse TempObject IsNot Nothing Then
                'If TempObject IsNot Nothing then something failed before end of Try.  If that happens
                'or if the return type isnot a data reader then we need to dispose objects.

                If MySqlConnection IsNot Nothing Then
                    'Verify SQL connection is closed
                    If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()

                    'Dispose of SQL Connection
                    MySqlConnection.Dispose()

                End If 'MySqlConnection IsNot Nothing

            End If 'TempDataReader IsNot Nothing

        End Try

        Return ReturnObject

    End Function

    ' Executes a stored procedure in the specified Database and 
    ' returns an object containing the return from the stored procedure 
    ' --------------------------------------------------------------------------
    ' StoredProcedureName: Name of the stored procedure to execute.
    ' ReturnType: Describes the output type expected.
    ' MyParameters: Collection of parameter objects for SP.
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing
        Dim TempObject As Object = Nothing
        Dim ReturnObject As Object = Nothing

        Try
            TempObject = New Object

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' execute the stored procedure and return the appropriate type 
                TempObject = DataAccess.ExecuteStoredProcedureCommon( _
                    MySqlConnection, _
                    storedProcedureName, _
                    returnType, _
                    parameters, _
                    Nothing)

            End Using
            ReturnObject = TempObject
            TempObject = Nothing

        Finally
            If returnType <> StoredProcedureReturnType.DataReader OrElse TempObject IsNot Nothing Then
                'If TempObject IsNot Nothing then something failed before end of Try.  If that happens
                'or if the return type isnot a data reader then we need to dispose objects.

                If MySqlConnection IsNot Nothing Then
                    'Verify SQL connection is closed
                    If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()

                    'Dispose of SQL Connection
                    MySqlConnection.Dispose()

                End If 'MySqlConnection IsNot Nothing

            End If 'TempDataReader IsNot Nothing

        End Try

        Return ReturnObject

    End Function

    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal sqlDatabase As String, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing
        Dim TempObject As Object = Nothing
        Dim ReturnObject As Object = Nothing

        Try
            TempObject = New Object

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                ' execute the stored procedure and return the appropriate type 
                TempObject = DataAccess.ExecuteStoredProcedureCommon( _
                    MySqlConnection, _
                    storedProcedureName, _
                    returnType, _
                    parameters, _
                    Nothing)

            End Using
            ReturnObject = TempObject
            TempObject = Nothing

        Finally
            If returnType <> StoredProcedureReturnType.DataReader OrElse TempObject IsNot Nothing Then
                'If TempObject IsNot Nothing then something failed before end of Try.  If that happens
                'or if the return type isnot a data reader then we need to dispose objects.

                If MySqlConnection IsNot Nothing Then
                    'Verify SQL connection is closed
                    If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()

                    'Dispose of SQL Connection
                    MySqlConnection.Dispose()

                End If 'MySqlConnection IsNot Nothing

            End If 'TempDataReader IsNot Nothing

        End Try

        Return ReturnObject

    End Function

    ' Executes a stored procedure in the specified Database and 
    ' returns an object containing the return from the stored procedure 
    ' --------------------------------------------------------------------------
    ' StoredProcedureName: Name of the stored procedure to execute.
    ' ReturnType: Describes the output type expected.
    ' Transaction: A transaction object which applies to the execution of the stored procedure.
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef transaction As System.Data.SqlClient.SqlTransaction) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing
        Dim TempObject As Object = Nothing
        Dim ReturnObject As Object = Nothing

        Try
            TempObject = New Object

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                If transaction Is Nothing Then
                    AshleyData.SQLConn(MySqlConnection, sqlDatabase)
                    transaction = MySqlConnection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MySqlConnection = transaction.Connection
                End If

                ' execute the stored procedure and return the appropriate type 
                TempObject = DataAccess.ExecuteStoredProcedureCommon( _
                    MySqlConnection, _
                    storedProcedureName, _
                    returnType, _
                    Nothing, _
                    transaction)

                ' Only Close and Dispose of Connection object for disconnected
                ' objects only
                If returnType <> StoredProcedureReturnType.DataReader Then
                    If MySqlConnection IsNot Nothing Then
                        If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                        MySqlConnection.Dispose()
                    End If
                End If

            End Using 'AshleyData

            ReturnObject = TempObject
            TempObject = Nothing

        Finally
            If returnType <> StoredProcedureReturnType.DataReader OrElse TempObject IsNot Nothing Then
                'If TempObject IsNot Nothing then something failed before end of Try.  If that happens
                'or if the return type isnot a data reader then we need to dispose objects.

                If MySqlConnection IsNot Nothing Then
                    'Verify SQL connection is closed
                    If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()

                    'Dispose of SQL Connection
                    MySqlConnection.Dispose()

                End If 'MySqlConnection IsNot Nothing

            End If 'TempDataReader IsNot Nothing

        End Try

        Return ReturnObject

    End Function

    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal sqlDatabase As String, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef transaction As System.Data.SqlClient.SqlTransaction) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing
        Dim TempObject As Object = Nothing
        Dim ReturnObject As Object = Nothing

        Try
            TempObject = New Object

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                If transaction Is Nothing Then
                    AshleyData.SQLConn(MySqlConnection, sqlDatabase)
                    transaction = MySqlConnection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MySqlConnection = transaction.Connection
                End If

                ' execute the stored procedure and return the appropriate type 
                TempObject = DataAccess.ExecuteStoredProcedureCommon( _
                    MySqlConnection, _
                    storedProcedureName, _
                    returnType, _
                    Nothing, _
                    transaction)

                ' Only Close and Dispose of Connection object for disconnected
                ' objects only
                If returnType <> StoredProcedureReturnType.DataReader Then
                    If MySqlConnection IsNot Nothing Then
                        If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()
                        MySqlConnection.Dispose()
                    End If
                End If

            End Using 'AshleyData
            ReturnObject = TempObject
            TempObject = Nothing

        Finally
            If returnType <> StoredProcedureReturnType.DataReader OrElse TempObject IsNot Nothing Then
                'If TempObject IsNot Nothing then something failed before end of Try.  If that happens
                'or if the return type isnot a data reader then we need to dispose objects.

                If MySqlConnection IsNot Nothing Then
                    'Verify SQL connection is closed
                    If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()

                    'Dispose of SQL Connection
                    MySqlConnection.Dispose()

                End If 'MySqlConnection IsNot Nothing

            End If 'TempDataReader IsNot Nothing

        End Try

        Return ReturnObject

    End Function

    ' Executes a stored procedure in the specified Database and 
    ' returns an object containing the return from the stored procedure 
    ' --------------------------------------------------------------------------
    ' StoredProcedureName: Name of the stored procedure to execute.
    ' ReturnType: Describes the output type expected.
    ' MyParameters: Collection of parameter objects for SP.
    ' Transaction: A transaction object which applies to the execution of the stored procedure.
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal sqlDatabase As Ashley.Data.DataAccess.SqlConnections, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection, _
        ByRef transaction As System.Data.SqlClient.SqlTransaction) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing
        Dim TempObject As Object = Nothing
        Dim ReturnObject As Object = Nothing

        Try
            TempObject = New Object

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                If transaction Is Nothing Then
                    AshleyData.SQLConn(MySqlConnection, sqlDatabase)
                    transaction = MySqlConnection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MySqlConnection = transaction.Connection
                End If

                ' execute the stored procedure and return the appropriate type 
                TempObject = DataAccess.ExecuteStoredProcedureCommon( _
                    MySqlConnection, _
                    storedProcedureName, _
                    returnType, _
                    parameters, _
                    transaction)

            End Using 'AshleyData

            ReturnObject = TempObject
            TempObject = Nothing

        Finally
            If returnType <> StoredProcedureReturnType.DataReader OrElse TempObject IsNot Nothing Then
                'If TempObject IsNot Nothing then something failed before end of Try.  If that happens
                'or if the return type isnot a data reader then we need to dispose objects.

                If MySqlConnection IsNot Nothing Then
                    'Verify SQL connection is closed
                    If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()

                    'Dispose of SQL Connection
                    MySqlConnection.Dispose()

                End If 'MySqlConnection IsNot Nothing

            End If 'TempDataReader IsNot Nothing

        End Try

        Return ReturnObject

    End Function

    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal sqlDatabase As String, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection, _
        ByRef transaction As System.Data.SqlClient.SqlTransaction) As Object

        Dim MySqlConnection As System.Data.SqlClient.SqlConnection = Nothing
        Dim TempObject As Object = Nothing
        Dim ReturnObject As Object = Nothing

        Try
            TempObject = New Object

            ' call the common ashley Data component to return an open connection 
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.SQLConn(MySqlConnection, sqlDatabase)

                If transaction Is Nothing Then
                    AshleyData.SQLConn(MySqlConnection, sqlDatabase)
                    transaction = MySqlConnection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MySqlConnection = transaction.Connection
                End If

                ' execute the stored procedure and return the appropriate type 
                TempObject = DataAccess.ExecuteStoredProcedureCommon( _
                    MySqlConnection, _
                    storedProcedureName, _
                    returnType, _
                    parameters, _
                    transaction)

            End Using 'AshleyData

            ReturnObject = TempObject
            TempObject = Nothing

        Finally
            If returnType <> StoredProcedureReturnType.DataReader OrElse TempObject IsNot Nothing Then
                'If TempObject IsNot Nothing then something failed before end of Try.  If that happens
                'or if the return type isnot a data reader then we need to dispose objects.

                If MySqlConnection IsNot Nothing Then
                    'Verify SQL connection is closed
                    If Not MySqlConnection.State = ConnectionState.Closed Then MySqlConnection.Close()

                    'Dispose of SQL Connection
                    MySqlConnection.Dispose()

                End If 'MySqlConnection IsNot Nothing

            End If 'TempDataReader IsNot Nothing

        End Try

        Return ReturnObject

    End Function

#End Region

#Region " Private Methods "

    ' Private method containing core stored procedure execution logic that is
    ' indepenMySqlDataAdapternt of whether a transaction has been specified.
    ' --------------------------------------------------------------------------
    ' Connection: An open SQLConnection object.</param>
    ' StoredProcedureName: The name of the stored procedure to run.</param>
    ' MyParameters: A collection of parameters to be passed to the stored procedure.
    ' ReturnType: Describes the type of Data expected to be returned.
    ' --------------------------------------------------------------------------
    ' Returns: An object representing an instance of the ReturnType. 
    Private Shared Function ExecuteStoredProcedureCommon( _
        ByVal connection As System.Data.SqlClient.SqlConnection, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection, _
        ByRef transaction As System.Data.SqlClient.SqlTransaction) As Object

        Dim iParameterIndex As Integer

        Using MySqlCommand As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(storedProcedureName, connection)
            ' set the command parameters 
            MySqlCommand.CommandType = CommandType.StoredProcedure
            MySqlCommand.CommandTimeout = COMMAND_TIMEOUT

            If Not parameters Is Nothing Then
                ' Add command parameters 
                For iParameterIndex = 1 To (parameters.Count)
                    MySqlCommand.Parameters.Add(parameters.Item(iParameterIndex))
                Next
            End If

            ' if a transaction object exists, associate it with the command object 
            If Not transaction Is Nothing Then
                MySqlCommand.Transaction = transaction
            End If

            ' execute the stored procedure and return the appropriate type 
            Select Case returnType

                Case StoredProcedureReturnType.DataReader
                    Return MySqlCommand.ExecuteReader(CommandBehavior.CloseConnection)

                Case StoredProcedureReturnType.DataTable

                    Using MySqlDataAdapter As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter(MySqlCommand)
                        Using TempDataTable As DataTable = New DataTable
                            ' fill the Datatable with the query results 
                            MySqlDataAdapter.Fill(TempDataTable)
                            Return TempDataTable
                        End Using
                    End Using 'MySqlDataAdapter

                Case StoredProcedureReturnType.RowsAffected
                    Return MySqlCommand.ExecuteNonQuery()

                Case StoredProcedureReturnType.Scalar
                    Return MySqlCommand.ExecuteScalar()

                Case StoredProcedureReturnType.XMLDataReader
                    Return MySqlCommand.ExecuteXmlReader()

                Case Else
                    Return Nothing

            End Select

        End Using 'MySqlCommand

    End Function

#End Region

End Class


