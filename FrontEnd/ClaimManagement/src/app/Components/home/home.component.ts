import { Component } from '@angular/core';
import { globalModules } from '../../global_module';
import { Router } from '@angular/router';
import { SurveyorService } from '../../Services/surveyor.service';
import { AccessoriesService } from '../../Services/accessories.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [globalModules],
  providers:[SurveyorService],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  title="Claim Management"

  constructor(private accessoriesService:AccessoriesService,private router:Router){}

  fetchClaim(claimId:string){
    this.router.navigate(['claim',claimId])
  }

  fetchSurveyors(estimatedLoss:string){
    this.accessoriesService.emitEstimatedLossValue(parseInt(estimatedLoss))
  }
}
