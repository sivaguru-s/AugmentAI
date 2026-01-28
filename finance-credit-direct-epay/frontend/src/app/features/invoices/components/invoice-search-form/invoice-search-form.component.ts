import { Component, EventEmitter, Output, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

/**
 * Default date range in days - 6 months (approximately 180 days).
 */
const DEFAULT_DATE_RANGE_DAYS = 180;

/**
 * Maximum allowed date age in years for validation.
 * Dates older than this are considered invalid.
 */
const MAX_DATE_AGE_YEARS = 200;

/**
 * Custom validator to ensure fromDate is before or equal to toDate.
 * This is a cross-field validator applied at the form group level.
 */
function dateRangeValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const fromDate = group.get('fromDate')?.value;
    const toDate = group.get('toDate')?.value;

    if (!fromDate || !toDate) {
      return null; // Required validators will handle missing dates
    }

    const from = new Date(fromDate);
    const to = new Date(toDate);

    if (from > to) {
      return { dateRangeInvalid: 'From date must be before or equal to To date' };
    }

    return null;
  };
}

/**
 * Custom validator to ensure date is not too old.
 * @param maxAgeYears Maximum allowed age in years.
 */
function maxDateAgeValidator(maxAgeYears: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null; // Required validator will handle empty value
    }

    const date = new Date(control.value);
    const minAllowedDate = new Date();
    minAllowedDate.setFullYear(minAllowedDate.getFullYear() - maxAgeYears);

    if (date < minAllowedDate) {
      return { dateTooOld: `Date cannot be older than ${maxAgeYears} years` };
    }

    return null;
  };
}

/**
 * Custom validator to ensure date is not in the future.
 */
function notFutureDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null; // Required validator will handle empty value
    }

    const date = new Date(control.value);
    const today = new Date();
    today.setHours(23, 59, 59, 999); // End of today

    if (date > today) {
      return { futureDate: 'Date cannot be in the future' };
    }

    return null;
  };
}

/**
 * Invoice Search Form Component.
 * Provides a form for searching invoices with date range and optional filters.
 */
@Component({
  selector: 'app-invoice-search-form',
  templateUrl: './invoice-search-form.component.html',
  styleUrls: ['./invoice-search-form.component.scss']
})
export class InvoiceSearchFormComponent implements OnInit, OnChanges {
  /** Input: From date from the store state */
  @Input() fromDate: Date | null = null;

  /** Input: To date from the store state */
  @Input() toDate: Date | null = null;

  /** Output: Emitted when user initiates a search */
  @Output() search = new EventEmitter<void>();

  /** Output: Emitted when search criteria changes */
  @Output() updateCriteria = new EventEmitter<any>();

  /** The reactive form for search criteria */
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
        Validators.required,
        maxDateAgeValidator(MAX_DATE_AGE_YEARS),
        notFutureDateValidator()
      ]],
      toDate: [this.toDate || defaultToDate, [
        Validators.required,
        maxDateAgeValidator(MAX_DATE_AGE_YEARS),
        notFutureDateValidator()
      ]]
    }, {
      validators: dateRangeValidator()
    });

    // Emit criteria changes when form values change
    this.searchForm.valueChanges.subscribe(value => {
      this.updateCriteria.emit(value);
    });
  }

  /**
   * Handle changes to input properties (dates from store).
   */
  ngOnChanges(changes: SimpleChanges): void {
    if (this.searchForm) {
      if (changes['fromDate'] && changes['fromDate'].currentValue) {
        this.searchForm.patchValue({ fromDate: changes['fromDate'].currentValue }, { emitEvent: false });
      }
      if (changes['toDate'] && changes['toDate'].currentValue) {
        this.searchForm.patchValue({ toDate: changes['toDate'].currentValue }, { emitEvent: false });
      }
    }
  }

  /**
   * Handle search button click with validation.
   * Only emits search event if form is valid.
   */
  onSearch(): void {
    // Mark all fields as touched to show validation errors
    this.searchForm.markAllAsTouched();

    if (this.searchForm.valid && this.isValidDateRange()) {
      this.search.emit();
    }
  }

  /**
   * Validate date range - fromDate must be before or equal to toDate.
   * @returns true if date range is valid, false otherwise.
   */
  isValidDateRange(): boolean {
    const fromDate = this.searchForm.get('fromDate')?.value;
    const toDate = this.searchForm.get('toDate')?.value;
    if (!fromDate || !toDate) {
      return false;
    }
    return new Date(fromDate) <= new Date(toDate);
  }

  /**
   * Check if the form has a date range validation error.
   * @returns true if there is a date range error, false otherwise.
   */
  hasDateRangeError(): boolean {
    return this.searchForm.hasError('dateRangeInvalid') && this.searchForm.touched;
  }

  /**
   * Get the error message for a specific form control.
   * @param controlName The name of the form control.
   * @returns The error message or null if no error.
   */
  getErrorMessage(controlName: string): string | null {
    const control = this.searchForm.get(controlName);
    if (!control || !control.errors || !control.touched) {
      return null;
    }

    if (control.errors['required']) {
      return 'This field is required';
    }
    if (control.errors['maxlength']) {
      return `Maximum length is ${control.errors['maxlength'].requiredLength} characters`;
    }
    if (control.errors['dateTooOld']) {
      return control.errors['dateTooOld'];
    }
    if (control.errors['futureDate']) {
      return control.errors['futureDate'];
    }

    return null;
  }

  /**
   * Get the date range error message.
   * @returns The error message or null if no error.
   */
  getDateRangeErrorMessage(): string | null {
    if (this.searchForm.hasError('dateRangeInvalid')) {
      return this.searchForm.errors?.['dateRangeInvalid'];
    }
    return null;
  }
}

