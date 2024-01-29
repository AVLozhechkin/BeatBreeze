import { Injectable } from '@angular/core';
import { User } from '../../models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UsersApiClient {
  private usersUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}
  public getCurrentUser() {
    return this.http.get<User>(this.usersUrl);
  }
}
