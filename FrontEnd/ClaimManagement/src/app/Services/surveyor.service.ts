import { EventEmitter, Injectable } from "@angular/core";
import { globalVariables } from "../global_module";
import { HttpClient } from "@angular/common/http";
import { Surveyor } from "../Models/surveyor.model";
import { firstValueFrom } from "rxjs";
import { CommonOutput } from "../Models/common-output.model";
import { RESULT } from "../Models/e.enum";
import { SurveyReport } from "../Models/survey-report.model";
import { UpdateSurveyReport } from "../Models/update-survey-report.model";


@Injectable()
export class SurveyorService{


    private BASE_URL="http://localhost:5179/api/"
    private header:{[key:string]:string}={"Authorization":`Bearer ${globalVariables.token}`}

    

    constructor(private http:HttpClient){}

    async getSurveyorsOnEstimatedLoss(estimatedLoss:number):Promise<CommonOutput>{
        try{
            const URL=this.BASE_URL+`surveyors/${estimatedLoss}`;
            const surveyors:Surveyor[]=await firstValueFrom(this.http.get<Surveyor[]>(URL,{headers:this.header}));
            return new CommonOutput(RESULT.SUCCESS,surveyors);
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err);
        }
    }

    async getSurveyReportByClaimId(claimId:string):Promise<CommonOutput>{
        try{
            const URL=this.BASE_URL+`surveyReport/${claimId}`;
            const surveyReport:SurveyReport=await firstValueFrom(this.http.get<SurveyReport>(URL,{headers:this.header}));
            return new CommonOutput(RESULT.SUCCESS,surveyReport);
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err);
        }
    }

    async addSurveyReport(surveyReport:SurveyReport):Promise<CommonOutput>{
        try{
            const URL=this.BASE_URL+`surveyors/new`;
            const output:CommonOutput=await firstValueFrom(this.http.post<CommonOutput>(URL,surveyReport,{headers:this.header}));
            return new CommonOutput(RESULT.SUCCESS,output);
        }
        catch(err:any){
            return new CommonOutput(RESULT.FAILURE,err);
        }
    }

    async updateSurveyReport(claimId:string,details:UpdateSurveyReport):Promise<CommonOutput>{
        

        return new CommonOutput(RESULT.SUCCESS,{});
    }

    
}