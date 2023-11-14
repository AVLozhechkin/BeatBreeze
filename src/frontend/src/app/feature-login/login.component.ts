import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from 'src/app/shared/services/auth.service';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';
import {ActionResultPanelComponent} from "../shared/components/action-result-panel/action-result-panel.component";

@Component({
  selector: 'cmp-login',
  templateUrl: './login.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, MatInputModule, MatButtonModule, ActionResultPanelComponent],
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  protected loginFormGroup: FormGroup = new FormGroup({
    email: new FormControl(),
    password: new FormControl(),
  });

  async submit() {
    const email = this.loginFormGroup.value.email;
    const password = this.loginFormGroup.value.password;
    await this.authService.login(email, password);
    await this.router.navigateByUrl('home');
  }
}
