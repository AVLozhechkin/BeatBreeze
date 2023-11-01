import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {History} from "../../models/history.model";

@Injectable({
  providedIn: 'root'
})
export class HistoriesApiClient {
  private apiUrl = "http://localhost:5229/api/histories";
  constructor(private http: HttpClient) {}

  getUserHistory() : Observable<History> {
    return this.http.get<History>(this.apiUrl);
  }

  addToHistory(songFileId: string): Observable<History>
  {
    const body = {
      songFileId
    }

    return this.http.post<History>(this.apiUrl + '/add', body);
  }
}
