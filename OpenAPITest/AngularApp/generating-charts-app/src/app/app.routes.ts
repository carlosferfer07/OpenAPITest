import { Routes } from '@angular/router';
import { LandingPage } from './core/pages/landing-page/landing-page';

export const routes: Routes = [
     { path: '', component: LandingPage },
     { path: 'report-generator', loadChildren: () => import('./report-generator/report-generator-module').then(m => m.ReportGeneratorModule) }
];
