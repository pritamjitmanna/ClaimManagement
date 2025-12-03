import { Component, ViewChild } from '@angular/core';
import { globalModules } from '../../global_module';
import { Roles } from '../../Models/e.enum';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [globalModules],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  @ViewChild('registerForm') registerForm!:NgForm;
  Roles:Roles[]=Object.values(Roles)


  onSubmit(){
    let formValue=this.registerForm.value;
    
    console.log(formValue);
  }
}
