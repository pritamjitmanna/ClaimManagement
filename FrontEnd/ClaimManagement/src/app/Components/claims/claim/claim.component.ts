import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ClaimDetail } from '../../../Models/claim-detail.model';
import { ClaimsService } from '../../../Services/claims.service';
import { CommonOutput } from '../../../Models/common-output.model';
import { ClaimStatus, RESULT, WITHDRAWSTATUS } from '../../../Models/e.enum';
import { globalModules, globalVariables } from '../../../global_module';
import { LoadingComponent } from '../../loading/loading.component';
import { AccessoriesService } from '../../../Services/accessories.service';
import { UpdateClaimModalComponent } from '../update-claim-modal/update-claim-modal.component';
import { UpdateClaim } from '../../../Models/update-claim.model';

@Component({
  selector: 'app-claim',
  standalone: true,
  imports: [globalModules,LoadingComponent,UpdateClaimModalComponent],
  providers:[ClaimsService],
  templateUrl: './claim.component.html',
  styleUrl: './claim.component.css'
})
export class ClaimComponent {

  claim!:ClaimDetail
  isLoading:boolean=false
  @ViewChild(UpdateClaimModalComponent)updateClaim!:UpdateClaimModalComponent;
  roles:string[]=globalVariables.role.getValue()

  constructor(private claimsService:ClaimsService,private accessoriesService:AccessoriesService,private route:ActivatedRoute,private router:Router){
    this.isLoading=true
    route.params.subscribe(
      async(params:Params)=>{
        const claimId=params["id"]
        const rolesArray=await firstValueFrom(globalVariables.role);
        if(sessionStorage.getItem(claimId)===null){

          let result:CommonOutput=await claimsService.getClaimById(claimId)
          
          if(result.result===RESULT.SUCCESS){
            this.claim=result.output
            sessionStorage.setItem(claimId,JSON.stringify(this.claim))
          }
          else{
            if(result.output.status===0||result.output.status>=500){
              router.navigate(['internalservererror'])
            }
            else{
              accessoriesService.alertShow("Claim Not found","danger")
              router.navigate([''])
              
            }
          }
        }
        else this.claim=JSON.parse(sessionStorage.getItem(claimId)!)
        if(rolesArray.includes("Surveyor")){
          if(this.claim.surveyorID!==null){
            const surveyorId=await firstValueFrom(globalVariables.profileId);
            // console.log(surveyorId)
            if(this.claim.surveyorID!==surveyorId){
              accessoriesService.alertShow("Unauthorized Access","danger")
              router.navigate([''])
            }
          }
        }
        this.isLoading=false
      }
    )
  }

  async acceptRejectClaim(flag:boolean){
    console.log(flag)

    let result:CommonOutput=await this.claimsService.acceptOrRejectClaim(this.claim.claimId,{AcceptReject:flag})

    if(result.result===RESULT.SUCCESS){
      this.claim.withdrawClaim=(flag?WITHDRAWSTATUS.ACCEPTED:WITHDRAWSTATUS.WITHDRAWN)
      this.accessoriesService.alertShow(`Claim ${flag?"Accepted":"Rejected"}`,"success")
    }
    else{
      if(result.output.status===0||result.output.status>=500){
        this.router.navigate(['internalservererror'])
      }
    }
  }

  async releaseSurveyorFees(){
    let result:CommonOutput=await this.claimsService.releaseSurveyorFees(this.claim.claimId)

    //Gets the object {estimateStartLimit: 5000, estimateEndLimit: 10000, fees: 1000}
    if(result.result===RESULT.SUCCESS){
      this.claim.surveyorFees=result.output['fees']
      this.accessoriesService.alertShow(`Surveyor Fees ${result.output['fees']} released for the claim Id: ${this.claim.claimId}`,'success')
    }
    else{
      if(result.output.status===0||result.output.status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(result.output.status===400){
        // console.log(result.output)
        this.accessoriesService.alertShow(result.output.error.output[0]['errorMessage'],'danger')
      }
    }
  }

  createSurveyReport(){
    this.router.navigate([`createsurveyreport/${this.claim.claimId}`],{state:{navigated:true}})
  }

  openModal(){
    this.updateClaim.openModal()
  }

  setNewValues(details:UpdateClaim){
    this.claim.claimStatus=details.ClaimStatus===null?this.claim.claimStatus:details.ClaimStatus
    this.claim.surveyorID=details.SurveyorID===null?this.claim.surveyorID:details.SurveyorID
    this.claim.insuranceCompanyApproval=details.InsuranceCompanyApproval===null?this.claim.insuranceCompanyApproval:details.InsuranceCompanyApproval

    sessionStorage.setItem(this.claim.claimId,JSON.stringify(this.claim))
  }


}
