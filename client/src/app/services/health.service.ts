import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, timer } from 'rxjs';
import { map, catchError, switchMap } from 'rxjs/operators';

export interface HealthStatus {
  status: string;
  components: {
    [key: string]: {
      status: string;
      description: string;
      data?: { [key: string]: any };
    };
  };
}

@Injectable({
  providedIn: 'root'
})
export class HealthService {
  private apiUrl = 'https://localhost:7001';

  constructor(private http: HttpClient) {}

  checkHealth(): Observable<HealthStatus> {
    return this.http.get<HealthStatus>(`${this.apiUrl}/health`)
      .pipe(
        catchError(error => {
          console.error('Health check failed:', error);
          throw error;
        })
      );
  }

  getHealthStream(): Observable<HealthStatus> {
    return timer(0, 5000).pipe(
      switchMap(() => this.checkHealth())
    );
  }
}