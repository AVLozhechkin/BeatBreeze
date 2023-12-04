import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import {User} from "../../models/user.model";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class AuthApiClient {
  private authUrl = "http://localhost:5229/api/auth";

  constructor(private http: HttpClient) {
  }

  login(email: string, password: string) : Observable<User> {

    const body = {
      email, password
    }

    const url = this.authUrl + '/login';

    return this.http.post<User>(url, body);
  }

  signUp(email: string, password: string, passwordConfirmation: string) {
    const body = {
      email,
      password,
      passwordConfirmation,
    };

    const url = this.authUrl + '/create-user';

    return this.http.post<User>(url, body);
  }

  logout() {
    const url = this.authUrl + 'logout'
    return this.http.get(url);
  }
}
