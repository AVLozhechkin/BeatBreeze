import { MusicFile } from './musicFile.model';

export interface PlaylistItem {
  id: string;
  addedAt: Date;
  musicFile: MusicFile;
}
