import {Component, inject} from '@angular/core';
import {RouterModule} from "@angular/router";
import {AuthService} from "../../core/services/auth.service";
import {CommonModule} from "@angular/common";
import {MatButtonModule} from "@angular/material/button";

@Component({
  selector: 'cmp-header',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule
  ],
  templateUrl: './header.component.html'
})
export class HeaderComponent {
  protected authService = inject(AuthService)

  signOut() {
    this.authService.logout()
  }
}
