// Mirrors ChartResponse on the API: everything Chart.js needs to draw the chart.
export interface ChartResponse {
  type: ChartType;
  title: string;
  labels: string[];
  datasets: { label: string; data: number[] }[];
}

export type ChartType = 'bar' | 'line' | 'pie';
