import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TheChartGenerator } from './pages/the-chart-generator/the-chart-generator';

const routes: Routes = [
  { path: '', component: TheChartGenerator}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportGeneratorRoutingModule { }
