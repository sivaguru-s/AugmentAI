import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-invoice-search-form',
  templateUrl: './invoice-search-form.component.html',
  styleUrls: ['./invoice-search-form.component.scss']
})
export class InvoiceSearchFormComponent implements OnInit {
  @Output() search = new EventEmitter<void>();
  @Output() updateCriteria = new EventEmitter<any>();

  searchForm!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      invoiceNumber: [''],
      creditNumber: [''],
      poNumber: [''],
      fromDate: [new Date(new Date().setDate(new Date().getDate() - 90))],
      toDate: [new Date()]
    });

    // Emit criteria changes
    this.searchForm.valueChanges.subscribe(value => {
      this.updateCriteria.emit(value);
    });
  }

  onSearch(): void {
    this.search.emit();
  }
}

