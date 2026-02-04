import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private readonly _loading$ = new BehaviorSubject<boolean>(false);
  readonly loading$: Observable<boolean> = this._loading$.asObservable();

  show(): void { this._loading$.next(true); }
  hide(): void { this._loading$.next(false); }
  set(value: boolean): void { this._loading$.next(value); }
}
