import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { map, catchError, switchMap, withLatestFrom } from 'rxjs/operators';
import { InvoiceService } from '../services/invoice.service';
import * as InvoiceActions from './invoice.actions';
import * as InvoiceSelectors from './invoice.selectors';

@Injectable()
export class InvoiceEffects {
  constructor(
    private actions$: Actions,
    private invoiceService: InvoiceService,
    private store: Store
  ) {}

  /**
   * Search invoices effect
   */
  searchInvoices$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InvoiceActions.searchInvoices),
      withLatestFrom(this.store.select(InvoiceSelectors.selectSearchCriteria)),
      switchMap(([action, currentCriteria]) => {
        // Merge action request with current criteria
        const request = {
          ...currentCriteria,
          ...action.request
        };

        return this.invoiceService.searchInvoices(request).pipe(
          map(response => InvoiceActions.searchInvoicesSuccess({ response })),
          catchError(error => of(InvoiceActions.searchInvoicesFailure({ 
            error: error.message || 'Failed to search invoices' 
          })))
        );
      })
    )
  );

  /**
   * Sort invoices effect - triggers new search
   */
  sortInvoices$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InvoiceActions.sortInvoices),
      map(() => InvoiceActions.searchInvoices({ request: {} }))
    )
  );

  /**
   * Change page effect - triggers new search
   */
  changePage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InvoiceActions.changePage),
      map(() => InvoiceActions.searchInvoices({ request: {} }))
    )
  );

  /**
   * Toggle show all effect - triggers new search
   */
  toggleShowAll$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InvoiceActions.toggleShowAll),
      map(() => InvoiceActions.searchInvoices({ request: {} }))
    )
  );

  /**
   * Export to Excel effect
   */
  exportToExcel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InvoiceActions.exportToExcel),
      withLatestFrom(this.store.select(InvoiceSelectors.selectSearchCriteria)),
      switchMap(([, criteria]) => {
        return this.invoiceService.exportToExcel(criteria).pipe(
          map(data => {
            // Trigger download
            const url = window.URL.createObjectURL(data);
            const link = document.createElement('a');
            link.href = url;
            link.download = 'EpaymentCustomerInvoices.xlsx';
            link.click();
            window.URL.revokeObjectURL(url);

            return InvoiceActions.exportToExcelSuccess({ data });
          }),
          catchError(error => of(InvoiceActions.exportToExcelFailure({ 
            error: error.message || 'Failed to export invoices' 
          })))
        );
      })
    )
  );

  /**
   * Load default date span effect
   */
  loadDefaultDateSpan$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InvoiceActions.loadDefaultDateSpan),
      switchMap(() => {
        return this.invoiceService.getDefaultDateSpan().pipe(
          map(days => InvoiceActions.loadDefaultDateSpanSuccess({ days })),
          catchError(error => of(InvoiceActions.loadDefaultDateSpanFailure({ 
            error: error.message || 'Failed to load default date span' 
          })))
        );
      })
    )
  );
}

