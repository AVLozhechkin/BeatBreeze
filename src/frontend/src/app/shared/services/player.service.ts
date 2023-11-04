import {computed, inject, Injectable, signal, WritableSignal} from '@angular/core';
import { Howl } from 'howler';
import { Song } from '../models/song.model';
import {HistoriesService} from "./histories.service";
import {QueueService} from "./queue.service";
import {DataProvider} from "../models/data-provider.model";
import {Playlist} from "../models/playlist.model";
import {ProvidersApiClient} from "./api/providers-api-client";
import {firstValueFrom, lastValueFrom} from "rxjs";

export type RepeatType = 'repeat-1' | 'repeat-all' | 'repeat-none'
export type PlayState = 'idle' | 'playing' | 'paused'
@Injectable({
  providedIn: 'root',
})
export class PlayerService {
  private historiesService = inject(HistoriesService)
  private queueService = inject(QueueService)
  private providersApiClient = inject(ProvidersApiClient)

  private currentHowl: Howl | undefined;

  private readonly _repeatType = signal<RepeatType>('repeat-none');
  private readonly _currentSong: WritableSignal<Song | undefined> = signal(undefined)
  private readonly _volume = signal(0.1)
  private readonly _seek= signal('0:00')
  private readonly _songDuration = signal('2:49')
  private readonly _songProgress = signal('0%')
  private readonly _playState = signal<PlayState>('idle')

  public readonly repeatType = computed(() => this._repeatType())
  public readonly song = computed(() => this._currentSong())
  public readonly volume = computed(() => this._volume())
  public readonly playState = computed(() => this._playState())
  public readonly seek = computed(() => this._seek())
  public readonly songDuration = computed(() => this._songDuration())
  public readonly songProgress = computed(() => this._songProgress())

  constructor() {}

  public playSongs(source: DataProvider | Playlist) {
    this.queueService.addToQueue(source)

    const song = this.queueService.getNextSong()

    this.startPlaying(song)
  }

  public playSong(source: Song) {
    this.queueService.addSongToQueue(source)

    const song = this.queueService.getNextSong()

    this.startPlaying(song)
  }

  private async startPlaying(song: Song)
  {
    if (this.currentHowl !== undefined) {
      this.currentHowl.unload()
    }

    const url = await firstValueFrom(this.providersApiClient.getSongUrl(song.id))
    console.log(url)

    this._currentSong.set(song)
    this.currentHowl = new Howl({
      src: url,
      html5: true,
      volume: 0.1,
    });
    this.historiesService.addToHistory(song)
    this.play()
  }

  public play() {
    this._playState.set('playing')
    this.currentHowl?.play();
    this.currentHowl?.on('play', () => {
      requestAnimationFrame(() => this.updateCurrentProgress());
    });

    this.currentHowl?.once('end', () => this.checkNext())
  }

  public pause() {
    if (this.currentHowl === undefined || !this.currentHowl.playing()) {
      return;
    }
    this.currentHowl?.pause()
    this._playState.set('paused');
  }

  public playNext() {
    if (this.queueService.queue().length === 0) {
      return
    }

    const song = this.queueService.getNextSong()

    this.startPlaying(song)
  }

  public playPrevious() {
    if (this.queueService.queue().length === 0) {
      return
    }

    const song = this.queueService.getPreviousSong()

    this.startPlaying(song)
  }

  public setSeek(width: number, clickX: number)
  {
    if (!this.currentHowl)
    {
      return
    }

    const percentage = clickX / width
    const seconds = this.currentHowl.duration() * percentage

    this.currentHowl.seek(seconds)
    this.updateCurrentProgress()
  }

  public updateCurrentProgress() {
    if (this.currentHowl === undefined) {
      return;
    }

    this._songDuration.set(PlayerService.convertSeekToTime(this.currentHowl.duration()))
    this._seek.set(PlayerService.convertSeekToTime(this.currentHowl.seek()));
    this._songProgress.set(`${(this.currentHowl.seek() / this.currentHowl.duration()) * 100}%`)

    if (this.currentHowl?.playing() || this.playState() === 'playing') {
      requestAnimationFrame(() => this.updateCurrentProgress());
    }
  }

  public checkNext() {
    this._playState.set('idle')
    console.log(this.repeatType() + ' ' + this.queueService.songsLeft())
    if (this.repeatType() === 'repeat-1') {
      this.play()
      return;
    }

    if (this.repeatType() === 'repeat-none' && this.queueService.songsLeft() === 0)
    {
      return;
    }

    if (this.repeatType() === 'repeat-all' || this.repeatType() === 'repeat-none') {
      const song = this.queueService.getNextSong();
      this.startPlaying(song);
      return;
    }

    return;
  }


  static convertSeekToTime(howlSeek: number) {
    const minutes = Math.floor(howlSeek / 60) || 0;

    const seconds = Math.floor(howlSeek - minutes * 60 || 0);

    return `${minutes}:${seconds < 10 ? 0 : ''}${seconds}`;
  }

  public setRepeatType(repeat: RepeatType) {
    console.log(repeat)
    this._repeatType.set(repeat)
  }
}
