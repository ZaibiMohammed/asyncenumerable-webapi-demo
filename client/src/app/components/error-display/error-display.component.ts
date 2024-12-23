import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ErrorService, ErrorMessage } from '../../services/error.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-error-display',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="error-container position-fixed top-0 end-0 p-3" style="z-index: 1050">
      <div *ngFor="let error of errors" 
           class="toast show mb-2" 
           [class.bg-danger]="error.type === 'error'"
           [class.bg-warning]="error.type === 'warning'"
           [class.bg-info]="error.type === 'info'"
           role="alert">
        <div class="toast-header">
          <strong class="me-auto">{{getTitle(error.type)}}</strong>
          <small>{{error.timestamp | date:'HH:mm:ss'}}</small>
          <button type="button" 
                  class="btn-close" 
                  (click)="removeError(error)"></button>
        </div>
        <div class="toast-body text-white">
          {{error.message}}
        </div>
      </div>
    </div>
  `
})
export class ErrorDisplayComponent implements OnInit, OnDestroy {
  errors: ErrorMessage[] = [];
  private subscription?: Subscription;

  constructor(private errorService: ErrorService) {}

  ngOnInit() {
    this.subscription = this.errorService.errors$.subscribe(error => {
      this.errors.push(error);
      setTimeout(() => this.removeError(error), 5000);
    });
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }

  removeError(error: ErrorMessage) {
    const index = this.errors.indexOf(error);
    if (index > -1) {
      this.errors.splice(index, 1);
    }
  }

  getTitle(type: string): string {
    switch (type) {
      case 'error':
        return 'Error';
      case 'warning':
        return 'Warning';
      case 'info':
        return 'Information';
      default:
        return 'Notification';
    }
  }
}