import { Component, inject } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MatSliderModule } from '@angular/material/slider';
import { FormsModule } from '@angular/forms';
import {
  faRepeat,
  faShuffle,
  faVolumeHigh,
} from '@fortawesome/free-solid-svg-icons';
import { MatIconModule } from '@angular/material/icon';
import { NgIf, NgSwitch, NgSwitchCase } from '@angular/common';
import { PlayerService, RepeatType } from '../shared/services/player.service';
import { MatButtonModule } from '@angular/material/button';
import {lastValueFrom} from "rxjs";

@Component({
  standalone: true,
  selector: 'cmp-player',
  imports: [
    FontAwesomeModule,
    MatSliderModule,
    FormsModule,
    MatIconModule,
    NgIf,
    NgSwitch,
    NgSwitchCase,
    MatButtonModule,
  ],
  templateUrl: './player.component.html',
  styles: ['.buttons-size { transform: scale(2)}']
})
export class PlayerComponent {
  protected readonly playerService = inject(PlayerService);
  protected currentVolume = 0.1;

  protected readonly faRepeat = faRepeat;
  protected readonly faShuffle = faShuffle;
  protected readonly faVolumeHigh = faVolumeHigh;

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

  protected readonly lastValueFrom = lastValueFrom;
}
