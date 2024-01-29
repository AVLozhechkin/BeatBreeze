import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, of, tap } from 'rxjs';
import { CacheService } from 'src/app/core/services/cache.service';
import { environment } from 'src/environments/environment';

@Injectable()
export class CacheInterceptor implements HttpInterceptor {
  private cacheService = inject(CacheService);
  private providersUrl = `${environment.apiUrl}/providers/songUrl/`;

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (!req.url.startsWith(this.providersUrl)) {
      return next.handle(req);
    }

    const cachedResponse = this.cacheService.getItem(req.url);

    if (cachedResponse) {
      return of(cachedResponse.item.clone());
    } else {
      return next.handle(req).pipe(
        tap((response) => {
          const expiresAt = new Date();
          expiresAt.setHours(expiresAt.getHours() + 1);
          this.cacheService.putItem(req.url, response, expiresAt);
        })
      );
    }
  }
}
