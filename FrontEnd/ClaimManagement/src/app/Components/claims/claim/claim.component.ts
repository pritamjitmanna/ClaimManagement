import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ClaimDetail } from '../../../Models/claim-detail.model';
import { ClaimsService } from '../../../Services/claims.service';
import { CommonOutput } from '../../../Models/common-output.model';
import { ClaimStatus, RESULT, WITHDRAWSTATUS } from '../../../Models/e.enum';
import { globalModules } from '../../../global_module';
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

  constructor(private claimsService:ClaimsService,private accessoriesService:AccessoriesService,private route:ActivatedRoute,private router:Router){
    this.isLoading=true
    route.params.subscribe(
      async(params:Params)=>{
        const claimId=params["id"]
        let result:CommonOutput=await claimsService.getClaimById(claimId)

        if(result.result===RESULT.SUCCESS){
          this.claim=result.output
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
        this.isLoading=false
      }
    )
  }

  acceptRejectClaim(flag:boolean){
    
  }

  openModal(){
    this.updateClaim.openModal()
  }

  setNewValues(details:UpdateClaim){
    this.claim.claimStatus=details.ClaimStatus===null?this.claim.claimStatus:details.ClaimStatus
    this.claim.surveyorID=details.SurveyorID===null?this.claim.surveyorID:details.SurveyorID
    this.claim.insuranceCompanyApproval=details.InsuranceCompanyApproval===null?this.claim.insuranceCompanyApproval:details.InsuranceCompanyApproval
  }


}
