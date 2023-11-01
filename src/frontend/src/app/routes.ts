import {Routes} from "@angular/router";
import {SignUpComponent} from "./feature-signup/sign-up.component";
import {LoginComponent} from "./feature-login/login.component";
import {WelcomeComponent} from "./feature-welcome/welcome.component";
import {HomeComponent} from "./feature-home/home.component";
import {ProvidersComponent} from "./feature-providers/providers.component";
import {ProviderComponent} from "./feature-provider/provider.component";
import {PlaylistsComponent} from "./feature-playlists/playlists.component";
import {PlaylistComponent} from "./feature-playlist/playlist.component";
import {SongQueueComponent} from "./feature-song-queue/song-queue.component";


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
