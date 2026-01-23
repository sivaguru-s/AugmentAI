import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';

// Angular Material Modules
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';

// Components
import { InvoiceSearchComponent } from './components/invoice-search/invoice-search.component';
import { InvoiceSearchFormComponent } from './components/invoice-search-form/invoice-search-form.component';
import { InvoiceDataTableComponent } from './components/invoice-data-table/invoice-data-table.component';
import { PaginationComponent } from './components/pagination/pagination.component';

// Shared Components
import { EpayNavComponent } from '../../shared/components/epay-nav/epay-nav.component';

// Services
import { InvoiceService } from './services/invoice.service';

// NgRx
import { invoiceReducer } from './store/invoice.reducer';
import { InvoiceEffects } from './store/invoice.effects';

const routes: Routes = [
  {
    path: '',
    component: InvoiceSearchComponent
  }
];

@NgModule({
  declarations: [
    InvoiceSearchComponent,
    InvoiceSearchFormComponent,
    InvoiceDataTableComponent,
    PaginationComponent,
    EpayNavComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    MatTableModule,
    MatCheckboxModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatTabsModule,
    StoreModule.forFeature('invoice', invoiceReducer),
    EffectsModule.forFeature([InvoiceEffects])
  ],
  providers: [
    InvoiceService
  ]
})
export class InvoicesModule { }

