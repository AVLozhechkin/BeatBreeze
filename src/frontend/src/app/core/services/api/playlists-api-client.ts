import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Playlist } from '../../models/playlist.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PlaylistsApiClient {
  private playlistsUrl = `${environment.apiUrl}/playlists`;

  constructor(private http: HttpClient) {}

  getUserPlaylists(includeItems: boolean): Observable<Playlist[]> {
    const url = `${this.playlistsUrl}?includeItems=${includeItems}`;

    return this.http.get<Playlist[]>(url);
  }

  getPlaylist(playlistId: string, includeItems: boolean): Observable<Playlist> {
    const url = `${this.playlistsUrl}/${playlistId}?includeItems=${includeItems}`;

    return this.http.get<Playlist>(url);
  }

  createPlaylist(name: string): Observable<Playlist> {
    const body = {
      name,
    };

    return this.http.post<Playlist>(this.playlistsUrl, body);
  }

  deletePlaylist(playlistId: string): Observable<Object> {
    return this.http.delete<object>(this.playlistsUrl + '/' + playlistId);
  }

  public addSong(
    playlistId: string,
    musicFileId: string
  ): Observable<Playlist> {
    const url = `${this.playlistsUrl}/${playlistId}/add`;

    const body = {
      musicFileId,
    };

    return this.http.post<Playlist>(url, body);
  }

  public removeSong(
    playlistId: string,
    musicFileId: string
  ): Observable<Playlist> {
    const url = `${this.playlistsUrl}/${playlistId}/remove`;

    const body = {
      musicFileId,
    };

    console.log(body);

    return this.http.post<Playlist>(url, body);
  }
}
