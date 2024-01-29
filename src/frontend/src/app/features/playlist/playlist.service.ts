import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { Playlist } from 'src/app/core/models/playlist.model';
import { PlaylistsApiClient } from 'src/app/core/services/api/playlists-api-client';

@Injectable({
  providedIn: 'root',
})
export class PlaylistService {
  private playlistsApiClient = inject(PlaylistsApiClient);

  private _playlist = new BehaviorSubject<Playlist | undefined>(undefined);
  public playlist = this._playlist.asObservable();

  public getPlaylist(playlistId: string, includeItems: boolean) {
    return this.playlistsApiClient
      .getPlaylist(playlistId, includeItems)
      .pipe(tap((playlist) => this._playlist.next(playlist)));
  }

  public deletePlaylist(playlistId: string) {
    return this.playlistsApiClient
      .deletePlaylist(playlistId)
      .pipe(tap(() => this._playlist.next(undefined)));
  }

  public removeSongFromPlaylist(songFileId: string) {
    const id = this._playlist.getValue()?.id;

    return this.playlistsApiClient
      .removeSong(id!, songFileId)
      .pipe(tap((playlist) => this._playlist.next(playlist)));
  }
}
