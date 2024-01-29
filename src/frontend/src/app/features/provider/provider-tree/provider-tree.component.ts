import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MatTreeFlatDataSource,
  MatTreeFlattener,
  MatTreeModule,
} from '@angular/material/tree';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { FlatTreeControl } from '@angular/cdk/tree';
import { FlatNode } from '../tree-node';
import { ProviderService } from '../provider.service';
import { PlaylistsService } from 'src/app/core/services/playlists.service';
import { MusicFile } from 'src/app/core/models/musicFile.model';
import { convertToTreeNode, transformer } from '../utils';
import { DataProvider } from 'src/app/core/models/data-provider.model';
import { Playlist } from 'src/app/core/models/playlist.model';
import { PlayerService2 } from 'src/app/core/services/player.service';

@Component({
  selector: 'ct-provider-tree',
  standalone: true,
  imports: [
    CommonModule,
    MatTreeModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
  ],
  templateUrl: './provider-tree.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProviderTreeComponent implements OnChanges {
  @Input({ required: true }) playlists!: Playlist[];
  @Input({ required: true }) provider!: DataProvider;

  protected readonly providerService = inject(ProviderService);
  protected readonly playerService = inject(PlayerService2);
  protected readonly playlistsService = inject(PlaylistsService);

  protected readonly treeControl = new FlatTreeControl<FlatNode>(
    (node) => node.level,
    (node) => node.expandable
  );

  private readonly treeFlattener = new MatTreeFlattener(
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

  protected dataSource = new MatTreeFlatDataSource(
    this.treeControl,
    this.treeFlattener
  );

  protected hasChild = (_: number, node: FlatNode) => node.expandable;

  ngOnChanges(_: SimpleChanges): void {
    const tree = convertToTreeNode(this.provider.musicFiles);

    this.dataSource.data = tree;
  }

  updateTree(provider: DataProvider) {
    const tree = convertToTreeNode(provider.musicFiles);

    this.dataSource.data = tree;
  }

  // Buttons

  protected addSongToPlaylist(file: MusicFile, playlistId: string) {
    this.playlistsService.addSongToPlaylist(playlistId, file.id).subscribe({
      next: (_) => console.log('Added to the playlist'),
      error: (err) => console.log(err),
    });
  }

  protected async addSongToQueue(file: MusicFile) {
    await this.playerService.addSongs([file]);
  }

  protected async playSong(song: MusicFile) {
    await this.playerService.setSongs([song]);
  }
}
