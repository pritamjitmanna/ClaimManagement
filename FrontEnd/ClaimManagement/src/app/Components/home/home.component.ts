import { Component, OnDestroy } from '@angular/core';
import { globalModules, globalVariables } from '../../global_module';
import { Router } from '@angular/router';
import { SurveyorService } from '../../Services/surveyor.service';
import { AccessoriesService } from '../../Services/accessories.service';
import { combineLatest, firstValueFrom, Subject, takeUntil, withLatestFrom } from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [globalModules],
  providers:[SurveyorService],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent{
  title="Claim Management"
  roles:string[]=[]
  constructor(private accessoriesService:AccessoriesService,private router:Router){
    globalVariables.role.subscribe((roles:string[])=>{
      this.roles=roles
    })
  }


  fetchClaim(claimId:string){
    this.router.navigate(['claim',claimId])
  }

  fetchSurveyors(estimatedLoss:string){
    this.accessoriesService.emitEstimatedLossValue(parseInt(estimatedLoss))
  }

  fetchSurveyReport(claimId:string){
    this.router.navigate(['surveyreport',claimId])
  }
}
