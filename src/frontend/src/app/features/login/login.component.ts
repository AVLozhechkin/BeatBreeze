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
import { AuthService } from 'src/app/core/services/auth.service';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterModule } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ResponseStatus } from 'src/app/shared/models/response-status.enum';

@Component({
  selector: 'cmp-login',
  templateUrl: './login.component.html',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    RouterModule,
    MatProgressSpinnerModule,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  protected responseStatus = signal<ResponseStatus>('idle');

  protected loginFormGroup: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.pattern(
        /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.* )[A-Za-z\d\W].{8,32}$/
      ),
    ]),
  });

  submit() {
    if (this.loginFormGroup.invalid) {
      return;
    }

    this.responseStatus.set('loading');

    const { email, password } = this.loginFormGroup.value;

    this.authService.login(email, password).subscribe({
      next: (user) => {
        console.log('Logged in as a ' + user.email);
        this.responseStatus.set('success');
        this.router.navigate(['providers']);
      },
      error: (err) => {
        if (err.status === 401) {
          this.responseStatus.set('unauthorized');
        } else {
          this.responseStatus.set('serverError');
        }
        console.log(err);
      },
    });
  }
}
