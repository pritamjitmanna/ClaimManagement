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
  fieldValidations:{[key:string]:string}={
    ClaimId:"",
    LabourCharges:"",
    PartsCost:"",
    PolicyClause:"",
    DepreciationCost:"",
    TotalAmount:"",
    AccidentDetails:""
  }

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
      this.accessoriesService.alertShow("Survey Report added successfully. Use the get survey report input field to get the report","success");
      this.router.navigate(['']);
    }
    else{
      let err_status=result.output.status
      let error_block=result.output.error.output
      if(err_status===0||err_status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(err_status===400){
        for(let prop of error_block){
          if(prop['property']==="TotalAmount")this.accessoriesService.alertShow(prop['errorMessage'],"danger")
          else{
            this.addSurveyReport.controls[prop['property']].setErrors({isErr:true})
            this.fieldValidations[prop['property']]=prop['errorMessage']
          }
        }
      }
      else{
        this.router.navigate([''])
        this.accessoriesService.alertShow("Unauthorized","danger")
      }   
    }
  }

}
