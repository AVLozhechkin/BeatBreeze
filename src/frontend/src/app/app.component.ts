import {Component, inject, OnInit} from '@angular/core';
import { PlayerComponent } from './features/player/player.component';
import {AuthService} from "./core/services/auth.service";
import {HeaderComponent} from "./features/header/header.component";
import {RouterOutlet} from "@angular/router";
import {HistoriesService} from "./core/services/histories.service";

@Component({
  standalone: true,
  imports: [PlayerComponent, HeaderComponent, RouterOutlet],
  selector: 'cmp-root',
  template: `
    <cmp-header></cmp-header>
    <div class="pb-36">
      <router-outlet></router-outlet>
    </div>
    <cmp-player></cmp-player>
  `
})
export class AppComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly historyService = inject(HistoriesService)


  ngOnInit() {
    this.authService.refresh()
      ?.subscribe();
    if (this.authService.user())
      this.historyService.fetchHistory()
        .subscribe({
          next: _ => console.log('Fetched history'),
          error: err => console.log(err)
        });
  }
}
