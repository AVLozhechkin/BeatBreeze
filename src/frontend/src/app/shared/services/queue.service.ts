import {computed, Injectable, signal} from "@angular/core";
import {Song} from "../models/song.model";
import {Playlist} from "../models/playlist.model";
import {DataProvider} from "../models/data-provider.model";

@Injectable({
  providedIn: 'root'
})
export class QueueService {
  private readonly _queue = signal<Song[]>([])
  private readonly _currentSongIndex = signal<number>(0)

  public readonly queue = computed(() => this._queue())
  public readonly songsLeft = computed(() =>  this._queue().length - this._currentSongIndex())

  public getNextSong() {
    const currentIndex = this._currentSongIndex();
    const song = this._queue()[currentIndex];

    if (currentIndex === this._queue().length - 1) {
      this._currentSongIndex.set(0)
    } else {
      this._currentSongIndex.set(currentIndex + 1)
    }

    return song
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

  public addToQueue(source: DataProvider | Playlist) {
    if (source.songFiles)
    {
      this._queue.set(source.songFiles)
      this._currentSongIndex.set(0)
    }
  }

  public addSongToQueue(source: Song) {
    this._queue.set([source])
    this._currentSongIndex.set(0)
  }
}
