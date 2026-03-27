import { createReducer, on } from '@ngrx/store';
import { InvoiceState, initialInvoiceState } from './invoice.state';
import * as InvoiceActions from './invoice.actions';

export const invoiceReducer = createReducer(
  initialInvoiceState,

  // Search invoices
  on(InvoiceActions.searchInvoices, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(InvoiceActions.searchInvoicesSuccess, (state, { response }) => ({
    ...state,
    invoices: response.result,
    isAnalyst: response.isAnalyst,
    loading: false,
    error: null,
    selectedInvoices: [] // Clear selection on new search
  })),

  on(InvoiceActions.searchInvoicesFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Sort invoices
  on(InvoiceActions.sortInvoices, (state, { column }) => {
    const isSameColumn = state.searchCriteria.sortColumn === column;
    return {
      ...state,
      searchCriteria: {
        ...state.searchCriteria,
        sortColumn: column,
        sortAscending: isSameColumn ? !state.searchCriteria.sortAscending : true
      }
    };
  }),

  // Change page
  on(InvoiceActions.changePage, (state, { pageNumber }) => ({
    ...state,
    searchCriteria: {
      ...state.searchCriteria,
      pageNumber
    }
  })),

  // Toggle show all
  on(InvoiceActions.toggleShowAll, (state) => ({
    ...state,
    searchCriteria: {
      ...state.searchCriteria,
      showAll: !state.searchCriteria.showAll,
      pageNumber: state.searchCriteria.showAll ? 1 : 0
    }
  })),

  // Toggle invoice selection
  on(InvoiceActions.toggleInvoiceSelection, (state, { invoiceNumber }) => {
    const isSelected = state.selectedInvoices.includes(invoiceNumber);
    return {
      ...state,
      selectedInvoices: isSelected
        ? state.selectedInvoices.filter(num => num !== invoiceNumber)
        : [...state.selectedInvoices, invoiceNumber]
    };
  }),

  // Select all invoices
  on(InvoiceActions.selectAllInvoices, (state, { selected }) => {
    if (!selected) {
      return {
        ...state,
        selectedInvoices: []
      };
    }

    // Select all selectable invoices
    const selectableInvoices = state.invoices?.items
      .filter(invoice => invoice.canSelect)
      .map(invoice => invoice.invoiceNumber) || [];

    return {
      ...state,
      selectedInvoices: selectableInvoices
    };
  }),

  // Export to Excel
  on(InvoiceActions.exportToExcel, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(InvoiceActions.exportToExcelSuccess, (state) => ({
    ...state,
    loading: false
  })),

  on(InvoiceActions.exportToExcelFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  // Update search criteria
  on(InvoiceActions.updateSearchCriteria, (state, { criteria }) => ({
    ...state,
    searchCriteria: {
      ...state.searchCriteria,
      ...criteria
    }
  })),

  // Reset search
  on(InvoiceActions.resetSearch, () => initialInvoiceState),

  // Load default date span
  on(InvoiceActions.loadDefaultDateSpanSuccess, (state, { days }) => ({
    ...state,
    searchCriteria: {
      ...state.searchCriteria,
      fromDate: new Date(new Date().setDate(new Date().getDate() - days)),
      toDate: new Date()
    }
  }))
);

