'(c) AshleyFurniture Industries, Inc. 2009
'All Rights Reserved

Option Explicit On
Option Strict On

Imports IBM.Data.DB2.iSeries

Public Class DB2DataAccess

#Region " Attribute Declarations "

    ' Enumeration of valid return types that can be specidied for a stored procedure 
    Public Enum StoredProcedureReturnType
        DataReader = 0
        DataTable = 1
        RowsAffected = 2
        Scalar = 3
    End Enum

#End Region

#Region " Constructor "

    Private Sub New()
        ' constructor is marked as private so this class 
        ' cannot be instantiated                     
    End Sub

#End Region

#Region " Public Methods "

    ' Returns a Sql Parameter object OVERLOADED 
    ' --------------------------------------------------------------------------
    ' commandText: The text of the query to run.
    Public Shared Function SetSQLParameterProperties( _
        ByVal parameterName As String, _
        ByVal parameterDbType As iDB2DbType, _
        ByVal parameterValue As Object) As iDB2Parameter

        Dim Parameter As New iDB2Parameter
        Parameter.ParameterName = parameterName
        Parameter.iDB2DbType = parameterDbType
        Parameter.Value = parameterValue
        Return Parameter

    End Function

    ' Returns a Sql Parameter object OVERLOADED ... parameterDirection
    ' --------------------------------------------------------------------------
    ' commandText: The text of the query to run.
    Public Shared Function SetSQLParameterProperties( _
        ByVal parameterName As String, _
        ByVal parameterDbType As iDB2DbType, _
        ByVal parameterValue As Object, _
        ByVal parameterSize As Integer, _
        ByVal parameterDirection As System.Data.ParameterDirection) As iDB2Parameter

        Dim Parameter As New iDB2Parameter
        Parameter.ParameterName = parameterName
        Parameter.iDB2DbType = parameterDbType
        Parameter.Value = parameterValue
        Parameter.Size = parameterSize
        Parameter.Direction = parameterDirection
        Return Parameter

    End Function


    ''' <summary>Returns a iDB2DataReader with the data returned from the command.</summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <returns>iDB2DataReader</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Shared Function GetDataReader( _
        ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
        ByVal commandText As String) As iDB2DataReader

        Dim MyDB2Connection As iDB2Connection = Nothing

        Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
            ' call the common ashley Data component to return an open connection 
            AshleyData.DB2Conn(MyDB2Connection, db2Database)

            ' instantiate the command object 
            Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                ' NOTE: the command behavior is set to close connection which means 
                ' the Database connection will be closed when the Data reader is closed 
                Return MyDB2Command.ExecuteReader(CommandBehavior.CloseConnection)
            End Using 'MyDB2Command

        End Using 'AshleyData

    End Function

    ''' <summary>Returns a iDB2DataReader with the data returned from the command.</summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Shared Function GetDataReader( _
        ByVal db2Database As String, _
        ByVal commandText As String) As iDB2DataReader

        Dim MyDB2Connection As iDB2Connection = Nothing

        Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
            ' call the common ashley Data component to return an open connection 
            AshleyData.DB2Conn(MyDB2Connection, db2Database)

            ' instantiate the command object 
            Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                ' NOTE: the command behavior is set to close connection which means 
                ' the Database connection will be closed when the Data reader is closed 
                Return MyDB2Command.ExecuteReader(CommandBehavior.CloseConnection)
            End Using 'MyDB2Command

        End Using 'AshleyData

    End Function

    ''' <summary>Returns a DataTable with the data returned from the command.</summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Shared Function GetDataTable( _
            ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
            ByVal commandText As String) As DataTable

        Dim TempDataTable As DataTable = Nothing
        Dim ReturnDataTable As DataTable = Nothing
        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            TempDataTable = New DataTable

            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                ' call the common ashley Data component to return an open connection 
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' instantiate the command object 
                Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                    'MyDB2Command.CommandTimeout = COMMAND_TIMEOUT

                    ' instantiate the Data aMySqlDataAdapterpter object 
                    Using MyiDB2DataAdapter As iDB2DataAdapter = New iDB2DataAdapter(MyDB2Command)
                        ' fill the Datatable with the query results 
                        MyiDB2DataAdapter.Fill(TempDataTable)

                    End Using 'MyiDB2DataAdapter

                End Using 'MyDB2Command

            End Using 'AshleyData

            ReturnDataTable = TempDataTable
            TempDataTable = Nothing

        Finally
            If TempDataTable IsNot Nothing Then TempDataTable.Dispose()

            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

        ' return the filled Datatable 
        Return ReturnDataTable

    End Function

    ''' <summary>Returns a DataTable with the data returned from the command.</summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Shared Function GetDataTable( _
        ByVal db2Database As String, _
        ByVal commandText As String) As DataTable

        Dim TempDataTable As DataTable = Nothing
        Dim ReturnDataTable As DataTable = Nothing
        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            TempDataTable = New DataTable

            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                ' call the common ashley Data component to return an open connection 
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' instantiate the command object 
                Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                    'MyDB2Command.CommandTimeout = COMMAND_TIMEOUT

                    ' instantiate the Data aMySqlDataAdapterpter object 
                    Using MyiDB2DataAdapter As iDB2DataAdapter = New iDB2DataAdapter(MyDB2Command)
                        ' fill the Datatable with the query results 
                        MyiDB2DataAdapter.Fill(TempDataTable)

                    End Using 'MyiDB2DataAdapter

                End Using 'MyDB2Command

            End Using 'AshleyData

            ReturnDataTable = TempDataTable
            TempDataTable = Nothing

        Finally
            If TempDataTable IsNot Nothing Then TempDataTable.Dispose()

            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

        ' return the filled Datatable 
        Return ReturnDataTable

    End Function

    ''' <summary>Returns Scalar (value from first cell of first row) returned from the command.</summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Shared Function GetScalarValue( _
        ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
        ByVal commandText As String) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                ' call the common ashley Data component to return an open connection 
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' instantiate the command object 
                Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                    'MyDB2Command.CommandTimeout = COMMAND_TIMEOUT

                    ' execute the command text and return the results 
                    Return MyDB2Command.ExecuteScalar

                End Using 'MyDB2Command

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Returns Scalar (value from first cell of first row) returned from the command.</summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    Public Shared Function GetScalarValue( _
        ByVal db2Database As String, _
        ByVal commandText As String) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                ' call the common ashley Data component to return an open connection 
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' instantiate the command object 
                Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                    'MyDB2Command.CommandTimeout = COMMAND_TIMEOUT

                    ' execute the command text and return the results 
                    Return MyDB2Command.ExecuteScalar

                End Using 'MyDB2Command

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Returns number of rows affected by the command.</summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <returns>Integer</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Overloads Shared Function ExecuteCommand( _
        ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
        ByVal commandText As String) As Integer

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                ' call the common ashley Data component to return an open connection 
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' instantiate the command object 
                Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                    'MyDB2Command.CommandTimeout = COMMAND_TIMEOUT

                    ' execute the command text and return the number of rows affected 
                    Return MyDB2Command.ExecuteNonQuery

                End Using

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Returns number of rows affected by the command.</summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <returns>Integer</returns>
    ''' <remarks></remarks>
    Public Overloads Shared Function ExecuteCommand( _
        ByVal db2Database As String, _
        ByVal commandText As String) As Integer

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                ' call the common ashley Data component to return an open connection 
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' instantiate the command object 
                Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                    'MyDB2Command.CommandTimeout = COMMAND_TIMEOUT

                    ' execute the command text and return the number of rows affected 
                    Return MyDB2Command.ExecuteNonQuery

                End Using

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Returns number of rows affected by the command.</summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <param name="transaction">Transaction</param>
    ''' <returns>Integer</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Overloads Shared Function ExecuteCommand( _
            ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
            ByVal commandText As String, _
            ByRef transaction As iDB2Transaction) As Integer

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                ' call the common ashley Data component to return an open connection 
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                If transaction Is Nothing Then
                    AshleyData.DB2Conn(MyDB2Connection, db2Database)
                    transaction = MyDB2Connection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MyDB2Connection = transaction.Connection
                End If

                ' instantiate the command object 
                Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                    'MyDB2Command.CommandTimeout = COMMAND_TIMEOUT

                    ' associate the transaction with the command
                    MyDB2Command.Transaction = transaction

                    ' execute the command text and return the number of rows affected 
                    Return MyDB2Command.ExecuteNonQuery

                End Using

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Returns number of rows affected by the command.</summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="commandText">The text of the query to run.</param>
    ''' <param name="transaction">Transaction</param>
    ''' <returns>Integer</returns>
    ''' <remarks></remarks>
    Public Overloads Shared Function ExecuteCommand( _
        ByVal db2Database As String, _
        ByVal commandText As String, _
        ByRef transaction As iDB2Transaction) As Integer

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                ' call the common ashley Data component to return an open connection 
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                If transaction Is Nothing Then
                    AshleyData.DB2Conn(MyDB2Connection, db2Database)
                    transaction = MyDB2Connection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MyDB2Connection = transaction.Connection
                End If

                ' instantiate the command object 
                Using MyDB2Command As iDB2Command = New iDB2Command(commandText, MyDB2Connection)
                    'MyDB2Command.CommandTimeout = COMMAND_TIMEOUT

                    ' associate the transaction with the command
                    MyDB2Command.Transaction = transaction

                    ' execute the command text and return the number of rows affected 
                    Return MyDB2Command.ExecuteNonQuery

                End Using

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Returns number of rows affected by the command.</summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="storedProcedureName">The name of the stored procedure run.</param>
    ''' <param name="returnType">What type of Object to Return.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Overloads Shared Function ExecuteStoredProcedure( _
            ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
            ByVal storedProcedureName As String, _
            ByVal returnType As StoredProcedureReturnType) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' execute the stored procedure and return the appropriate type 
                Return ExecuteStoredProcedureCommon( _
                    MyDB2Connection, _
                    storedProcedureName, _
                    returnType, _
                    Nothing, _
                    Nothing)

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Returns number of rows affected by the command.</summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="storedProcedureName">The name of the stored procedure run.</param>
    ''' <param name="returnType">What type of Object to Return.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal db2Database As String, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' execute the stored procedure and return the appropriate type 
                Return ExecuteStoredProcedureCommon( _
                    MyDB2Connection, _
                    storedProcedureName, _
                    returnType, _
                    Nothing, _
                    Nothing)

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function


    ''' <summary>Executes a stored procedure in the specified Database and returns an object containing the return from the stored procedure </summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="storedProcedureName">Name of the stored procedure to execute.</param>
    ''' <param name="returnType">Describes the output type expected.</param>
    ''' <param name="parameters">Collection of parameter objects for SP.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Overloads Shared Function ExecuteStoredProcedure( _
            ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
            ByVal storedProcedureName As String, _
            ByVal returnType As StoredProcedureReturnType, _
            ByRef parameters As Collection) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' execute the stored procedure and return the appropriate type 
                Return ExecuteStoredProcedureCommon( _
                    MyDB2Connection, _
                    storedProcedureName, _
                    returnType, _
                    parameters, _
                    Nothing)

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Executes a stored procedure in the specified Database and returns an object containing the return from the stored procedure </summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="storedProcedureName">Name of the stored procedure to execute.</param>
    ''' <param name="returnType">Describes the output type expected.</param>
    ''' <param name="parameters">Collection of parameter objects for SP.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal db2Database As String, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' execute the stored procedure and return the appropriate type 
                Return ExecuteStoredProcedureCommon( _
                    MyDB2Connection, _
                    storedProcedureName, _
                    returnType, _
                    parameters, _
                    Nothing)

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Executes a stored procedure in the specified Database and returns an object containing the return from the stored procedure </summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="storedProcedureName">Name of the stored procedure to execute.</param>
    ''' <param name="returnType">Describes the output type expected.</param>
    ''' <param name="transaction">Transaction.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef transaction As iDB2Transaction) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' execute the stored procedure and return the appropriate type 
                Return ExecuteStoredProcedureCommon( _
                    MyDB2Connection, _
                    storedProcedureName, _
                    returnType, _
                    Nothing, _
                    transaction)

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Executes a stored procedure in the specified Database and returns an object containing the return from the stored procedure </summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="storedProcedureName">Name of the stored procedure to execute.</param>
    ''' <param name="returnType">Describes the output type expected.</param>
    ''' <param name="transaction">Transaction.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal db2Database As String, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef transaction As iDB2Transaction) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                AshleyData.DB2Conn(MyDB2Connection, db2Database)

                ' execute the stored procedure and return the appropriate type 
                Return ExecuteStoredProcedureCommon( _
                    MyDB2Connection, _
                    storedProcedureName, _
                    returnType, _
                    Nothing, _
                    transaction)

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Executes a stored procedure in the specified Database and returns an object containing the return from the stored procedure </summary>
    ''' <param name="db2Database">Obsolete.  Please specify the database value as a String. Example: 'AFI_Dynamic'</param>
    ''' <param name="storedProcedureName">Name of the stored procedure to execute.</param>
    ''' <param name="returnType">Describes the output type expected.</param>
    ''' <param name="parameters">Collection of parameter objects for SP.</param>
    ''' <param name="transaction">Transaction.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    <ObsoleteAttribute("This method is obsolete.  Please switch Database parameter value to a String. Example: 'AFI_Dynamic'")> _
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal db2Database As Ashley.Data.DataAccess.Db2Connections, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection, _
        ByRef transaction As iDB2Transaction) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                If transaction Is Nothing Then
                    AshleyData.DB2Conn(MyDB2Connection, db2Database)
                    transaction = MyDB2Connection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MyDB2Connection = transaction.Connection
                End If

                ' execute the stored procedure and return the appropriate type 
                Return ExecuteStoredProcedureCommon( _
                    MyDB2Connection, _
                    storedProcedureName, _
                    returnType, _
                    parameters, _
                    transaction)

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

    ''' <summary>Executes a stored procedure in the specified Database and returns an object containing the return from the stored procedure </summary>
    ''' <param name="db2Database">The safeFileEntry name of the database server. Example: 'AFI_Dynamic'</param>
    ''' <param name="storedProcedureName">Name of the stored procedure to execute.</param>
    ''' <param name="returnType">Describes the output type expected.</param>
    ''' <param name="parameters">Collection of parameter objects for SP.</param>
    ''' <param name="transaction">Transaction.</param>
    ''' <returns>Object</returns>
    ''' <remarks></remarks>
    Public Overloads Shared Function ExecuteStoredProcedure( _
        ByVal db2Database As String, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection, _
        ByRef transaction As iDB2Transaction) As Object

        Dim MyDB2Connection As iDB2Connection = Nothing

        Try
            Using AshleyData As Ashley.Data.DataAccess = New Ashley.Data.DataAccess
                If transaction Is Nothing Then
                    AshleyData.DB2Conn(MyDB2Connection, db2Database)
                    transaction = MyDB2Connection.BeginTransaction()
                Else
                    ' get the connection from the transaction 
                    MyDB2Connection = transaction.Connection
                End If

                ' execute the stored procedure and return the appropriate type 
                Return ExecuteStoredProcedureCommon( _
                    MyDB2Connection, _
                    storedProcedureName, _
                    returnType, _
                    parameters, _
                    transaction)

            End Using 'AshleyData

        Finally
            If MyDB2Connection IsNot Nothing Then
                'SqlConnections are one of the few objects that need both close and dispose called  
                If Not MyDB2Connection.State = ConnectionState.Closed Then MyDB2Connection.Close()
                MyDB2Connection.Dispose()
            End If

        End Try

    End Function

#End Region

#Region " Private Methods "

    ''' <summary>Used within this class to execut all procedures in the same manner.</summary>
    ''' <param name="connection"></param>
    ''' <param name="storedProcedureName"></param>
    ''' <param name="returnType"></param>
    ''' <param name="parameters"></param>
    ''' <param name="transaction"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteStoredProcedureCommon( _
        ByVal connection As iDB2Connection, _
        ByVal storedProcedureName As String, _
        ByVal returnType As StoredProcedureReturnType, _
        ByRef parameters As Collection, _
        ByRef transaction As iDB2Transaction) As Object

        Dim iParameterIndex As Integer

        Using MyDB2Command As New iDB2Command(storedProcedureName, connection)
            MyDB2Command.CommandType = CommandType.StoredProcedure
            MyDB2Command.CommandTimeout = 300

            If Not parameters Is Nothing Then
                ' Add command parameters 
                For iParameterIndex = 1 To (parameters.Count)
                    MyDB2Command.Parameters.Add(parameters.Item(iParameterIndex))
                Next
            End If

            ' if a transaction object exists, associate it with the command object 
            If Not transaction Is Nothing Then
                MyDB2Command.Transaction = transaction
            End If

            ' execute the stored procedure and return the appropriate type 
            Select Case returnType

                Case StoredProcedureReturnType.DataReader
                    Return MyDB2Command.ExecuteReader(CommandBehavior.CloseConnection)

                Case StoredProcedureReturnType.DataTable
                    Using MyiDB2DataAdapter As iDB2DataAdapter = New iDB2DataAdapter(MyDB2Command)
                        Using TempDataTable As DataTable = New DataTable
                            ' fill the Datatable with the query results 
                            MyiDB2DataAdapter.Fill(TempDataTable)
                            Return TempDataTable
                        End Using
                    End Using 'MyiDB2DataAdapter

                Case StoredProcedureReturnType.RowsAffected
                    Return MyDB2Command.ExecuteNonQuery()

                Case StoredProcedureReturnType.Scalar
                    Return MyDB2Command.ExecuteScalar()

                Case Else
                    Return Nothing

            End Select

        End Using 'MySqlCommand

    End Function

#End Region

End Class
