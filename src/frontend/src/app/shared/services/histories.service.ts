import {computed, inject, Injectable, signal} from '@angular/core';
import {HistoriesApiClient} from "./api/histories-api-client.service";
import {History} from "../models/history.model";
import {Song} from "../models/song.model";

@Injectable({
  providedIn: 'root'
})
export class HistoriesService {
  historiesApiClient = inject(HistoriesApiClient)

  private readonly _history = signal<History | undefined>(undefined);
  private readonly _isLoading = signal<boolean>(false)

  public readonly history = computed(() => this._history())
  public readonly isLoading = computed(() => this._isLoading())

  public getHistory() {
    this._isLoading.set(true)
    this.historiesApiClient.getUserHistory()
      .subscribe({
        next: history => {
          this._history.set(history)
          this._isLoading.set(false)
        }
      })
  }

  public addToHistory(song: Song)
  {
    this._isLoading.set(true)
    this.historiesApiClient.addToHistory(song.id)
      .subscribe({
        next: history => {
          this._history.set(history)
          this._isLoading.set(false)
        }
      })
  }
}
