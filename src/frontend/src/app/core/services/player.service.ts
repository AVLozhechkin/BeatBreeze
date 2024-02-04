import { computed, inject, Injectable, OnDestroy, signal } from '@angular/core';
import { MusicFile } from '../models/musicFile.model';
import { ProvidersApiClient } from './api/providers-api-client';
import { firstValueFrom } from 'rxjs';
import { AlertService } from './alert.service';

export type RepeatType = 'repeat-one' | 'repeat-all' | 'repeat-none';
export type PlayState = 'playing' | 'paused' | 'unloaded' | 'loading';

@Injectable({
  providedIn: 'root',
})
export class PlayerService2 implements OnDestroy {
  private providersApiClient = inject(ProvidersApiClient);
  private alertService = inject(AlertService);

  private audio = new Audio();

  private _currentSongIndex = signal(0);

  private _sourcePlaylist = signal<MusicFile[]>([]);
  private _currentPlaylist = signal<MusicFile[]>([]);
  public currentPlaylist = this._currentPlaylist.asReadonly();

  public currentSong = computed(() => {
    return this._currentPlaylist()[this._currentSongIndex()];
  });
  public songProgress = computed(() => {
    return `${(this.currentTime() / this.duration()) * 100}%`;
  });

  private _currentTime = signal(0);
  public currentTime = this._currentTime.asReadonly();

  private _duration = signal(213);
  public duration = this._duration.asReadonly();

  private _playState = signal<PlayState>('unloaded');
  public playState = this._playState.asReadonly();

  private _repeatType = signal<RepeatType>('repeat-one');
  public repeatType = this._repeatType.asReadonly();

  private _isMuted = signal(false);
  public isMuted = this._isMuted.asReadonly();

  private _volume = signal(0.05);
  public volume = this._volume.asReadonly();

  private _isShuffled = signal(false);
  public isShuffled = this._isShuffled.asReadonly();

  constructor() {
    this.audio.volume = this.volume();

    this.audio.addEventListener('ended', () => this.handleSongEnd());
    this.audio.addEventListener('timeupdate', () => this.handleTimeUpdate());
    this.audio.addEventListener('loadedmetadata', () =>
      this.handleLoadedMetadata()
    );
    this.audio.addEventListener('play', () => this.handlePlay());
    this.audio.addEventListener('pause', () => this.handlePause());
    this.audio.addEventListener('error', () => this.handleError());
  }

  ngOnDestroy(): void {
    this.audio.removeEventListener('ended', () => this.handleSongEnd());
    this.audio.removeEventListener('timeupdate', () => this.handleTimeUpdate());
    this.audio.removeEventListener('loadedmetadata', () =>
      this.handleLoadedMetadata()
    );
    this.audio.removeEventListener('play', () => this.handlePlay());
    this.audio.removeEventListener('pause', () => this.handlePause());
    this.audio.removeEventListener('error', () => this.handleError());
  }

  async setSongs(files: MusicFile[], index: number = 0) {
    if (index < 0 || files.length <= index) {
      throw new Error(
        'Index cant be less than zero or more than playlist size'
      );
    }
    this._sourcePlaylist.set(files);
    this._currentPlaylist.set(files);
    this._currentSongIndex.set(index);

    this.audio.pause();

    await this.setSongLink(files[index]);

    this.play();
  }

  addSongs(musicFiles: MusicFile[]) {
    this._sourcePlaylist.update((playlist) => {
      return playlist.concat(musicFiles);
    });

    this._currentPlaylist.update((playlist) => {
      return playlist.concat(musicFiles);
    });
  }

  removeSong(index: number) {
    const source = this._sourcePlaylist();
    const current = this._currentPlaylist();

    const song = current[index];

    const indexInSource = source.findIndex((s) => s.id === song.id);
    source.splice(indexInSource, 1);
    current.splice(index, 1);

    this._sourcePlaylist.set(source);
    this._currentPlaylist.set(current);
  }

  play() {
    const playState = this.playState();

    if (playState === 'playing' || playState === 'unloaded') {
      return;
    }

    return this.audio.play();
  }

  pause() {
    const playState = this.playState();

    if (playState == 'paused' || playState == 'unloaded') {
      return;
    }

    this.audio.pause();
  }

  setSeek(seconds: number) {
    if (this.playState() === 'unloaded') {
      return;
    }

    this._currentTime.set(seconds);
    this.audio.currentTime = seconds;
  }

  setVolume(volume: number) {
    if (volume < 0 || volume > 1) {
      return;
    }

    this._volume.set(volume);
    this.audio.volume = volume;
  }

  setMute(isMuted: boolean) {
    this._isMuted.set(isMuted);
    this.audio.muted = isMuted;
  }

  setRepeatType(type: RepeatType) {
    this._repeatType.set(type);
  }

  public shuffle() {
    this._isShuffled.set(true);
    const currentSong = this.currentPlaylist()[this._currentSongIndex()];
    this._currentPlaylist.set(
      this._sourcePlaylist()
        .map((value) => ({ value, sort: Math.random() }))
        .sort((a, b) => a.sort - b.sort)
        .map(({ value }) => value)
    );
    this._currentSongIndex.set(
      this.currentPlaylist().findIndex((s) => s.id === currentSong.id)
    );
  }

  public unshuffle() {
    this._isShuffled.set(false);

    const currentSong = this.currentPlaylist()[this._currentSongIndex()];

    this._currentPlaylist.set(this._sourcePlaylist());

    this._currentSongIndex.set(
      this.currentPlaylist().findIndex((s) => s.id === currentSong.id)
    );
  }

  async playNext() {
    this.audio.pause();
    this._currentTime.set(0);

    let currentIndex = this._currentSongIndex();
    const queue = this._currentPlaylist();

    if (currentIndex === queue.length - 1) {
      this._currentSongIndex.set(0);
      currentIndex = 0;
    } else {
      this._currentSongIndex.set(currentIndex + 1);
      currentIndex++;
    }

    await this.setSongLink(queue[currentIndex]);
    this.audio.play();
  }

  async playPrevious() {
    this.audio.pause();
    this._currentTime.set(0);

    let currentIndex = this._currentSongIndex();

    const queue = this._currentPlaylist();

    if (currentIndex === 0) {
      this._currentSongIndex.set(queue.length - 1);
      currentIndex = queue.length - 1;
    } else {
      this._currentSongIndex.set(currentIndex - 1);
      currentIndex = currentIndex - 1;
    }

    await this.setSongLink(queue[currentIndex]);

    this.audio.play();
  }

  // event handlers

  async handleError() {
    if (this.audio.error!.code === 4) {
      this.setSongLink(this.currentSong(), true);

      this.play();
    } else {
      this.alertService.showError(410);
    }
  }

  handlePlay() {
    this._playState.set('playing');
  }

  handlePause() {
    this._playState.set('paused');
  }

  handleTimeUpdate(): any {
    this._currentTime.set(this.audio.currentTime);
  }

  handleLoadedMetadata(): any {
    this._duration.set(this.audio.duration);
  }

  async handleSongEnd() {
    const repeatType = this.repeatType();

    console.log(repeatType);

    const songs = this._currentPlaylist();

    if (
      repeatType === 'repeat-one' ||
      (repeatType === 'repeat-all' && songs.length === 1)
    ) {
      this._currentTime.set(0);
      this.audio.currentTime = 0;
      this.play();
      return;
    }

    const currentIndex = this._currentSongIndex();

    // when current song was last song + repeat all = play first song
    if (currentIndex === songs.length - 1 && repeatType === 'repeat-all') {
      this._currentSongIndex.set(0);

      await this.setSongLink(songs[0]);

      this.play();

      return;
    }

    // when current song was last song + repeat none = reset and pause
    if (currentIndex === songs.length - 1 && repeatType === 'repeat-none') {
      this._playState.set('paused');
      this._currentTime.set(0);
      this.audio.currentTime = 0;

      return;
    }

    // when current song was not last = set next song and play
    this._currentSongIndex.update((i) => i + 1);

    await this.setSongLink(songs[this._currentSongIndex()]);

    this.play();
  }

  private async setSongLink(musicFile: MusicFile, notCached: boolean = false) {
    this._playState.set('loading');

    const link = await firstValueFrom(
      this.providersApiClient.getSongUrl(musicFile.id, notCached)
    );

    this.audio.src = link;
  }
}
