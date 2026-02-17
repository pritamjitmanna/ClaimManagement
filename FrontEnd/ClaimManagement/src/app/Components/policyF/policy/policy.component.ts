import { Component } from '@angular/core';
import { PolicyResponse } from '../../../Models/policy-response.model';
import { ClaimsService } from '../../../Services/claims.service';
import { AccessoriesService } from '../../../Services/accessories.service';
import { LoadingComponent } from '../../loading/loading.component';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonOutput } from '../../../Models/common-output.model';
import { RESULT } from '../../../Models/e.enum';
import { globalModules } from '../../../global_module';

@Component({
  selector: 'app-policy',
  standalone: true,
  imports: [globalModules,LoadingComponent],
  providers:[ClaimsService],
  templateUrl: './policy.component.html',
  styleUrl: './policy.component.css'
})
export class PolicyComponent {

  policy!:PolicyResponse;
  isLoading:boolean=false

  constructor(private claimsService:ClaimsService,private accessoriesService:AccessoriesService,private route:ActivatedRoute,private router:Router){
    this.isLoading=true
    route.params.subscribe(
      async(params)=>{
        const policyNo=params["id"]
        if(sessionStorage.getItem(policyNo)===null){
          let result:CommonOutput=await claimsService.getPolicyByPolicyNumber(policyNo)
          
          if(result.result===RESULT.SUCCESS){
            this.policy=result.output
            sessionStorage.setItem(policyNo,JSON.stringify(this.policy))
          }
          else{
            if(result.output.status===0||result.output.status>=500){
              router.navigate(['internalservererror'])
            }
            else{
              accessoriesService.alertShow("Policy doesn't exist","danger")
              router.navigate([''])
              
            }
          }
        }
        else this.policy=JSON.parse(sessionStorage.getItem(policyNo)!)
        this.isLoading=false
      }
    )
  }

}
