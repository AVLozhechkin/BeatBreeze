import { TreeNode } from './tree-node';
import { Song } from '../shared/models/song.model';

export function convertToTreeNode(songs: Song[]): TreeNode<Song>[] {
  const root: TreeNode<Song> = {
    label: 'root',
    file: undefined,
    size: 0,
    children: [],
  };

  songs.forEach((song) => {
    const segments = song.path.split('/');

    let currentNode: TreeNode<Song> = root;

    for (let i = 1; i < segments.length; i++) {
      let existingNode = currentNode.children?.find(
        (node) => node.label == segments[i]
      );

      if (existingNode === undefined) {
        let newNode: TreeNode<Song> = {
          label: segments[i],
          file: song,
          size: 0,
          children: [],
        };

        currentNode.children?.push(newNode);
        currentNode = newNode;
      } else {
        currentNode = existingNode;
        currentNode.file = undefined;
        currentNode.size += song.size;
      }
    }

    currentNode.label = song.name;
  });

  return root.children;
}
