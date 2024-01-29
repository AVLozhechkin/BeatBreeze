import {
  ChangeDetectionStrategy,
  Component,
  Input,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlaylistItem } from 'src/app/core/models/playlist-item.model';
import { ActiveButtonComponent } from 'src/app/shared/components/active-button/active-button.component';
import { PlaylistService } from '../playlist.service';

@Component({
  selector: 'cmp-playlist-item-buttons',
  standalone: true,
  imports: [CommonModule, ActiveButtonComponent],
  templateUrl: './playlist-item-buttons.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlaylistItemButtonsComponent {
  private playlistService = inject(PlaylistService);

  @Input({ required: true }) playlistItem!: PlaylistItem;
  protected isRemoving = signal(false);

  remove() {
    this.isRemoving.set(true);

    this.playlistService
      .removeSongFromPlaylist(this.playlistItem.musicFile.id)
      .subscribe({
        error: (err) => {
          console.log(err);
          this.isRemoving.set(false);
        },
        complete: () => this.isRemoving.set(false),
      });
  }
}
