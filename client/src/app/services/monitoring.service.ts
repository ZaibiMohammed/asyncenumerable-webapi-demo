import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface StreamingMetrics {
  endpointName: string;
  itemsStreamed: number;
  batchesStreamed: number;
  averageBatchSize: number;
  totalStreamingTime: string;
  itemsPerSecond: number;
  memoryUsedBytes: number;
  timestamp: string;
}

@Injectable({
  providedIn: 'root'
})
export class MonitoringService {
  private apiUrl = 'https://localhost:7001/api/monitoring';

  constructor(private http: HttpClient) {}

  getMetrics(): Observable<StreamingMetrics[]> {
    return this.http.get<StreamingMetrics[]>(`${this.apiUrl}/metrics`);
  }
}