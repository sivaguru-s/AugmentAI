# EPay Logging - Quick Start Guide

## 🚀 Quick Setup (5 Minutes)

### Step 1: Verify Configuration ✅

Your Web.config already has:
```xml
<add key="LogFilePath" value="\\aazeus-fnukap01\c$\Logs\epay"/>
<add key="LogLevel" value="Info"/>
```

### Step 2: Test the Logger

1. **Navigate to the test page:**
   ```
   http://your-server/epay/LogTest.aspx
   ```

2. **Click "Test Network Path"** to verify permissions

3. **Click "Run Logging Test"** to generate test logs

4. **Check the results** - it will tell you exactly where to find the logs

---

## 📝 Common Issues & Quick Fixes

### Issue: "Directory does NOT exist"

**Fix on server `aazeus-fnukap01`:**
```powershell
# Run as Administrator
New-Item -Path "C:\Logs\epay" -ItemType Directory -Force
```

### Issue: "Permission denied" or "UnauthorizedAccessException"

**Fix on server `aazeus-fnukap01`:**
```powershell
# Run as Administrator
# Replace 'YourAppPoolName' with your actual app pool name
icacls "C:\Logs\epay" /grant "IIS APPPOOL\YourAppPoolName:(OI)(CI)F" /T

# Or grant to Network Service:
icacls "C:\Logs\epay" /grant "NETWORK SERVICE:(OI)(CI)F" /T

# Or grant to the web server machine account:
icacls "C:\Logs\epay" /grant "DOMAIN\WebServerName$:(OI)(CI)F" /T
```

### Issue: Still not working?

**Temporary workaround - Use local path:**

Update Web.config:
```xml
<add key="LogFilePath" value="C:\inetpub\wwwroot\EPay\Logs"/>
```

Then check logs in: `C:\inetpub\wwwroot\EPay\Logs\EPay_YYYYMMDD.log`

---

## 💡 Usage Examples

### Basic Logging in Your Code

```vb
' At the top of your .vb file - Logger is already available globally

' Info logging
Logger.Info("User logged in successfully", "Login.aspx")

' Warning
Logger.Warning("Database connection slow", "main.aspx")

' Error with exception
Try
    ' Your code
Catch ex As Exception
    Logger.Error("Failed to load data", ex, "main.aspx")
End Try
```

### LoadGrid() Logging Example

```vb
Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    Try
        Logger.Info("Page_Load started", "main.aspx")
        
        If Not IsPostBack Then
            ' Log before LoadGrid
            Logger.Info("Loading initial grid data", "main.aspx")
            
            Dim startTime As DateTime = DateTime.Now
            Me.LoadGrid(1)
            Dim duration As TimeSpan = DateTime.Now.Subtract(startTime)
            
            ' Log after LoadGrid
            Logger.Info(String.Format("Grid loaded in {0:F2} seconds", duration.TotalSeconds), "main.aspx")
        End If
        
    Catch ex As Exception
        Logger.Error("Error in Page_Load", ex, "main.aspx")
    End Try
End Sub
```

---

## 🔍 Where to Find Logs

### Option 1: Network File (Primary)
```
\\aazeus-fnukap01\c$\Logs\epay\EPay_20250303.log
```

### Option 2: Windows Event Viewer (Fallback)
1. Open **Event Viewer** on the web server
2. Navigate to: **Windows Logs → Application**
3. Filter by source: **"EPay Application"**

---

## 📊 Log File Format

```
2025-03-03 10:15:22.123 | [INFO   ] | Thread:5  | Source:main.aspx | User:DOMAIN\jsmith | Page_Load started
2025-03-03 10:15:22.145 | [INFO   ] | Thread:5  | Source:main.aspx | User:DOMAIN\jsmith | Loading initial grid data
2025-03-03 10:15:24.567 | [INFO   ] | Thread:5  | Source:main.aspx | User:DOMAIN\jsmith | Grid loaded in 2.42 seconds
2025-03-03 10:16:10.234 | [ERROR  ] | Thread:8  | Source:main.aspx | User:DOMAIN\jsmith | Failed to load data
Exception: Timeout expired...
StackTrace: at System.Data.SqlClient...
```

---

## ✅ Verification Checklist

Run through this checklist:

1. [ ] Navigate to `http://your-server/epay/LogTest.aspx`
2. [ ] Click "Test Network Path" - should show ✅ green checkmarks
3. [ ] Click "Run Logging Test" - should show ✅ success message
4. [ ] Open log file: `\\aazeus-fnukap01\c$\Logs\epay\EPay_YYYYMMDD.log`
5. [ ] Search for the test ID shown on the test page
6. [ ] Verify you see all test log entries

If any step fails, see **LOGGING_TROUBLESHOOTING.md** for detailed solutions.

---

## 🆘 Need Help?

1. **Run the test page first:** `http://your-server/epay/LogTest.aspx`
2. **Check Event Viewer** if file logging fails
3. **Review:** `docs/LOGGING_TROUBLESHOOTING.md`
4. **Contact IT** if network path permissions are needed

---

## Copyright

Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.  
This software is proprietary and confidential.

