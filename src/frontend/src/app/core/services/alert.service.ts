import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

interface Error {
  color: string;
  code: ErrorCode;
}

type ErrorCode = 400 | 410 | 500;

export interface Alert {
  type: 'success' | 'info' | 'warn' | 'error';
  text: string;
}

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  private _alert = new Subject<Alert>();

  public alert = this._alert.asObservable();

  showError(code: ErrorCode) {
    this._alert.next({
      type: 'error',
      text: code.toString(),
    });
  }
  //showError(text: string) {
  //  this._alert.next({
  //    type: 'error',
  //    text: text,
  //  });
  //}
}
