import { Component, ViewChild } from '@angular/core';
import { globalModules } from '../../global_module';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { HttpClient } from '@angular/common/http';
import { RESULT } from '../../Models/e.enum';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [globalModules],
  providers:[AuthService],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  @ViewChild("login") login!:NgForm

  constructor(private authService:AuthService,private router:Router){}

  async onSubmit(){
    let data=await this.authService.login(this.login.value)
    if(data.result==RESULT.FAILURE){
      if(data.output.status===0||data.output.status>=500){
        this.router.navigate(['internalservererror'])
      }
      else{
        
      }
    }
    else{
      this.router.navigate([''])
    }
    
  }


}
