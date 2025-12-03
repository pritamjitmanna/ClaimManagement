import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ClaimDetail } from "../Models/claim-detail.model";
import { first, firstValueFrom, map } from "rxjs";
import { CommonOutput } from "../Models/common-output.model";
import { ClaimStatus, RESULT } from "../Models/e.enum";
import { globalVariables } from "../global_module";
import { UpdateClaim } from "../Models/update-claim.model";

@Injectable()
export class ClaimsService{

    private BASE_URL="http://localhost:5179/api/"
    private header:{[key:string]:string}={"Authorization":`Bearer ${globalVariables.token}`}


    constructor(private http:HttpClient){}

    async getOpenClaims():Promise<CommonOutput>{
        try{
            const URL=this.BASE_URL+"claims"
            const openClaims:ClaimDetail[]=await firstValueFrom(this.http.get<ClaimDetail[]>(URL,{headers:this.header}).pipe(
                map(res=>res.map(response=>this.assignNull(response)))
            ))
            return new CommonOutput(RESULT.SUCCESS,openClaims)
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err)
        }
    }

    async getClosedClaims(){

        try{
            const URL=this.BASE_URL+"claims/closed"
            const closedClaims:ClaimDetail[]=await firstValueFrom(this.http.get<ClaimDetail[]>(URL,{headers:this.header}).pipe(
                map(res=>res.map(response=>this.assignNull(response)))
            ))
            return new CommonOutput(RESULT.SUCCESS,closedClaims)
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err.status)
        }
    }

    async getClaimById(claimId:string){
        try{
            const URL=this.BASE_URL+`claims/${claimId}`
            const claim:ClaimDetail=await firstValueFrom(this.http.get<ClaimDetail>(URL,{headers:this.header}).pipe(
                map(response=>this.assignNull(response))
            ))
            return new CommonOutput(RESULT.SUCCESS,claim)
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err)
        }
    }


    async addClaim(details:{PolicyNo:string,EstimatedLoss:number,DateOfAccident:Date}){
        try{
            let URL=this.BASE_URL+'claims/'
            if(globalVariables.role.value.includes("InsuranceCompany"))URL+='new'
            else URL+='addclaim'
            const result:CommonOutput=await firstValueFrom(this.http.post<CommonOutput>(URL,details,{headers:this.header}))
            return result
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err)
        }
    }

    async updateClaim(claimId:string,details:UpdateClaim){
        try{
            let URL=this.BASE_URL+`claims/${claimId}/update`
            const result:CommonOutput=await firstValueFrom(this.http.put<CommonOutput>(URL,details,{headers:this.header}))
            return result
        }   
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err)
        }
    }

    async acceptOrRejectClaim(claimId:string,details:{AcceptReject:boolean}){
        try{
            let URL=this.BASE_URL+`claims/${claimId}`
            const result=await firstValueFrom(this.http.patch<any>(URL,details,{headers:this.header}))
            return new CommonOutput(RESULT.SUCCESS)
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err)
        }
    }

    async releaseSurveyorFees(claimId:string){

        try{
            let URL=this.BASE_URL+`surveyorfees/${claimId}`
            const result=await firstValueFrom(this.http.patch<any>(URL,null,{headers:this.header}))
            return result;
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err)
        }


    }

    


    private assignNull=(response:ClaimDetail)=>new ClaimDetail(
        response.claimId,
        response.policyNo,
        response.estimatedLoss,
        response.dateOfAccident,
        response.amtApprovedBySurveyor,
        response.insuranceCompanyApproval,
        response.withdrawClaim,
        response.claimStatus,
        response.surveyorID ?? null,
        response.surveyorFees ?? null
    )

}