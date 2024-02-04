import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PlayerService2 } from 'src/app/core/services/player.service';

export const queueGuard: CanActivateFn = (route, state) => {
  const playerService = inject(PlayerService2);

  if (playerService.currentPlaylist().length > 1) {
    return true;
  }

  const router: Router = inject(Router);

  return router.createUrlTree(['providers']);
};
