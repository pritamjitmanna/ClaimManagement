import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const permissionGuard: CanActivateFn = (route, state) => {
  const router:Router=inject(Router)

  if(router.getCurrentNavigation()?.extras.state===undefined){
    router.navigate(['/'])
    return false
  }

  return true;
};
