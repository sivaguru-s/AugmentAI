import { Component, EventEmitter, Output, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

const DEFAULT_DATE_RANGE_DAYS = 180;
const MAX_DATE_AGE_YEARS = 200;

function dateRangeValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const fromDate = group.get('fromDate')?.value;
    const toDate = group.get('toDate')?.value;
    if (!fromDate || !toDate) return null;
    const from = new Date(fromDate);
    const to = new Date(toDate);
    if (from > to) return { dateRangeInvalid: 'From date must be before or equal to To date' };
    return null;
  };
}

function maxDateAgeValidator(maxAgeYears: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const date = new Date(control.value);
    const minAllowedDate = new Date();
    minAllowedDate.setFullYear(minAllowedDate.getFullYear() - maxAgeYears);
    if (date < minAllowedDate) return { dateTooOld: `Date cannot be older than ${maxAgeYears} years` };
    return null;
  };
}

function notFutureDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const date = new Date(control.value);
    const today = new Date();
    today.setHours(23, 59, 59, 999);
    if (date > today) return { futureDate: 'Date cannot be in the future' };
    return null;
  };
}

@Component({
  selector: 'epay-invoice-search-form',
  templateUrl: './invoice-search-form.component.html',
  styleUrls: ['./invoice-search-form.component.scss'],
  standalone: false
})
export class InvoiceSearchFormComponent implements OnInit, OnChanges {
  @Input() fromDate: Date | null = null;
  @Input() toDate: Date | null = null;
  @Output() search = new EventEmitter<void>();
  @Output() updateCriteria = new EventEmitter<any>();
  searchForm!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    const defaultFromDate = new Date(new Date().setDate(new Date().getDate() - DEFAULT_DATE_RANGE_DAYS));
    const defaultToDate = new Date();

    this.searchForm = this.fb.group({
      invoiceNumber: ['', [Validators.maxLength(9)]],
      creditNumber: ['', [Validators.maxLength(15)]],
      poNumber: ['', [Validators.maxLength(25)]],
      fromDate: [this.fromDate || defaultFromDate, [
        Validators.required, maxDateAgeValidator(MAX_DATE_AGE_YEARS), notFutureDateValidator()
      ]],
      toDate: [this.toDate || defaultToDate, [
        Validators.required, maxDateAgeValidator(MAX_DATE_AGE_YEARS), notFutureDateValidator()
      ]]
    }, { validators: dateRangeValidator() });

    this.searchForm.valueChanges.subscribe(value => this.updateCriteria.emit(value));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.searchForm) {
      if (changes['fromDate']?.currentValue) {
        this.searchForm.patchValue({ fromDate: changes['fromDate'].currentValue }, { emitEvent: false });
      }
      if (changes['toDate']?.currentValue) {
        this.searchForm.patchValue({ toDate: changes['toDate'].currentValue }, { emitEvent: false });
      }
    }
  }

  onSearch(): void {
    this.searchForm.markAllAsTouched();
    if (this.searchForm.valid && this.isValidDateRange()) {
      this.search.emit();
    }
  }

  isValidDateRange(): boolean {
    const fromDate = this.searchForm.get('fromDate')?.value;
    const toDate = this.searchForm.get('toDate')?.value;
    if (!fromDate || !toDate) return false;
    return new Date(fromDate) <= new Date(toDate);
  }

  hasDateRangeError(): boolean {
    return this.searchForm.hasError('dateRangeInvalid') && this.searchForm.touched;
  }

  getErrorMessage(controlName: string): string | null {
    const control = this.searchForm.get(controlName);
    if (!control || !control.errors || !control.touched) return null;
    if (control.errors['required']) return 'This field is required';
    if (control.errors['maxlength']) return `Maximum length is ${control.errors['maxlength'].requiredLength} characters`;
    if (control.errors['dateTooOld']) return control.errors['dateTooOld'];
    if (control.errors['futureDate']) return control.errors['futureDate'];
    return null;
  }

  getDateRangeErrorMessage(): string | null {
    if (this.searchForm.hasError('dateRangeInvalid')) return this.searchForm.errors?.['dateRangeInvalid'];
    return null;
  }
}

