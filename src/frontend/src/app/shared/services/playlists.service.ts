import { inject, Injectable, signal } from '@angular/core';
import { PlaylistsApiClient } from './api/playlists-api-client';
import {Playlist} from "../models/playlist.model";

@Injectable({
  providedIn: 'root'
})
export class PlaylistsService {
  playlistsApiClient = inject(PlaylistsApiClient)

  private readonly _playlists = signal<Playlist[] | undefined>(undefined);
  private readonly _isLoading = signal<boolean>(false);

  public readonly playlists = this._playlists.asReadonly()
  public readonly isLoading = this._isLoading.asReadonly()

  public fetchPlaylists() {
    this._isLoading.set(true);
    this.playlistsApiClient.getUserPlaylists()
      .subscribe({
        next: (playlists) =>  {
          this._playlists.set(playlists);
          this._isLoading.set(false);
        }
      })
  }

  public createPlaylist(playlistName: string) {
    this._isLoading.set(true);
    this.playlistsApiClient.createPlaylist(playlistName)
      .subscribe({
        next: (createdPlaylist: Playlist) => {
          this._playlists.mutate(playlists => {
            if (playlists)
              playlists.push(createdPlaylist)
            playlists = [createdPlaylist]
          });

          this._isLoading.set(false);
        }
      })
  }

  addSongToPlaylist(playlistId: string, songFileId: string) {
    this.playlistsApiClient.addSong(playlistId, songFileId)
      .subscribe()
  }
}
