import { Injectable } from "@angular/core";
import { globalVariables } from "../global_module";
import { HttpClient } from "@angular/common/http";
import { CommonOutput } from "../Models/common-output.model";
import { RESULT } from "../Models/e.enum";
import { ClaimStatusReport } from "../Models/claim-status-reports.model";
import { firstValueFrom } from "rxjs";
import { PaymentStatusReport } from "../Models/payment-status-reports.model";


@Injectable()
export class IrdaService{
    private BASE_URL="http://localhost:5179/IRDA/"
    private header:{[key:string]:string}={"Authorization":`Bearer ${globalVariables.token}`}

    constructor(private http:HttpClient){}

    async getClaimStatus(month:number,year:number):Promise<CommonOutput>{
        const URL=this.BASE_URL+`claimStatus/report/${month}/${year}`;
        try{
            const claimStatusReport:ClaimStatusReport[]=await firstValueFrom(this.http.get<ClaimStatusReport[]>(URL,{headers:this.header}));
            return new CommonOutput(RESULT.SUCCESS,claimStatusReport);
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err);
        }
    }

    async getPaymentStatus(month:number,year:number):Promise<CommonOutput>{
        const URL=this.BASE_URL+`paymentStatus/report/${month}/${year}`;
        try{
            const paymentStatusReport:PaymentStatusReport=await firstValueFrom(this.http.get<PaymentStatusReport>(URL,{headers:this.header}));
            return new CommonOutput(RESULT.SUCCESS,paymentStatusReport);
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err);
        }
    }

    async pullClaimStatus(month:number,year:number):Promise<CommonOutput>{

        //In the backend there is a badrequest 400 returned which is the invalid month and year error result.

        const URL=this.BASE_URL+`claimStatus/pull/${month}/${year}`;
        try{
            const claimStatusReport:ClaimStatusReport[]=await firstValueFrom(this.http.get<ClaimStatusReport[]>(URL,{headers:this.header}));
            return new CommonOutput(RESULT.SUCCESS,claimStatusReport);
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err);
        }
    }

    async pullPaymentStatus(month:number,year:number):Promise<CommonOutput>{

        //In the backend there is a badrequest 400 returned which is the invalid month and year error result which is already covered and also if the value retrieved is not following the requirements in IRDA, which is mainly a backend problem.
        const URL=this.BASE_URL+`paymentStatus/pull/${month}/${year}`;
        try{
            const paymentStatusReport:PaymentStatusReport=await firstValueFrom(this.http.get<PaymentStatusReport>(URL,{headers:this.header}));
            return new CommonOutput(RESULT.SUCCESS,paymentStatusReport);
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err);
        }
    }
}