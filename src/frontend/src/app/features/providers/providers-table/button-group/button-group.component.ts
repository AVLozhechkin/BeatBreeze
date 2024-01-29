import {
  ChangeDetectionStrategy,
  Component,
  inject,
  Input,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { ProvidersService } from '../../providers.service';
import { DataProvider } from '../../../../core/models/data-provider.model';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { ActiveButtonComponent } from '../../../../shared/components/active-button/active-button.component';

@Component({
  selector: 'cmp-button-group',
  standalone: true,
  templateUrl: './button-group.component.html',
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    RouterLink,
    MatProgressSpinnerModule,
    ActiveButtonComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ButtonGroupComponent {
  protected providersService = inject(ProvidersService);
  protected isUpdating = signal(false);
  protected isRemoving = signal(false);

  @Input() provider!: DataProvider;

  updateProvider() {
    this.isUpdating.set(true);

    this.providersService.updateProvider(this.provider.id, false).subscribe({
      error: (err) => console.log(err),
      complete: () => this.isUpdating.set(false),
    });
  }

  removeProvider() {
    this.isRemoving.set(true);

    this.providersService.removeProvider(this.provider.id).subscribe({
      error: (err) => console.log(err),
      complete: () => this.isRemoving.set(false),
    });
  }
}
