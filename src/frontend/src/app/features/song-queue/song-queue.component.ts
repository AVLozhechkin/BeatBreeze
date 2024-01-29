import {
  ChangeDetectionStrategy,
  Component,
  effect,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { PlayerService2 } from 'src/app/core/services/player.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MusicFile } from 'src/app/core/models/musicFile.model';

@Component({
  selector: 'ct-song-queue',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule],
  templateUrl: './song-queue.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SongQueueComponent {
  protected playerService = inject(PlayerService2);
  protected columns = ['position', 'name', 'buttons'];
  // protected queue = signal<MusicFile[]>([]);

  constructor() {
    //effect(() => {
    //  this.queue.set(this.playerService.currentPlaylist());
    //});
  }

  shuffle() {
    this.playerService.shuffle();
  }
  unshuffle() {
    this.playerService.unshuffle();
  }
  removeSong(index: number) {
    this.playerService.removeSong(index);
  }
}
