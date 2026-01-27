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
 * Initial state
 * Note: customerNumber is set to a default value for development.
 * In production, this should come from authentication/session.
 */
export const initialInvoiceState: InvoiceState = {
  searchCriteria: {
    customerNumber: '100000', // Default customer number for development - change as needed
    shipToNumber: '',
    allShipTos: false,
    securityMHS: '',
    invoiceNumber: null,
    creditNumber: null,
    poNumber: null,
    fromDate: new Date(new Date().setDate(new Date().getDate() - 90)), // Default 90 days
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

