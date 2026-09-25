# Onbase Chatbot Requirements related clarifications:

## Questions

- Can the business confirm that AS400 will be the authoritative source for invoice, payment, spend, tax, and reconciliation values, while OnBase will be the authoritative source for invoice PDFs and attached documents?
- Which business function owns the initial MVP?
- Accounts Payable
- Accounting
- Reconciliation
- Treasury
- Are invoice insights and reconciliation insights part of the same MVP, or should reconciliation be handled in a later phase?
- 10 example prompts are mandatory for the first release with expected answers?
- Which use cases are:
- Must-have
- Should-have
- Future phase
- Out of scope
- Is the initial scope limited to vendor invoices, or must the chatbot also support:
- Payments
- Purchase orders
- Credit memos
- Debit memos
- AP holds
- Reconciliation records
- Supporting invoice documents
- What business decision should each answer support? For example:
- Researching a vendor
- Reviewing monthly spend
- Investigating tax exceptions
- Closing a reconciliation
- Preparing management reports
> **Recommended MVP boundary: Read-only conversational insights with invoice listing, spend summary, comparison, trend, reconciliation summary. Keep transactional actions outside the first release.**

## Authoritative data sources

- For each data element, which system is authoritative?
- Who is the finance-data SME who can validate those structures?
- What should happen if AS400 and OnBase contain conflicting metadata?
### Prompt 1: Vendor spend summary

- What does “total spend” mean?
- Invoiced amount
- Posted amount
- Paid amount
- Net amount after credits
- Amount in a reporting currency
- Should voided, reversed, duplicate, rejected, or unpaid invoices be included? Should the answer include invoice count as well as spend?
### Prompt 2: Invoice count and list

- What qualifies as a unique invoice?
- Which columns are mandatory in the result or the prompt response?
- Invoice number
- Vendor
- Invoice date
- Posting date
- Amount
- Currency
- Status
- Transaction number
- What is the maximum number of invoices that may be displayed?
- Should large result sets be paginated, summarized, or exported?
- What sorting should be used by default?
### Prompt 3: Vendor comparison

- Must compared vendors use the same currency, category mapping, geography, and time period?
- What is a “major delta”?
- Absolute amount threshold
- Percentage threshold
- Both
- Should the comparison include amount, invoice count, average invoice value, and service mix?
- How should a new vendor with no prior-period baseline be represented?
### Prompt 4: Invoice-line extraction

- Are quantity, rate/hour, line total, and tax available as structured data?
### Prompt 5: Tax anomalies

- What constitutes:
- Missing tax
- Invalid tax
- Unusual rate
- Inconsistent tax treatment
- Are the tax rules dependent on country, state, legal entity, vendor, service type, or effective date?
- What wording is acceptable so that the chatbot does not present a tax observation as a final compliance conclusion?
### Prompt 6: Vendor movement and trends

- Does “current year versus previous year” mean calendar year or fiscal year?
- Should comparisons use year-to-date aligned periods?
- How are “top vendors” selected?
- Current-year spend
- Previous-year spend
- Combined spend
- Invoice count
- Should zero-activity months appear in the trend?
### Prompt 7: Top 10 invoices and anomalies

- Are invoices ranked by gross amount, net amount, paid amount, or reporting-currency amount?
- Should credits and negative invoices be included?
- Which anomaly types are expected?
- Unusually high amount
- Duplicate invoice
- Unusual tax
- Rate variance
- Quantity variance
- Vendor/category mismatch
- Missing purchase order
### Prompt 8 and 9: Reconciliations

- What identifies a specific reconciliation?
- Can users see all reconciliations, or only specific accounts, entities, or regions?
### Prompt 10: Month-over-month changes

- Which date controls monthly comparison?
- Invoice date
- Posting date
- Payment date
- Document date
- Should percentage change be omitted when the prior month is zero?
- Are partial months compared with full months?
- Should the answer cite:
- AS400 query reference
- OnBase document
- Both
## 6. Security and global access

- Who may use the chatbot?
- Will access be granted through:
- Microsoft Entra ID group
- Finance role
- Is authorization evaluated using the current user for every request?
- Can one user share a chatbot result with another user?
- Must sensitive bank, tax, payment, or personal information be masked?
## 7. Important edge cases

### Vendor and master-data edge cases

- Vendor name matches multiple vendor IDs.
- Vendor has aliases or changed names.
- Two vendors have similar names.
- Vendor exists in AS400 but has no OnBase document.
- Vendor is inactive but has historical invoices.
- User supplies an invalid vendor, partial name, abbreviation, or spelling variation.
### Date edge cases

- User says “Q1” without specifying year.
- Fiscal quarter differs from calendar quarter.
- User says “last month” on the first day of a month.
- Invoice date and posting date fall in different periods.
- Current month is incomplete.
- Leap year, year-end, and fiscal-year rollover.
- Source systems use different time zones.
### Financial edge cases

- Multiple currencies in one result.
- Voided, reversed, rejected, or duplicate record.
- Invoice paid partially.
- Tax-exempt invoice.
- Multiple tax components on one invoice.
### Query edge cases

- User requests an unsupported period or too much history.
- User asks for all invoices without filters.
- User asks a follow-up question with ambiguous context.
- User combines several intents in one prompt.
- Query returns more rows than the chat limit.
- Query returns no results.
- Query returns partial results.

### Data-source mapping

| Data element | Clarification required |
| --- | --- |
| Invoice number | AS400, OnBase, or both |
| Vendor ID and name | AS400 vendor master or another master |
| Invoice date | Posting date, invoice date, or document date |
| Amount | Gross, net, paid, or open amount |
| Currency | Transaction or reporting currency |
| Payment status | AS400 or another payment system |
| Service/category | Existing structured field or derived mapping |
| Invoice lines | AS400 line data, OnBase metadata, or PDF extraction |
| Tax amount/rate | Structured tax table or PDF |
| Reconciliation status | AS400, OnBase, or another platform |
