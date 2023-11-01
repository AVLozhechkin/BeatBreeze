import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Song } from '../shared/models/song.model';
import { PlaylistsService } from '../shared/services/playlists.service';
import { PlayerService } from '../shared/services/player.service';

@Component({
  selector: 'cmp-playlist-table',
  standalone: true,
  imports: [
    CommonModule,
    MatProgressBarModule,
    MatButtonModule,
    MatTableModule,
    RouterLink,
  ],
  templateUrl: './playlist.component.html',
})
export class PlaylistComponent implements OnInit {
  protected readonly playlistsService = inject(PlaylistsService);
  protected readonly columns = ['position', 'name', 'buttons'];
  private readonly playerService = inject(PlayerService);
  protected playlistId: string | undefined;
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  constructor() {}

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('playlistId');

    if (id === null) {
      await this.router.navigateByUrl('playlists');
    }

    this.playlistsService.setCurrentPlaylist(id!);
  }

  play(playlistItem: Song) {
    this.playerService.playSong(playlistItem);
  }

  playAll() {
    const playlist = this.playlistsService.currentPlaylist()!;
    this.playerService.playSongs(playlist);
  }

  remove(playlistItem: Song) {
    this.playlistsService.removeSongFromPlaylist(playlistItem.id);
  }
}
