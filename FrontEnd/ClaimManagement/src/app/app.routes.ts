import { mapToCanActivate, Routes } from '@angular/router';
import { RegisterComponent } from './Components/register/register.component';
import { LoginComponent } from './Components/login/login.component';
import { HomeComponent } from './Components/home/home.component';
import { AddClaimComponent } from './Components/claims/add-claim/add-claim.component';
import { ClaimComponent } from './Components/claims/claim/claim.component';
import { ClaimsComponent } from './Components/claims/claims.component';
import { IrdaComponent } from './Components/irda/irda.component';
import { AddSurveyReportComponent } from './Components/surveyreports/add-survey-report/add-survey-report.component';
import { SurveyReportComponent } from './Components/surveyreports/survey-report/survey-report.component';
import { authGuard } from './Gaurds/auth.guard';
import { Notfound404Component } from './Components/notfound404/notfound404.component';
import { Internalservererror500Component } from './Components/internalservererror500/internalservererror500.component';

export const routes: Routes = [
    {
        path:'',
        component:HomeComponent
    },
    {
        path:'register',
        component:RegisterComponent
    },
    {
        path:'login',
        component:LoginComponent
    },
    {
        path:'addclaim',
        component:AddClaimComponent,
        canActivate:[authGuard],
        data:{
            role:[
                "Insurer"
            ]
        }   
    },
    {
        path:'claims',
        component:ClaimsComponent,
        canActivate:[authGuard],
        data:{
            role:[
                "InsuranceCompany"
            ]
        }
    },
    {
        path:'claim/:id',
        component:ClaimComponent,
        canActivate:[authGuard],
        data:{
            role:[
                "InsuranceCompany",
                "Insurer"
            ]
        }
    },
    {
        path:'irda',
        component:IrdaComponent,
        canActivate:[authGuard],
        data:{
            role:[
                "IRDA"
            ]
        }
    },
    {
        path:'createsurveyreport',
        component:AddSurveyReportComponent,
        canActivate:[authGuard],
        data:{
            role:[
                "Surveyor"
            ]
        }
    },
    {
        path:'surveyreport',
        component:SurveyReportComponent,
        canActivate:[authGuard],
        data:{
            role:[
                "Surveyor"
            ]
        }
    },
    {
        path:'notfound',
        component:Notfound404Component
    },
    {
        path:'internalservererror',
        component:Internalservererror500Component
    }
];
