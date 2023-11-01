import {computed, inject, Injectable, signal} from '@angular/core';
import {User} from '../models/user.model';
import { Observable, throwError} from 'rxjs';
import {AuthApiClient} from "./api/auth-api-client";
import {UsersApiClient} from "./api/users-api-client";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private authApiClient: AuthApiClient = inject(AuthApiClient)
  private usersApiClient: UsersApiClient = inject(UsersApiClient)

  private userLocalStorageKey: string = "CloudMusicPlayer_User"

  private readonly _user = signal<User | undefined>(undefined)
  private readonly _isAuthenticated = signal<boolean>(false)

  public readonly user = computed(() => this._user());
  public readonly isAuthenticated = computed(() => this._isAuthenticated())

  login(email: string, password: string) {
    this.authApiClient.login(email, password)
      .subscribe({
        next: (user: User) => {
          this._user.set(user);
          this._isAuthenticated.set(true);
          this.saveUserIntoLocalStorage(user)
        }
      })
  }

  signUp(email: string, name: string, password: string, passwordConfirmation: string) {
    this.authApiClient.signUp(email, name, password, passwordConfirmation)
      .subscribe({
        next: (user: User) => {
          this._user.set(user);
          this._isAuthenticated.set(true);
          this.saveUserIntoLocalStorage(user)
        }
      })
  }

  logout() {
    this.authApiClient.logout()
      .subscribe({
        next: () => {
          this._user.set(undefined);
          this._isAuthenticated.set(false);
          this.removeUserFromLocalStorage()
        }
      })
  }

  handleError(name: string, url: string, body: any): Observable<never> {
    console.log(
      `An error occured in ${name} function when requesting to ${url} with the next body: ${body}`
    );

    return throwError(
      () => new Error('Something bad happened; please try again later.')
    );
  }

  refresh() {
    if (!document.cookie)
    {
      this._user.set(undefined);
      return;
    }

    const userFromLocalStorage = this.loadUserFromLocalStorage()

    if (userFromLocalStorage) {
      this._user.set(userFromLocalStorage);
      this._isAuthenticated.set(true);
    }

    this.usersApiClient.getCurrentUser()
      .subscribe({
        next: (user: User) => {
          this._user.set(user);
          this._isAuthenticated.set(true);
          this.saveUserIntoLocalStorage(user)
        }
      })
  }

  private saveUserIntoLocalStorage(user: User) {
    window.localStorage.setItem(this.userLocalStorageKey, JSON.stringify(user));
  }

  private removeUserFromLocalStorage() {
    window.localStorage.removeItem(this.userLocalStorageKey)
  }

  private loadUserFromLocalStorage(): User | null {
    let json = window.localStorage.getItem(this.userLocalStorageKey)

    if (json === null)
    {
      return null;
    }

    return JSON.parse(json)
  }
}
