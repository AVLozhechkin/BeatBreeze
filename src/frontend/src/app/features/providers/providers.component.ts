import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ProviderTypes } from '../../core/models/provider-types.model';
import { ProvidersService } from './providers.service';
import { ButtonGroupComponent } from './providers-table/button-group/button-group.component';
import { ContentState } from '../../shared/models/content-state.enum';
import { ProvidersTableComponent } from './providers-table/providers-table.component';

@Component({
  selector: 'ct-providers',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    RouterLink,
    MatProgressBarModule,
    ButtonGroupComponent,
    ProvidersTableComponent,
  ],
  templateUrl: './providers.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProvidersComponent implements OnInit {
  protected providersService = inject(ProvidersService);
  protected contentState = signal<ContentState>('notInitialized');

  constructor() {}

  ngOnInit(): void {
    this.contentState.set('loading');
    this.providersService.fetchProviders(false).subscribe({
      error: (err) => {
        console.log(err);
        this.contentState.set('error');
      },
      complete: () => {
        this.contentState.set('initialized');
      },
    });
  }

  async addProvider(providerType: ProviderTypes) {
    await this.providersService.addProvider(providerType);
  }
}
