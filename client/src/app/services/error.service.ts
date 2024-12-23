import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ErrorMessage {
  message: string;
  type: 'error' | 'warning' | 'info';
  timestamp: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorService {
  private errorSubject = new Subject<ErrorMessage>();
  errors$ = this.errorSubject.asObservable();

  showError(message: string, type: 'error' | 'warning' | 'info' = 'error') {
    this.errorSubject.next({
      message,
      type,
      timestamp: new Date()
    });
  }
}