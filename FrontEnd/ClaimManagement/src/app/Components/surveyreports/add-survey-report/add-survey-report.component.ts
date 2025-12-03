import { Component, ViewChild } from '@angular/core';
import { SurveyorService } from '../../../Services/surveyor.service';
import { AccessoriesService } from '../../../Services/accessories.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { globalModules } from '../../../global_module';
import { CommonOutput } from '../../../Models/common-output.model';
import { NgForm } from '@angular/forms';
import { RESULT } from '../../../Models/e.enum';
import { ClaimDetail } from '../../../Models/claim-detail.model';

@Component({
  selector: 'app-add-survey-report',
  standalone: true,
  imports: [globalModules],
  providers:[SurveyorService],
  templateUrl: './add-survey-report.component.html',
  styleUrl: './add-survey-report.component.css'
})
export class AddSurveyReportComponent {


  @ViewChild("addSurveyReport") addSurveyReport!:NgForm;
  claim!:ClaimDetail;

  constructor(private surveyorService:SurveyorService,private accessoriesService:AccessoriesService,private router:Router,private route:ActivatedRoute){
    route.params.subscribe(
      (params:Params)=>{
        let claimId:string=params['id'];
        this.claim=JSON.parse(sessionStorage.getItem(claimId)!);
      }
    )
  }


  async onSubmit(){
    let result:CommonOutput=await this.surveyorService.addSurveyReport(this.addSurveyReport.value);

    if(result.result===RESULT.SUCCESS){
      this.accessoriesService.alertShow("Survey Report added successfully","success");
      this.router.navigate(['']);
    }
    else{
      console.log(result.output);
    }
  }

}
