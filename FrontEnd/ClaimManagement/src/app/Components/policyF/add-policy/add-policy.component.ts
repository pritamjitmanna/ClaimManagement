import { Component, ViewChild } from '@angular/core';
import { globalModules, globalVariables } from '../../../global_module';
import { NgFor } from '@angular/common';
import { NgForm } from '@angular/forms';
import { ClaimsService } from '../../../Services/claims.service';
import { AccessoriesService } from '../../../Services/accessories.service';
import { Router } from '@angular/router';
import { CommonOutput } from '../../../Models/common-output.model';
import { RESULT } from '../../../Models/e.enum';

@Component({
  selector: 'app-add-policy',
  standalone: true,
  imports: [globalModules],
  providers:[ClaimsService],
  templateUrl: './add-policy.component.html',
  styleUrl: './add-policy.component.css'
})
export class AddPolicyComponent {


  @ViewChild("addPolicy") addPolicy!:NgForm;
  policyUserId:string="";
  fieldValidations:{
    [key:string]:string
  }={
    InsuredFirstName:"",
    InsuredLastName:"",
    DateOfInsurance:"",
    VehicleNo:"",
  }

  constructor(private claimsService:ClaimsService,private accessoriesService:AccessoriesService,private router:Router){
    globalVariables.userId.subscribe(
      (id:string)=>{
        this.policyUserId=id;
      }
    )
  }




  async submitPolicy(){
    // console.log(this.addPolicy.value);
    let result:CommonOutput=await this.claimsService.addNewPolicy(this.addPolicy.value)
    console.log(result);
    if(result.result===RESULT.SUCCESS){
      this.accessoriesService.alertShow(`Congratulations! Your new policy is created. The Policy No is: ${result.output}`,"success")
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
          this.addPolicy.controls[prop['property']].setErrors({isErr:true})
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
