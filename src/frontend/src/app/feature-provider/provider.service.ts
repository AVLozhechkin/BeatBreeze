import { computed, inject, Injectable, signal } from '@angular/core';
import {ProvidersApiClient} from "../shared/services/api/providers-api-client";
import {DataProvider} from "../shared/models/data-provider.model";
import {convertToTreeNode} from "./utils";

@Injectable({
  providedIn: 'root',
})
export class ProviderService {
  private readonly providersApiClient = inject(ProvidersApiClient);

  private readonly _provider = signal<DataProvider | undefined>(undefined);
  private readonly _isLoading = signal<boolean>(false);

  public readonly provider = computed(() => this._provider());
  public readonly isLoading = computed(() => this._isLoading());

  constructor() {}

  public fetchProvider(providerId: string) {
    this._isLoading.set(true);
    this.providersApiClient.getProvider(providerId).subscribe({
      next: (fetchedProvider) => {
        this._provider.set(fetchedProvider);
        this._isLoading.set(false);
      },
    });
  }

  public updateProvider(providerId: string) {
    this._isLoading.set(true);
    this.providersApiClient.updateProvider(providerId)
      .subscribe({
        next: (provider) => {
          this._provider.set(provider);
            this._isLoading.set(false);
        },
    });
  }

  public getProviderTree(providerId: string) {
    if (this.provider())
    {
      return convertToTreeNode(this.provider()?.songFiles!);
    }
    return undefined;
  }
}
