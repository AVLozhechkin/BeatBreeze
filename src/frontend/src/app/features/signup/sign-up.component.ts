import {Component, inject} from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import {AuthService} from "../../core/services/auth.service";
import {MatButtonModule} from "@angular/material/button";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {Router} from "@angular/router";

@Component({
  selector: 'cmp-sign-up',
  templateUrl: './sign-up.component.html',
  standalone: true,
    imports: [ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule],
})
export class SignUpComponent {
  private readonly authService: AuthService = inject(AuthService);
  private router = inject(Router);

  protected signUpFormGroup: FormGroup = new FormGroup({
    email: new FormControl(),
    password: new FormControl(),
    passwordConfirmation: new FormControl(),
  });

  submit() {
    const email = this.signUpFormGroup.value.email;
    const password = this.signUpFormGroup.value.password;
    const passwordConfirmation = this.signUpFormGroup.value.passwordConfirmation;
    this.authService.signUp(email, password, passwordConfirmation)
      .subscribe({
        next: user=> {
          console.log('Logged in as ' + user.email)
          this.router.navigateByUrl('home');
        },
        error: err => console.log(err)
      })
  }
}
