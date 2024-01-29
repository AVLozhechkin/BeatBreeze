import { inject, Injectable } from '@angular/core';
import { ProvidersApiClient } from '../../core/services/api/providers-api-client';
import { DataProvider } from '../../core/models/data-provider.model';
import { BehaviorSubject, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProviderService {
  private readonly providersApiClient = inject(ProvidersApiClient);

  private _provider = new BehaviorSubject<DataProvider | undefined>(undefined);

  constructor() {}

  public fetchProvider(providerId: string, includeFiles: boolean) {
    return this.providersApiClient.getProvider(providerId, includeFiles).pipe(
      tap((provider) => {
        console.log(provider);
        this._provider.next(provider);
      })
    );
  }

  public updateProvider(providerId: string, includeFiles: boolean) {
    return this.providersApiClient
      .updateProvider(providerId, includeFiles)
      .pipe(
        tap((provider) => {
          this._provider.next(provider);
        })
      );
  }

  public removeProvider() {
    return this.providersApiClient
      .removeProvider(this._provider.value!.id)
      .pipe(
        tap(() => {
          this._provider.next(undefined);
        })
      );
  }
}
