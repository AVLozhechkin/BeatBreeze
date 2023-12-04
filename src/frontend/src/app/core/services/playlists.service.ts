import { inject, Injectable, signal } from '@angular/core';
import { PlaylistsApiClient } from './api/playlists-api-client';
import {tap} from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class PlaylistsService {
  playlistsApiClient = inject(PlaylistsApiClient);

  private readonly _isLoading = signal<boolean>(false);
  public readonly isLoading = this._isLoading.asReadonly();

  public getPlaylists() {
    this._isLoading.set(true);

    return this.playlistsApiClient
      .getUserPlaylists()
      .pipe(
        tap(_ => this._isLoading.set(false))
      );
  }

  public getPlaylist(playlistId: string) {
    this._isLoading.set(true);

    return this.playlistsApiClient
      .getPlaylist(playlistId)
      .pipe(
        tap(_ => this._isLoading.set(false))
      );
  }

  public createPlaylist(playlistName: string) {
    this._isLoading.set(true);

    return this.playlistsApiClient
      .createPlaylist(playlistName)
      .pipe(
        tap(_ => this._isLoading.set(false))
      )
  }

  public addSongToPlaylist(playlistId: string, songFileId: string) {
    this._isLoading.set(true)
    return this.playlistsApiClient
      .addSong(playlistId, songFileId)
      .pipe(
        tap(_ => this._isLoading.set(false))
      );
  }

  public removeSongFromPlaylist(playlistId: string, songFileId: string) {
    this._isLoading.set(true)
    return this.playlistsApiClient
      .removeSong(playlistId, songFileId)
      .pipe(
        tap(_ => this._isLoading.set(false))
      )
  }
}
