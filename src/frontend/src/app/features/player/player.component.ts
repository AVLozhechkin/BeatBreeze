import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatSliderModule } from '@angular/material/slider';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import {
  PlayerService2,
  RepeatType,
} from 'src/app/core/services/player.service';
import { FormatTimePipe } from 'src/app/shared/pipes/format-time.pipe';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  standalone: true,
  selector: 'ct-player',
  imports: [
    MatSliderModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
    FormatTimePipe,
  ],
  templateUrl: './player.component.html',
  styles: ['.buttons-size { transform: scale(2)}'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlayerComponent {
  protected readonly playerService = inject(PlayerService2);
  public currentVolume = 0.1;

  setSeek(event: MouseEvent) {
    const { width } = (
      event.currentTarget as HTMLElement
    ).getBoundingClientRect();

    const percentage = event.clientX / width;
    const seconds = this.playerService.duration() * percentage;

    this.playerService.setSeek(seconds);
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
    this.playerService.shuffle();
  }

  unshuffle() {
    this.playerService.unshuffle();
  }

  unmute() {
    this.playerService.setMute(false);
  }
  mute() {
    this.playerService.setMute(true);
  }

  updateVolume() {
    this.playerService.setVolume(this.currentVolume);
  }
}
