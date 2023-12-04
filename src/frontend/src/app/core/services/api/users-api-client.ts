import {Injectable} from "@angular/core";
import {User} from "../../models/user.model";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class UsersApiClient {
  private usersUrl = "http://localhost:5229/api/users";

  constructor(private http: HttpClient) {}
  public getCurrentUser()
  {
    return this.http.get<User>(this.usersUrl)
  }
}
