import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlertService } from 'src/app/core/services/alert.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'cmp-alert',
  standalone: true,
  imports: [CommonModule],
  template: ``,
})
export class AlertComponent {
  private alertService = inject(AlertService);
  private toastr = inject(ToastrService);

  constructor() {
    this.alertService.alert.subscribe({
      next: (alert) => {
        this.toastr.error(alert.text, undefined, {
          positionClass: 'toast-bottom-margin',
        });
      },
    });
  }
}
