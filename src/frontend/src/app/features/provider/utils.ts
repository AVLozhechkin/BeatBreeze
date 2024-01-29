import { FlatNode, TreeNode } from './tree-node';
import { MusicFile } from '../../core/models/musicFile.model';

export function convertToTreeNode(songs: MusicFile[]): TreeNode<MusicFile>[] {
  const root: TreeNode<MusicFile> = {
    label: 'root',
    file: undefined,
    size: 0,
    children: [],
  };

  songs.forEach((song) => {
    const segments = song.path.split('/');

    let currentNode: TreeNode<MusicFile> = root;

    for (let i = 1; i < segments.length; i++) {
      let existingNode = currentNode.children?.find(
        (node) => node.label == segments[i]
      );

      if (existingNode === undefined) {
        let newNode: TreeNode<MusicFile> = {
          label: segments[i],
          file: song,
          size: 0,
          children: [],
        };

        sortedInsert(currentNode.children, newNode);

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

export function transformer(
  node: TreeNode<MusicFile> | undefined,
  level: number
): FlatNode {
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

function sortedInsert(
  children: TreeNode<MusicFile>[],
  item: TreeNode<MusicFile>
) {
  let low = 0,
    high = children.length;

  while (low < high) {
    let mid = (low + high) >>> 1;
    if (children[mid].label < item.label) low = mid + 1;
    else high = mid;
  }

  children.splice(low, 0, item);
}
