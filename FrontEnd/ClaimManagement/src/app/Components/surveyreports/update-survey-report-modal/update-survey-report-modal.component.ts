import { Component, Input, ViewChild } from '@angular/core';
import { globalModules } from '../../../global_module';
import { SurveyReport } from '../../../Models/survey-report.model';
import { NgForm } from '@angular/forms';
import { SurveyorService } from '../../../Services/surveyor.service';
import { AccessoriesService } from '../../../Services/accessories.service';
import { Router } from '@angular/router';
import { UpdateSurveyReport } from '../../../Models/update-survey-report.model';
import { RESULT } from '../../../Models/e.enum';

@Component({
  selector: 'app-update-survey-report-modal',
  standalone: true,
  imports: [globalModules],
  templateUrl: './update-survey-report-modal.component.html',
  styleUrl: './update-survey-report-modal.component.css'
})
export class UpdateSurveyReportModalComponent {

  @Input() surveyReport!:SurveyReport;
  @ViewChild('updateSurveyReport') updateSurveyReport!:NgForm;
  @Input() setNewValues!:(details:UpdateSurveyReport)=>void
  isModelOpen:boolean=false
  isTotalAmountError:boolean=false
  fieldValidations:{
    [key:string]:string
  }={
    "LabourCharges":"",
    "PartsCost":"",
    "DepreciationCost":"",
    "AccidentDetails":"",
    "TotalAmount":""
  }


  constructor(private surveyorService:SurveyorService,private accessoriesService:AccessoriesService,private router:Router){}


  async onSubmit(){
    let details:UpdateSurveyReport={
      labourCharges:this.updateSurveyReport.value['LabourCharges']===this.surveyReport.labourCharges?null:this.updateSurveyReport.value['LabourCharges'],
      partsCost:this.updateSurveyReport.value['PartsCost']===this.surveyReport.partsCost?null:this.updateSurveyReport.value['PartsCost'],
      depreciationCost:this.updateSurveyReport.value['DepreciationCost']===this.surveyReport.depreciationCost?null:this.updateSurveyReport.value['DepreciationCost'],
      accidentDetails:this.updateSurveyReport.value['AccidentDetails']===this.surveyReport.accidentDetails?null:this.updateSurveyReport.value['AccidentDetails']
    }
    let result=await this.surveyorService.updateSurveyReport(this.surveyReport.claimId,details)
    if(result.result===RESULT.SUCCESS){
      this.accessoriesService.alertShow(`Survey Report for Claim ID: ${this.surveyReport.claimId} has been updated`,"success")
      this.setNewValues(details)
      this.closeModal()
    }
    else{
      let err_status=result.output.status
      let error_block=result.output.error.output
      if(err_status===0||err_status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(err_status===400){
        console.log(error_block)
        for(let prop of error_block){
          if(prop['property']==="TotalAmount"){
            this.isTotalAmountError=true
            setTimeout(() => {
              this.isTotalAmountError=false
            }, 3000);
          }
          else this.updateSurveyReport.controls[prop['property']].setErrors({isErr:true})
            this.fieldValidations[prop['property']]=prop['errorMessage']
        }
      }
      else{
        this.router.navigate([''])
        this.accessoriesService.alertShow("Unauthorized","danger")
      }
    }
  }

  openModal(){
    this.isModelOpen=true
    setTimeout(() => {   
      this.updateSurveyReport.form.patchValue({
        "LabourCharges":this.surveyReport.labourCharges,
        "PartsCost":this.surveyReport.partsCost,
        "DepreciationCost":this.surveyReport.depreciationCost,
        "AccidentDetails":this.surveyReport.accidentDetails
      })
    }, 100);
  }
  

  closeModal(){
    this.isModelOpen=false
  }


}
