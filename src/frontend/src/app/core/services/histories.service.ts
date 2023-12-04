import {inject, Injectable, signal} from '@angular/core';
import {HistoriesApiClient} from "./api/histories-api-client";
import {History} from "../models/history.model";
import {Song} from "../models/song.model";
import {tap} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class HistoriesService {
  historiesApiClient = inject(HistoriesApiClient)

  private readonly _history = signal<History | undefined>(undefined);
  private readonly _isLoading = signal<boolean>(false)

  public readonly history = this._history.asReadonly()
  public readonly isLoading = this._isLoading.asReadonly()

  public fetchHistory() {
    this._isLoading.set(true)
    return this.historiesApiClient
      .getUserHistory()
      .pipe(
        tap(history => {
          this._history.set(history)
          this._isLoading.set(false)
        })
      )
  }

  public addToHistory(song: Song)
  {
    this._isLoading.set(true)
    return this.historiesApiClient
      .addToHistory(song.id)
      .pipe(
        tap(history => {
          this._history.set(history)
          this._isLoading.set(false)
        })
      )
  }
}
