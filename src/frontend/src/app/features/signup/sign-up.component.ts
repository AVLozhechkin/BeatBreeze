import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import CustomFormValidators from '../../shared/utils/custom-form-validators';
import { ResponseStatus } from 'src/app/shared/models/response-status.enum';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'cmp-sign-up',
  templateUrl: './sign-up.component.html',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    RouterLink,
    MatProgressSpinnerModule,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SignUpComponent {
  private readonly authService: AuthService = inject(AuthService);
  private router = inject(Router);

  protected responseStatus = signal<ResponseStatus>('idle');

  protected signUpFormGroup: FormGroup = new FormGroup(
    {
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.pattern(
          /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.* )[A-Za-z\d\W].{8,32}$/
        ),
      ]),
      passwordConfirmation: new FormControl('', Validators.required),
    },
    {
      validators: [
        CustomFormValidators.match('password', 'passwordConfirmation'),
      ],
    }
  );

  submit() {
    if (this.signUpFormGroup.invalid) {
      return;
    }

    this.responseStatus.set('loading');

    const { email, password, passwordConfirmation } =
      this.signUpFormGroup.value;

    this.authService.signUp(email, password, passwordConfirmation).subscribe({
      next: (user) => {
        console.log('Logged in as a ' + user.email);
        this.responseStatus.set('success');
        this.router.navigateByUrl('providers');
      },
      error: (err) => {
        if (err.status === 400) {
          this.responseStatus.set('badRequest');
        } else {
          this.responseStatus.set('serverError');
        }
        console.log(err);
      },
    });
  }
}
