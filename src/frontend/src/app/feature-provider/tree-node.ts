import {Song} from "../shared/models/song.model";

export interface TreeNode<T> {
  label: string,
  size: number,
  file: Song | undefined,
  children: TreeNode<T>[]
}

export interface FlatNode {
  expandable: boolean,
  name: string,
  file: Song | undefined,
  level: number
}
