import { computed, inject, Injectable, signal } from '@angular/core';
import { ProvidersApiClient } from '../shared/services/api/providers-api-client';
import {DataProvider} from "../shared/models/data-provider.model";
import {ProviderTypes} from "../shared/models/provider-types.model";


@Injectable({
  providedIn: 'root',
})
export class ProvidersService {
  private readonly providersApiClient = inject(ProvidersApiClient);

  private readonly _providers = signal<DataProvider[]>([]);
  private readonly _isLoading = signal<boolean>(false);

  public readonly providers = computed(() => this._providers());
  public readonly isLoading = computed(() => this._isLoading());

  constructor() {}

  public fetchProviders() {
    this._isLoading.set(true);
    this.providersApiClient.getProviders().subscribe({
      next: (fetchedProviders) => {
        this._providers.set(fetchedProviders);
        this._isLoading.set(false);
      },
    });
  }

  public updateProvider(providerId: string) {
    this._isLoading.set(true);
    this.providersApiClient.updateProvider(providerId).subscribe({
      next: (provider) => {
        this._providers.mutate((providers) => {
          const index = providers.findIndex((p) => p.id === provider.id);

          if (index < 0) {
            // refresh?
            return;
          }

          providers[index] = provider;
        });
        this._isLoading.set(false);
      },
    });
  }

  addProvider(providerType: ProviderTypes) {
    this.providersApiClient.addProvider(providerType);
  }

  removeProvider(providerId: string) {
    this.providersApiClient.removeProvider(providerId).subscribe({
      next: (_) => {
        this.fetchProviders();
      },
    });
  }
}
