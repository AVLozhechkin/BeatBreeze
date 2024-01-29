import { inject, Injectable, signal } from '@angular/core';
import { ProvidersApiClient } from '../../core/services/api/providers-api-client';
import { DataProvider } from '../../core/models/data-provider.model';
import { ProviderTypes } from '../../core/models/provider-types.model';
import { BehaviorSubject, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProvidersService {
  private readonly providersApiClient = inject(ProvidersApiClient);

  private readonly _isLoading = signal<boolean>(false);
  private readonly _providers = new BehaviorSubject<DataProvider[]>([]);

  public readonly isLoading = this._isLoading.asReadonly();
  public readonly providers = this._providers.asObservable();

  constructor() {}

  public fetchProviders(includeFiles: boolean) {
    this._isLoading.set(true);
    return this.providersApiClient.getProviders(includeFiles).pipe(
      tap((providers) => {
        this._providers.next(providers);
        this._isLoading.set(false);
      })
    );
  }

  public updateProvider(providerId: string, includeFiles: boolean) {
    this._isLoading.set(true);
    return this.providersApiClient
      .updateProvider(providerId, includeFiles)
      .pipe(
        tap((provider) => {
          const providers = this._providers.value;

          const index = providers.findIndex((p) => p.id === provider.id);

          if (index < 0) {
            // refresh?
            console.log(
              'Something went wrong: No data provider in providers list after update'
            );
            return;
          }

          providers[index] = provider;

          this._providers.next(providers);

          this._isLoading.set(false);
        })
      );
  }

  async addProvider(providerType: ProviderTypes) {
    this.providersApiClient.addProvider(providerType);
  }

  removeProvider(providerId: string) {
    this._isLoading.set(true);
    return this.providersApiClient.removeProvider(providerId).pipe(
      tap((_) => {
        const providers = this._providers.value;

        const index = providers.findIndex((p) => p.id === providerId);

        if (index < 0) {
          // refresh?
          console.log(
            'Something went wrong: No data provider in providers list after remove'
          );
          return;
        }

        providers.splice(index, 1);

        this._providers.next(providers);

        this._isLoading.set(false);
      })
    );
  }
}
