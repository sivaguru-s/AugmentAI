# EPay Logging Troubleshooting Guide

## Current Configuration

**Web.config Settings:**
```xml
<add key="LogFilePath" value="\\aazeus-fnukap01\c$\Logs\epay"/>
<add key="LogLevel" value="Info"/>
```

---

## Common Issues and Solutions

### ✅ **Issue 1: Logs Not Appearing in Network Path**

**Symptoms:**
- No log files created in `\\aazeus-fnukap01\c$\Logs\epay`
- No errors displayed in application

**Root Cause:**
- IIS Application Pool identity doesn't have write permissions to the network path
- Network path is not accessible from the web server

**Solutions:**

#### Option A: Check Windows Event Viewer (Fallback Logs)
1. Open **Windows Event Viewer** on the web server
2. Navigate to: **Windows Logs → Application**
3. Look for events from source: **"EPay Application"**
4. These will contain the log entries if file logging failed

#### Option B: Grant Network Permissions
1. Identify the IIS Application Pool identity:
   - Open **IIS Manager**
   - Find your EPay application
   - Note the Application Pool name
   - Go to **Application Pools** → Right-click your pool → **Advanced Settings**
   - Check the **Identity** (usually `ApplicationPoolIdentity` or `NetworkService`)

2. Grant permissions on the network share:
   ```powershell
   # On the server aazeus-fnukap01, run as Administrator:
   icacls "C:\Logs\epay" /grant "IIS APPPOOL\YourAppPoolName:(OI)(CI)F" /T
   
   # Or grant to NetworkService:
   icacls "C:\Logs\epay" /grant "NETWORK SERVICE:(OI)(CI)F" /T
   
   # Or grant to specific domain account:
   icacls "C:\Logs\epay" /grant "DOMAIN\WebServerName$:(OI)(CI)F" /T
   ```

#### Option C: Use Local Path for Testing
Update Web.config temporarily:
```xml
<add key="LogFilePath" value="C:\Logs\EPay"/>
```

Then check if logs appear in `C:\Logs\EPay` on the web server.

---

### ✅ **Issue 2: Logger Class Not Found**

**Symptoms:**
- Compilation error: "Type 'Logger' is not defined"

**Solution:**
Ensure `Logger.vb` is in the `Classes` folder and the file is included in the project:
1. Right-click `Classes/Logger.vb` in Solution Explorer
2. Select **Properties**
3. Ensure **Build Action** is set to **Compile**

---

### ✅ **Issue 3: Logger Not Initializing**

**Symptoms:**
- Application runs but no logs appear anywhere (not in file or Event Viewer)

**Solution:**
Add explicit initialization in `Global.asax.vb`:

```vb
Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
    ' Initialize logger on application start
    Logger.Initialize()
    Logger.Info("EPay Application Started", "Global.asax")
End Sub
```

---

### ✅ **Issue 4: Permission Denied Errors**

**Symptoms:**
- Error in Event Viewer: "UnauthorizedAccessException"
- Logs appear in Event Viewer but not in file

**Solution:**

1. **Check folder exists:**
   ```powershell
   # On aazeus-fnukap01 server:
   Test-Path "C:\Logs\epay"
   # If False, create it:
   New-Item -Path "C:\Logs\epay" -ItemType Directory -Force
   ```

2. **Check network path accessibility from web server:**
   ```powershell
   # On the web server, run:
   Test-Path "\\aazeus-fnukap01\c$\Logs\epay"
   
   # Try to create a test file:
   "Test" | Out-File "\\aazeus-fnukap01\c$\Logs\epay\test.txt"
   ```

3. **Grant full permissions:**
   - Right-click `C:\Logs\epay` on aazeus-fnukap01
   - Properties → Security → Edit
   - Add the web server's machine account: `DOMAIN\WebServerName$`
   - Grant **Full Control**

---

### ✅ **Issue 5: Logs Not Showing Recent Entries**

**Symptoms:**
- Log file exists but doesn't update with new entries

**Solution:**

1. **Check log file is not locked:**
   ```powershell
   # Check if file is in use:
   Get-Process | Where-Object {$_.Modules.FileName -like "*EPay_*.log*"}
   ```

2. **Recycle Application Pool:**
   - Open IIS Manager
   - Right-click Application Pool → **Recycle**
   - Try the action again

3. **Check file permissions:**
   - Ensure IIS identity has **Modify** permissions on the log file

---

## 🧪 **Testing the Logger**

### Test 1: Create a Test Page

Create `LogTest.aspx` in the root:

```aspx
<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LogTest.aspx.vb" Inherits="LogTest" %>
<!DOCTYPE html>
<html>
<head><title>Logger Test</title></head>
<body>
    <h1>EPay Logger Test</h1>
    <form runat="server">
        <asp:Button ID="btnTest" runat="server" Text="Test Logger" OnClick="btnTest_Click" />
        <br /><br />
        <asp:Label ID="lblResult" runat="server" />
    </form>
</body>
</html>
```

Create `LogTest.aspx.vb`:

```vb
Partial Public Class LogTest
    Inherits System.Web.UI.Page

    Protected Sub btnTest_Click(sender As Object, e As EventArgs)
        Try
            ' Test all log levels
            Logger.Debug("This is a DEBUG message", "LogTest")
            Logger.Info("This is an INFO message", "LogTest")
            Logger.Warning("This is a WARNING message", "LogTest")
            Logger.Error("This is an ERROR message", "LogTest")
            
            ' Test exception logging
            Try
                Throw New Exception("Test exception for logging")
            Catch ex As Exception
                Logger.Error("Caught test exception", ex, "LogTest")
            End Try
            
            Logger.Fatal("This is a FATAL message", "LogTest")
            
            lblResult.Text = "✅ Logging test completed!<br/>" & _
                           "Check log file at: \\aazeus-fnukap01\c$\Logs\epay\EPay_" & DateTime.Now.ToString("yyyyMMdd") & ".log<br/>" & _
                           "Or check Windows Event Viewer → Application → 'EPay Application'"
            lblResult.ForeColor = Drawing.Color.Green
            
        Catch ex As Exception
            lblResult.Text = "❌ Error: " & ex.Message
            lblResult.ForeColor = Drawing.Color.Red
        End Try
    End Sub
End Class
```

### Test 2: Check Windows Event Viewer

1. Open **Event Viewer** on the web server
2. Navigate to: **Windows Logs → Application**
3. Filter by source: **"EPay Application"**
4. You should see log entries if file logging failed

### Test 3: Manual File Check

```powershell
# On the web server, run PowerShell as Administrator:

# Check if path is accessible
Test-Path "\\aazeus-fnukap01\c$\Logs\epay"

# List log files
Get-ChildItem "\\aazeus-fnukap01\c$\Logs\epay" -Filter "EPay_*.log"

# View latest log file
Get-Content "\\aazeus-fnukap01\c$\Logs\epay\EPay_$(Get-Date -Format 'yyyyMMdd').log" -Tail 20
```

---

## 🔍 **Diagnostic Checklist**

Run through this checklist:

- [ ] Logger.vb file exists in `Classes` folder
- [ ] Logger.vb has `Imports System.Web` at the top
- [ ] Web.config has `LogFilePath` and `LogLevel` settings
- [ ] Network path `\\aazeus-fnukap01\c$\Logs\epay` exists
- [ ] Folder `C:\Logs\epay` exists on aazeus-fnukap01 server
- [ ] IIS Application Pool identity has write permissions to the folder
- [ ] Web server can access the network path (test with `Test-Path`)
- [ ] Application Pool has been recycled after changes
- [ ] Windows Event Viewer shows "EPay Application" entries (fallback)
- [ ] No compilation errors in Visual Studio

---

## 📊 **Expected Log Output**

When working correctly, you should see:

**File:** `\\aazeus-fnukap01\c$\Logs\epay\EPay_20250303.log`

```
2025-03-03 10:15:22.123 | [INFO   ] | Thread:5    | Source:Logger.Initialize | Logger initialized successfully
2025-03-03 10:15:25.456 | [INFO   ] | Thread:8    | Source:main.aspx | User:DOMAIN\jsmith | Page_Load started
2025-03-03 10:15:25.478 | [INFO   ] | Thread:8    | Source:main.aspx | User:DOMAIN\jsmith | Loading initial grid data - DateRange: 02/01/2025 to 03/03/2025, SortColumn: opidagedt, SortAsc: True
2025-03-03 10:15:27.890 | [INFO   ] | Thread:8    | Source:main.aspx | User:DOMAIN\jsmith | Grid loaded successfully in 2.41 seconds - Page: 1
```

---

## 🆘 **Still Not Working?**

If logging still doesn't work after trying all solutions:

1. **Check IIS Application Pool Identity:**
   ```powershell
   Import-Module WebAdministration
   Get-Item "IIS:\AppPools\YourAppPoolName" | Select-Object processModel
   ```

2. **Enable impersonation temporarily in Web.config:**
   ```xml
   <system.web>
     <identity impersonate="true" userName="DOMAIN\ServiceAccount" password="Password"/>
   </system.web>
   ```

3. **Use local path as fallback:**
   ```xml
   <add key="LogFilePath" value="C:\inetpub\wwwroot\EPay\Logs"/>
   ```

4. **Check Application Event Log for detailed errors**

---

## Copyright

Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.  
This software is proprietary and confidential.

