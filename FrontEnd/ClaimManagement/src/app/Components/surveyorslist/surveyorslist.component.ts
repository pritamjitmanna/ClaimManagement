import { Component, OnInit } from '@angular/core';
import { globalModules } from '../../global_module';
import { SurveyorService } from '../../Services/surveyor.service';
import { Surveyor } from '../../Models/surveyor.model';
import { Router } from '@angular/router';
import { AccessoriesService } from '../../Services/accessories.service';
import { RESULT } from '../../Models/e.enum';

@Component({
  selector: 'app-surveyorslist',
  standalone: true,
  imports: [globalModules],
  providers:[SurveyorService],
  templateUrl: './surveyorslist.component.html',
  styleUrl: './surveyorslist.component.css'
})
export class SurveyorslistComponent implements OnInit{
  public estimatedLoss!:number;
  public surveyors: Surveyor[] = [];
  public isSidebarOpen: boolean = false;

  constructor(private surveyorService:SurveyorService,private accessoriesService:AccessoriesService,private router:Router) {
    
  }

  ngOnInit():void {

    this.accessoriesService.surveyorEstimatedLossEmitter.subscribe(
      async (data:number)=>{
        this.estimatedLoss=data
        this.openSidebar();
        let result=await this.surveyorService.getSurveyorsOnEstimatedLoss(data);

        if(result.result===RESULT.SUCCESS){
          this.surveyors=(result.output!==null?result.output:[])
          console.log(this.surveyors)
        }
        else{
          if(result.output.status===0||result.output.status>=500){
            this.router.navigate(['internalservererror'])
          }
          else if(result.output.status===403){
            //Forbidden
          }
        }
      }
    )
  }

  openSidebar() {
    this.isSidebarOpen = true;
  }

  closeSidebar() {
    this.isSidebarOpen = false;
  }
}
