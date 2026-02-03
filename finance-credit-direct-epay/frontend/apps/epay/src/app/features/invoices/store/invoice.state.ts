import { InvoiceDto, PagedResult } from '../models/invoice.model';

/**
 * Invoice feature state interface
 */
export interface InvoiceState {
  // Search criteria
  searchCriteria: {
    customerNumber: string;
    shipToNumber: string;
    allShipTos: boolean;
    securityMHS: string;
    invoiceNumber: string | null;
    creditNumber: string | null;
    poNumber: string | null;
    fromDate: Date;
    toDate: Date;
    sortColumn: string | null;
    sortAscending: boolean;
    pageNumber: number;
    pageSize: number;
    showAll: boolean;
  };

  // Search results
  invoices: PagedResult<InvoiceDto> | null;

  // Selected invoices for payment
  selectedInvoices: string[]; // Array of invoice numbers

  // UI state
  loading: boolean;
  error: string | null;

  // User authorization
  isAnalyst: boolean;
}

/**
 * Default date range in days - 6 months (approximately 180 days)
 */
const DEFAULT_DATE_RANGE_DAYS = 180;

/**
 * Initial state
 * Note: customerNumber and securityMHS are set to default values for development.
 * In production, these should come from authentication/session.
 *
 * IMPORTANT: The stored procedure requires a valid securityMHS value that exists
 * in Ashley.dbo.tblSecurityCustomer for the given customer number.
 * Common values: 'MASTERXX' (for testing), or the actual MHS code for the customer.
 */
export const initialInvoiceState: InvoiceState = {
  searchCriteria: {
    customerNumber: '4444400', // Default customer number for development - change as needed
    shipToNumber: '',
    allShipTos: true,  // Set to true to search all ship-tos
    securityMHS: 'MASTERXX', // Default security MHS for development - REQUIRED for stored procedure
    invoiceNumber: null,
    creditNumber: null,
    poNumber: null,
    fromDate: new Date(new Date().setDate(new Date().getDate() - DEFAULT_DATE_RANGE_DAYS)), // Default 6 months
    toDate: new Date(),
    sortColumn: null,
    sortAscending: true,
    pageNumber: 1,
    pageSize: 500,
    showAll: false
  },
  invoices: null,
  selectedInvoices: [],
  loading: false,
  error: null,
  isAnalyst: false
};

