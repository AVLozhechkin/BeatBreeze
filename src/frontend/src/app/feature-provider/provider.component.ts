import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MatTreeFlatDataSource,
  MatTreeFlattener,
  MatTreeModule,
} from '@angular/material/tree';
import { ActivatedRoute, Router } from '@angular/router';
import { Song } from '../shared/models/song.model';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FlatNode, TreeNode } from './tree-node';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatMenuModule } from '@angular/material/menu';
import { PlaylistsService } from '../shared/services/playlists.service';
import { PlayerService } from '../shared/services/player.service';
import { ProviderService } from './provider.service';
import { QueueService } from '../shared/services/queue.service';

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

  private _transformer = (
    node: TreeNode<Song> | undefined,
    level: number
  ): FlatNode => {
    if (!node) {
      return {
        expandable: false,
        file: undefined,
        name: '',
        level: 0,
      };
    }
    return {
      expandable: !!node.children && node.children.length > 0,
      file: node.file,
      name: node.label,
      level: level,
    };
  };

  public readonly treeControl = new FlatTreeControl<FlatNode>(
    (node) => node.level,
    (node) => node.expandable
  );

  public readonly treeFlattener = new MatTreeFlattener(
    this._transformer,
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
  private isDataSourceInitialized = false;

  async ngOnInit(): Promise<void> {
    const id = this.activatedRoute.snapshot.paramMap.get('providerId');

    if (id === null) {
      await this.router.navigateByUrl('providers');
    }

    this.providerId = id!;

    this.providerService.fetchProvider(this.providerId);
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
    this.playlistsService.addSongToPlaylist(playlistId, file.id);
  }

  addSongToQueue(file: Song) {
    this.queueService.addToQueue([file]);
  }

  async playSong(song: Song) {
    await this.playerService.playSongs([song]);
  }
}
