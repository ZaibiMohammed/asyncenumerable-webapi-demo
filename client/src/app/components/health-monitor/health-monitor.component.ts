import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HealthService, HealthStatus } from '../../services/health.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-health-monitor',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card mt-4">
      <div class="card-header bg-info text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">System Health Status</h5>
        <span class="badge bg-light text-dark">Last Check: {{lastCheck | date:'medium'}}</span>
      </div>
      <div class="card-body">
        <div *ngIf="healthStatus" class="row">
          <div class="col-12">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <h6 class="mb-0">Overall Status</h6>
              <span [class]="getStatusBadgeClass(healthStatus.status)">
                {{healthStatus.status}}
              </span>
            </div>

            <div *ngFor="let component of getComponents()" class="mb-3">
              <div class="d-flex justify-content-between align-items-center">
                <span>{{component.name}}</span>
                <span [class]="getStatusBadgeClass(component.status)">
                  {{component.status}}
                </span>
              </div>
              <small class="text-muted d-block">{{component.description}}</small>
              
              <div *ngIf="component.data" class="mt-2 small">
                <div *ngFor="let item of getDataItems(component.data)" class="d-flex">
                  <span class="text-muted me-2">{{item.key}}:</span>
                  <span>{{item.value | number:'1.0-2'}}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div *ngIf="error" class="alert alert-danger">
          {{error}}
        </div>

        <div *ngIf="!healthStatus && !error" class="text-center p-3">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>
      </div>
    </div>
  `
})
export class HealthMonitorComponent implements OnInit, OnDestroy {
  healthStatus: HealthStatus | null = null;
  error: string | null = null;
  lastCheck = new Date();
  private subscription?: Subscription;

  constructor(private healthService: HealthService) {}

  ngOnInit() {
    this.subscription = this.healthService.getHealthStream().subscribe({
      next: (status) => {
        this.healthStatus = status;
        this.error = null;
        this.lastCheck = new Date();
      },
      error: (error) => {
        this.error = 'Failed to fetch health status';
        console.error('Health check error:', error);
      }
    });
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }

  getStatusBadgeClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'healthy':
        return 'badge bg-success';
      case 'degraded':
        return 'badge bg-warning text-dark';
      case 'unhealthy':
        return 'badge bg-danger';
      default:
        return 'badge bg-secondary';
    }
  }

  getComponents() {
    if (!this.healthStatus?.components) return [];
    
    return Object.entries(this.healthStatus.components).map(([name, data]) => ({
      name,
      status: data.status,
      description: data.description,
      data: data.data
    }));
  }

  getDataItems(data: any) {
    return Object.entries(data).map(([key, value]) => ({
      key: key.replace(/([A-Z])/g, ' $1').trim(),
      value
    }));
  }
}