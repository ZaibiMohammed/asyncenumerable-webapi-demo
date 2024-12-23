import { Component, OnInit } from '@angular/core';
import { ProductService } from './services/product.service';
import { ErrorService } from './services/error.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductListComponent } from './components/product-list/product-list.component';
import { FilterPanelComponent } from './components/filter-panel/filter-panel.component';
import { MetricsDashboardComponent } from './components/metrics-dashboard/metrics-dashboard.component';
import { HealthMonitorComponent } from './components/health-monitor/health-monitor.component';
import { ErrorDisplayComponent } from './components/error-display/error-display.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ProductListComponent, 
    FilterPanelComponent,
    MetricsDashboardComponent,
    HealthMonitorComponent,
    ErrorDisplayComponent
  ],
  template: `
    <app-error-display></app-error-display>

    <div class="container-fluid mt-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h1>Product Streaming Demo</h1>
        <span class="badge bg-primary">Streaming Performance Demo</span>
      </div>
      
      <div class="row">
        <div class="col-md-8">
          <!-- Main Content -->
          <app-filter-panel
            [categories]="categories"
            (filtersChanged)="onFiltersChanged($event)">
          </app-filter-panel>

          <app-product-list
            [products]="products"
            [loading]="loading"
            [error]="error">
          </app-product-list>
        </div>

        <div class="col-md-4">
          <!-- Monitoring Panels -->
          <app-metrics-dashboard></app-metrics-dashboard>
          <app-health-monitor></app-health-monitor>
        </div>
      </div>
    </div>
  `
})
export class AppComponent implements OnInit {
  products: any[] = [];
  categories: string[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private productService: ProductService,
    private errorService: ErrorService
  ) {}

  ngOnInit() {
    this.loadCategories();
  }

  private loadCategories() {
    this.productService.getCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: (error) => {
        this.error = 'Failed to load categories';
        this.errorService.showError('Failed to load product categories');
      }
    });
  }

  onFiltersChanged(filters: any) {
    this.loading = true;
    this.products = [];
    this.error = null;

    this.productService.getProductsStream(filters).subscribe({
      next: (product) => {
        this.products.push(product);
      },
      error: (error) => {
        this.error = 'Failed to load products';
        this.errorService.showError('Error streaming products: ' + error.message);
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}