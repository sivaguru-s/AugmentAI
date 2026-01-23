import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { InvoiceSearchRequest, InvoiceSearchResponse } from '../models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly apiUrl = `${environment.apiUrl}/api/invoice`;

  constructor(private http: HttpClient) {}

  /**
   * Search invoices with pagination
   */
  searchInvoices(request: Partial<InvoiceSearchRequest>): Observable<InvoiceSearchResponse> {
    return this.http.post<InvoiceSearchResponse>(`${this.apiUrl}/search`, request);
  }

  /**
   * Get default date span in days
   */
  getDefaultDateSpan(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/default-date-span`);
  }

  /**
   * Export invoices to Excel
   */
  exportToExcel(request: Partial<InvoiceSearchRequest>): Observable<Blob> {
    return this.http.post(`${this.apiUrl}/export`, request, {
      responseType: 'blob',
      headers: new HttpHeaders({
        'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      })
    });
  }
}

