import { Component, OnInit, OnDestroy } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import * as InvoiceActions from '../../store/invoice.actions';
import * as InvoiceSelectors from '../../store/invoice.selectors';
import { InvoiceDto, PagedResult } from '../../models/invoice.model';

@Component({
  selector: 'app-invoice-search',
  templateUrl: './invoice-search.component.html',
  styleUrls: ['./invoice-search.component.scss']
})
export class InvoiceSearchComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  // Observables from store
  invoices$: Observable<InvoiceDto[]>;
  pagedResult$: Observable<PagedResult<InvoiceDto> | null>;
  loading$: Observable<boolean>;
  error$: Observable<string | null>;
  selectedInvoices$: Observable<string[]>;
  selectedCount$: Observable<number>;
  isAnalyst$: Observable<boolean>;
  fromDate$: Observable<Date>;
  toDate$: Observable<Date>;
  showAll$: Observable<boolean>;
  currentPage$: Observable<number>;
  totalPages$: Observable<number>;
  hasNextPage$: Observable<boolean>;
  hasPreviousPage$: Observable<boolean>;

  constructor(private store: Store) {
    // Initialize observables
    this.invoices$ = this.store.select(InvoiceSelectors.selectInvoices);
    this.pagedResult$ = this.store.select(InvoiceSelectors.selectPagedResult);
    this.loading$ = this.store.select(InvoiceSelectors.selectLoading);
    this.error$ = this.store.select(InvoiceSelectors.selectError);
    this.selectedInvoices$ = this.store.select(InvoiceSelectors.selectSelectedInvoices);
    this.selectedCount$ = this.store.select(InvoiceSelectors.selectSelectedCount);
    this.isAnalyst$ = this.store.select(InvoiceSelectors.selectIsAnalyst);
    this.fromDate$ = this.store.select(InvoiceSelectors.selectFromDate);
    this.toDate$ = this.store.select(InvoiceSelectors.selectToDate);
    this.showAll$ = this.store.select(InvoiceSelectors.selectShowAll);
    this.currentPage$ = this.store.select(InvoiceSelectors.selectCurrentPage);
    this.totalPages$ = this.store.select(InvoiceSelectors.selectTotalPages);
    this.hasNextPage$ = this.store.select(InvoiceSelectors.selectHasNextPage);
    this.hasPreviousPage$ = this.store.select(InvoiceSelectors.selectHasPreviousPage);
  }

  ngOnInit(): void {
    // Load default date span and trigger initial search
    this.store.dispatch(InvoiceActions.loadDefaultDateSpan());
    
    // Wait a bit for date span to load, then search
    setTimeout(() => {
      this.onSearch();
    }, 500);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Handle search button click
   */
  onSearch(): void {
    this.store.dispatch(InvoiceActions.searchInvoices({ request: {} }));
  }

  /**
   * Handle sort column click
   */
  onSort(column: string): void {
    this.store.dispatch(InvoiceActions.sortInvoices({ column }));
  }

  /**
   * Handle page change
   */
  onPageChange(pageNumber: number): void {
    this.store.dispatch(InvoiceActions.changePage({ pageNumber }));
  }

  /**
   * Handle show all checkbox toggle
   */
  onToggleShowAll(): void {
    this.store.dispatch(InvoiceActions.toggleShowAll());
  }

  /**
   * Handle invoice selection toggle
   */
  onToggleInvoiceSelection(invoiceNumber: string): void {
    this.store.dispatch(InvoiceActions.toggleInvoiceSelection({ invoiceNumber }));
  }

  /**
   * Handle select all checkbox toggle
   */
  onSelectAll(selected: boolean): void {
    this.store.dispatch(InvoiceActions.selectAllInvoices({ selected }));
  }

  /**
   * Handle export to Excel button click
   */
  onExportToExcel(): void {
    this.store.dispatch(InvoiceActions.exportToExcel());
  }

  /**
   * Handle make payment button click
   */
  onMakePayment(): void {
    // TODO: Navigate to confirmation page
    console.log('Make payment clicked');
  }

  /**
   * Handle search criteria update
   */
  onUpdateSearchCriteria(criteria: any): void {
    this.store.dispatch(InvoiceActions.updateSearchCriteria({ criteria }));
  }

  /**
   * Check if invoice is selected
   */
  isInvoiceSelected(invoiceNumber: string): Observable<boolean> {
    return this.store.select(InvoiceSelectors.selectIsInvoiceSelected(invoiceNumber));
  }
}

