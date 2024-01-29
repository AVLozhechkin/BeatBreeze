import { Injectable } from '@angular/core';

interface CacheItem {
  expires: Date;
  item: any;
}

@Injectable({
  providedIn: 'root',
})
export class CacheService {
  private cache: Map<string, CacheItem> = new Map();

  putItem(key: string, item: any, expiresAt: Date) {
    this.cache.set(key, {
      item: item,
      expires: expiresAt,
    });
  }

  getItem(key: string): any | null {
    const item = this.cache.get(key);

    if (item === undefined) {
      return null;
    }

    if (item.expires < new Date()) {
      this.cache.delete(key);
      return null;
    }

    return item;
  }

  removeItem(key: string) {
    this.cache.delete(key);
  }
}
