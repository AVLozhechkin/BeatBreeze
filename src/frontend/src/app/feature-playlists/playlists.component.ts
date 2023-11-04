import {Component, inject, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTreeModule } from '@angular/material/tree';
import { RouterLink } from '@angular/router';
import {CreatePlaylistPanelComponent} from "./create-playlist-panel/create-playlist-panel.component";
import {PlaylistsService} from "../shared/services/playlists.service";

@Component({
  selector: 'cmp-playlists',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTreeModule,
    RouterLink,
    CreatePlaylistPanelComponent,
  ],
  templateUrl: './playlists.component.html',
})
export class PlaylistsComponent implements OnInit
{
  playlistService = inject(PlaylistsService);
  columns = ['position', 'name', 'updatedAt', 'size', 'buttons'];

  ngOnInit(): void {
    this.playlistService.fetchPlaylists()
  }
}
