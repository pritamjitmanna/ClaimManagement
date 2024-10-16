import { EventEmitter, Injectable } from "@angular/core";
import { globalVariables } from "../global_module";
import { HttpClient } from "@angular/common/http";
import { Surveyor } from "../Models/surveyor.model";
import { firstValueFrom } from "rxjs";
import { CommonOutput } from "../Models/common-output.model";
import { RESULT } from "../Models/e.enum";


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

    
}