import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {User} from "../../models/user.model";

@Injectable({
  providedIn: 'root'
})
export class AuthApiClient {
  private apiUrl = "http://localhost:5229/api/auth/";
  constructor(private http: HttpClient) {}

  login(email: string, password: string) : Observable<User> {

    const body = {
      email, password
    }

    const url = this.apiUrl + 'login';

    return this.http.post<User>(url, body);
  }

  signUp(email: string, name: string, password: string, passwordConfirmation: string) {
    const body = {
      email,
      name,
      password,
      passwordConfirmation,
    };

    const url = this.apiUrl + 'create-user';

    return this.http.post<User>(url, body);
  }

  logout() {
    const url = this.apiUrl + 'logout'
    return this.http.get(url);
  }
}
