import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

// router.getCurrentNavigation() is only populated during an ongoing navigation; it will be null on page refresh or when a user directly enters the URL. That means this guard can unintentionally block valid accesses.

// In order to use this guard effectively, ensure that navigations relying on state are done programmatically within the app using the Router's navigate method with the state parameter. For example:
// this.router.navigate(['target-route'], { state: { key: 'value' } });
// or if using routerLink in templates:
// <a [routerLink]="['/target-route']" [state]="{ key: 'value' }">Link</a>
// This way, the state will be present during navigation, and the guard will function as intended.

export const permissionGuard: CanActivateFn = (route, state) => {
  const router:Router=inject(Router)

  if(router.getCurrentNavigation()?.extras.state===undefined){
    router.navigate(['/'])
    return false
  }

  return true;
};
