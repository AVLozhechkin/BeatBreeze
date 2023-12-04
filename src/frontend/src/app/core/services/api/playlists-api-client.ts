import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {Playlist} from "../../models/playlist.model";

@Injectable({
  providedIn: 'root'
})
export class PlaylistsApiClient {

  private playlistsUrl = "http://localhost:5229/api/playlists";

  constructor(private http: HttpClient) {}

  getUserPlaylists(): Observable<Playlist[]> {
    return this.http.get<Playlist[]>(this.playlistsUrl);
  }

  getPlaylist(playlistId: string): Observable<Playlist> {
    return this.http.get<Playlist>(this.playlistsUrl + "/" + playlistId);
  }

  createPlaylist(name: string): Observable<Playlist> {
    const body = {
      name
    }

    return this.http.post<Playlist>(this.playlistsUrl, body)
  }

  deletePlaylist(playlistId: string): Observable<Object> {
    return this.http.delete<object>(this.playlistsUrl );
  }

  public addSong(playlistId: string, songFileId: string): Observable<Playlist>
  {
    const url = this.playlistsUrl + "/addSong"

    const body = {
      playlistId,
      songFileId
    }

    return this.http.post<Playlist>(url, body);
  }

  public removeSong(playlistId: string, songFileId: string): Observable<Playlist>
  {
    const url = this.playlistsUrl + "/removeSong"

    const body = {
      playlistId,
      songFileId
    }

    return this.http.post<Playlist>(url, body);
  }
}
