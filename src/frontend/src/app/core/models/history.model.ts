import {Song} from "./song.model";

export interface History {
  id: string,
  historyItems: Song[]
}
