import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent implements OnChanges {
  @Input() currentPage: number | null = 1;
  @Input() totalPages: number | null = 1;
  @Input() hasNext: boolean | null = false;
  @Input() hasPrevious: boolean | null = false;
  
  @Output() pageChange = new EventEmitter<number>();

  pageNumbers: number[] = [];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['currentPage'] || changes['totalPages']) {
      this.generatePageNumbers();
    }
  }

  generatePageNumbers(): void {
    const current = this.currentPage || 1;
    const total = this.totalPages || 1;
    const pages: number[] = [];

    // Show max 10 page numbers
    const maxPages = 10;
    let startPage = Math.max(1, current - Math.floor(maxPages / 2));
    let endPage = Math.min(total, startPage + maxPages - 1);

    // Adjust start if we're near the end
    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    this.pageNumbers = pages;
  }

  onPageClick(page: number): void {
    if (page !== this.currentPage && page >= 1 && page <= (this.totalPages || 1)) {
      this.pageChange.emit(page);
    }
  }

  onPrevious(): void {
    if (this.hasPrevious && this.currentPage) {
      this.pageChange.emit(this.currentPage - 1);
    }
  }

  onNext(): void {
    if (this.hasNext && this.currentPage) {
      this.pageChange.emit(this.currentPage + 1);
    }
  }
}

