import { inject, Injectable, signal } from '@angular/core';
import { User } from '../models/user.model';
import { catchError, tap, throwError } from 'rxjs';
import { AuthApiClient } from './api/auth-api-client';
import { UsersApiClient } from './api/users-api-client';
import { LocalStorageService } from './local-storage.service';
import { CookieService } from './cookie.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private authApiClient: AuthApiClient = inject(AuthApiClient);
  private usersApiClient: UsersApiClient = inject(UsersApiClient);
  private localStorageService: LocalStorageService =
    inject(LocalStorageService);
  private cookieService = inject(CookieService);

  private readonly _user = signal<User | undefined | null>(undefined);
  public readonly user = this._user.asReadonly();

  login(email: string, password: string) {
    return this.authApiClient
      .login(email, password)
      .pipe(tap((user) => this.setUser(user)));
  }

  signUp(email: string, password: string, passwordConfirmation: string) {
    return this.authApiClient
      .signUp(email, password, passwordConfirmation)
      .pipe(tap((user) => this.setUser(user)));
  }

  logout() {
    return this.authApiClient.logout().pipe(
      tap((_) => {
        this.localStorageService.deleteUser();
        this._user.set(undefined);
      })
    );
  }

  refresh() {
    // check if we have a cookie. If not then we are not authenticated

    const cookie = this.cookieService.getAppCookie();

    if (!cookie) {
      this._user.set(undefined);
      return;
    }

    // If we have a cookie then we need to get a user data from localStorage
    const userFromLocalStorage = this.localStorageService.getUser();

    // If user exists in LocalStorage then we set it
    if (userFromLocalStorage) {
      this._user.set(userFromLocalStorage);
    }

    // Todo SET EVERYTHING TO NULL
    return this.usersApiClient.getCurrentUser().pipe(
      catchError((err: HttpErrorResponse, _) => {
        if (err.status === 401) {
          this._user.set(undefined);
          document.cookie = '';
          this.localStorageService.deleteUser();
        }

        return throwError(() => err);
      }),
      tap((user) => this.setUser(user))
    );
  }

  private setUser(user: User) {
    this._user.set(user);
    this.localStorageService.saveUser(user);
  }
}
