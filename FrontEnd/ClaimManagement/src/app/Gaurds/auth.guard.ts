import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { AuthService } from '../Services/auth.service';
import { firstValueFrom } from 'rxjs';
import { globalVariables } from '../global_module';

export const authGuard: CanActivateFn = async(route, state):Promise<boolean | UrlTree> => {

  //Commented it to make everything available for implementing them

  // const router=inject(Router)

  // const isAuth=await firstValueFrom(globalVariables.isAuthenticated)
  // let roles=await firstValueFrom(globalVariables.role)
  // if(typeof(roles)==="string")roles=[roles]
  // let expectedRoles:string[]=Array.from(route.data['role'])

  // if (isAuth==false || (route.data['role'] && expectedRoles.some(item=>roles.includes(item))===false)) {
  //   router.navigate(['login']);
  //   return false;
  // }
  return true

};
