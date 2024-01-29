import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ActiveButtonComponent } from '../../../shared/components/active-button/active-button.component';
import { PlaylistsService } from 'src/app/core/services/playlists.service';
import {
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ResponseStatus } from 'src/app/shared/models/response-status.enum';

@Component({
  selector: 'ct-create-playlist-dialog',
  standalone: true,
  templateUrl: './create-playlist-dialog.component.html',
  imports: [
    CommonModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    FormsModule,
    ActiveButtonComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreatePlaylistDialogComponent {
  protected isCreating = false;
  protected responseStatus: ResponseStatus = 'idle';

  private playlistsService = inject(PlaylistsService);
  playlistNameControl = new FormControl('', [
    Validators.required,
    Validators.minLength(1),
  ]);

  constructor(public dialogRef: MatDialogRef<CreatePlaylistDialogComponent>) {}

  create() {
    console.log(this.playlistNameControl);

    if (this.playlistNameControl.invalid) {
      return;
    }

    this.isCreating = true;

    const name = this.playlistNameControl.value!;
    this.playlistsService.createPlaylist(name).subscribe({
      next: (_) => {
        this.isCreating = false;
        this.responseStatus = 'success';
        console.log('Created playlist: ' + name);
        this.dialogRef.close();
      },
      error: (err) => {
        console.log(err);
        if (err.status === 400) {
          this.responseStatus = 'badRequest';
        } else {
          this.responseStatus = 'serverError';
        }
        this.isCreating = false;
        console.log(this.responseStatus);
      },
    });
  }
}
