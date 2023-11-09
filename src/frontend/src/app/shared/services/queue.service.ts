import {computed, Injectable, signal} from "@angular/core";
import {Song} from "../models/song.model";

@Injectable({
  providedIn: 'root'
})
export class QueueService {
  private readonly _queue = signal<Song[]>([])
  private readonly _currentSongIndex = signal<number>(0)

  public readonly queue = this._queue.asReadonly();
  public readonly songsLeft = computed(() =>  this._queue().length - this._currentSongIndex())
  public readonly currentIndex = this._currentSongIndex.asReadonly()

  public getNextSong() {
    const currentIndex = this._currentSongIndex();

    if (currentIndex === this._queue().length - 1) {
      this._currentSongIndex.set(0)
    } else {
      this._currentSongIndex.set(currentIndex + 1)
    }

    return this._queue()[this._currentSongIndex()];
  }

  public getPreviousSong() {
    let currentIndex = this._currentSongIndex();

    const queue = this.queue()

    if (currentIndex === 0) {
      this._currentSongIndex.set(queue.length - 1)
      currentIndex = queue.length - 1
    } else {
      this._currentSongIndex.set(currentIndex - 1)
      currentIndex = currentIndex - 1
    }

    return queue[currentIndex]
  }

  public getSongByIndex(index: number) {
    if (index < 0 || index > this._queue().length)
    {
      return
    }

    const song = this._queue()[index];
    this._currentSongIndex.set(index)

    return song
  }

  public addToQueue(songs: Song[], index: number = 0) {
    if (songs)
    {
      this._queue.set(songs)
      this._currentSongIndex.set(index)
    }
  }
}
