import { MusicFile } from '../../core/models/musicFile.model';

export interface TreeNode<T> {
  label: string;
  size: number;
  file: MusicFile | undefined;
  children: TreeNode<T>[];
}

export interface FlatNode {
  expandable: boolean;
  name: string;
  file: MusicFile | undefined;
  level: number;
}
