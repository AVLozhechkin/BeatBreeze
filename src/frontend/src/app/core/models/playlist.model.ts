import { PlaylistItem } from './playlist-item.model';

export interface Playlist {
  id: string;
  name: string;
  createdAt: Date;
  updatedAt: Date;
  size: number;
  playlistItems: PlaylistItem[] | undefined;
}
