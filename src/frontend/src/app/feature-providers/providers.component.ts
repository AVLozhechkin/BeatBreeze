import {Component, inject, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {MatTableModule} from "@angular/material/table";
import {MatButtonModule} from "@angular/material/button";
import {RouterLink} from "@angular/router";
import {MatProgressBarModule} from "@angular/material/progress-bar";
import {ProviderTypes} from "../shared/models/provider-types.model";
import {ProvidersService} from "./providers.service";

@Component({
  selector: 'cmp-providers',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, RouterLink, MatProgressBarModule],
  templateUrl: './providers.component.html'
})
export class ProvidersComponent implements OnInit{
  protected providersService = inject(ProvidersService)
  protected displayedColumns = ["index", "name", "connectedAt", "updatedAt", "buttons"]

  constructor() {}

  ngOnInit(): void {
    this.providersService.fetchProviders()
  }

  updateProvider(providerId: string) {
    this.providersService.updateProvider(providerId)
  }

  removeProvider(providerId: string) {
    this.providersService.removeProvider(providerId)
  }

  addProvider(providerType: ProviderTypes) {
    this.providersService.addProvider(providerType)
  }
}
