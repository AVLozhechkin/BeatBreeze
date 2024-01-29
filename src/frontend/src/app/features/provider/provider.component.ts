import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { PlaylistsService } from '../../core/services/playlists.service';
import { ProviderService } from './provider.service';
import { Playlist } from '../../core/models/playlist.model';
import { DataProvider } from '../../core/models/data-provider.model';
import { combineLatest } from 'rxjs';
import { ContentState } from 'src/app/shared/models/content-state.enum';
import { ProviderTreeComponent } from './provider-tree/provider-tree.component';
import { ActiveButtonComponent } from '../../shared/components/active-button/active-button.component';
import { MatChipsModule } from '@angular/material/chips';
import { PlayerService2 } from 'src/app/core/services/player.service';
import { MatButton, MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'ct-provider',
  standalone: true,
  templateUrl: './provider.component.html',
  imports: [
    CommonModule,
    MatProgressBarModule,
    ProviderTreeComponent,
    MatChipsModule,
    MatButtonModule,
    ActiveButtonComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProviderComponent implements OnInit {
  protected readonly providerService = inject(ProviderService);
  protected readonly playlistsService = inject(PlaylistsService);
  protected readonly playerService = inject(PlayerService2);

  protected contentState = signal<ContentState>('notInitialized');

  protected isUpdating = signal(false);
  protected isRemoving = signal(false);

  protected playlists: Playlist[] | undefined;
  protected provider: DataProvider | undefined;

  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('providerId');

    if (id === null) {
      const _ = this.router.navigateByUrl('providers');
      return;
    }

    this.contentState.set('loading');

    combineLatest([
      this.providerService.fetchProvider(id, true),
      this.playlistsService.getPlaylists(false),
    ]).subscribe({
      next: ([provider, playlists]) => {
        this.provider = provider;
        this.playlists = playlists;
        this.contentState.set('initialized');
      },
      error: (err) => {
        console.log(err);
        const _ = this.router.navigateByUrl('providers');
        this.contentState.set('error');
      },
    });
  }

  protected playAll() {
    const sorted = this.provider?.musicFiles.sort((a, b) => {
      if (a.path < b.path) {
        return -1;
      }
      if (a.path > b.path) {
        return 1;
      }
      return 0;
    });
    this.playerService.setSongs(sorted!);
  }

  protected update() {
    this.isUpdating.set(true);

    this.providerService.updateProvider(this.provider!.id, true).subscribe({
      next: (provider) => {
        this.provider = provider;
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => this.isUpdating.set(false),
    });
  }

  protected remove() {
    this.isRemoving.set(true);

    this.providerService.removeProvider().subscribe({
      next: () => {
        this.router.navigate(['..']);
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => this.isRemoving.set(false),
    });
  }
}
