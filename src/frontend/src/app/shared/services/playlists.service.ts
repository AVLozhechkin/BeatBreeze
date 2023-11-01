import {computed, inject, Injectable, signal} from '@angular/core';
import {PlaylistsApiClient} from "./api/playlists-api-client";
import {Playlist} from "../models/playlist.model";

@Injectable({
  providedIn: 'root'
})
export class PlaylistsService {
  playlistsApiClient = inject(PlaylistsApiClient)

  private readonly _playlists = signal<Playlist[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _currentPlaylistId = signal<string | undefined>(undefined);

  public readonly playlists = computed(() => this._playlists())
  public readonly isLoading = computed(() => this._isLoading())
  public readonly currentPlaylist = computed(() => this._playlists().find(p => p.id === this._currentPlaylistId()))

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

  public createPlaylist() {
    this._isLoading.set(true);
    this.playlistsApiClient.createPlaylist("PlaylistModel name")
      .subscribe({
        next: (createdPlaylist: Playlist) => {
          this._playlists.mutate(playlists => {
            playlists.push(createdPlaylist)
          });

          this._isLoading.set(false);
        }
      })
  }

  public setCurrentPlaylist(playlistId: string) {
    this._currentPlaylistId.set(playlistId);
  }

  public addSongToPlaylist(playlistId: string, songFileId: string)
  {
    this.playlistsApiClient.addSong(playlistId, songFileId)
      .subscribe({
        next: updatedPlaylist => {
          console.log(updatedPlaylist)
          this._playlists.mutate(playlists => {
            const updatedIndex = playlists.findIndex(p => p.id === updatedPlaylist.id);

            if (updatedIndex < 0){
              return
            }

            playlists[updatedIndex] = updatedPlaylist;
          })
        }
      });
  }

  public removeSongFromPlaylist(songFileId: string) {
    this.playlistsApiClient.removeSong(this._currentPlaylistId()!, songFileId)
      .subscribe({
        next: updatedPlaylist => {
          console.log(updatedPlaylist)
          this._playlists.mutate(playlists => {
            const updatedIndex = playlists.findIndex(p => p.id === updatedPlaylist.id);

            if (updatedIndex < 0){
              return
            }

            playlists[updatedIndex] = updatedPlaylist;
          })
        }
      });
  }
}
