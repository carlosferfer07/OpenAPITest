import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatIconModule } from "@angular/material/icon";
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { LoadingService } from './shared/services/loading-service';
import { CommonModule } from '@angular/common';
import { Toolbar } from './components/toolbar/toolbar';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MatProgressSpinner, CommonModule, Toolbar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {  
  public get loading$() { return this.loadingService.loading$; }
  constructor(private loadingService: LoadingService) {}
}
