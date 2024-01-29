import {
  ChangeDetectionStrategy,
  Component,
  Input,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Playlist } from 'src/app/core/models/playlist.model';
import { ActiveButtonComponent } from '../../../shared/components/active-button/active-button.component';
import { RouterModule } from '@angular/router';
import { PlaylistsService } from 'src/app/core/services/playlists.service';
import { MatButtonModule } from '@angular/material/button';
import { PlayerService2 } from 'src/app/core/services/player.service';

@Component({
  selector: 'ct-playlists-buttons',
  standalone: true,
  templateUrl: './playlists-buttons.component.html',
  imports: [CommonModule, MatButtonModule, ActiveButtonComponent, RouterModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlaylistsButtonsComponent {
  private playerService = inject(PlayerService2);
  private playlistsService = inject(PlaylistsService);

  @Input({ required: true }) playlist!: Playlist;
  isEnquing = signal(false);
  isDeleting = signal(false);

  async play() {
    this.isEnquing.set(true);

    this.playlistsService.getPlaylist(this.playlist.id, true).subscribe({
      next: async (playlist) => {
        console.log(playlist);
        await this.playerService.setSongs(
          playlist.playlistItems!.map((pi) => pi.musicFile)
        );
        this.isEnquing.set(false);
      },
      error: (err) => {
        console.log(err);
        this.isEnquing.set(false);
      },
    });
  }

  deletePlaylist() {
    this.isDeleting.set(true);

    this.playlistsService.deletePlaylist(this.playlist.id).subscribe({
      next: (_) => {
        console.log('Removed provider with id ' + this.playlist.id);
      },
      error: (err) => console.log(err),
      complete: () => this.isDeleting.set(false),
    });
  }
}
