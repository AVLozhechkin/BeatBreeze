import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonGroupComponent } from './button-group/button-group.component';
import { MatTableModule } from '@angular/material/table';
import { ProvidersService } from '../providers.service';

@Component({
  selector: 'cmp-providers-table',
  standalone: true,
  imports: [CommonModule, ButtonGroupComponent, MatTableModule],
  templateUrl: './providers-table.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProvidersTableComponent {
  protected providersService = inject(ProvidersService);
  protected displayedColumns = [
    'index',
    'name',
    'type',
    'connectedAt',
    'updatedAt',
    'buttons',
  ];
}
