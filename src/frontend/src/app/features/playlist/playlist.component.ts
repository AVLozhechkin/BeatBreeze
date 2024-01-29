import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PlaylistItem } from 'src/app/core/models/playlist-item.model';
import { MatIconModule } from '@angular/material/icon';
import { ContentState } from 'src/app/shared/models/content-state.enum';
import { PlaylistService } from './playlist.service';
import { PlaylistItemButtonsComponent } from './playlist-item-buttons/playlist-item-buttons.component';
import { firstValueFrom } from 'rxjs';
import { PlayerService2 } from 'src/app/core/services/player.service';

@Component({
  selector: 'ct-playlist-table',
  standalone: true,
  templateUrl: './playlist.component.html',
  imports: [
    CommonModule,
    MatProgressBarModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    RouterLink,
    PlaylistItemButtonsComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlaylistComponent implements OnInit {
  protected readonly playlistService = inject(PlaylistService);

  protected contentState = signal<ContentState>('notInitialized');

  private readonly playerService = inject(PlayerService2);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  constructor() {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('playlistId');

    if (id === null) {
      this.router.navigateByUrl('playlists');
      return;
    }

    this.contentState.set('loading');

    this.playlistService.getPlaylist(id, true).subscribe({
      error: (err) => {
        console.log(err);
        this.contentState.set('error');
      },
      complete: () => this.contentState.set('initialized'),
    });
  }

  async play(playlistItem: PlaylistItem) {
    // TODO Put all playlist in queue
    await this.playerService.setSongs([playlistItem.musicFile]);
  }

  async playAll() {
    const playlist = await firstValueFrom(this.playlistService.playlist);

    await this.playerService.setSongs(
      playlist!.playlistItems!.map((pi) => pi.musicFile)!
    );
  }
}
