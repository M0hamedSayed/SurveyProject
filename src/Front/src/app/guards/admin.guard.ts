import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/logic/user.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const user = inject(UserService);
  const router = inject(Router);

  const isAdmin = user.select('roles')()?.includes('Admin');

  if (isAdmin) return true;
  else {
    router.navigate(['survey']);
    return false;
  }
};
