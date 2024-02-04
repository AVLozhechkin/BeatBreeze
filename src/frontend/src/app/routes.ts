import { Routes } from '@angular/router';
import { queueGuard } from './shared/guards/queue.guard';
import { authGuard } from './shared/guards/auth.guard';

export const DEFAULT_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'providers',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/login/login.component').then((c) => c.LoginComponent),
  },
  {
    path: 'signup',
    loadComponent: () =>
      import('./features/signup/sign-up.component').then(
        (c) => c.SignUpComponent
      ),
  },
  {
    path: 'queue',
    loadComponent: () =>
      import('./features/song-queue/song-queue.component').then(
        (c) => c.SongQueueComponent
      ),
    canActivate: [queueGuard],
  },
  {
    path: 'providers',
    loadChildren: () => PROVIDER_ROUTES,
    title: 'Providers',
    canActivate: [authGuard],
  },
  {
    path: 'playlists',
    loadChildren: () => PLAYLIST_ROUTES,
    title: 'Playlists',
  },
];
export const PROVIDER_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/providers/providers.component').then(
        (c) => c.ProvidersComponent
      ),
  },
  {
    path: ':providerId',
    loadComponent: () =>
      import('./features/provider/provider.component').then(
        (c) => c.ProviderComponent
      ),
  },
];

export const PLAYLIST_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/playlists/playlists.component').then(
        (c) => c.PlaylistsComponent
      ),
  },
  {
    path: ':playlistId',
    loadComponent: () =>
      import('./features/playlist/playlist.component').then(
        (c) => c.PlaylistComponent
      ),
  },
];
