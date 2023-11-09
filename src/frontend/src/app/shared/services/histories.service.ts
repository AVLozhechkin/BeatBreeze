import {computed, inject, Injectable, signal} from '@angular/core';
import {HistoriesApiClient} from "./api/histories-api-client.service";
import {History} from "../models/history.model";
import {Song} from "../models/song.model";
import {lastValueFrom} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class HistoriesService {
  historiesApiClient = inject(HistoriesApiClient)

  private readonly _history = signal<History | undefined>(undefined);
  private readonly _isLoading = signal<boolean>(false)

  public readonly history = this._history.asReadonly()
  public readonly isLoading = this._isLoading.asReadonly()

  public async fetchHistory() {
    this._isLoading.set(true)
    const history = await lastValueFrom(this.historiesApiClient.getUserHistory())
    this._history.set(history)
    this._isLoading.set(false)
  }

  public async addToHistory(song: Song)
  {
    this._isLoading.set(true)
    const history = await lastValueFrom(this.historiesApiClient.addToHistory(song.id))
    this._history.set(history)
    this._isLoading.set(false)
  }
}
