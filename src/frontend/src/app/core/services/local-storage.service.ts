import { Injectable } from '@angular/core';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class LocalStorageService {
  public static readonly userLocalStorageKey: string = 'BeatBreeze_User';

  public getUser(): User | null {
    const json = window.localStorage.getItem(
      LocalStorageService.userLocalStorageKey
    );

    if (json === null) {
      return null;
    }

    return JSON.parse(json);
  }

  public saveUser(user: User) {
    window.localStorage.setItem(
      LocalStorageService.userLocalStorageKey,
      JSON.stringify(user)
    );
  }

  public deleteUser() {
    window.localStorage.removeItem(LocalStorageService.userLocalStorageKey);
  }
}
