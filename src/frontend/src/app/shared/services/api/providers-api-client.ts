import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {DataProvider} from "../../models/data-provider.model";

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
    return this.http.get<DataProvider[]>(this.apiUrl + "/data-providers");
  }

  updateProvider(providerId: string): Observable<DataProvider> {
    const url = this.apiUrl + "/" + providerId + '/update'
    return this.http.get<DataProvider>(url)
  }

  removeProvider(providerId: string): Observable<void> {
    const url = this.apiUrl + '/' + providerId
    return this.http.delete<void>(url)
  }

  addYandex() {
    window.open(this.apiUrl + '/add-yandex-provider', '_self')
  }
}
