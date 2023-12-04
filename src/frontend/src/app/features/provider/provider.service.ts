import { computed, inject, Injectable, signal } from '@angular/core';
import {ProvidersApiClient} from "../../core/services/api/providers-api-client";
import {DataProvider} from "../../core/models/data-provider.model";
import {convertToTreeNode} from "./utils";
import {tap} from "rxjs";

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
    return this.providersApiClient
      .getProvider(providerId)
      .pipe(
        tap(fetchedProvider => {
          this._provider.set(fetchedProvider);
          this._isLoading.set(false);
        })
      )
  }

  public updateProvider(providerId: string) {
    this._isLoading.set(true);
    return this.providersApiClient
      .updateProvider(providerId)
      .pipe(
        tap(updatedProvider => {
          this._provider.set(updatedProvider);
          this._isLoading.set(false);
        })
      );
  }

  public getProviderTree(providerId: string) {
    if (this.provider())
    {
      return convertToTreeNode(this.provider()?.songFiles!);
    }
    return undefined;
  }
}
