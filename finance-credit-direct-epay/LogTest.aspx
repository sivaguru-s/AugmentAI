<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LogTest.aspx.vb" Inherits="LogTest" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EPay Logger Test</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; }
        h1 { color: #333; }
        .test-section { background: #f5f5f5; padding: 20px; margin: 20px 0; border-radius: 5px; }
        .success { color: green; font-weight: bold; }
        .error { color: red; font-weight: bold; }
        .info { color: blue; }
        pre { background: #fff; padding: 10px; border: 1px solid #ddd; overflow-x: auto; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>🔍 EPay Logger Test Page</h1>
        
        <div class="test-section">
            <h2>Configuration</h2>
            <asp:Label ID="lblConfig" runat="server" />
        </div>
        
        <div class="test-section">
            <h2>Test Logging</h2>
            <asp:Button ID="btnTestLogging" runat="server" Text="Run Logging Test" OnClick="btnTestLogging_Click" />
            <br /><br />
            <asp:Label ID="lblTestResult" runat="server" />
        </div>
        
        <div class="test-section">
            <h2>Test Network Path Access</h2>
            <asp:Button ID="btnTestPath" runat="server" Text="Test Network Path" OnClick="btnTestPath_Click" />
            <br /><br />
            <asp:Label ID="lblPathResult" runat="server" />
        </div>
        
        <div class="test-section">
            <h2>Instructions</h2>
            <ol>
                <li>Click <strong>"Run Logging Test"</strong> to test all log levels</li>
                <li>Click <strong>"Test Network Path"</strong> to verify network access</li>
                <li>Check the log file location shown above</li>
                <li>If file logging fails, check <strong>Windows Event Viewer → Application → "EPay Application"</strong></li>
            </ol>
        </div>
    </form>
</body>
</html>

