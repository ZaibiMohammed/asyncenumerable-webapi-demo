import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="position-relative">
      <!-- Loading Indicator -->
      <div *ngIf="loading" class="position-absolute top-0 start-0 w-100 text-center p-3 bg-light">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <div class="mt-2">Loading products...</div>
      </div>

      <!-- Error Message -->
      <div *ngIf="error" class="alert alert-danger">
        {{ error }}
      </div>

      <!-- Products Table -->
      <div class="table-responsive">
        <table class="table table-striped table-hover">
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Category</th>
              <th>Price</th>
              <th>Stock</th>
              <th>Last Updated</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let product of products">
              <td>{{ product.id }}</td>
              <td>{{ product.name }}</td>
              <td>{{ product.category }}</td>
              <td>{{ product.price | currency }}</td>
              <td>{{ product.stock }}</td>
              <td>{{ product.lastUpdated | date:'medium' }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- No Products Message -->
      <div *ngIf="!loading && !error && products.length === 0" class="text-center p-3">
        No products found.
      </div>
    </div>
  `
})
export class ProductListComponent {
  @Input() products: any[] = [];
  @Input() loading = false;
  @Input() error: string | null = null;
}