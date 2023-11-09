import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Song } from '../shared/models/song.model';
import { PlayerService } from '../shared/services/player.service';
import {PlaylistService} from "./playlist.service";

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
  protected readonly playlistService = inject(PlaylistService);
  protected readonly columns = ['position', 'name', 'buttons'];
  protected playlistId: string | undefined;
  private readonly playerService = inject(PlayerService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  constructor() {}

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('playlistId');

    if (id === null) {
      await this.router.navigateByUrl('playlists');
    }

    this.playlistService.fetchPlaylist(id!);
  }

  async play(playlistItem: Song) {
    await this.playerService.playSongs([playlistItem]);
  }

  async playAll() {
    const playlist = this.playlistService.playlist()!;
    await this.playerService.playSongs(playlist.songFiles);
  }

  remove(playlistItem: Song) {
    this.playlistService.removeSongFromPlaylist(playlistItem.id);
  }
}
