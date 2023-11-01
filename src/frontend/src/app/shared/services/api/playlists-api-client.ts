import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {DataProvider} from "../../models/data-provider.model";
import {Playlist} from "../../models/playlist.model";

@Injectable({
  providedIn: 'root'
})
export class PlaylistsApiClient {
  private apiUrl = "http://localhost:5229/api/playlists";
  constructor(
    private http: HttpClient
  ) {
  }

  getUserPlaylists(): Observable<Playlist[]> {
    return this.http.get<Playlist[]>(this.apiUrl);
  }

  getPlaylist(playlistId: string): Observable<Playlist> {
    return this.http.get<Playlist>(this.apiUrl + "/" + playlistId);
  }

  createPlaylist(name: string): Observable<Playlist> {
    const body = {
      name
    }

    return this.http.post<Playlist>(this.apiUrl, body);
  }

  updatePlaylist(playlistId: string): Observable<Playlist> {
    const url = this.apiUrl + "/" + playlistId + "/update"
    return this.http.get<Playlist>(url)
  }

  deletePlaylist(playlistId: string): Observable<Object> {
    return this.http.delete('')
  }

  public addSong(playlistId: string, songFileId: string): Observable<Playlist>
  {
    const url = this.apiUrl + "/addSong"

    const body = {
      playlistId,
      songFileId
    }

    return this.http.post<Playlist>(url, body)
  }

  public removeSong(playlistId: string, songFileId: string): Observable<Playlist>
  {
    const url = this.apiUrl + "/removeSong"

    const body = {
      playlistId,
      songFileId
    }

    console.log(body)

    return this.http.post<Playlist>(url, body)
  }
}
