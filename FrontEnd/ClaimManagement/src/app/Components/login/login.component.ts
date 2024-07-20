import { Component, ViewChild } from '@angular/core';
import { globalModules } from '../../global_module';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../Services/AuthService.service';
import { HttpClient } from '@angular/common/http';
import { RESULT } from '../../Models/e.enum';

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

  constructor(private authService:AuthService){}

  async onSubmit(){
    console.log(this.login)
    let data=await this.authService.login(this.login.value)
    if(data.result==RESULT.SUCCESS){
      localStorage.setItem("loginData",JSON.stringify(data.output))
    }
    else{
      console.log(data.output)
    }

  }


}
