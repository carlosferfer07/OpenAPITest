import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AiChartPlan } from '../models/ai-chart-plan';
import { Observable } from 'rxjs';
import { AiChartRequest } from '../models/ai-chart-request';

@Injectable({
  providedIn: 'root',
})
export class ReportGenerator {
  private readonly baseUrl = '/api/ChartGeneration';

  constructor(private http: HttpClient) {}

  generateFromPrompt(prompt: string): Observable<AiChartPlan> {
    const body: AiChartRequest = { prompt };
    return this.http.post<AiChartPlan>(`${this.baseUrl}/generate-from-prompt`, body);
  }
}
