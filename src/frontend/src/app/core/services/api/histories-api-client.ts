import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import {History} from "../../models/history.model";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class HistoriesApiClient {
  private historiesUrl = "http://localhost:5229/api/histories";

  constructor(private http: HttpClient) {}
  getUserHistory() : Observable<History> {

    return this.http.get<History>(this.historiesUrl);
  }

  addToHistory(songFileId: string): Observable<History>
  {
    const body = {
      songFileId
    }

    return this.http.post<History>(this.historiesUrl + '/add', body);
  }
}
