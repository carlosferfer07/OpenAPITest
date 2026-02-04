export interface AiChartPlan {
  dataset: Dataset;
  type: ChartType;
  title: string;
  year?: number | null;
  from?: string | null;
  to?: string | null;
  employeeId?: number | null;
  labels: string[];
  datasets: { label: string; data: number[] }[];
}

export type Dataset = 'overtime_by_month' | 'hours_by_employee' | 'absences_by_week';
export type ChartType = 'bar' | 'line' | 'pie';