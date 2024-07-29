import { Component, ViewChild } from '@angular/core';
import { globalModules } from '../../../global_module';
import { NgForm } from '@angular/forms';
import { ClaimsService } from '../../../Services/claims.service';
import { CommonOutput } from '../../../Models/common-output.model';
import { RESULT } from '../../../Models/e.enum';
import { AccessoriesService } from '../../../Services/accessories.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-claim',
  standalone: true,
  providers:[ClaimsService,AccessoriesService],
  imports: [globalModules],
  templateUrl: './add-claim.component.html',
  styleUrl: './add-claim.component.css'
})
export class AddClaimComponent {

  @ViewChild("addClaim") addClaim!:NgForm
  fieldValidations:{
    [key:string]:string
  }={
    EstimatedLoss:"",
    PolicyNo:"",
    DateOfAccident:"",
  }

  constructor(private claimsService:ClaimsService,private accessoriesService:AccessoriesService,private router:Router){}

  async submitClaim(){
    let result:CommonOutput=await this.claimsService.addClaim(this.addClaim.value)
    if(result.result===RESULT.SUCCESS){
      this.accessoriesService.alertShow(`Congratulations! Your new claim is created. The claim Id is: ${result.output}`,"success")
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
          this.addClaim.controls[prop['property']].setErrors({isErr:true})
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
