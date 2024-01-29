import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'cmp-active-button',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
  templateUrl: './active-button.component.html',
})
export class ActiveButtonComponent {
  @Input() color: 'primary' | 'accent' | 'warn' = 'primary';
  @Input() isLoading = false;
  @Input() text = '';
  @Input() disabled = false;
}
