import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ReportGenerator } from '../../services/report-generator';
import { AiChartPlan } from '../../models/ai-chart-plan';
import type { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../shared/services/loading-service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-the-chart-generator',
  standalone: true,
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatIconModule, MatButtonModule, BaseChartDirective, CommonModule],
  templateUrl: './the-chart-generator.html',
  styleUrl: './the-chart-generator.css'  
})
export class TheChartGenerator {
 
  prompt = 'Show overtime by month for 2025 as a bar chart';
  loading = false;
  error = '';

  title = '';
  chartType: ChartType = 'bar';
  chartData?: ChartConfiguration['data'];
  chartOptions: ChartConfiguration['options'];

  constructor(private reportService: ReportGenerator
    , private loadingService: LoadingService
  ) {}

  run(): void {
    this.error = '';
    const textRequest = (this.prompt || '').trim();    
    if (!textRequest) {
      this.error = 'Prompt is required.';
      return;
    }

    this.loadingService.show();
    this.reportService.generateFromPrompt(textRequest)
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (chart) => {         
          this.title = chart.title;
          this.chartType = chart.type;
          this.chartData = { labels: chart.labels, datasets: chart.datasets };
        },
        error: (err) => {
          this.error = err?.error?.message ?? err?.error ?? err?.message ?? 'Request failed';
        }
      });
  }
}
