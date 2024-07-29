import { LowerCasePipe } from "@angular/common";
import { Component } from "@angular/core";
import { Router, RouterLink } from "@angular/router";
import { globalModules } from "../../global_module";
import { ClaimsService } from "../../Services/claims.service";
import { ClaimDetail } from "../../Models/claim-detail.model";
import { CommonOutput } from "../../Models/common-output.model";
import { RESULT } from "../../Models/e.enum";
import { LoadingComponent } from "../loading/loading.component";


@Component({
    selector:'app-claims',
    standalone:true,
    imports:[globalModules,LoadingComponent],
    providers:[ClaimsService],
    templateUrl:'./claims.component.html',
    styleUrl:'./claims.component.css'
})
export class ClaimsComponent{
    
    claims:ClaimDetail[]=[]
    openClaims:ClaimDetail[]|undefined=undefined
    closedClaims:ClaimDetail[]|undefined=undefined
    tab:string="Open"

    isLoading=false

    constructor(private claimsService:ClaimsService,private router:Router){
        this.onClick('Open')
    }


    async onClick(type:string){
        this.tab=type
        this.isLoading=true

        if(this.tab==='Open'){
            if(this.openClaims===undefined){
                await this.getAllOpenClaims()
            }
            this.claims=this.openClaims!
        }
        else{
            if(this.closedClaims===undefined){
                await this.getAllClosedClaims()
            }
            this.claims=this.closedClaims!
        }
        this.isLoading=false
    }

    async getAllOpenClaims(){
        let output:CommonOutput=await this.claimsService.getOpenClaims()

        if(output.result===RESULT.SUCCESS){
            this.openClaims=output.output===null?[]:output.output
        }
        else{
            if(output.output.status===0||output.output.status>=500){
                this.router.navigate(['internalservererror'])
            }
        }
    }

    async getAllClosedClaims(){
        let output:CommonOutput=await this.claimsService.getClosedClaims()

        if(output.result===RESULT.SUCCESS){
            this.closedClaims=output.output===null?[]:output.output
        }
        else{
            if(output.output.status===0||output.output.status>=500){
                this.router.navigate(['internalservererror'])
            }
        }
    }

}
