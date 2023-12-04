export interface Song {
  id: string;
  size: number;
  name: string;
  type: string;
  url: string | undefined;
  expires_at: Date | undefined,
  path: string;
}
