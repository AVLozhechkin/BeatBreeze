import {Routes} from "@angular/router";
import {SignUpComponent} from "./features/signup/sign-up.component";
import {LoginComponent} from "./features/login/login.component";
import {WelcomeComponent} from "./features/welcome/welcome.component";
import {HomeComponent} from "./features/home/home.component";
import {ProvidersComponent} from "./features/providers/providers.component";
import {ProviderComponent} from "./features/provider/provider.component";
import {PlaylistsComponent} from "./features/playlists/playlists.component";
import {PlaylistComponent} from "./features/playlist/playlist.component";
import {SongQueueComponent} from "./features/song-queue/song-queue.component";


export const DEFAULT_ROUTES: Routes = [
  {
    path: '',
    component: WelcomeComponent,
  },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'signup',
    component: SignUpComponent
  },
  {
    path: 'queue',
    component: SongQueueComponent
  },
  {
    path: 'providers',
    loadChildren: () => PROVIDER_ROUTES,
    title: 'Providers',
  },
  {
    path: 'playlists',
    loadChildren: () => PLAYLIST_ROUTES,
    title: 'Playlists',
  }
]
export const PROVIDER_ROUTES: Routes = [
  { path: '', component: ProvidersComponent },
  { path: ':providerId', component: ProviderComponent }
]

export const PLAYLIST_ROUTES: Routes = [
  { path: '', component: PlaylistsComponent },
  { path: ':playlistId', component: PlaylistComponent }
]
