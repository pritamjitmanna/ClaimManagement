import { Component, ViewChild } from '@angular/core';
import { globalModules } from '../../global_module';
import { RESULT, Roles } from '../../Models/e.enum';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { comparePasswordValidator } from '../../Directives/compare-password.validator';
import { Router } from '@angular/router';
import { CommonOutput } from '../../Models/common-output.model';
import { AccessoriesService } from '../../Services/accessories.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [globalModules,ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm!:FormGroup;
  Roles:Roles[]=Object.values(Roles)

  constructor(private authService:AuthService,private accessoriesService:AccessoriesService,private router:Router){
    this.registerForm = new FormGroup({
      UserName: new FormControl('', Validators.required),
      EmailAddress: new FormControl('', [Validators.required, Validators.email]),
      Password: new FormControl('', Validators.required),
      cPassword: new FormControl('', Validators.required),
      Roles: new FormControl('', Validators.required)
    },comparePasswordValidator)
  }


  async onSubmit(){
    let formValue=this.registerForm.value;
    let result:CommonOutput=await this.authService.register({
      userName:this.registerForm.value.UserName,
      emailAddress:this.registerForm.value.EmailAddress,
      password:this.registerForm.value.Password,
      roles:[this.registerForm.value.Roles]
    });
    if(result.result===RESULT.SUCCESS){
      this.router.navigate(['/login'])
      this.accessoriesService.alertShow("Registration Successful! Please login with the required credentials.","success")
    }
    else{
      if(result.output.status===0||result.output.status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(result.output.status===409){
        this.accessoriesService.alertShow("User with this email address already exists.","danger")
      }
    }
}



}
