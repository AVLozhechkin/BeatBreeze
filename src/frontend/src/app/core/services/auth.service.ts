import {inject, Injectable, signal} from '@angular/core';
import {User} from '../models/user.model';
import {tap} from 'rxjs';
import {AuthApiClient} from './api/auth-api-client';
import {UsersApiClient} from './api/users-api-client';
import {LocalStorageService} from "./local-storage.service";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private authApiClient: AuthApiClient = inject(AuthApiClient);
  private usersApiClient: UsersApiClient = inject(UsersApiClient);
  private localStorageService: LocalStorageService = inject(LocalStorageService)

  private readonly _user = signal<User | undefined | null>(undefined);
  public readonly user = this._user.asReadonly();

  login(email: string, password: string) {
    return this.authApiClient
      .login(email, password)
      .pipe(
        tap(user => this.setUser(user))
      )
  }

  signUp(
    email: string,
    password: string,
    passwordConfirmation: string
  ) {
    return this.authApiClient
      .signUp(email, password, passwordConfirmation)
      .pipe(
        tap(user => this.setUser(user))
      )
  }

  logout() {
    this.localStorageService.deleteUser()
    this._user.set(undefined)
    this.authApiClient.logout()
  }

  refresh() {
    // check if we have a cookie. If not then we are not authenticated
    if (!document.cookie) {
      this._user.set(undefined);
      return;
    }

    // If we have a cookie then we need to get a yser data from localStorage
    const userFromLocalStorage = this.localStorageService.getUser();

    // If user exists in LocalStorage then we set it
    if (userFromLocalStorage) {
      this._user.set(userFromLocalStorage);
    }

    return this.usersApiClient.getCurrentUser()
      .pipe(
        tap(user => this.setUser(user))
      )
  }

  private setUser(user: User){
    this._user.set(user)
    this.localStorageService.saveUser(user)
  }
}
