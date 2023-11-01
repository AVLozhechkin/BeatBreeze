import { Component, inject } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MatSliderModule } from '@angular/material/slider';
import { FormsModule } from '@angular/forms';
import {
  faBackwardStep,
  faForwardStep,
  faRepeat,
  faShuffle,
  faVolumeHigh,
  faCircle,
} from '@fortawesome/free-solid-svg-icons';
import { MatIconModule } from '@angular/material/icon';
import {
  faCirclePause,
  faCirclePlay,
} from '@fortawesome/free-regular-svg-icons';
import { NgIf, NgSwitch, NgSwitchCase } from '@angular/common';
import { PlayerService, RepeatType } from '../shared/services/player.service';
import { MatButtonModule } from '@angular/material/button';

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
})
export class PlayerComponent {
  protected readonly playerService = inject(PlayerService);
  protected currentVolume = 0.1;

  protected readonly faCircle = faCircle;
  protected readonly faCirclePlay = faCirclePlay;
  protected readonly faCirclePause = faCirclePause;
  protected readonly faForwardStep = faForwardStep;
  protected readonly faBackwardStep = faBackwardStep;
  protected readonly faRepeat = faRepeat;
  protected readonly faShuffle = faShuffle;
  protected readonly faVolumeHigh = faVolumeHigh;

  setSeek(event: MouseEvent) {
    const { width } = (
      event.currentTarget as HTMLElement
    ).getBoundingClientRect();
    this.playerService.setSeek(width, event.clientX);
  }

  playPrevious() {
    this.playerService.playPrevious();
  }

  playNext() {
    this.playerService.playNext();
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
}
