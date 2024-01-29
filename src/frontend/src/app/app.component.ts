import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
} from '@angular/core';
import { PlayerComponent } from './features/player/player.component';
import { AuthService } from './core/services/auth.service';
import { HeaderComponent } from './features/header/header.component';
import { RouterOutlet } from '@angular/router';
import { AlertComponent } from './shared/components/alert/alert.component';

@Component({
  standalone: true,
  selector: 'cmp-root',
  template: `
    <cmp-header></cmp-header>
    <div class="pb-36">
      <router-outlet></router-outlet>
    </div>
    <cmp-alert></cmp-alert>
    <cmp-player></cmp-player>
  `,
  imports: [PlayerComponent, HeaderComponent, RouterOutlet, AlertComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent implements OnInit {
  private readonly authService = inject(AuthService);

  ngOnInit() {
    this.authService.refresh()?.subscribe();
  }
}
