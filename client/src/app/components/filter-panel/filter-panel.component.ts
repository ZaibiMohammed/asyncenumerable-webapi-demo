import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filter-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="card mb-4">
      <div class="card-body">
        <h5 class="card-title">Filters</h5>
        <div class="row g-3">
          <div class="col-md-3">
            <label class="form-label">Category</label>
            <select class="form-select" [(ngModel)]="filters.category" (change)="applyFilters()">
              <option value="">All Categories</option>
              <option *ngFor="let category of categories" [value]="category">
                {{category}}
              </option>
            </select>
          </div>
          
          <div class="col-md-2">
            <label class="form-label">Min Price</label>
            <input type="number" class="form-control" 
                   [(ngModel)]="filters.minPrice" 
                   (change)="applyFilters()">
          </div>
          
          <div class="col-md-2">
            <label class="form-label">Max Price</label>
            <input type="number" class="form-control" 
                   [(ngModel)]="filters.maxPrice" 
                   (change)="applyFilters()">
          </div>
          
          <div class="col-md-2">
            <label class="form-label">Page Size</label>
            <select class="form-select" [(ngModel)]="filters.pageSize" (change)="applyFilters()">
              <option [value]="10">10</option>
              <option [value]="20">20</option>
              <option [value]="50">50</option>
              <option [value]="100">100</option>
            </select>
          </div>
        </div>
      </div>
    </div>
  `
})
export class FilterPanelComponent {
  @Input() categories: string[] = [];
  @Output() filtersChanged = new EventEmitter<any>();

  filters = {
    category: '',
    minPrice: null as number | null,
    maxPrice: null as number | null,
    pageSize: 20
  };

  applyFilters() {
    this.filtersChanged.emit(this.filters);
  }
}