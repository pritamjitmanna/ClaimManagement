import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { globalModules, globalVariables } from '../../global_module';
import { CommonOutput } from '../../Models/common-output.model';
import { SurveyorService } from '../../Services/surveyor.service';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';
import { RESULT } from '../../Models/e.enum';
import { AccessoriesService } from '../../Services/accessories.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  providers:[SurveyorService],
  imports: [globalModules],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {

  @ViewChild('surveyorProfileForm') surveyorProfileForm!: NgForm;
  fieldValidations:{
    [key:string]:string
  }={
    FirstName:"",
    LastName:"",
    EstimateLimit:""
  }

  constructor(private surveyorService:SurveyorService,private accessoriesService:AccessoriesService,private router:Router){}

  async onSubmit(){
    let value=this.surveyorProfileForm.value;
    let username=await firstValueFrom(globalVariables.username);
    let result:CommonOutput=await this.surveyorService.addSurveyorDetails(username,value);
    if(result.result===RESULT.SUCCESS){
          this.accessoriesService.alertShow(`Congratulations! Your Profile is added`,"success")
          globalVariables.profileSet.next(true);
          globalVariables.userId.next(result.output);
          this.router.navigate([''])
        }
        else{
          let err_status=result.output.status
          let error_block=result.output.error.output
    
          if(err_status===0||err_status>=500){
            this.router.navigate(['internalservererror'])
          }
          else if(err_status===400){
            for(let prop of error_block){
              this.surveyorProfileForm.controls[prop['property']].setErrors({isErr:true})
              this.fieldValidations[prop['property']]=prop['errorMessage']
            }
          }
          else{
            this.router.navigate([''])
            this.accessoriesService.alertShow("Unauthorized","danger")
          }
        }

  }
  
}
