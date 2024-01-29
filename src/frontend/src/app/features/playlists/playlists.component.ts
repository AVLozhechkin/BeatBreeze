import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTreeModule } from '@angular/material/tree';
import { RouterLink } from '@angular/router';
import { PlaylistsService } from '../../core/services/playlists.service';
import { ContentState } from 'src/app/shared/models/content-state.enum';
import { MatDialog } from '@angular/material/dialog';
import { CreatePlaylistDialogComponent } from './create-playlist-dialog/create-playlist-dialog.component';
import { PlaylistsButtonsComponent } from './playlists-buttons/playlists-buttons.component';

@Component({
  selector: 'ct-playlists',
  standalone: true,
  templateUrl: './playlists.component.html',
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTreeModule,
    RouterLink,
    PlaylistsButtonsComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlaylistsComponent implements OnInit {
  protected playlistsService = inject(PlaylistsService);
  private dialog = inject(MatDialog);

  protected contentState = signal<ContentState>('notInitialized');

  ngOnInit(): void {
    this.contentState.set('loading');

    this.playlistsService.getPlaylists(false).subscribe({
      error: (err) => {
        console.log(err);
        this.contentState.set('error');
      },
      complete: () => this.contentState.set('initialized'),
    });
  }

  openCreatePlaylistDialog() {
    this.dialog.open(CreatePlaylistDialogComponent, {
      width: '400px',
    });
  }
}
