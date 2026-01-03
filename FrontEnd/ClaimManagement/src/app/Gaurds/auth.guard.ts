import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { AuthService } from '../Services/auth.service';
import { firstValueFrom } from 'rxjs';
import { globalVariables } from '../global_module';


export const authGuard: CanActivateFn = async(route, state):Promise<boolean | UrlTree> => {

  //Commented it to make everything available for implementing them

  const router=inject(Router)
  

  if(router.getCurrentNavigation()?.extras.state?.['navigated']){
    return true
  }
// console.log(router.getCurrentNavigation())
  const isAuth=await firstValueFrom(globalVariables.isAuthenticated)
  let roles=await firstValueFrom(globalVariables.role)
  let profileSet=await firstValueFrom(globalVariables.profileSet)
  if(typeof(roles)==="string")roles=[roles]
  let expectedRoles:string[] | null=(route.data['role']?Array.from(route.data['role']):null)

  if (isAuth==false || (route.data['role'] &&  expectedRoles && expectedRoles.some(item=>roles.includes(item))===false)) {
    router.navigate(['login']);
    return false;
  }

  if(roles.includes("Surveyor") && profileSet===false){
    router.navigate(['profile'])
    return false
  }
  return true

};

