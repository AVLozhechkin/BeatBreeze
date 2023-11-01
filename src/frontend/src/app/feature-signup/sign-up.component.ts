import {Component, inject} from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import {AuthService} from "../shared/services/auth.service";
import {MatButtonModule} from "@angular/material/button";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";

@Component({
  selector: 'cmp-sign-up',
  templateUrl: './sign-up.component.html',
  standalone: true,
    imports: [ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule],
})
export class SignUpComponent {
  private readonly authService: AuthService = inject(AuthService);

  protected signUpFormGroup: FormGroup = new FormGroup({
    email: new FormControl(),
    password: new FormControl(),
    passwordConfirmation: new FormControl(),
  });

  submit() {
    console.log(this.signUpFormGroup.value);
    const email = this.signUpFormGroup.value.email;
    const name = this.signUpFormGroup.value.email;
    const password = this.signUpFormGroup.value.password;
    const passwordConfirmation = this.signUpFormGroup.value.passwordConfirmation;
    this.authService.signUp(email, name, password, passwordConfirmation)
  }
}
