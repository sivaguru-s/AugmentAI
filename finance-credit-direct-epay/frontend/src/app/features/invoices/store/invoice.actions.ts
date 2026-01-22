import { createAction, props } from '@ngrx/store';
import { InvoiceSearchRequest, InvoiceSearchResponse } from '../models/invoice.model';

/**
 * Search invoices
 */
export const searchInvoices = createAction(
  '[Invoice] Search Invoices',
  props<{ request: Partial<InvoiceSearchRequest> }>()
);

export const searchInvoicesSuccess = createAction(
  '[Invoice] Search Invoices Success',
  props<{ response: InvoiceSearchResponse }>()
);

export const searchInvoicesFailure = createAction(
  '[Invoice] Search Invoices Failure',
  props<{ error: string }>()
);

/**
 * Sort invoices
 */
export const sortInvoices = createAction(
  '[Invoice] Sort Invoices',
  props<{ column: string }>()
);

/**
 * Change page
 */
export const changePage = createAction(
  '[Invoice] Change Page',
  props<{ pageNumber: number }>()
);

/**
 * Toggle show all invoices
 */
export const toggleShowAll = createAction(
  '[Invoice] Toggle Show All'
);

/**
 * Select/deselect invoice
 */
export const toggleInvoiceSelection = createAction(
  '[Invoice] Toggle Invoice Selection',
  props<{ invoiceNumber: string }>()
);

export const selectAllInvoices = createAction(
  '[Invoice] Select All Invoices',
  props<{ selected: boolean }>()
);

/**
 * Export to Excel
 */
export const exportToExcel = createAction(
  '[Invoice] Export To Excel'
);

export const exportToExcelSuccess = createAction(
  '[Invoice] Export To Excel Success',
  props<{ data: Blob }>()
);

export const exportToExcelFailure = createAction(
  '[Invoice] Export To Excel Failure',
  props<{ error: string }>()
);

/**
 * Update search criteria
 */
export const updateSearchCriteria = createAction(
  '[Invoice] Update Search Criteria',
  props<{ criteria: Partial<InvoiceSearchRequest> }>()
);

/**
 * Reset search
 */
export const resetSearch = createAction(
  '[Invoice] Reset Search'
);

/**
 * Load default date span
 */
export const loadDefaultDateSpan = createAction(
  '[Invoice] Load Default Date Span'
);

export const loadDefaultDateSpanSuccess = createAction(
  '[Invoice] Load Default Date Span Success',
  props<{ days: number }>()
);

export const loadDefaultDateSpanFailure = createAction(
  '[Invoice] Load Default Date Span Failure',
  props<{ error: string }>()
);

