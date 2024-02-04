import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CookieService {
  private readonly AppCookieName = 'CloudTunes';

  getAppCookie() {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${this.AppCookieName}=`);

    if (parts.length === 2) {
      return parts.pop()!.split(';').shift();
    }

    return undefined;
  }
}
