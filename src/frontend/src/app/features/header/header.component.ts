import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { PlayerService2 } from 'src/app/core/services/player.service';

@Component({
  selector: 'ct-header',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule],
  templateUrl: './header.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  protected authService = inject(AuthService);
  protected playerService = inject(PlayerService2);
  protected router = inject(Router);

  signOut() {
    this.authService.logout().subscribe({
      complete: () => {
        this.router.navigate(['login']);
      },
    });
  }
}
