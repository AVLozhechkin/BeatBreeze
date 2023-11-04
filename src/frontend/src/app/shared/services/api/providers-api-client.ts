import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {DataProvider} from "../../models/data-provider.model";
import {ProviderTypes} from "../../models/provider-types.model";

@Injectable({
  providedIn: 'root'
})
export class ProvidersApiClient {
  private apiUrl = "http://localhost:5229/api/providers";
  constructor(
    private http: HttpClient
  ) {
  }

  getProviders(): Observable<DataProvider[]> {
    return this.http.get<DataProvider[]>(this.apiUrl);
  }

  getProvider(providerId: string): Observable<DataProvider> {
    return this.http.get<DataProvider>(this.apiUrl + "/" + providerId);
  }


  updateProvider(providerId: string): Observable<DataProvider> {
    const url = this.apiUrl + "/" + providerId
    return this.http.post<DataProvider>(url, null)
  }

  removeProvider(providerId: string): Observable<void> {
    const url = this.apiUrl + '/' + providerId
    return this.http.delete<void>(url)
  }

  getSongUrl(songFileId: string) {
    const url = this.apiUrl + '/songUrl/' + songFileId;

    return this.http.get(url, { responseType: 'text'});
  }

  addProvider(providerType: ProviderTypes)
  {
    window.open(this.apiUrl + '/add-provider/' + providerType, '_self')
  }
}
