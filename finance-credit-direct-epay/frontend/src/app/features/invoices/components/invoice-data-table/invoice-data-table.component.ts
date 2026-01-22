import { Component, Input, Output, EventEmitter } from '@angular/core';
import { InvoiceDto } from '../../models/invoice.model';

@Component({
  selector: 'app-invoice-data-table',
  templateUrl: './invoice-data-table.component.html',
  styleUrls: ['./invoice-data-table.component.scss']
})
export class InvoiceDataTableComponent {
  @Input() invoices: InvoiceDto[] | null = [];
  @Input() selectedInvoices: string[] | null = [];
  @Input() loading: boolean | null = false;
  
  @Output() sort = new EventEmitter<string>();
  @Output() toggleSelection = new EventEmitter<string>();
  @Output() selectAll = new EventEmitter<boolean>();

  displayedColumns: string[] = [
    'select',
    'status',
    'invoiceNumber',
    'creditNumber',
    'shipTo',
    'invoiceDate',
    'orderNumber',
    'tripNumber',
    'rppNumber',
    'poNumber',
    'invoiceAmount',
    'amountPaid',
    'balance',
    'code',
    'days'
  ];

  currentSortColumn: string | null = null;
  sortAscending: boolean = true;

  onSort(column: string): void {
    if (this.currentSortColumn === column) {
      this.sortAscending = !this.sortAscending;
    } else {
      this.currentSortColumn = column;
      this.sortAscending = true;
    }
    this.sort.emit(column);
  }

  onToggleSelection(invoiceNumber: string): void {
    this.toggleSelection.emit(invoiceNumber);
  }

  onSelectAll(event: any): void {
    this.selectAll.emit(event.checked);
  }

  isSelected(invoiceNumber: string): boolean {
    return this.selectedInvoices?.includes(invoiceNumber) || false;
  }

  isAllSelected(): boolean {
    if (!this.invoices || this.invoices.length === 0) {
      return false;
    }
    const selectableInvoices = this.invoices.filter(inv => inv.canSelect);
    return selectableInvoices.length > 0 && 
           selectableInvoices.every(inv => this.isSelected(inv.invoiceNumber));
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('en-US', { 
      minimumFractionDigits: 2, 
      maximumFractionDigits: 2 
    });
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US');
  }
}

