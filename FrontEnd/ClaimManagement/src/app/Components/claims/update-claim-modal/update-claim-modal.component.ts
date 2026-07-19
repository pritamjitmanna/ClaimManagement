import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { ClaimDetail } from '../../../Models/claim-detail.model';
import { ClaimStatus, RESULT } from '../../../Models/e.enum';
import { globalModules } from '../../../global_module';
import { NgForm } from '@angular/forms';
import { ClaimsService } from '../../../Services/claims.service';
import { CommonOutput } from '../../../Models/common-output.model';
import { UpdateClaim } from '../../../Models/update-claim.model';
import { AccessoriesService } from '../../../Services/accessories.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-update-claim-modal',
  standalone: true,
  imports: [globalModules],
  templateUrl: './update-claim-modal.component.html',
  styleUrl: './update-claim-modal.component.css'
})
export class UpdateClaimModalComponent {
  @Input() claim!: ClaimDetail;
  @Input() setNewValues!:(details:UpdateClaim)=>void
  @ViewChild('updateClaim') updateClaim!:NgForm;
  isModelOpen:boolean=false
  fieldValidations:{
    [key:string]:string
  }={
    SurveyorUserId:""
  }

  constructor(private claimsService:ClaimsService,private accessoriesService:AccessoriesService,private router:Router){
    this.accessoriesService.surveyorDetailsEmitter.subscribe(
      (data:{surveyorUserId:string, toShow:string})=>{
        //For now, we are not using toShow property but in future if we want to show surveyor name or any other form in the input field then we can use it
        this.updateClaim.form.patchValue({
          "SurveyorUserId":data.surveyorUserId
        })
      }
    )
  }

  claimStatusOptions:ClaimStatus[] = Object.values(ClaimStatus);

  async onSubmit() {
    let details:UpdateClaim={
      ClaimStatus:this.updateClaim.value['ClaimStatus']===this.claim.claimStatus?null:this.updateClaim.value['ClaimStatus'],
      SurveyorUserId:this.updateClaim.value['SurveyorUserId']===this.claim.surveyorUserId?null:this.updateClaim.value['SurveyorUserId'],
      InsuranceCompanyApproval:this.updateClaim.value['InsuranceCompanyApproval']===this.claim.insuranceCompanyApproval?null:this.updateClaim.value['InsuranceCompanyApproval']
    }
    let result:CommonOutput=await this.claimsService.updateClaim(this.claim.claimId,details)
    if(result.result===RESULT.SUCCESS){
      if(details.ClaimStatus===null&&details.SurveyorUserId===null&&details.InsuranceCompanyApproval===null){
        this.accessoriesService.alertShow("No changes were made to the claim","warning")
      }
      else{
        this.accessoriesService.alertShow(`Your Claim with id:${this.claim.claimId} has been updated`,"success")
      }
      this.setNewValues(details)
      this.closeModal()
    }
    else{
      let err_status=result.output.status
      let error_block=result.output.error.output

      if(err_status===0||err_status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(err_status===400){
        for(let prop of error_block){
          this.updateClaim.controls[prop['property']].setErrors({isErr:true})
          this.fieldValidations[prop['property']]=prop['errorMessage']
        }
      }
      else{
        this.router.navigate([''])
        this.accessoriesService.alertShow("Unauthorized","danger")
      }
    }
  }

  openModal(){
    this.isModelOpen=true
    setTimeout(() => {
      this.updateClaim.form.patchValue({
        "ClaimStatus":this.claim.claimStatus,
        "InsuranceCompanyApproval":this.claim.insuranceCompanyApproval
      })
      if(this.claim.surveyorUserId===null){}
      else{
        this.updateClaim.form.patchValue({
          "SurveyorUserId":this.claim.surveyorUserId
        })
      }
    }, 100);
    
  }

  closeModal(){
    this.isModelOpen=false
    this.accessoriesService.emitEstimatedLossValue(-1)
  }

  getSurveyors(){
    
    this.accessoriesService.emitEstimatedLossValue(this.claim.estimatedLoss)
  }

}
