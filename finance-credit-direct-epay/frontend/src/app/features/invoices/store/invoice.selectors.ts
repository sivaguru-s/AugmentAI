import { createFeatureSelector, createSelector } from '@ngrx/store';
import { InvoiceState } from './invoice.state';

/**
 * Feature selector
 */
export const selectInvoiceState = createFeatureSelector<InvoiceState>('invoice');

/**
 * Search criteria selectors
 */
export const selectSearchCriteria = createSelector(
  selectInvoiceState,
  (state) => state.searchCriteria
);

export const selectFromDate = createSelector(
  selectSearchCriteria,
  (criteria) => criteria.fromDate
);

export const selectToDate = createSelector(
  selectSearchCriteria,
  (criteria) => criteria.toDate
);

export const selectShowAll = createSelector(
  selectSearchCriteria,
  (criteria) => criteria.showAll
);

/**
 * Invoice data selectors
 */
export const selectInvoices = createSelector(
  selectInvoiceState,
  (state) => state.invoices?.items || []
);

export const selectPagedResult = createSelector(
  selectInvoiceState,
  (state) => state.invoices
);

export const selectTotalPages = createSelector(
  selectPagedResult,
  (result) => result?.totalPages || 0
);

export const selectCurrentPage = createSelector(
  selectPagedResult,
  (result) => result?.pageNumber || 1
);

export const selectHasNextPage = createSelector(
  selectPagedResult,
  (result) => result?.hasNextPage || false
);

export const selectHasPreviousPage = createSelector(
  selectPagedResult,
  (result) => result?.hasPreviousPage || false
);

/**
 * Selection selectors
 */
export const selectSelectedInvoices = createSelector(
  selectInvoiceState,
  (state) => state.selectedInvoices
);

export const selectSelectedCount = createSelector(
  selectSelectedInvoices,
  (selected) => selected.length
);

export const selectIsInvoiceSelected = (invoiceNumber: string) => createSelector(
  selectSelectedInvoices,
  (selected) => selected.includes(invoiceNumber)
);

export const selectAllSelected = createSelector(
  selectInvoices,
  selectSelectedInvoices,
  (invoices, selected) => {
    const selectableInvoices = invoices.filter(inv => inv.canSelect);
    return selectableInvoices.length > 0 && 
           selectableInvoices.every(inv => selected.includes(inv.invoiceNumber));
  }
);

/**
 * UI state selectors
 */
export const selectLoading = createSelector(
  selectInvoiceState,
  (state) => state.loading
);

export const selectError = createSelector(
  selectInvoiceState,
  (state) => state.error
);

export const selectIsAnalyst = createSelector(
  selectInvoiceState,
  (state) => state.isAnalyst
);

