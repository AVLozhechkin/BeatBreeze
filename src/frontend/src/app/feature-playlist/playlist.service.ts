import {computed, inject, Injectable, signal} from '@angular/core';
import {PlaylistsApiClient} from "../shared/services/api/playlists-api-client";
import {Playlist} from "../shared/models/playlist.model";

@Injectable({
  providedIn: 'root'
})
export class PlaylistService {
  playlistsApiClient = inject(PlaylistsApiClient)

  private readonly _playlist = signal<Playlist | undefined>(undefined);
  private readonly _isLoading = signal<boolean>(false);

  public readonly playlist = computed(() => this._playlist())
  public readonly isLoading = computed(() => this._isLoading())

  public fetchPlaylist(providerId: string) {
    this._isLoading.set(true);
    this.playlistsApiClient.getPlaylist(providerId)
      .subscribe({
        next: (playlist) =>  {
          this._playlist.set(playlist);
          this._isLoading.set(false);
        }
      })
  }

  public removeSongFromPlaylist(songFileId: string) {
    this.playlistsApiClient.removeSong(this.playlist()?.id!, songFileId)
      .subscribe({
        next: updatedPlaylist => {
          this._playlist.set(updatedPlaylist)
        }
      });
  }
}
