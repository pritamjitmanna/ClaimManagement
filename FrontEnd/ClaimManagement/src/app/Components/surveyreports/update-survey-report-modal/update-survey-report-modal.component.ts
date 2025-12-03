import { Component, Input, ViewChild } from '@angular/core';
import { globalModules } from '../../../global_module';
import { SurveyReport } from '../../../Models/survey-report.model';
import { NgForm } from '@angular/forms';
import { SurveyorService } from '../../../Services/surveyor.service';
import { AccessoriesService } from '../../../Services/accessories.service';
import { Router } from '@angular/router';

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

  isModelOpen:boolean=false
  fieldValidations:{
    [key:string]:string
  }={
    "LabourCharges":"",
    "PartsCost":"",
    "DepreciationCost":"",
    "AccidentDetails":""
  }


  constructor(private surveyorService:SurveyorService,private accessoriesService:AccessoriesService,private router:Router){}


  onSubmit(){
    
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
