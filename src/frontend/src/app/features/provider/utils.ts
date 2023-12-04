import {FlatNode, TreeNode} from './tree-node';
import { Song } from '../../core/models/song.model';

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

        sortedInsert(currentNode.children, newNode)

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


export function transformer(node: TreeNode<Song> | undefined, level: number): FlatNode  {
  if (!node) {
    return {
      expandable: false,
      file: undefined,
      name: '',
      level: 0,
    };
  }
  return {
    expandable: !!node.children && node.children.length > 0,
    file: node.file,
    name: node.label,
    level: level,
  };
}

function sortedInsert(children: TreeNode<Song>[], item: TreeNode<Song>) {
  let low = 0,
    high = children.length;

  while (low < high) {
    let mid = (low + high) >>> 1;
    if (children[mid].label < item.label) low = mid + 1;
    else high = mid;
  }

  children.splice(low, 0, item)
}
