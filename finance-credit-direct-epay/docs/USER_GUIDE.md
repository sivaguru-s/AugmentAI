# User Guide - Finance Credit Direct EPay

**Version:** 1.0  
**Last Updated:** January 9, 2026  
**Audience:** Customers and Credit Analysts  

---

## Table of Contents

- [Introduction](#introduction)
- [Getting Started](#getting-started)
- [Customer Features](#customer-features)
- [Analyst Features](#analyst-features)
- [Frequently Asked Questions](#frequently-asked-questions)
- [Troubleshooting](#troubleshooting)
- [Support](#support)

---

## Introduction

### What is EPay?

The Finance Credit Direct EPay system is a web-based application that allows Ashley Furniture customers to:
- View open invoices online
- Select multiple invoices for payment
- Submit electronic payments via ACH (Automated Clearing House)
- Track payment status and history
- Download payment confirmations

### Benefits

**For Customers:**
- ✅ Pay multiple invoices in one transaction
- ✅ Faster payment processing
- ✅ Reduced paperwork
- ✅ 24/7 access to invoice information
- ✅ Immediate payment confirmation
- ✅ Secure ACH payments

**For Ashley Furniture:**
- ✅ Improved cash flow
- ✅ Reduced manual processing
- ✅ Better payment tracking
- ✅ Enhanced customer service

### System Requirements

**Browser:**
- Internet Explorer 8+ (recommended: IE 11)
- Firefox 3.6+
- Chrome 10+
- Safari 5+

**Plugins:**
- JavaScript enabled
- Cookies enabled
- PDF reader (for confirmation reports)

**Network:**
- Internet connection
- Access to Ashley Furniture network (for internal users)

---

## Getting Started

### Logging In

1. **Navigate to EPay:**
   - Internal users: `http://ashleydirect.com/EPay`
   - External users: Contact your Ashley representative for access

2. **Authenticate:**
   - Use your Ashley Furniture credentials
   - If you don't have credentials, contact IT Support

3. **Select Customer Account:**
   - Use the account selection dropdown
   - Select the customer number you want to manage
   - Click "Select" or press Enter

### Navigation

**Main Menu:**
- **Open Invoices** - View and pay invoices
- **Payment History** - View past payments
- **Analyst Report** - Credit analyst reporting (analysts only)
- **User List** - View EPay users (analysts only)
- **Admin Maintenance** - Administrative functions (analysts only)

**User Controls:**
- **Account Selection** - Change customer account
- **Logout** - Sign out of the application
- **Help** - Access help documentation

---

## Customer Features

### Viewing Open Invoices

#### Step 1: Access Open Invoices

1. Click **"Open Invoices"** in the navigation menu
2. The main invoice screen will display

#### Step 2: Filter Invoices

**Date Range:**
- **From Date:** Start of date range (default: 90 days ago)
- **To Date:** End of date range (default: today)
- Click calendar icon to select dates

**Search Filters:**
- **PO Number:** Enter customer PO number (partial match)
- **Invoice Number:** Enter invoice number (partial match)
- **Credit Number:** Enter credit memo number (partial match)

**Options:**
- ☑ **Show Credits** - Include credit memos in results
- ☑ **Show Old Credits** - Include credits older than 1 year

#### Step 3: Search

1. Enter filter criteria
2. Click **"Search"** button
3. Results will display in the grid below

#### Step 4: Review Results

**Grid Columns:**
- **Select** - Checkbox to select invoice for payment
- **Customer #** - Customer number
- **Ship To** - Ship-to location number
- **Invoice #** - Invoice number
- **Credit #** - Credit memo number (if applicable)
- **Invoice Date** - Date invoice was created
- **Order #** - Order number
- **PO #** - Customer purchase order number
- **Invoice Amt** - Original invoice amount
- **Amt Paid** - Amount already paid
- **Balance** - Remaining balance due
- **Aging Code** - Aging category (00=Current, 01=31-60, etc.)
- **Days** - Days since invoice date
- **Order Date** - Date order was placed
- **RPP #** - RPP number (if applicable)
- **Trip #** - Trip number (if applicable)

**Grid Features:**
- **Sorting:** Click column headers to sort
- **Paging:** Use page numbers at bottom to navigate
- **Select All:** Check box in header to select all invoices on current page

### Making a Payment

#### Step 1: Select Invoices

1. Review open invoices
2. Check the box next to each invoice you want to pay
3. Or click **"Select All"** to select all invoices on the page

**Important Notes:**
- ⚠️ You can only select invoices that are not already in a pending payment
- ⚠️ Credits (negative amounts) will reduce your total payment
- ⚠️ Make sure to review the total before submitting

#### Step 2: Initiate Payment

1. Click **"Make Payment"** button at the bottom of the page
2. You will be redirected to the confirmation page

#### Step 3: Review Payment Confirmation

**Confirmation Page Shows:**
- **Reference Number:** Unique identifier for this payment batch
- **Total Amount:** Sum of all selected invoices
- **Invoice List:** Detailed list of invoices included

**Review the following:**
- ✓ Reference number (save for your records)
- ✓ Total amount is correct
- ✓ All intended invoices are included
- ✓ No unintended invoices are included

#### Step 4: Confirm or Cancel

**Option 1: Proceed with Payment**
1. Click **"OK"** button
2. You will be redirected to US Bank's payment gateway
3. Status will be updated to "Sent"

**Option 2: Cancel Payment**
1. Click **"Cancel"** button
2. Confirm cancellation in the popup
3. Payment batch will be deleted
4. You will return to the invoice selection page

#### Step 5: Complete Payment at US Bank

**US Bank Payment Gateway:**
1. Review payment details
2. Enter bank account information:
   - Bank routing number
   - Account number
   - Account type (Checking/Savings)
3. Review and accept terms
4. Click **"Submit Payment"**
5. Save confirmation number provided by US Bank

**Important:**
- ⚠️ Payment will be processed via ACH (typically 2-3 business days)
- ⚠️ Ensure sufficient funds in your account
- ⚠️ Save the US Bank confirmation number
- ⚠️ You will receive email confirmation (if email provided)

#### Step 6: Return to EPay

1. After completing payment at US Bank, you will be redirected back to EPay
2. Your payment status will be updated
3. You can view payment details in Payment History

### Viewing Payment History

#### Step 1: Access Payment History

1. Click **"Payment History"** in the navigation menu
2. The history page will display

#### Step 2: Filter History

**Filter Options:**
- **From Date:** Start of date range
- **To Date:** End of date range
- **Customer Number:** Filter by customer (leave blank for all)
- **Reference Number:** Filter by specific payment
- **Confirmation Number:** Filter by US Bank confirmation

#### Step 3: Search

1. Enter filter criteria
2. Click **"Search"** button
3. Results will display in the grid

#### Step 4: Review Payment Status

**Payment Statuses:**
- **Sent** - Payment submitted to US Bank, awaiting processing
- **Verifying** - Payment being verified by US Bank
- **Confirmed** - Payment completed successfully

**Grid Columns:**
- **Reference #** - EPay reference number
- **Customer #** - Customer number
- **Invoice #** - Invoice number
- **Amount** - Payment amount
- **Status** - Current payment status
- **Confirmation #** - US Bank confirmation number
- **Date Added** - Date payment was created
- **User** - User who created the payment

### Exporting to Excel

#### Export Invoice List

1. View open invoices (search first)
2. Click **Excel icon** at top of grid
3. File will download as `EpaymentCustomerInvoices.xls`
4. Open in Microsoft Excel

**Export Includes:**
- All invoices (not just current page)
- All columns from the grid
- Currency formatting preserved

---

## Analyst Features

### Analyst Report

**Purpose:** View and analyze EPay payment activity across customers

#### Step 1: Access Analyst Report

1. Click **"Analyst Report"** in the navigation menu
2. Requires EPAYANLYST authorization

#### Step 2: Filter Report

**Filter Options:**
- **Customer Number:** Filter by specific customer
- **Reference Number:** Filter by payment reference
- **Confirmation Number:** Filter by US Bank confirmation
- **Payment Date:** Filter by date payment was created
- **Credit Territory:** Filter by credit territory

#### Step 3: Generate Report

1. Enter filter criteria
2. Click **"Search"** button
3. Results will display in the grid

#### Step 4: Analyze Results

**Grid Columns:**
- Reference number
- Customer information
- Invoice details
- Payment amount
- Status
- Confirmation number
- Date and user information
- Credit territory

**Features:**
- Sortable columns
- Pagination
- Export to Excel

### User List

**Purpose:** View list of customers using EPay

#### Step 1: Access User List

1. Click **"User List"** in the navigation menu

#### Step 2: Filter Users

**Filter Options:**
- **Customer Number:** Filter by customer number (partial match)
- **Customer Name:** Filter by customer name (partial match)
- **Bill-to State:** Filter by state
- **Credit Territory:** Filter by territory
- **Terms Code:** Filter by payment terms

#### Step 3: Generate List

1. Enter filter criteria
2. Click **"Search"** button
3. Results will display in the grid

#### Step 4: Review Users

**Grid Columns:**
- Customer number
- Customer name
- Bill-to state
- Credit territory
- Terms code
- Contact email

**Features:**
- Pagination
- Export to Excel

### Admin Maintenance

**Purpose:** Administrative functions for managing payments

⚠️ **WARNING:** These functions permanently delete data. Use with caution.

#### Cancel Unconfirmed Payments

1. Click **"Admin Maintenance"** in navigation
2. Select **"Cancel Unconfirmed Payments"** tab
3. Enter customer number and invoice number
4. Click **"Delete"** button
5. Confirm deletion

**Use Case:** Remove payments that were created but never submitted to US Bank

#### Delete EPay Records

1. Select **"Delete EPay Records"** tab
2. Enter customer number and reference number
3. Click **"Delete"** button
4. Confirm deletion

**Use Case:** Remove entire payment batch (all invoices)

---

## Frequently Asked Questions

### General Questions

**Q: Who can use EPay?**
A: EPay is available to authorized Ashley Furniture customers. Contact your Ashley representative to request access.

**Q: Is there a fee to use EPay?**
A: No, EPay is provided free of charge to Ashley Furniture customers.

**Q: What payment methods are supported?**
A: EPay supports ACH (Automated Clearing House) payments only. Credit card payments are not supported.

**Q: How long does it take for my payment to process?**
A: ACH payments typically take 2-3 business days to process and post to your account.

**Q: Can I pay invoices for multiple ship-to locations?**
A: Yes, you can select invoices from multiple ship-to locations in a single payment batch.

**Q: Is my payment information secure?**
A: Yes, all payment information is transmitted securely via HTTPS encryption. Bank account details are entered directly on US Bank's secure gateway and are not stored in EPay.

### Invoice Questions

**Q: Why don't I see all my invoices?**
A: Check the following:
- Date range filter (default is last 90 days)
- "Show Credits" option is checked (if looking for credit memos)
- Invoice may already be in a pending payment
- Invoice may be paid or closed

**Q: What does "No Invoices to send!" mean?**
A: This message appears when:
- No invoices are selected
- All selected invoices are already in a pending payment
- Selected invoices are not eligible for payment

**Q: Can I pay a partial amount on an invoice?**
A: No, EPay requires full payment of the invoice balance. For partial payments, contact your Ashley representative.

**Q: What is a credit memo?**
A: A credit memo is a negative invoice that reduces your balance. Credits are automatically applied when you select them for payment.

**Q: Why is my invoice balance different from the original amount?**
A: The balance reflects:
- Original invoice amount
- Minus any payments already made
- Minus any credits applied
- Plus any additional charges

### Payment Questions

**Q: Can I cancel a payment after submitting?**
A:
- **Before clicking OK on confirmation page:** Yes, click "Cancel" button
- **After clicking OK (sent to US Bank):** Contact US Bank immediately
- **After completing at US Bank:** Contact your bank to stop payment (may incur fees)

**Q: What is a reference number?**
A: The reference number is a unique identifier for your payment batch. Save this number for your records and use it to track payment status.

**Q: I didn't receive a confirmation number from US Bank. What should I do?**
A: Contact US Bank customer service immediately. Have your reference number ready.

**Q: Can I make multiple payments in one day?**
A: Yes, you can create multiple payment batches. Each will have a unique reference number.

**Q: How do I know if my payment was successful?**
A:
1. Check Payment History for status
2. Look for "Confirmed" status
3. Verify US Bank confirmation number is present
4. Check your bank account for the debit

**Q: What if I entered the wrong bank account information?**
A: Contact US Bank customer service immediately to cancel or modify the payment.

### Technical Questions

**Q: Why am I getting "Can not call USBank from Test!!" message?**
A: This message appears in development/staging environments. US Bank integration is only enabled in production. This is normal and expected in test environments.

**Q: The page is loading slowly. What can I do?**
A:
- Reduce the date range filter
- Clear your browser cache
- Close other browser tabs
- Contact IT Support if problem persists

**Q: I'm getting a timeout error. What should I do?**
A:
- Reduce the date range
- Try again in a few minutes
- Contact IT Support if problem persists

**Q: Can I use EPay on my mobile device?**
A: EPay is optimized for desktop browsers. Mobile access may work but is not officially supported.

**Q: Which browser should I use?**
A: Internet Explorer 11 is recommended. Firefox and Chrome are also supported.

---

## Troubleshooting

### Common Issues and Solutions

#### Issue: Cannot Login

**Symptoms:**
- Login page doesn't load
- Credentials rejected
- Access denied message

**Solutions:**
1. Verify you have EPay access (contact your Ashley representative)
2. Check your username and password
3. Clear browser cache and cookies
4. Try a different browser
5. Contact IT Support

#### Issue: No Invoices Displayed

**Symptoms:**
- Grid is empty after searching
- "No records found" message

**Solutions:**
1. **Check date range:**
   - Expand date range (try last 180 days)
   - Verify dates are in correct order (From < To)

2. **Check filters:**
   - Clear all search filters
   - Try searching without filters
   - Check "Show Credits" option

3. **Verify customer selection:**
   - Ensure correct customer is selected
   - Try selecting "All Ship-Tos"

4. **Check invoice status:**
   - Invoices may already be paid
   - Invoices may be in pending payment

#### Issue: Cannot Select Invoice

**Symptoms:**
- Checkbox is disabled
- Invoice appears grayed out
- "Already in EPay" message

**Solutions:**
1. **Invoice already in payment:**
   - Check Payment History for pending payment
   - Cancel pending payment if needed
   - Try selecting invoice again

2. **Invoice not eligible:**
   - Invoice may be paid
   - Invoice may be on hold
   - Contact your Ashley representative

#### Issue: Payment Fails to Submit

**Symptoms:**
- Error message after clicking "Make Payment"
- Redirect fails
- Timeout error

**Solutions:**
1. **Check selections:**
   - Ensure at least one invoice is selected
   - Verify invoices are not already in payment

2. **Try again:**
   - Click "Make Payment" again
   - Refresh the page and reselect invoices

3. **Clear browser cache:**
   - Clear cache and cookies
   - Close and reopen browser

4. **Contact Support:**
   - If problem persists, contact IT Support
   - Have reference number ready (if generated)

#### Issue: US Bank Page Doesn't Load

**Symptoms:**
- Blank page after clicking OK
- Redirect fails
- Timeout error

**Solutions:**
1. **Check popup blocker:**
   - Disable popup blocker for this site
   - Allow redirects

2. **Check internet connection:**
   - Verify you have internet access
   - Try accessing other websites

3. **Check firewall:**
   - Ensure US Bank gateway is not blocked
   - Contact IT if behind corporate firewall

4. **Try different browser:**
   - Use Internet Explorer or Chrome
   - Disable browser extensions

#### Issue: Payment Status Not Updating

**Symptoms:**
- Status stuck on "Sent"
- Confirmation number not showing
- Payment completed but status unchanged

**Solutions:**
1. **Wait for processing:**
   - ACH payments take 2-3 business days
   - Status may not update immediately

2. **Check with US Bank:**
   - Verify payment was completed
   - Get confirmation number

3. **Contact Support:**
   - Provide reference number
   - Provide US Bank confirmation number
   - Request manual status update

#### Issue: Excel Export Not Working

**Symptoms:**
- Excel icon doesn't respond
- Download fails
- File is corrupted

**Solutions:**
1. **Check popup blocker:**
   - Allow downloads from this site

2. **Check browser settings:**
   - Ensure downloads are enabled
   - Check download folder permissions

3. **Try different browser:**
   - Use Internet Explorer
   - Disable browser extensions

4. **Reduce dataset:**
   - Apply filters to reduce number of records
   - Try exporting smaller date range

---

## Best Practices

### For Customers

**Payment Planning:**
- ✅ Review invoices regularly (weekly)
- ✅ Plan payments to align with cash flow
- ✅ Group invoices by payment date
- ✅ Keep records of reference numbers
- ✅ Save US Bank confirmation numbers

**Invoice Management:**
- ✅ Use filters to find specific invoices
- ✅ Sort by due date to prioritize payments
- ✅ Review aging codes to identify overdue invoices
- ✅ Export to Excel for offline analysis
- ✅ Reconcile with your accounting system

**Payment Processing:**
- ✅ Double-check total amount before submitting
- ✅ Verify bank account information at US Bank
- ✅ Save confirmation numbers immediately
- ✅ Monitor payment status in Payment History
- ✅ Contact support if issues arise

**Security:**
- ✅ Log out when finished
- ✅ Don't share login credentials
- ✅ Use secure internet connection
- ✅ Keep browser updated
- ✅ Report suspicious activity

### For Analysts

**Reporting:**
- ✅ Run reports regularly to monitor activity
- ✅ Use filters to identify trends
- ✅ Export data for analysis
- ✅ Share insights with management
- ✅ Document unusual patterns

**User Support:**
- ✅ Respond promptly to customer inquiries
- ✅ Verify customer access before troubleshooting
- ✅ Document common issues and solutions
- ✅ Escalate technical issues to IT Support
- ✅ Follow up on resolved issues

**Administrative Functions:**
- ✅ Use admin functions carefully
- ✅ Verify data before deleting
- ✅ Document all administrative actions
- ✅ Communicate with customers about changes
- ✅ Keep audit trail

---

## Tips and Tricks

### Keyboard Shortcuts

- **Tab** - Move to next field
- **Shift+Tab** - Move to previous field
- **Enter** - Submit form (on buttons)
- **Spacebar** - Toggle checkbox
- **Ctrl+F** - Find on page (browser function)

### Efficient Searching

**Quick Searches:**
- Leave all filters blank to see all invoices
- Use partial matches for PO/Invoice numbers
- Sort by "Days" to find oldest invoices first
- Use "Show Credits" to see available credits

**Advanced Filtering:**
- Combine multiple filters for precise results
- Use date range to focus on specific period
- Export to Excel for complex analysis
- Save common searches as browser bookmarks

### Payment Strategies

**Batch Payments:**
- Group invoices by due date
- Combine small invoices to reduce transactions
- Include credits to reduce total payment
- Schedule payments to align with cash flow

**Payment Tracking:**
- Create spreadsheet with reference numbers
- Track status changes
- Reconcile with bank statements
- Archive confirmation numbers

---

## Support

### Getting Help

**Self-Service:**
1. Check this User Guide
2. Review FAQ section
3. Check Troubleshooting section
4. Search knowledge base (if available)

**Contact Support:**

**For Technical Issues:**
- **Email:** itsupport@ashleyfurniture.com
- **Phone:** (608) 123-4567
- **Hours:** Monday-Friday, 8:00 AM - 5:00 PM CST

**For Payment Issues:**
- **Email:** credit@ashleyfurniture.com
- **Phone:** (608) 123-4568
- **Hours:** Monday-Friday, 8:00 AM - 5:00 PM CST

**For US Bank Issues:**
- **Phone:** US Bank Customer Service
- **Website:** https://www.usbank.com
- **Have Ready:** Reference number, confirmation number

**When Contacting Support, Provide:**
- Your name and customer number
- Description of the issue
- Steps you've already tried
- Error messages (exact text)
- Reference number (if applicable)
- Screenshots (if helpful)

### Feedback

We value your feedback! Help us improve EPay:

**Submit Feedback:**
- **Email:** epay-feedback@ashleyfurniture.com
- **Subject:** EPay Feedback

**Include:**
- What you like
- What could be improved
- Feature requests
- Usability suggestions

---

## Glossary

**ACH (Automated Clearing House):** Electronic network for financial transactions in the United States. Used for direct deposit and electronic payments.

**Aging Code:** Category indicating how long an invoice has been outstanding (00=Current, 01=31-60 days, etc.).

**Balance:** Remaining amount due on an invoice after payments and credits.

**Confirmation Number:** Unique identifier provided by US Bank after payment is completed.

**Credit Memo:** Negative invoice that reduces customer balance. Often issued for returns or adjustments.

**EPay:** Electronic payment system for Ashley Furniture invoices.

**Invoice:** Bill for goods or services provided by Ashley Furniture.

**Payment Batch:** Group of invoices submitted together in a single payment.

**PO Number:** Purchase Order number assigned by customer.

**Reference Number:** Unique identifier for an EPay payment batch.

**Ship-To:** Delivery location for customer orders.

**Status:** Current state of payment (Sent, Verifying, Confirmed).

---

## Appendix

### Sample Workflows

#### Workflow 1: Pay All Overdue Invoices

1. Login to EPay
2. Select customer account
3. Click "Open Invoices"
4. Set date range to last 180 days
5. Click "Search"
6. Sort by "Days" column (descending)
7. Select all invoices over 60 days old
8. Click "Make Payment"
9. Review confirmation
10. Click "OK"
11. Complete payment at US Bank
12. Save confirmation number

#### Workflow 2: Pay Specific Invoice by PO Number

1. Login to EPay
2. Select customer account
3. Click "Open Invoices"
4. Enter PO Number in filter
5. Click "Search"
6. Verify invoice details
7. Select invoice
8. Click "Make Payment"
9. Review confirmation
10. Click "OK"
11. Complete payment at US Bank
12. Save confirmation number

#### Workflow 3: Track Payment Status

1. Login to EPay
2. Click "Payment History"
3. Enter reference number or date range
4. Click "Search"
5. Review payment status
6. Note confirmation number
7. Export to Excel if needed

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**

**Document Version:** 1.0
**Last Updated:** January 9, 2026
**For Internal Use Only**


