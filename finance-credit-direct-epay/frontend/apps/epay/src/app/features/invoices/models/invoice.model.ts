/**
 * Invoice data model
 */
export interface InvoiceDto {
  customerNumber: string;
  shipToNumber: string;
  invoiceNumber: string;
  creditNumber: number;
  invoiceDate: Date;
  invoiceAmount: number;
  amountPaid: number;
  balance: number;
  categoryCode: string;
  poNumber: string;
  orderNumber: string;
  orderDate: Date | null;
  rppNumber: string;
  days: number;
  tripNumber: number | null;
  ePayStatus: string | null;
  ePayReferenceNumber: number | null;
  hideDetail: boolean;
  canSelect: boolean;
}

/**
 * Paged result wrapper
 */
export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalRecords: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * Invoice search request
 */
export interface InvoiceSearchRequest {
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
}

/**
 * Invoice search response
 */
export interface InvoiceSearchResponse {
  result: PagedResult<InvoiceDto>;
  fromDate: Date;
  toDate: Date;
  isAnalyst: boolean;
}

