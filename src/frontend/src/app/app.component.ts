import {Component, inject, OnInit} from '@angular/core';
import { PlayerComponent } from './feature-player/player.component';
import {AuthService} from "./shared/services/auth.service";
import {HeaderComponent} from "./feature-header/header.component";
import {RouterOutlet} from "@angular/router";
import {HistoriesService} from "./shared/services/histories.service";

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
    this.authService.refresh();
    this.historyService.getHistory();
  }
}
