import { computed, inject, Injectable, signal } from '@angular/core';
import { ProvidersApiClient } from './api/providers-api-client';
import { DataProvider } from '../models/data-provider.model';
import { convertToTreeNode } from '../../feature-provider/utils';
import { ProviderTypes } from '../models/provider-types.model';

@Injectable({
  providedIn: 'root',
})
export class ProvidersService {
  private readonly providersApiClient = inject(ProvidersApiClient);

  private readonly _providers = signal<DataProvider[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _isInitialized = signal<boolean>(false);

  public readonly providers = computed(() => this._providers());
  public readonly isLoading = computed(() => this._isLoading());
  public readonly isInitialized = computed(() => this._isInitialized());

  constructor() {}

  public fetchProviders() {
    this._isLoading.set(true);
    this.providersApiClient.getProviders().subscribe({
      next: (fetchedProviders) => {
        this._providers.set(fetchedProviders);
        this._isLoading.set(false);
        this._isInitialized.set(true);
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

  public getProviderTree(providerId: string) {
    const providers = this._providers();
    const provider = providers.find((p) => p.id === providerId);
    if (!provider) {
      return [undefined];
    }

    return convertToTreeNode(provider.songFiles);
  }

  addProvider(providerType: ProviderTypes) {
    this.providersApiClient.addYandex();
  }

  removeProvider(providerId: string) {
    this.providersApiClient.removeProvider(providerId).subscribe({
      next: (_) => {
        this.fetchProviders();
      },
    });
  }
}
