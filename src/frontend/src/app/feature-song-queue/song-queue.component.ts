import {Component, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import {MatTableModule} from "@angular/material/table";
import {QueueService} from "../shared/services/queue.service";

@Component({
  selector: 'cmp-song-queue',
  standalone: true,
  imports: [CommonModule, MatTableModule],
  templateUrl: './song-queue.component.html'
})
export class SongQueueComponent {
  protected queueService = inject(QueueService)
  protected columns = ['position', 'name', 'buttons']
  protected readonly indexedDB = indexedDB;
}
