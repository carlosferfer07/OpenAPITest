import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ChartResponse } from '../models/chart-response';
import { Observable } from 'rxjs';
import { AiChartRequest } from '../models/ai-chart-request';

@Injectable({
  providedIn: 'root',
})
export class ReportGenerator {
  private readonly baseUrl = '/api/ChartGeneration';

  constructor(private http: HttpClient) {}

  generateFromPrompt(prompt: string): Observable<ChartResponse> {
    const body: AiChartRequest = { prompt };
    return this.http.post<ChartResponse>(`${this.baseUrl}/generate-from-prompt`, body);
  }
}
