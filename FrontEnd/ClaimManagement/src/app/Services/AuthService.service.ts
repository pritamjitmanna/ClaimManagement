import { HttpClient } from "@angular/common/http";
import { Injectable, Output } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { CommonOutput } from "../Models/common-output.model";
import { RESULT } from "../Models/e.enum";


@Injectable()
export class AuthService{

    private BASE_URL="http://localhost:5179/auth/"  
    
    constructor(private http:HttpClient){}

    async login(creds:{emailAddress:string,password:string}):Promise<CommonOutput>{


        const URL=this.BASE_URL+"login"
        try {
            const data = await firstValueFrom(this.http.post<{ emailAddress: string, password: string }>(URL, creds));
            return new CommonOutput(RESULT.SUCCESS,data)
        } catch (err:any) {
            return new CommonOutput(RESULT.FAILURE,err.status)
        }

    }


}