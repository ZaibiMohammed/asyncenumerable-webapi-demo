import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonitoringService, StreamingMetrics } from '../../services/monitoring.service';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-metrics-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card mt-4">
      <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Streaming Metrics Dashboard</h5>
        <span class="badge bg-light text-dark">Last Update: {{lastUpdate | date:'medium'}}</span>
      </div>
      <div class="card-body">
        <div class="row">
          <!-- Latest Metrics -->
          <div class="col-md-6">
            <h6>Latest Stream Performance</h6>
            <div class="table-responsive">
              <table class="table table-sm" *ngIf="latestMetrics">
                <tbody>
                  <tr>
                    <th>Items Streamed:</th>
                    <td>{{latestMetrics.itemsStreamed}}</td>
                  </tr>
                  <tr>
                    <th>Streaming Rate:</th>
                    <td>{{latestMetrics.itemsPerSecond | number:'1.1-1'}} items/second</td>
                  </tr>
                  <tr>
                    <th>Memory Used:</th>
                    <td>{{latestMetrics.memoryUsedBytes | number:'1.0-0'}} bytes</td>
                  </tr>
                  <tr>
                    <th>Average Batch Size:</th>
                    <td>{{latestMetrics.averageBatchSize | number:'1.0-1'}} items</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Historical Data -->
          <div class="col-md-6">
            <h6>Recent Streams</h6>
            <div class="table-responsive">
              <table class="table table-sm table-hover">
                <thead>
                  <tr>
                    <th>Time</th>
                    <th>Items</th>
                    <th>Rate</th>
                  </tr>
                </thead>
                <tbody>
                  <tr *ngFor="let metric of recentMetrics">
                    <td>{{metric.timestamp | date:'HH:mm:ss'}}</td>
                    <td>{{metric.itemsStreamed}}</td>
                    <td>{{metric.itemsPerSecond | number:'1.1-1'}}/s</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class MetricsDashboardComponent implements OnInit, OnDestroy {
  latestMetrics?: StreamingMetrics;
  recentMetrics: StreamingMetrics[] = [];
  lastUpdate: Date = new Date();
  private subscription?: Subscription;

  constructor(private monitoringService: MonitoringService) {}

  ngOnInit() {
    // Poll metrics every 2 seconds
    this.subscription = interval(2000)
      .pipe(
        switchMap(() => this.monitoringService.getMetrics())
      )
      .subscribe(metrics => {
        if (metrics.length > 0) {
          this.latestMetrics = metrics[metrics.length - 1];
          this.recentMetrics = metrics.slice(-5);
          this.lastUpdate = new Date();
        }
      });
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }
}