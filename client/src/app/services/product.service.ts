import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'https://localhost:7001/api/products';

  constructor(private http: HttpClient) {}

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/categories`);
  }

  getProductsStream(filters: any): Observable<any> {
    let params = new HttpParams();
    
    if (filters.pageSize) {
      params = params.set('pageSize', filters.pageSize);
    }
    if (filters.category) {
      params = params.set('category', filters.category);
    }
    if (filters.minPrice) {
      params = params.set('minPrice', filters.minPrice);
    }
    if (filters.maxPrice) {
      params = params.set('maxPrice', filters.maxPrice);
    }

    return new Observable<any>(observer => {
      const reader = this.http.get(`${this.apiUrl}/stream`, {
        params,
        responseType: 'text',
        observe: 'body'
      }).subscribe({
        next: (response) => {
          // Split the response into lines and parse each line as JSON
          const lines = response.split('\n').filter(line => line.trim());
          lines.forEach(line => {
            try {
              const product = JSON.parse(line);
              observer.next(product);
            } catch (e) {
              console.error('Error parsing product:', e);
            }
          });
          observer.complete();
        },
        error: (error) => observer.error(error),
        complete: () => observer.complete()
      });

      // Cleanup subscription on unsubscribe
      return () => reader.unsubscribe();
    });
  }
}