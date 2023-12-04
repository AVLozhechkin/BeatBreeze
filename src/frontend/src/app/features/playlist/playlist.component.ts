import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Song } from '../../core/models/song.model';
import { PlayerService } from '../../core/services/player.service';
import {PlaylistsService} from "../../core/services/playlists.service";
import {Playlist} from "../../core/models/playlist.model";

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
  private readonly playlistsService = inject(PlaylistsService);

  protected playlistId: string | undefined;
  protected playlist: Playlist | undefined;
  protected isLoading = true;

  private readonly playerService = inject(PlayerService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  constructor() {}

   ngOnInit() {
    const id = this.route.snapshot.paramMap.get('playlistId');

    if (id === null) {
      this.router.navigateByUrl('playlists');
      return
    }

    this.playlistsService
      .getPlaylist(id)
      .subscribe({
        next: playlist => {
          this.playlist = playlist;
          this.isLoading = false
        },
        error: err => {
          console.log(err)
          this.isLoading = false
        }
      })
  }

  async play(playlistItem: Song) {
    await this.playerService.playSongs([playlistItem]);
  }

  async playAll() {
    await this.playerService.playSongs(this.playlist!.songFiles!);
  }

  remove(playlistItem: Song) {
    this.playlistsService
      .removeSongFromPlaylist(this.playlistId!, playlistItem.id)
      .subscribe({
        next: playlist => {
          this.playlist = playlist
          this.isLoading = false
        },
        error: err => {
          console.log(err)
          this.isLoading = false
        }
      });
  }
}
