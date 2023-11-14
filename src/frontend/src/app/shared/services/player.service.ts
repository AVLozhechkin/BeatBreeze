import { inject, Injectable, signal } from '@angular/core';
import { Howl } from 'howler';
import { Song } from '../models/song.model';
import { HistoriesService } from './histories.service';
import { QueueService } from './queue.service';
import { ProvidersApiClient } from './api/providers-api-client';
import { firstValueFrom } from 'rxjs';

export type RepeatType = 'repeat-1' | 'repeat-all' | 'repeat-none';
export type PlayState = 'idle' | 'playing' | 'paused' | 'unloaded';

@Injectable({
  providedIn: 'root',
})
export class PlayerService {
  private historiesService = inject(HistoriesService);
  private queueService = inject(QueueService);

  private providersApiClient = inject(ProvidersApiClient);

  private readonly _currentHowl = signal<Howl | undefined>(undefined);
  private readonly _locked = signal(false);
  private readonly _repeatType = signal<RepeatType>('repeat-none');
  private readonly _currentSong = signal<Song | undefined>(undefined);
  private readonly _seek = signal('0:00');
  private readonly _songDuration = signal('2:49');
  private readonly _songProgress = signal('0%');
  private readonly _playState = signal<PlayState>('idle');
  public readonly _muted = signal(false);
  public readonly _volume = signal(0.1);

  public readonly repeatType = this._repeatType.asReadonly();
  public readonly song = this._currentSong.asReadonly();
  public readonly playState = this._playState.asReadonly();
  public readonly seek = this._seek.asReadonly();
  public readonly songDuration = this._songDuration.asReadonly();
  public readonly songProgress = this._songProgress.asReadonly();
  public readonly isMuted = this._muted.asReadonly();
  public readonly volume = this._volume.asReadonly();

  constructor() {}

  public async playSongs(source: Song[]) {
    if (this._locked()) return;

    this._locked.set(true);

    this.queueService.setToQueue(source);

    const song = this.queueService.getSongByIndex(
      this.queueService.currentIndex()
    )!;

    await this.startPlaying(song);

    this._locked.set(false);
  }

  private async startPlaying(song: Song) {
    if (this._currentHowl() !== undefined) {
      this._currentHowl()!.unload();
      console.log('Unloading ' + this._currentSong()?.name);
      this._playState.set('idle');
    }

    const url = await firstValueFrom(
      this.providersApiClient.getSongUrl(song.id)
    );

    this._currentSong.set(song);
    const howl = new Howl({
      src: url,
      html5: true,
      volume: this.volume(),
    });
    console.log('Setting ' + song.name);
    this._currentHowl.set(howl);

    this.historiesService.addToHistory(song);
    console.log('play');
    this.play();

    howl.on('end', () => this.checkNext());
  }

  public play() {
    if (
      this._playState() === 'playing' ||
      this._playState() === 'unloaded' ||
      this._currentHowl()?.playing()
    )
      return;

    this._locked.set(true);

    this._playState.set('playing');
    this._currentHowl()?.play();
    this._currentHowl()?.on('play', () => {
      requestAnimationFrame(() => this.updateCurrentProgress());
    });

    this._locked.set(false);
  }

  public pause() {
    if (
      this._playState() === 'paused' ||
      this._playState() === 'unloaded' ||
      this._playState() === 'idle' ||
      !this._currentHowl()!.playing() ||
      this._locked()
    ) {
      return;
    }

    this._locked.set(true);

    this._currentHowl()?.pause();
    this._playState.set('paused');

    this._locked.set(false);
  }

  public async playNext() {
    if (this.queueService.queue().length === 1 || this._locked()) {
      return;
    }

    this._locked.set(true);

    const song = this.queueService.getNextSong();
    await this.startPlaying(song);

    this._locked.set(false);
  }

  public async playPrevious() {
    if (this.queueService.queue().length === 1 || this._locked()) {
      return;
    }

    this._locked.set(true);

    const song = this.queueService.getPreviousSong();

    await this.startPlaying(song);

    this._locked.set(false);
  }

  public setRepeatType(repeat: RepeatType) {
    this._repeatType.set(repeat);
  }

  public setSeek(width: number, clickX: number) {
    if (!this._currentHowl()) {
      return;
    }

    const percentage = clickX / width;
    const seconds = this._currentHowl()!.duration() * percentage;

    this._currentHowl()!.seek(seconds);
    this.updateCurrentProgress();
  }

  public setVolume(input: number) {
    console.log(input);
    this._volume.set(input);
    this._currentHowl.update((howl) => {
      howl?.volume(this._volume());
      return howl;
    });
  }

  public mute() {
    this._muted.set(true);
    this._currentHowl.update((howl) => {
      howl?.volume(0);
      return howl;
    });
  }

  public unmute() {
    this._muted.set(false);
    this._currentHowl.update((howl) => {
      howl?.volume(this.volume());
      return howl;
    });
  }

  private updateCurrentProgress() {
    if (this._currentHowl() === undefined) {
      return;
    }

    this._songDuration.set(
      PlayerService.convertSeekToTime(this._currentHowl()!.duration())
    );
    this._seek.set(
      PlayerService.convertSeekToTime(this._currentHowl()!.seek())
    );
    this._songProgress.set(
      `${
        (this._currentHowl()!.seek() / this._currentHowl()!.duration()) * 100
      }%`
    );

    if (this._currentHowl()?.playing() || this.playState() === 'playing') {
      requestAnimationFrame(() => this.updateCurrentProgress());
    }
  }

  private async checkNext() {
    this._playState.set('idle');
    console.log(
      this.repeatType() +
        ' left: ' +
        this.queueService.songsLeft() +
        ' index: ' +
        this.queueService.currentIndex()
    );
    if (this.repeatType() === 'repeat-1') {
      this.play();
      return;
    }

    if (
      this.repeatType() === 'repeat-none' &&
      this.queueService.songsLeft() === 1
    ) {
      return;
    }

    if (
      this.repeatType() === 'repeat-all' ||
      this.repeatType() === 'repeat-none'
    ) {
      const song = this.queueService.getNextSong();
      await this.startPlaying(song);
      return;
    }

    return;
  }

  private static convertSeekToTime(howlSeek: number) {
    const minutes = Math.floor(howlSeek / 60) || 0;

    const seconds = Math.floor(howlSeek - minutes * 60 || 0);

    return `${minutes}:${seconds < 10 ? 0 : ''}${seconds}`;
  }
}
