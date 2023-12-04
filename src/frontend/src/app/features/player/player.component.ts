import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatSliderModule } from '@angular/material/slider';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { PlayerService, RepeatType } from '../../core/services/player.service';
import { MatButtonModule } from '@angular/material/button';
import { lastValueFrom } from 'rxjs';
import { QueueService } from '../../core/services/queue.service';

@Component({
  standalone: true,
  selector: 'cmp-player',
  imports: [MatSliderModule, FormsModule, MatIconModule, MatButtonModule],
  templateUrl: './player.component.html',
  styles: ['.buttons-size { transform: scale(2)}'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlayerComponent {
  protected readonly playerService = inject(PlayerService);
  protected readonly queueService = inject(QueueService);
  public currentVolume = 0.1;

  setSeek(event: MouseEvent) {
    const { width } = (
      event.currentTarget as HTMLElement
    ).getBoundingClientRect();
    this.playerService.setSeek(width, event.clientX);
  }

  async playPrevious() {
    await this.playerService.playPrevious();
  }

  async playNext() {
    await this.playerService.playNext();
  }

  play() {
    this.playerService.play();
  }
  pause() {
    this.playerService.pause();
  }

  setRepeat(repeat: RepeatType) {
    this.playerService.setRepeatType(repeat);
  }

  shuffle() {
    this.queueService.shuffle();
  }

  unshuffle() {
    this.queueService.unshuffle();
  }

  unmute() {
    this.playerService.unmute();
  }
  mute() {
    this.playerService.mute();
  }

  updateVolume() {
    this.playerService.setVolume(this.currentVolume);
  }

  protected readonly lastValueFrom = lastValueFrom;
}
