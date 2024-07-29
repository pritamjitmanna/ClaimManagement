import { HttpClient } from "@angular/common/http";
import { EventEmitter, Injectable, Output } from "@angular/core";
import {jwtDecode} from 'jwt-decode';
import { BehaviorSubject, count, firstValueFrom, Observable } from "rxjs";


import { CommonOutput } from "../Models/common-output.model";
import { RESULT } from "../Models/e.enum";
import { Router } from "@angular/router";
import { globalVariables } from "../global_module";



@Injectable({
    providedIn:"root"
})
export class AuthService{
    private JWTObj:{token:string,expiration:Date}={token:"",expiration:new Date()}

    private BASE_URL="http://localhost:5179/auth/"  
    
    constructor(private http:HttpClient,private router:Router){}


    async login(creds:{emailAddress:string,password:string}):Promise<CommonOutput>{

        const URL=this.BASE_URL+"login"
        try {
            const data = await firstValueFrom(this.http.post<{ emailAddress: string, password: string }>(URL, creds));
            localStorage.setItem("loginData",JSON.stringify(data))
            globalVariables.isAuthenticated.next(true);
            this.decodeTokenUserRole()
            return new CommonOutput(RESULT.SUCCESS)
        } catch (err:any) {
            return new CommonOutput(RESULT.FAILURE,err)
        }
        
    }
    
    logout(){
        localStorage.removeItem("loginData")
        globalVariables.isAuthenticated.next(false);
        this.router.navigate([''])
    }
    
    decodeTokenUserRole(){
        try{
            const json=localStorage.getItem('loginData')
            if(json===null){
                this.router.navigate(['/login'])
                return
            }
            this.JWTObj=JSON.parse(json)
            if(new Date()>new Date(this.JWTObj.expiration)){
                localStorage.removeItem('loginData')
                this.router.navigate(['/login'])
                return
            }

            globalVariables.isAuthenticated.next(true);
            globalVariables.token=this.JWTObj["token"]
            const decodedToken:{[key:string]: string}=jwtDecode(this.JWTObj["token"])
            globalVariables.username.next(decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"])
            var temp=decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
            let roles;
            if(typeof(temp)==='string')roles=[temp]
            else roles=temp
            globalVariables.role.next(roles)
            
          }
          catch{
            this.router.navigate([''])
          }
    }


}