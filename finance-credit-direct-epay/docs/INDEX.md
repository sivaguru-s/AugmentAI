# Documentation Index - Finance Credit Direct EPay

**Version:** 1.0  
**Last Updated:** January 9, 2026  
**Status:** Complete  

---

## Overview

This directory contains comprehensive documentation for the Finance Credit Direct EPay system, an ASP.NET Web Forms application that enables Ashley Furniture customers to make electronic payments for open invoices via US Bank's ACH gateway.

---

## Documentation Structure

### 📋 [README.md](./README.md)
**Purpose:** Project overview, features, installation, and usage  
**Audience:** Developers, IT Staff, Project Managers  
**Contents:**
- Project overview and business context
- Features and capabilities
- Technology stack and architecture overview
- Prerequisites and dependencies
- Installation instructions
- Configuration guide
- Usage examples
- Security warnings
- Support information

**When to use:** Start here for a general understanding of the project.

---

### 🔒 [SECURITY_AUDIT.md](./SECURITY_AUDIT.md)
**Purpose:** Security vulnerabilities and remediation recommendations  
**Audience:** Security Team, Developers, IT Management  
**Status:** ⚠️ **CRITICAL ISSUES IDENTIFIED**

**Contents:**
- Executive summary of security findings
- Critical vulnerabilities (SQL injection, hardcoded credentials)
- High/Medium/Low severity issues
- Remediation priority and timeline
- Code remediation examples
- Compliance impact (PCI-DSS, SOX)
- Testing recommendations

**Key Findings:**
- 🔴 **CRITICAL:** SQL Injection vulnerabilities (multiple instances)
- 🔴 **CRITICAL:** Hardcoded credentials in Web.config
- 🟡 **HIGH:** Weak authentication in debug mode
- 🟡 **MEDIUM:** Insufficient input validation
- 🟡 **MEDIUM:** Sensitive data exposure in logs
- 🟡 **MEDIUM:** Outdated framework (.NET 3.5)

**When to use:** **BEFORE** deploying to production. Address all critical issues immediately.

---

### 🏗️ [ARCHITECTURE.md](./ARCHITECTURE.md)
**Purpose:** System architecture, design patterns, and technical details  
**Audience:** Developers, Architects, Technical Leads  

**Contents:**
- System overview and context diagram
- Three-tier architecture pattern
- Component architecture (Presentation, Business Logic, Data Access)
- Data architecture (database schema, stored procedures)
- Integration architecture (US Bank, DB2, Common Assemblies)
- Security architecture
- Deployment architecture
- Performance and scalability considerations
- Monitoring and observability
- Disaster recovery
- Future architecture recommendations

**When to use:** Understanding system design, planning changes, or troubleshooting complex issues.

---

### 📡 [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)
**Purpose:** Internal APIs, stored procedures, and data contracts  
**Audience:** Developers, Database Administrators  

**Contents:**
- Business Logic API (Common class methods)
- SQL Server stored procedures
- Data contracts and table schemas
- US Bank integration API
- Error handling
- Best practices
- Common query patterns

**Key APIs:**
- `GetTotals()` - Calculate payment total
- `UpdateEpayStatus()` - Update payment status
- `InsertEpayRecords()` - Create payment batch
- `LoadInvoices()` - Retrieve open invoices
- `LoadEpayAnalystReport()` - Get analyst data

**When to use:** Developing new features, integrating with EPay, or troubleshooting data issues.

---

### 🚀 [DEPLOYMENT.md](./DEPLOYMENT.md)
**Purpose:** Deployment procedures, environment configuration, and operations  
**Audience:** DevOps, IT Operations, Release Managers  

**Contents:**
- Prerequisites and server requirements
- Environment configuration (Dev, Staging, Production)
- Pre-deployment steps (security review, build, package)
- Deployment procedures (backup, deploy, verify)
- Post-deployment verification
- Rollback procedures
- Troubleshooting common deployment issues
- Monitoring and maintenance
- Deployment checklist

**When to use:** Deploying to any environment, setting up new servers, or recovering from failures.

---

### 👥 [USER_GUIDE.md](./USER_GUIDE.md)
**Purpose:** End-user documentation for customers and analysts  
**Audience:** Customers, Credit Analysts, Support Staff  

**Contents:**
- Introduction and benefits
- Getting started (login, navigation)
- Customer features (viewing invoices, making payments, payment history)
- Analyst features (reports, user lists, admin functions)
- Frequently Asked Questions (FAQ)
- Troubleshooting common issues
- Best practices
- Tips and tricks
- Support contact information
- Glossary

**When to use:** Training new users, answering customer questions, or troubleshooting user issues.

---

## Quick Reference

### For Developers

**Getting Started:**
1. Read [README.md](./README.md) - Overview and setup
2. Review [SECURITY_AUDIT.md](./SECURITY_AUDIT.md) - **CRITICAL** security issues
3. Study [ARCHITECTURE.md](./ARCHITECTURE.md) - System design
4. Reference [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) - APIs and data

**Making Changes:**
1. Review security audit before making changes
2. Use parameterized queries (never string concatenation)
3. Implement input validation
4. Add unit tests
5. Update documentation

### For IT Operations

**Deploying:**
1. Read [DEPLOYMENT.md](./DEPLOYMENT.md) - Deployment procedures
2. Review [SECURITY_AUDIT.md](./SECURITY_AUDIT.md) - Security requirements
3. Follow deployment checklist
4. Verify post-deployment
5. Monitor for errors

**Troubleshooting:**
1. Check [DEPLOYMENT.md](./DEPLOYMENT.md) - Common issues
2. Review [ARCHITECTURE.md](./ARCHITECTURE.md) - System components
3. Check logs and monitoring
4. Escalate to development team if needed

### For End Users

**Using EPay:**
1. Read [USER_GUIDE.md](./USER_GUIDE.md) - Complete user guide
2. Check FAQ section for common questions
3. Review troubleshooting section for issues
4. Contact support if needed

### For Security Team

**Security Review:**
1. **START HERE:** [SECURITY_AUDIT.md](./SECURITY_AUDIT.md) - Complete security audit
2. Review [ARCHITECTURE.md](./ARCHITECTURE.md) - Security architecture
3. Check [DEPLOYMENT.md](./DEPLOYMENT.md) - Deployment security
4. Verify remediation of critical issues

---

## Document Relationships

```
README.md (Start Here)
    ├── SECURITY_AUDIT.md (⚠️ CRITICAL - Read Before Deployment)
    ├── ARCHITECTURE.md (Technical Deep Dive)
    │   ├── Component Architecture
    │   ├── Data Architecture
    │   └── Integration Architecture
    ├── API_DOCUMENTATION.md (Developer Reference)
    │   ├── Business Logic API
    │   ├── Stored Procedures
    │   └── Data Contracts
    ├── DEPLOYMENT.md (Operations Guide)
    │   ├── Environment Setup
    │   ├── Deployment Procedures
    │   └── Troubleshooting
    └── USER_GUIDE.md (End User Documentation)
        ├── Customer Features
        ├── Analyst Features
        └── FAQ & Troubleshooting
```

---

## Critical Warnings

### ⚠️ SECURITY ISSUES

**DO NOT deploy to production without addressing:**

1. **SQL Injection Vulnerabilities**
   - Location: `Classes/Common.vb` (multiple methods)
   - Location: `SQL/usp_OrderAndInvoiceReportingOpenInvoices2_CREATE.sql`
   - Impact: Complete database compromise
   - Action: Replace all string concatenation with parameterized queries

2. **Hardcoded Credentials**
   - Location: `Web.config` (lines 22-24, commented but in source control)
   - Impact: Unauthorized access, compliance violations
   - Action: Remove immediately, rotate passwords, use secure storage

3. **Debug Mode / Custom Errors**
   - Location: `Web.config`
   - Impact: Information disclosure
   - Action: Set `debug="false"` and `customErrors mode="On"`

**See [SECURITY_AUDIT.md](./SECURITY_AUDIT.md) for complete details and remediation.**

---

## Version History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-01-09 | Augment AI | Initial documentation creation |

---

## Contributing

### Updating Documentation

**When to update:**
- Code changes that affect functionality
- New features added
- Security issues discovered or fixed
- Deployment procedures change
- User workflows change

**How to update:**
1. Edit the relevant markdown file
2. Update version history
3. Update "Last Updated" date
4. Commit changes with descriptive message
5. Notify team of documentation updates

### Documentation Standards

**Formatting:**
- Use Markdown format
- Include table of contents for long documents
- Use code blocks with syntax highlighting
- Include examples and screenshots where helpful
- Use emojis sparingly for visual cues

**Content:**
- Write for the target audience
- Be clear and concise
- Include examples
- Keep up-to-date
- Cross-reference related documents

---

## Support

### Documentation Issues

**Found an error or have a suggestion?**
- Email: documentation@ashleyfurniture.com
- Subject: EPay Documentation Feedback

**Include:**
- Document name
- Section/page
- Description of issue or suggestion
- Your contact information

---

## License

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**

This software and documentation are proprietary and confidential. Unauthorized copying, distribution, or use of this software or documentation, via any medium, is strictly prohibited.

---

**For questions or support, contact:**
- **Development Team:** IT Development
- **Security Team:** Information Security
- **Operations Team:** IT Operations
