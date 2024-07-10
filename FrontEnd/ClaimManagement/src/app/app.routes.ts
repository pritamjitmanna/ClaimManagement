import { Routes } from '@angular/router';
import { RegisterComponent } from './Components/register/register.component';
import { LoginComponent } from './Components/login/login.component';
import { HomeComponent } from './Components/home/home.component';
import { AddClaimComponent } from './Components/claims/add-claim/add-claim.component';
import { ClaimComponent } from './Components/claims/claim/claim.component';
import { ClaimsComponent } from './Components/claims/claims.component';
import { IrdaComponent } from './Components/irda/irda.component';
import { AddSurveyReportComponent } from './Components/surveyreports/add-survey-report/add-survey-report.component';
import { SurveyReportComponent } from './Components/surveyreports/survey-report/survey-report.component';

export const routes: Routes = [
    {path:'',component:HomeComponent},
    {path:'register',component:RegisterComponent},
    {path:'login',component:LoginComponent},
    {path:'addclaim',component:AddClaimComponent},
    {path:'claims',component:ClaimsComponent},
    {path:'claim',component:ClaimComponent},
    {path:'irda',component:IrdaComponent},
    {path:'createsurveyreport',component:AddSurveyReportComponent},
    {path:'surveyreport',component:SurveyReportComponent}
];
