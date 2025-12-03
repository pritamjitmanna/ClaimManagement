import { Component, ViewChild } from '@angular/core';
import { SurveyorService } from '../../../Services/surveyor.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { CommonOutput } from '../../../Models/common-output.model';
import { RESULT } from '../../../Models/e.enum';
import { SurveyReport } from '../../../Models/survey-report.model';
import { AccessoriesService } from '../../../Services/accessories.service';
import { UpdateSurveyReportModalComponent } from "../update-survey-report-modal/update-survey-report-modal.component";

@Component({
  selector: 'app-survey-report',
  standalone: true,
  imports: [UpdateSurveyReportModalComponent],
  providers:[SurveyorService],
  templateUrl: './survey-report.component.html',
  styleUrl: './survey-report.component.css'
})
export class SurveyReportComponent {

  isLoading:boolean=false;
  surveyReport!:SurveyReport;
  @ViewChild(UpdateSurveyReportModalComponent) updateSurveyReportModal!:UpdateSurveyReportModalComponent;

  constructor(private surveyorService:SurveyorService,private accessoriesService:AccessoriesService,private route:ActivatedRoute,private router:Router){
    this.isLoading=true;
    route.params.subscribe(
      async(params:Params)=>{
        const claimId=params["id"];
        let result:CommonOutput=await surveyorService.getSurveyReportByClaimId(claimId);

        if(result.result===RESULT.SUCCESS){
          this.surveyReport=result.output;
        }
        else{
          if(result.output.status===0||result.output.status>=500){
            router.navigate(['internalservererror']);
          }
          else{
            accessoriesService.alertShow("Survey Report Not found","danger");
            router.navigate(['']);
          }
        }
        this.isLoading=false;
      } 
    )

  }

  openModal(){
    this.updateSurveyReportModal.openModal();
  }

}
