import { Component,OnInit } from '@angular/core';
import { globalModules, globalVariables } from '../../global_module';
import { AuthService } from '../../Services/auth.service';
import { Router } from '@angular/router';
import { BreadcrumbComponent } from '../Notification/breadcrumb/breadcrumb.component';



@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [globalModules,BreadcrumbComponent],
  providers:[AuthService],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent  {

  username:string=""
  isAuthenticated:boolean=false
  roles:string[]=[]
  constructor(private authService:AuthService){
    this.authService.decodeTokenUserRole()
    globalVariables.username.subscribe((value:string)=>{
      this.username=value
    })
    globalVariables.role.subscribe(
      (roles:string[])=>{
        this.roles=roles
      }
    )
    globalVariables.isAuthenticated.subscribe((value:boolean)=>{
      this.isAuthenticated=value
    })
  }


  logoutClick(){
    this.authService.logout()
  }



}
