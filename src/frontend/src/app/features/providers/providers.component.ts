import {Component, inject, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {MatTableModule} from "@angular/material/table";
import {MatButtonModule} from "@angular/material/button";
import {RouterLink} from "@angular/router";
import {MatProgressBarModule} from "@angular/material/progress-bar";
import {ProviderTypes} from "../../core/models/provider-types.model";
import {ProvidersService} from "./providers.service";
import {ButtonGroupComponent} from "./providers-table/button-group/button-group.component";
import {ContentState} from "../../shared/models/content-state.enum";
import {ProvidersTableComponent} from "./providers-table/providers-table.component";

@Component({
  selector: 'cmp-providers',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, RouterLink, MatProgressBarModule, ButtonGroupComponent, ProvidersTableComponent],
  templateUrl: './providers.component.html'
})
export class ProvidersComponent implements OnInit{
  protected providersService = inject(ProvidersService)
  protected contentState: ContentState = "notInitialized";

  constructor() {}

  ngOnInit(): void {
    this.contentState = "loading"

    this.providersService
      .fetchProviders()
      .subscribe({
        next: (fetchedProviders) => {
          console.log('Fetched ' + fetchedProviders.length + ' providers')
          this.contentState = "initialized"
        },
        error: err => {
          console.log(err)
          this.contentState = "error"
        }
      });
  }

  async addProvider(providerType: ProviderTypes) {
    await this.providersService.addProvider(providerType)
  }
}
