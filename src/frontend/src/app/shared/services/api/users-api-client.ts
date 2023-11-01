import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {User} from "../../models/user.model";

@Injectable({
  providedIn: 'root'
})
export class UsersApiClient {
  private apiUrl = "http://localhost:5229/api/users";
  constructor(private http: HttpClient) {}

  public getCurrentUser()
  {
    return this.http.get<User>(this.apiUrl);
  }
}
