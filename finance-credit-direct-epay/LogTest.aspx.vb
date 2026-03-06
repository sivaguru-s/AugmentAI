Imports System.IO
Imports System.Configuration

Partial Public Class LogTest
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DisplayConfiguration()
        End If
    End Sub

    Private Sub DisplayConfiguration()
        Try
            Dim logPath As String = ConfigurationManager.AppSettings("LogFilePath")
            Dim logLevel As String = ConfigurationManager.AppSettings("LogLevel")
            Dim expectedFileName As String = String.Format("EPay_{0:yyyyMMdd}.log", DateTime.Now)
            Dim fullPath As String = Path.Combine(logPath, expectedFileName)

            lblConfig.Text = String.Format("<div class='info'>" & _
                                          "<strong>Log Path:</strong> {0}<br/>" & _
                                          "<strong>Log Level:</strong> {1}<br/>" & _
                                          "<strong>Expected Log File:</strong> {2}<br/>" & _
                                          "<strong>Current User:</strong> {3}<br/>" & _
                                          "<strong>App Pool Identity:</strong> {4}" & _
                                          "</div>", _
                                          logPath, _
                                          logLevel, _
                                          fullPath, _
                                          If(User IsNot Nothing AndAlso User.Identity IsNot Nothing, User.Identity.Name, "Not authenticated"), _
                                          System.Security.Principal.WindowsIdentity.GetCurrent().Name)
        Catch ex As Exception
            lblConfig.Text = "<span class='error'>Error reading configuration: " & ex.Message & "</span>"
        End Try
    End Sub

    Protected Sub btnTestLogging_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim testId As String = Guid.NewGuid().ToString().Substring(0, 8)
            Dim results As New System.Text.StringBuilder()

            results.Append("<div class='success'>✅ Logging test started (ID: " & testId & ")</div>")
            results.Append("<pre>")

            ' Test Debug level
            Logger.Debug("TEST [" & testId & "] - This is a DEBUG message", "LogTest")
            results.AppendLine("✓ Debug log written")

            ' Test Info level
            Logger.Info("TEST [" & testId & "] - This is an INFO message", "LogTest")
            results.AppendLine("✓ Info log written")

            ' Test Warning level
            Logger.Warning("TEST [" & testId & "] - This is a WARNING message", "LogTest")
            results.AppendLine("✓ Warning log written")

            ' Test Error level
            Logger.Error("TEST [" & testId & "] - This is an ERROR message", "LogTest")
            results.AppendLine("✓ Error log written")

            ' Test exception logging
            Try
                Throw New InvalidOperationException("Test exception for logging [" & testId & "]")
            Catch ex As Exception
                Logger.Error("TEST [" & testId & "] - Caught test exception", ex, "LogTest")
                results.AppendLine("✓ Exception log written")
            End Try

            ' Test Fatal level
            Logger.Fatal("TEST [" & testId & "] - This is a FATAL message", "LogTest")
            results.AppendLine("✓ Fatal log written")

            results.Append("</pre>")

            ' Add instructions
            Dim logPath As String = ConfigurationManager.AppSettings("LogFilePath")
            Dim expectedFileName As String = String.Format("EPay_{0:yyyyMMdd}.log", DateTime.Now)
            Dim fullPath As String = Path.Combine(logPath, expectedFileName)

            results.Append("<div class='info'><strong>Next Steps:</strong><br/>")
            results.Append("1. Check log file: <code>" & fullPath & "</code><br/>")
            results.Append("2. Search for test ID: <code>" & testId & "</code><br/>")
            results.Append("3. If file doesn't exist, check Windows Event Viewer:<br/>")
            results.Append("&nbsp;&nbsp;&nbsp;- Open Event Viewer<br/>")
            results.Append("&nbsp;&nbsp;&nbsp;- Navigate to: Windows Logs → Application<br/>")
            results.Append("&nbsp;&nbsp;&nbsp;- Filter by source: 'EPay Application'<br/>")
            results.Append("&nbsp;&nbsp;&nbsp;- Search for test ID: " & testId & "<br/>")
            results.Append("</div>")

            lblTestResult.Text = results.ToString()

        Catch ex As Exception
            lblTestResult.Text = "<span class='error'>❌ Test failed: " & ex.Message & "<br/>" & _
                               "Stack Trace:<br/><pre>" & ex.StackTrace & "</pre></span>"
        End Try
    End Sub

    Protected Sub btnTestPath_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim logPath As String = ConfigurationManager.AppSettings("LogFilePath")
            Dim results As New System.Text.StringBuilder()

            results.Append("<pre>")
            results.AppendLine("Testing network path access...")
            results.AppendLine("Path: " & logPath)
            results.AppendLine("")

            ' Test 1: Check if directory exists
            results.AppendLine("Test 1: Directory.Exists()")
            If Directory.Exists(logPath) Then
                results.AppendLine("✅ Directory exists")
            Else
                results.AppendLine("❌ Directory does NOT exist")
                results.AppendLine("Attempting to create directory...")
                Try
                    Directory.CreateDirectory(logPath)
                    results.AppendLine("✅ Directory created successfully")
                Catch createEx As Exception
                    results.AppendLine("❌ Failed to create directory: " & createEx.Message)
                End Try
            End If

            results.AppendLine("")

            ' Test 2: Try to write a test file
            results.AppendLine("Test 2: Write test file")
            Dim testFileName As String = Path.Combine(logPath, "test_" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".txt")
            Try
                File.WriteAllText(testFileName, "Test write at " & DateTime.Now.ToString())
                results.AppendLine("✅ Test file written successfully")
                results.AppendLine("File: " & testFileName)

                ' Try to read it back
                Dim content As String = File.ReadAllText(testFileName)
                results.AppendLine("✅ Test file read successfully")
                results.AppendLine("Content: " & content)

                ' Delete test file
                File.Delete(testFileName)
                results.AppendLine("✅ Test file deleted successfully")

            Catch writeEx As Exception
                results.AppendLine("❌ Failed to write test file: " & writeEx.Message)
                results.AppendLine("Exception Type: " & writeEx.GetType().Name)
                If TypeOf writeEx Is UnauthorizedAccessException Then
                    results.AppendLine("")
                    results.AppendLine("⚠️ PERMISSION ISSUE DETECTED!")
                    results.AppendLine("The IIS Application Pool identity does not have write permissions.")
                    results.AppendLine("Current identity: " & System.Security.Principal.WindowsIdentity.GetCurrent().Name)
                    results.AppendLine("")
                    results.AppendLine("Solution:")
                    results.AppendLine("1. On server 'aazeus-fnukap01', grant permissions to the folder:")
                    results.AppendLine("   icacls ""C:\Logs\epay"" /grant ""IIS APPPOOL\YourAppPoolName:(OI)(CI)F"" /T")
                    results.AppendLine("2. Or use a local path in Web.config:")
                    results.AppendLine("   <add key=""LogFilePath"" value=""C:\Logs\EPay""/>")
                End If
            End Try

            results.Append("</pre>")
            lblPathResult.Text = results.ToString()

        Catch ex As Exception
            lblPathResult.Text = "<span class='error'>❌ Path test failed: " & ex.Message & "<br/>" & _
                               "Stack Trace:<br/><pre>" & ex.StackTrace & "</pre></span>"
        End Try
    End Sub

End Class

