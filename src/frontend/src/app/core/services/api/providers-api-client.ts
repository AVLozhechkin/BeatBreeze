import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DataProvider } from '../../models/data-provider.model';
import { ProviderTypes } from '../../models/provider-types.model';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ProvidersApiClient {
  private providersUrl = `${environment.apiUrl}/providers`;

  constructor(private http: HttpClient) {}

  getProviders(includeFiles: boolean): Observable<DataProvider[]> {
    const url = `${this.providersUrl}?includeFiles=${includeFiles}`;
    return this.http.get<DataProvider[]>(url);
  }

  getProvider(
    providerId: string,
    includeFiles: boolean
  ): Observable<DataProvider> {
    const url = `${this.providersUrl}/${providerId}?includeFiles=${includeFiles}`;

    return this.http.get<DataProvider>(url);
  }

  updateProvider(
    providerId: string,
    includeFiles: boolean
  ): Observable<DataProvider> {
    const url = `${this.providersUrl}/${providerId}?includeFiles=${includeFiles}`;

    return this.http.patch<DataProvider>(url, null);
  }

  removeProvider(providerId: string): Observable<void> {
    const url = `${this.providersUrl}/${providerId}`;
    return this.http.delete<void>(url);
  }

  getSongUrl(songFileId: string, notCached: boolean = false) {
    const url = `${this.providersUrl}/songUrl/${songFileId}?notCached=${notCached}`;

    return this.http.get(url, { responseType: 'text' });
  }

  addProvider(providerType: ProviderTypes) {
    return window.open(
      `${this.providersUrl}/add-provider/${providerType}`,
      '_self'
    );
  }
}
