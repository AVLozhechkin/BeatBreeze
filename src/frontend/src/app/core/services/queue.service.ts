import { computed, Injectable, signal } from '@angular/core';
import { Song } from '../models/song.model';

@Injectable({
  providedIn: 'root',
})
export class QueueService {
  private readonly _sourceQueue = signal<Song[]>([]);
  private readonly _currentQueue = signal<Song[]>([]);
  private readonly _currentSongIndex = signal<number>(0);
  private readonly _isShuffled = signal(false);

  public readonly queue = this._currentQueue.asReadonly();
  public readonly songsLeft = computed(
    () => this._currentQueue().length - this._currentSongIndex()
  );
  public readonly currentIndex = this._currentSongIndex.asReadonly();
  public readonly isShuffled = this._isShuffled.asReadonly();

  public getNextSong() {
    const currentIndex = this._currentSongIndex();

    if (currentIndex === this._currentQueue().length - 1) {
      this._currentSongIndex.set(0);
    } else {
      this._currentSongIndex.set(currentIndex + 1);
    }

    return this._currentQueue()[this._currentSongIndex()];
  }

  public getPreviousSong() {
    let currentIndex = this._currentSongIndex();

    const queue = this.queue();

    if (currentIndex === 0) {
      this._currentSongIndex.set(queue.length - 1);
      currentIndex = queue.length - 1;
    } else {
      this._currentSongIndex.set(currentIndex - 1);
      currentIndex = currentIndex - 1;
    }

    return queue[currentIndex];
  }

  public getSongByIndex(index: number) {
    if (index < 0 || index > this._currentQueue().length) {
      return;
    }

    const song = this._currentQueue()[index];
    this._currentSongIndex.set(index);

    return song;
  }

  public setToQueue(songs: Song[], index: number = 0) {
    if (songs) {
      this._sourceQueue.set(songs);
      this._currentQueue.set(songs);
      this._currentSongIndex.set(index);
    }
  }

  public addToQueue(songs: Song[]) {
    if (songs) {
      this._sourceQueue.update((queue) => {
        queue.push(...songs);
        return queue;
      });
      this._currentQueue.set(this._sourceQueue());

      if (this._isShuffled()) {
        this.shuffle();
      }
    }
  }

  public shuffle() {
    this._isShuffled.set(true);
    const currentSong = this._currentQueue()[this._currentSongIndex()];
    this._currentQueue.set(
      this._sourceQueue()
        .map((value) => ({ value, sort: Math.random() }))
        .sort((a, b) => a.sort - b.sort)
        .map(({ value }) => value)
    );
    this._currentSongIndex.set(
      this._currentQueue().findIndex((s) => s.id === currentSong.id)
    );
  }

  public unshuffle() {
    this._isShuffled.set(false);
    this._currentQueue.set(this._sourceQueue());
  }
}
