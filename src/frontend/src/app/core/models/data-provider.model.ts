import { MusicFile } from './musicFile.model';
import { ProviderTypes } from './provider-types.model';

export interface DataProvider {
  id: string;
  name: string;
  userId: string;
  providerType: ProviderTypes;
  addedAt: Date;
  updatedAt: Date;
  musicFiles: MusicFile[];
}
