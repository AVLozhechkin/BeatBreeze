import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import {DataProvider} from "../../models/data-provider.model";
import {ProviderTypes} from "../../models/provider-types.model";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class ProvidersApiClient  {
  private providersUrl = "http://localhost:5229/api/providers";

  constructor(private http: HttpClient) {}

  getProviders(): Observable<DataProvider[]> {
    return this.http.get<DataProvider[]>(this.providersUrl)
  }

  getProvider(providerId: string): Observable<DataProvider> {
    return this.http.get<DataProvider>(this.providersUrl + "/" + providerId);
  }


  updateProvider(providerId: string): Observable<DataProvider> {
    const url = this.providersUrl + "/" + providerId
    return this.http.post<DataProvider>(url, null)
  }

  removeProvider(providerId: string): Observable<void> {
    const url = this.providersUrl + '/' + providerId
    return this.http.delete<void>(url)
  }

  getSongUrl(songFileId: string) {
    const url = this.providersUrl + '/songUrl/' + songFileId;
    // TODO Make shareReplay caching based on expiresIn
    return this.http.get(url, { responseType: "text"});
  }

  addProvider(providerType: ProviderTypes)
  {
    return window.open(this.providersUrl + '/add-provider/' + providerType, '_self')
  }
}
