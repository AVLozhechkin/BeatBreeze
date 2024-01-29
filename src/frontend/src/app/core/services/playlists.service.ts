import { inject, Injectable } from '@angular/core';
import { PlaylistsApiClient } from './api/playlists-api-client';
import { BehaviorSubject, tap } from 'rxjs';
import { Playlist } from '../models/playlist.model';

@Injectable({
  providedIn: 'root',
})
export class PlaylistsService {
  private playlistsApiClient = inject(PlaylistsApiClient);

  private _playlists = new BehaviorSubject<Playlist[]>([]);

  public playlists = this._playlists.asObservable();

  public getPlaylists(includeItems: boolean) {
    return this.playlistsApiClient
      .getUserPlaylists(includeItems)
      .pipe(tap((playlists) => this._playlists.next(playlists)));
  }

  public getPlaylist(playlistId: string, includeItems: boolean) {
    return this.playlistsApiClient.getPlaylist(playlistId, includeItems);
  }

  public createPlaylist(playlistName: string) {
    return this.playlistsApiClient.createPlaylist(playlistName).pipe(
      tap((playlist) => {
        const playlists = this._playlists.getValue();
        playlists?.push(playlist);
        this._playlists.next(playlists);
      })
    );
  }

  public deletePlaylist(playlistId: string) {
    return this.playlistsApiClient.deletePlaylist(playlistId).pipe(
      tap((_) => {
        const playlists = this._playlists.getValue();

        const updatedPlaylists = playlists?.filter((p) => p.id !== playlistId);

        this._playlists.next(updatedPlaylists);
      })
    );
  }

  public addSongToPlaylist(playlistId: string, songFileId: string) {
    return this.playlistsApiClient.addSong(playlistId, songFileId);
  }

  public removeSongFromPlaylist(playlistId: string, songFileId: string) {
    return this.playlistsApiClient.removeSong(playlistId, songFileId);
  }
}
