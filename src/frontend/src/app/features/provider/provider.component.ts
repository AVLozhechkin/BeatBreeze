import {Component, inject, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MatTreeFlatDataSource,
  MatTreeFlattener,
  MatTreeModule,
} from '@angular/material/tree';
import { ActivatedRoute, Router } from '@angular/router';
import { Song } from '../../core/models/song.model';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FlatNode, TreeNode } from './tree-node';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatMenuModule } from '@angular/material/menu';
import { PlaylistsService } from '../../core/services/playlists.service';
import { PlayerService } from '../../core/services/player.service';
import { ProviderService } from './provider.service';
import { QueueService } from '../../core/services/queue.service';
import {Playlist} from "../../core/models/playlist.model";
import {DataProvider} from "../../core/models/data-provider.model";
import {transformer} from "./utils";

@Component({
  selector: 'cmp-provider',
  standalone: true,
  imports: [
    CommonModule,
    MatTreeModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatMenuModule,
  ],
  templateUrl: './provider.component.html',
})
export class ProviderComponent implements OnInit {
  protected readonly providerService = inject(ProviderService);
  protected readonly playerService = inject(PlayerService);
  protected readonly playlistsService = inject(PlaylistsService);
  protected readonly queueService = inject(QueueService);

  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  private providerId: string | undefined;
  private isDataSourceInitialized = false;

  protected playlists: Playlist[] | undefined;
  protected provider: DataProvider | undefined;

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('providerId');

    if (id === null) {
      const _ = this.router.navigateByUrl('providers');
      return
    }

    this.providerId = id;

    this.providerService
      .fetchProvider(this.providerId)
      .subscribe({
        next: provider => this.provider = provider,
        error: err => {
          console.log(err)
          const _ = this.router.navigateByUrl('providers');
        }
      });

    this.playlistsService
      .getPlaylists()
      .subscribe({
        next: playlists => this.playlists = playlists,
        error: err => console.log(err)
      });
  }

  public getDataSource() {
    if (this.isDataSourceInitialized) {
      return this.dataSource;
    }

    this.dataSource.data = this.providerService.getProviderTree(
      this.providerId!
    )!;
    this.isDataSourceInitialized = true;
    return this.dataSource;
  }

  hasChild = (_: number, node: FlatNode) => node.expandable;

  addSongToPlaylist(file: Song, playlistId: string) {
    this.playlistsService
      .addSongToPlaylist(playlistId, file.id)
      .subscribe({
        next: _ => console.log('Added to the playlist'),
        error: err => console.log(err)
      });
  }

  addSongToQueue(file: Song) {
    this.queueService.addToQueue([file]);
  }

  async playSong(song: Song) {
    await this.playerService.playSongs([song]);
  }

  public readonly treeControl = new FlatTreeControl<FlatNode>(
    (node) => node.level,
    (node) => node.expandable
  );

  public readonly treeFlattener = new MatTreeFlattener(
    transformer,
    (node) => node.level,
    (node) => node.expandable,
    (node) => {
      if (node) {
        return node.children;
      }

      return [];
    }
  );

  private dataSource = new MatTreeFlatDataSource(
    this.treeControl,
    this.treeFlattener
  );
}
