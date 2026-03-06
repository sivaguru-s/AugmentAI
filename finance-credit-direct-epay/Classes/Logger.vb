Imports System.IO
Imports System.Text
Imports System.Configuration
Imports System.Threading
Imports System.Web

''' <summary>
''' Thread-safe file logger for EPay application that writes to a shared network path.
''' Automatically creates log files if they don't exist.
''' </summary>
''' <remarks>
''' Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.
''' This software is proprietary and confidential.
''' </remarks>
Public Class Logger

#Region " Private Fields "

    Private Shared ReadOnly _lockObject As New Object()
    Private Shared _logFilePath As String = String.Empty
    Private Shared _isInitialized As Boolean = False
    Private Const DEFAULT_LOG_PATH As String = "\\shared-server\logs\EPay"
    Private Const MAX_LOG_FILE_SIZE_MB As Integer = 50
    Private Const MAX_RETRY_ATTEMPTS As Integer = 3

#End Region

#Region " Enums "

    ''' <summary>
    ''' Log severity levels
    ''' </summary>
    Public Enum LogLevel
        Debug = 0
        Info = 1
        Warning = 2
        [Error] = 3
        Fatal = 4
    End Enum

#End Region

#Region " Initialization "

    ''' <summary>
    ''' Initialize the logger with configuration from Web.config
    ''' </summary>
    Public Shared Sub Initialize()
        SyncLock _lockObject
            If _isInitialized Then Return

            Try
                ' Read log path from Web.config, fallback to default
                Dim configPath As String = ConfigurationManager.AppSettings("LogFilePath")
                If String.IsNullOrEmpty(configPath) Then
                    configPath = DEFAULT_LOG_PATH
                End If

                ' Create log file path with date
                Dim fileName As String = String.Format("EPay_{0:yyyyMMdd}.log", DateTime.Now)
                _logFilePath = Path.Combine(configPath, fileName)

                ' Ensure directory exists
                EnsureDirectoryExists(configPath)

                _isInitialized = True

                ' Write initialization message
                WriteLog(LogLevel.Info, "Logger initialized successfully", "Logger.Initialize")

            Catch ex As Exception
                ' Fallback to local path if shared path fails
                Dim localPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs")
                Dim fileName As String = String.Format("EPay_{0:yyyyMMdd}.log", DateTime.Now)
                _logFilePath = Path.Combine(localPath, fileName)
                EnsureDirectoryExists(localPath)
                _isInitialized = True

                WriteLog(LogLevel.Warning, "Failed to initialize with shared path, using local path: " & ex.Message, "Logger.Initialize")
            End Try
        End SyncLock
    End Sub

    ''' <summary>
    ''' Ensure the log directory exists, create if necessary
    ''' </summary>
    Private Shared Sub EnsureDirectoryExists(ByVal directoryPath As String)
        Try
            If Not Directory.Exists(directoryPath) Then
                Directory.CreateDirectory(directoryPath)
            End If
        Catch ex As Exception
            Throw New IOException("Failed to create log directory: " & directoryPath, ex)
        End Try
    End Sub

#End Region

#Region " Public Logging Methods "

    ''' <summary>
    ''' Write a debug log entry
    ''' </summary>
    Public Shared Sub Debug(ByVal message As String, Optional ByVal source As String = "")
        WriteLog(LogLevel.Debug, message, source)
    End Sub

    ''' <summary>
    ''' Write an info log entry
    ''' </summary>
    Public Shared Sub Info(ByVal message As String, Optional ByVal source As String = "")
        WriteLog(LogLevel.Info, message, source)
    End Sub

    ''' <summary>
    ''' Write a warning log entry
    ''' </summary>
    Public Shared Sub Warning(ByVal message As String, Optional ByVal source As String = "")
        WriteLog(LogLevel.Warning, message, source)
    End Sub

    ''' <summary>
    ''' Write an error log entry
    ''' </summary>
    Public Shared Sub [Error](ByVal message As String, Optional ByVal source As String = "")
        WriteLog(LogLevel.Error, message, source)
    End Sub

    ''' <summary>
    ''' Write an error log entry with exception details
    ''' </summary>
    Public Shared Sub [Error](ByVal message As String, ByVal ex As Exception, Optional ByVal source As String = "")
        Dim fullMessage As String = String.Format("{0}{1}Exception: {2}{1}StackTrace: {3}", _
                                                   message, Environment.NewLine, ex.Message, ex.StackTrace)
        WriteLog(LogLevel.Error, fullMessage, source)
    End Sub

    ''' <summary>
    ''' Write a fatal error log entry
    ''' </summary>
    Public Shared Sub Fatal(ByVal message As String, Optional ByVal source As String = "")
        WriteLog(LogLevel.Fatal, message, source)
    End Sub

    ''' <summary>
    ''' Write a fatal error log entry with exception details
    ''' </summary>
    Public Shared Sub Fatal(ByVal message As String, ByVal ex As Exception, Optional ByVal source As String = "")
        Dim fullMessage As String = String.Format("{0}{1}Exception: {2}{1}StackTrace: {3}", _
                                                   message, Environment.NewLine, ex.Message, ex.StackTrace)
        WriteLog(LogLevel.Fatal, fullMessage, source)
    End Sub

#End Region

#Region " Core Logging Logic "

    ''' <summary>
    ''' Core method to write log entries to file
    ''' </summary>
    Private Shared Sub WriteLog(ByVal level As LogLevel, ByVal message As String, ByVal source As String)
        If Not _isInitialized Then
            Initialize()
        End If

        Dim retryCount As Integer = 0
        Dim success As Boolean = False

        While retryCount < MAX_RETRY_ATTEMPTS AndAlso Not success
            Try
                SyncLock _lockObject
                    ' Check if log file needs rotation (daily or size-based)
                    CheckLogRotation()

                    ' Format log entry
                    Dim logEntry As String = FormatLogEntry(level, message, source)

                    ' Write to file with retry logic
                    Using writer As New StreamWriter(_logFilePath, True, Encoding.UTF8)
                        writer.WriteLine(logEntry)
                        writer.Flush()
                    End Using

                    success = True
                End SyncLock

            Catch ex As IOException
                retryCount += 1
                If retryCount >= MAX_RETRY_ATTEMPTS Then
                    ' Log to Windows Event Log as fallback
                    WriteToEventLog(level, message, source, ex)
                Else
                    ' Wait before retry
                    Thread.Sleep(100)
                End If

            Catch ex As UnauthorizedAccessException
                ' Permission issue - log to event log
                WriteToEventLog(level, message, source, ex)
                Exit While

            Catch ex As Exception
                ' Unexpected error - log to event log
                WriteToEventLog(level, message, source, ex)
                Exit While
            End Try
        End While
    End Sub

    ''' <summary>
    ''' Format log entry with timestamp, level, source, and message
    ''' </summary>
    Private Shared Function FormatLogEntry(ByVal level As LogLevel, ByVal message As String, ByVal source As String) As String
        Dim sb As New StringBuilder()

        ' Timestamp
        sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
        sb.Append(" | ")

        ' Log Level
        sb.Append(String.Format("[{0,-7}]", level.ToString().ToUpper()))
        sb.Append(" | ")

        ' Thread ID
        sb.Append(String.Format("Thread:{0,-4}", Thread.CurrentThread.ManagedThreadId))
        sb.Append(" | ")

        ' Source
        If Not String.IsNullOrEmpty(source) Then
            sb.Append(String.Format("Source:{0}", source))
            sb.Append(" | ")
        End If

        ' User (if available)
        Try
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.User IsNot Nothing Then
                Dim userName As String = HttpContext.Current.User.Identity.Name
                If Not String.IsNullOrEmpty(userName) Then
                    sb.Append(String.Format("User:{0}", userName))
                    sb.Append(" | ")
                End If
            End If
        Catch
            ' Ignore if HttpContext not available
        End Try

        ' Message
        sb.Append(message)

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Check if log file needs rotation (daily or size-based)
    ''' </summary>
    Private Shared Sub CheckLogRotation()
        Try
            ' Check if file exists
            If Not File.Exists(_logFilePath) Then
                Return ' File will be created automatically
            End If

            Dim fileInfo As New FileInfo(_logFilePath)

            ' Check if file is from a different day
            Dim currentDate As String = DateTime.Now.ToString("yyyyMMdd")
            Dim fileName As String = Path.GetFileNameWithoutExtension(_logFilePath)

            If Not fileName.Contains(currentDate) Then
                ' Rotate to new file for new day
                Dim configPath As String = Path.GetDirectoryName(_logFilePath)
                Dim newFileName As String = String.Format("EPay_{0}.log", currentDate)
                _logFilePath = Path.Combine(configPath, newFileName)
                Return
            End If

            ' Check file size (rotate if > MAX_LOG_FILE_SIZE_MB)
            Dim maxSizeBytes As Long = MAX_LOG_FILE_SIZE_MB * 1024 * 1024
            If fileInfo.Length > maxSizeBytes Then
                ' Archive current file
                Dim archivePath As String = _logFilePath.Replace(".log", String.Format("_{0:HHmmss}.log", DateTime.Now))
                File.Move(_logFilePath, archivePath)
            End If

        Catch ex As Exception
            ' Ignore rotation errors - continue with current file
        End Try
    End Sub

    ''' <summary>
    ''' Fallback: Write to Windows Event Log if file logging fails
    ''' </summary>
    Private Shared Sub WriteToEventLog(ByVal level As LogLevel, ByVal message As String, ByVal source As String, ByVal ex As Exception)
        Try
            Dim eventLogSource As String = "EPay Application"
            Dim eventLogName As String = "Application"

            ' Create event source if it doesn't exist
            If Not Diagnostics.EventLog.SourceExists(eventLogSource) Then
                Diagnostics.EventLog.CreateEventSource(eventLogSource, eventLogName)
            End If

            ' Map log level to event log entry type
            Dim entryType As Diagnostics.EventLogEntryType
            Select Case level
                Case LogLevel.Debug, LogLevel.Info
                    entryType = Diagnostics.EventLogEntryType.Information
                Case LogLevel.Warning
                    entryType = Diagnostics.EventLogEntryType.Warning
                Case Else
                    entryType = Diagnostics.EventLogEntryType.Error
            End Select

            ' Write to event log
            Dim eventMessage As String = String.Format("Source: {0}{1}Message: {2}{1}File Logging Error: {3}", _
                                                       source, Environment.NewLine, message, ex.Message)
            Diagnostics.EventLog.WriteEntry(eventLogSource, eventMessage, entryType)

        Catch
            ' Silently fail if event log also fails
        End Try
    End Sub

#End Region

End Class

