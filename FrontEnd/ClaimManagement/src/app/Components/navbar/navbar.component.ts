import { Component,OnInit } from '@angular/core';
import { globalModules } from '../../global_module';
import { AuthService } from '../../Services/auth.service';
import { Router } from '@angular/router';



@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [globalModules],
  providers:[AuthService],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent  {

  
  constructor(private authService:AuthService){
    this.authService.decodeTokenUserRole()
  }


  logoutClick(){
    this.authService.logout()
  }



}
