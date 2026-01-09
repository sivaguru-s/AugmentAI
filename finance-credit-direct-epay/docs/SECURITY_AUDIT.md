# Security Audit Report - Finance Credit Direct EPay

**Version:** 1.0  
**Date:** January 9, 2026  
**Status:** Critical Issues Identified  
**Auditor:** Augment AI Security Analysis  

---

## Executive Summary

This security audit has identified **CRITICAL** vulnerabilities in the Finance Credit Direct EPay application that require immediate attention. The application handles sensitive financial data and payment processing, making these vulnerabilities particularly severe.

### Risk Level: **CRITICAL** 🔴

**Key Findings:**
- 🔴 **SQL Injection vulnerabilities** (Multiple instances)
- 🔴 **Hardcoded credentials** in configuration files
- 🟡 **Weak authentication** in debug mode
- 🟡 **Insufficient input validation**
- 🟡 **Sensitive data exposure** in logs and error messages
- 🟡 **Outdated framework** (.NET 3.5)

---

## Critical Vulnerabilities

### 1. SQL Injection Vulnerabilities (CRITICAL 🔴)

**Severity:** CRITICAL  
**CVSS Score:** 9.8  
**CWE:** CWE-89 (SQL Injection)

#### 1.1 String Concatenation in SQL Queries

**Location:** `Classes/Common.vb`

**Vulnerable Code Examples:**

```vb
' Line 62 - GetTotals method
sbSQL = "EXEC Datawhse.dbo.usp_GetEpayTotal '" & custNum & "'," & referenceNumber

' Line 84 - UpdateEpayStatus method
.Append("'" & custNum & "'," & referenceNumber & ",'" & status & "', '" & sUserChanged & "'")

' Line 198 - DeleteEpayRecords method
sbSQL = "EXEC Datawhse.dbo.usp_DeleteEpay '" & custNum & "'," & referenceNumber & ", '" & sUserChanged & "'"

' Line 216 - DeleteEpayUnconfirmedPayment method
sbSQL = "EXEC Datawhse.dbo.usp_DeleteEpayUnconfirmedPayment '" & custNum & "', '" & invoiceNumber & "', '" & sUserChanged & "'"

' Line 236 - LoadEpayInvoicesByRefNumber method
sbSQL = "EXEC Datawhse.dbo.usp_GetEpayInvoicesByRefNumber '" & custNum & "'," & referenceNumber

' Line 305-306 - LoadEpayAnalystReport method
sbSQL.Append("EXEC Datawhse.dbo.usp_GetEpayAnalystReport '" & customerNumber & "','" & referenceNumber & "','" _
     & ConfirmationNo & "','" & paymentAdded & "','" & CreditTerritory & "','" & sortBy & "', ")

' Line 382-386 - LoadUnconfirmedPayments method
.Append("'" & sCustomerNumber & "', ")
.Append("'" & sShipToNumber & "', ")
.Append("'" & sSecurityMHS & "', ")
.Append("'" & sCreditTerritory & "', ")
.Append("'" & sSortColumn & "', ")

' Line 420-427 - LoadEpayUsers method
.Append("'" & sCustomerNumber & "', ")
.Append("'" & sCustomerName & "', ")
.Append("'" & sBillToState & "', ")
.Append("'" & sCreditTerritory & "', ")
.Append("'" & sTermsCode & "', ")
```

**Impact:**
- Attackers can execute arbitrary SQL commands
- Complete database compromise possible
- Data exfiltration of sensitive financial information
- Data manipulation or deletion
- Potential privilege escalation

**Exploitation Example:**
```
custNum = "1011000'; DROP TABLE tblEpay; --"
```

#### 1.2 Dynamic SQL in Stored Procedures

**Location:** `SQL/usp_OrderAndInvoiceReportingOpenInvoices2_CREATE.sql`

**Vulnerable Code (Lines 137-144):**
```sql
AND T1.opicusno=''' + @customerNumber + '''  
AND T1.opidagedt <= ''' + cast(@FromDate as varchar) + ''' 
AND T1.opidagedt >= ''' + cast(@ToDate as varchar) + ''' 
AND ((T4.acrec <> ''S''
        AND T1.opicusno=T3.seccusno
        AND T1.opishpno=T3.secshpno
        AND T3.secMhs_name = ''' + @securityMHS + ''')
    OR (T4.acrec = ''S'')) '
```

**Additional vulnerable concatenations (Lines 147-169):**
```sql
SELECT @sqlString = @sqlString + 'AND (T1.opishpno=''' + @shiptoNumber + ''' '
SELECT @sqlString = @sqlString + 'AND T1.opiinvno LIKE ''' + @searchInvoice + '%'' '
SELECT @sqlString = @sqlString + 'AND T1.opicrmnr LIKE ''' + @searchCredit + '%'' '
SELECT @sqlString = @sqlString + 'AND T1.opiponum LIKE ''' + @searchPO + '%'' '
```

**Impact:**
- Second-order SQL injection
- Bypass of security controls
- Access to unauthorized customer data

**Recommendation:**
```sql
-- Use parameterized queries instead
WHERE T1.opicusno = @customerNumber
  AND T1.opidagedt <= @FromDate
  AND T1.opidagedt >= @ToDate
  AND T3.secMhs_name = @securityMHS
```

### 2. Hardcoded Credentials (CRITICAL 🔴)

**Severity:** CRITICAL  
**CVSS Score:** 9.1  
**CWE:** CWE-798 (Use of Hard-coded Credentials)

**Location:** `Web.config` (Lines 22-24)

**Vulnerable Code:**
```xml
<!--<identity impersonate="true" userName="AshleyFurniture\AppDBUserDev" password="Jbbfy19"/>-->
<!--<identity impersonate="true" userName="AshleyFurniture\AppDBUserStage" password="Rpghw84"/>-->
<!--<identity impersonate="true" userName="AshleyFurniture\AppDBUserProd" password="Wprtub52"/>-->
```

**Impact:**
- Credentials exposed in source control
- Potential unauthorized database access
- Lateral movement in network
- Compliance violations (PCI-DSS, SOX)

**Recommendation:**
- Remove all hardcoded credentials immediately
- Use Windows Authentication or Azure Key Vault
- Rotate all exposed credentials
- Audit access logs for unauthorized use

### 3. Weak Authentication in Debug Mode (HIGH 🟡)

**Severity:** HIGH
**CVSS Score:** 7.5
**CWE:** CWE-287 (Improper Authentication)

**Location:** `Classes/EpayBasePage.vb` (Lines 49-65)

**Vulnerable Code:**
```vb
#If DEBUG Then
    Protected Overloads Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If String.IsNullOrEmpty(SessionData("LOGON_USER")) Then
            'Impersonate an end user
            SessionData("LOGON_USER") = "tkiel"
            SessionData("SECURITY_MHS") = GetUserMHS(SessionData("LOGON_USER"))
            SessionData("APPLICATION_SECURITY_ACCESS") = GetApplicationSecurityAccess(SessionData("LOGON_USER"))

            'Set customer selection
            SessionData("CUSTOMERNUMBER") = "1011000"
            SessionData("SHIPTONUMBER") = ""
            SessionData("ALLSHIPTOS") = "TRUE"
            GetSelectedAccountContactInformation()
        End If
    End Sub
#End If
```

**Impact:**
- Bypass authentication in debug builds
- Unauthorized access to customer data
- Risk if debug build deployed to production

**Recommendation:**
- Never deploy debug builds to production
- Implement proper authentication even in debug mode
- Use feature flags instead of compilation directives

### 4. Insufficient Input Validation (MEDIUM 🟡)

**Severity:** MEDIUM
**CVSS Score:** 6.5
**CWE:** CWE-20 (Improper Input Validation)

#### 4.1 Minimal Validation in LoadInvoices

**Location:** `Classes/Common.vb` (Lines 351-353)

**Vulnerable Code:**
```vb
parameters.Add(New SqlClient.SqlParameter("@searchInvoice", sInvoiceNumber.Replace("'", String.Empty)))
parameters.Add(New SqlClient.SqlParameter("@searchCredit", sCreditNumber.Replace("'", String.Empty)))
parameters.Add(New SqlClient.SqlParameter("@searchPo", sPONumber.Replace("'", "''")))
```

**Issues:**
- Only removes single quotes, not comprehensive sanitization
- Inconsistent escaping methods
- No length validation
- No format validation

**Recommendation:**
```vb
' Implement comprehensive validation
If Not String.IsNullOrWhiteSpace(sInvoiceNumber) Then
    If Not Regex.IsMatch(sInvoiceNumber, "^[0-9]{1,9}$") Then
        Throw New ArgumentException("Invalid invoice number format")
    End If
End If
```

#### 4.2 Date Validation Issues

**Location:** `main.aspx.vb` (Line 171)

**Vulnerable Code:**
```vb
If txtFromDate.Text = Nothing OrElse txtToDate.Text = Nothing OrElse
   Not IsDate(txtFromDate.Text) = True OrElse Not IsDate(txtToDate.Text) = True OrElse
   txtFromDate.Text <= DateAdd(DateInterval.Year, -200, Today) OrElse
   txtToDate.Text <= DateAdd(DateInterval.Year, -200, Today) Then
```

**Issues:**
- Weak date validation
- No range checking
- Potential for date manipulation attacks

### 5. Sensitive Data Exposure (MEDIUM 🟡)

**Severity:** MEDIUM
**CVSS Score:** 6.1
**CWE:** CWE-532 (Information Exposure Through Log Files)

#### 5.1 Trace Warnings with Sensitive Data

**Location:** Multiple files

**Examples:**
```vb
' Common.vb Line 64
System.Web.HttpContext.Current.Trace.Warn("Totals sbSQL:" & sbSQL.ToString)

' Common.vb Line 314
System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

' Common.vb Line 398
System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)
```

**Impact:**
- SQL queries with customer data logged
- Potential exposure of sensitive financial information
- Compliance violations

**Recommendation:**
- Remove trace statements from production code
- Implement structured logging with data masking
- Use proper log levels (Debug, Info, Warning, Error)

#### 5.2 Custom Errors Disabled

**Location:** `Web.config` (Line 25)

**Vulnerable Code:**
```xml
<customErrors mode="Off"/>
```

**Impact:**
- Detailed error messages exposed to users
- Stack traces reveal application structure
- Information disclosure vulnerability

**Recommendation:**
```xml
<customErrors mode="RemoteOnly" defaultRedirect="~/Error.aspx">
  <error statusCode="404" redirect="~/NotFound.aspx"/>
  <error statusCode="500" redirect="~/Error.aspx"/>
</customErrors>
```

### 6. Outdated Framework and Dependencies (MEDIUM 🟡)

**Severity:** MEDIUM
**CVSS Score:** 5.9
**CWE:** CWE-1104 (Use of Unmaintained Third Party Components)

**Location:** `EPay.vbproj` (Line 23), `Web.config`

**Issues:**
- .NET Framework 3.5 (Released 2007, End of Support)
- ASP.NET Web Forms (Legacy technology)
- ActiveReports 7.1 (Outdated version)
- No security patches available

**Impact:**
- Known vulnerabilities in framework
- No security updates
- Difficult to maintain
- Compliance issues

**Recommendation:**
- Migrate to .NET 6/8 (LTS versions)
- Consider migrating to ASP.NET Core
- Update all third-party dependencies
- Implement dependency scanning in CI/CD

### 7. Insecure Direct Object References (MEDIUM 🟡)

**Severity:** MEDIUM
**CVSS Score:** 6.5
**CWE:** CWE-639 (Authorization Bypass Through User-Controlled Key)

**Location:** `Confirmation.aspx.vb` (Lines 27-39)

**Vulnerable Code:**
```vb
If Request.Params("CusNo") <> Nothing Then
    cmdCancel.Enabled = False
    cmdOK.Enabled = False
End If

If Request.Params("RefNo") <> "" Then
    lblReferenceNumber.Text = Request.Params("RefNo")
    lblTotalAmt.Text = FormatCurrency(GetTotal(Request.Params("RefNo")), 2)
    GetReport()
End If
```

**Issues:**
- Direct use of user-supplied reference numbers
- No authorization check
- Users can access other customers' payment confirmations

**Recommendation:**
```vb
' Validate user has access to this reference number
Dim refNo As Integer = Integer.Parse(Request.Params("RefNo"))
If Not ValidateUserAccessToReference(refNo, SessionData("CUSTOMERNUMBER")) Then
    Response.Redirect("~/Unauthorized.aspx")
    Return
End If
```

### 8. Cross-Site Scripting (XSS) Potential (LOW 🟢)

**Severity:** LOW
**CVSS Score:** 4.3
**CWE:** CWE-79 (Cross-Site Scripting)

**Location:** `main.aspx.vb` (Lines 261-336)

**Issues:**
- Direct binding of database values to UI controls
- Limited output encoding
- Potential for stored XSS if data is compromised

**Recommendation:**
- Use ASP.NET's built-in encoding (Server.HtmlEncode)
- Implement Content Security Policy (CSP)
- Validate and sanitize all output

---

## Remediation Priority

### Immediate Actions (Within 24-48 Hours)

1. **Remove hardcoded credentials from Web.config**
   - Rotate all exposed passwords
   - Implement secure credential storage
   - Audit access logs

2. **Fix SQL Injection in Common.vb**
   - Replace all string concatenation with parameterized queries
   - Priority: Methods handling user input (LoadInvoices, LoadEpayAnalystReport)

3. **Disable custom errors in production**
   - Set `<customErrors mode="RemoteOnly">`
   - Implement proper error pages

### Short-term Actions (Within 1-2 Weeks)

4. **Refactor stored procedures**
   - Remove dynamic SQL construction
   - Use proper parameterization
   - Add input validation

5. **Implement comprehensive input validation**
   - Add regex validation for all user inputs
   - Implement length and format checks
   - Add business logic validation

6. **Remove trace statements**
   - Audit all Trace.Warn calls
   - Implement structured logging
   - Add data masking for sensitive information

### Medium-term Actions (Within 1-3 Months)

7. **Implement authorization checks**
   - Add reference number ownership validation
   - Implement role-based access control
   - Add audit logging for sensitive operations

8. **Security testing**
   - Conduct penetration testing
   - Implement automated security scanning
   - Add security tests to CI/CD pipeline

### Long-term Actions (3-6 Months)

9. **Framework modernization**
   - Plan migration to .NET 6/8
   - Evaluate ASP.NET Core migration
   - Update all third-party dependencies

10. **Security architecture review**
    - Implement defense in depth
    - Add Web Application Firewall (WAF)
    - Implement security monitoring

---

## Code Remediation Examples

### Example 1: Fix SQL Injection in Common.vb

**Before (Vulnerable):**
```vb
Public Shared Function GetTotals(ByVal custNum As String, ByVal referenceNumber As Integer) As SqlDataReader
    Dim sbSQL As String
    Try
        sbSQL = "EXEC Datawhse.dbo.usp_GetEpayTotal '" & custNum & "'," & referenceNumber
        Return DataAccess.GetDataReader("AFI_Batch", sbSQL.ToString)
    Catch ex As Exception
        Throw
    End Try
End Function
```

**After (Secure):**
```vb
Public Shared Function GetTotals(ByVal custNum As String, ByVal referenceNumber As Integer) As SqlDataReader
    Dim parameters As New Collection
    Try
        ' Validate inputs
        If Not Regex.IsMatch(custNum, "^[0-9]{7}$") Then
            Throw New ArgumentException("Invalid customer number format")
        End If

        If referenceNumber <= 0 Then
            Throw New ArgumentException("Invalid reference number")
        End If

        ' Use parameterized query
        parameters.Add(New SqlClient.SqlParameter("@CustomerNumber", custNum))
        parameters.Add(New SqlClient.SqlParameter("@ReferenceNumber", referenceNumber))

        Return DataAccess.ExecuteStoredProcedure("AFI_Batch",
                                                 "Datawhse.dbo.usp_GetEpayTotal",
                                                 DataAccess.StoredProcedureReturnType.DataReader,
                                                 parameters)
    Catch ex As Exception
        ' Log error without exposing sensitive data
        LogError("GetTotals failed", ex)
        Throw
    Finally
        If parameters IsNot Nothing Then parameters = Nothing
    End Try
End Function
```

### Example 2: Fix Stored Procedure SQL Injection

**Before (Vulnerable):**
```sql
SELECT @sqlString = @sqlString + 'AND T1.opicusno=''' + @customerNumber + '''
    AND T1.opidagedt <= ''' + cast(@FromDate as varchar) + '''
    AND T1.opidagedt >= ''' + cast(@ToDate as varchar) + ''' '
EXEC(@sqlString)
```

**After (Secure):**
```sql
-- Use parameterized WHERE clause instead of dynamic SQL
WHERE T1.opicusno = @customerNumber
  AND T1.opidagedt <= @FromDate
  AND T1.opidagedt >= @ToDate
  AND T3.secMhs_name = @securityMHS
```

### Example 3: Implement Authorization Check

**New Code:**
```vb
Private Function ValidateUserAccessToReference(ByVal refNo As Integer, ByVal customerNumber As String) As Boolean
    Dim parameters As New Collection
    Dim result As Object

    Try
        parameters.Add(New SqlClient.SqlParameter("@ReferenceNumber", refNo))
        parameters.Add(New SqlClient.SqlParameter("@CustomerNumber", customerNumber))

        result = DataAccess.ExecuteStoredProcedure("AFI_Batch",
                                                   "Datawhse.dbo.usp_ValidateReferenceAccess",
                                                   DataAccess.StoredProcedureReturnType.Scalar,
                                                   parameters)

        Return CBool(result)
    Catch ex As Exception
        LogError("Authorization check failed", ex)
        Return False
    End Try
End Function
```

---

## Compliance Impact

### PCI-DSS Violations

- **Requirement 6.5.1:** SQL Injection vulnerabilities present
- **Requirement 8.2.1:** Hardcoded credentials violate password policies
- **Requirement 10.2:** Insufficient audit logging
- **Requirement 6.2:** Outdated software components

### SOX Compliance Issues

- Inadequate access controls
- Insufficient audit trails
- Data integrity concerns

### Recommendations

1. Conduct formal PCI-DSS assessment
2. Implement compensating controls immediately
3. Document remediation plan
4. Schedule follow-up audit

---

## Testing Recommendations

### Security Testing Checklist

- [ ] SQL Injection testing (automated and manual)
- [ ] Authentication bypass testing
- [ ] Authorization testing
- [ ] Input validation testing
- [ ] Session management testing
- [ ] Error handling testing
- [ ] Logging and monitoring review
- [ ] Dependency vulnerability scanning

### Tools Recommended

- **SAST:** SonarQube, Checkmarx
- **DAST:** OWASP ZAP, Burp Suite
- **Dependency Scanning:** OWASP Dependency-Check, Snyk
- **Penetration Testing:** Manual testing by certified professionals

---

## Conclusion

The Finance Credit Direct EPay application has **CRITICAL** security vulnerabilities that require immediate remediation. The SQL injection vulnerabilities and hardcoded credentials pose significant risks to the organization's financial data and customer information.

**Immediate actions required:**
1. Remove hardcoded credentials (TODAY)
2. Fix SQL injection vulnerabilities (THIS WEEK)
3. Implement proper error handling (THIS WEEK)
4. Schedule security assessment (NEXT WEEK)

**Risk if not addressed:**
- Data breach
- Financial loss
- Regulatory penalties
- Reputational damage
- Legal liability

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**
This software is proprietary and confidential. Unauthorized copying, distribution, or use of this software, via any medium, is strictly prohibited.


