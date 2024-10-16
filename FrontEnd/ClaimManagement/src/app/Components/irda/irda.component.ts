import { Component, ViewChild } from '@angular/core';
import { globalModules } from '../../global_module';
import { ClaimStatusReport } from '../../Models/claim-status-reports.model';
import { PaymentStatusReport } from '../../Models/payment-status-reports.model';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { CommonOutput } from '../../Models/common-output.model';
import { IrdaService } from '../../Services/irda.service';
import { NgForm } from '@angular/forms';
import { RESULT } from '../../Models/e.enum';
import { Router } from '@angular/router';
import { AccessoriesService } from '../../Services/accessories.service';

@Component({
  selector: 'app-irda',
  standalone: true,
  imports: [globalModules],
  providers:[IrdaService],
  animations: [
    trigger('slideInOut', [
      state('void', style({ height: '0px', overflow: 'hidden' })),
      state('*', style({ height: '*', overflow: 'hidden' })),
      transition(':enter', [style({ height: '0px', overflow: 'hidden' }), animate('300ms ease-out')]),
      transition(':leave', [animate('300ms ease-in', style({ height: '0px', overflow: 'hidden' }))])
    ])
  ],
  templateUrl: './irda.component.html',
  styleUrl: './irda.component.css'
})
export class IrdaComponent {

  @ViewChild('MonthYear') MonthYear!:NgForm

  showStatusReport: boolean = false;
  showPaymentReport:boolean=false;

  claimStatusReports:ClaimStatusReport[]=[]
  paymentStatusReport!:PaymentStatusReport

  constructor(private irdaService:IrdaService,private accessoriesService:AccessoriesService,private router:Router){}

  async getPaymentStatus(){
    this.showStatusReport=false;

    let month:number=Number(this.MonthYear.value['month-year'].split('-')[1])
    let year:number=Number(this.MonthYear.value['month-year'].split('-')[0])
    
    let result:CommonOutput=await this.irdaService.getPaymentStatus(month,year)
    if(result.result==RESULT.SUCCESS){
      this.paymentStatusReport=result.output
    }
    else{
      let err_status=result.output.status
      let error_block=result.output.error.output
      if(err_status===0||err_status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(err_status===403){
        this.router.navigate([''])
        this.accessoriesService.alertShow("Unauthorized","danger")
      }
    }

    this.showPaymentReport=!this.showPaymentReport
  }

  async getClaimStatus(){
    this.showPaymentReport=false;

    let month:number=Number(this.MonthYear.value['month-year'].split('-')[1])
    let year:number=Number(this.MonthYear.value['month-year'].split('-')[0])
    
    let result:CommonOutput=await this.irdaService.getClaimStatus(month,year)
    if(result.result==RESULT.SUCCESS){
      this.claimStatusReports=result.output
    }
    else{
      let err_status=result.output.status
      let error_block=result.output.error.output
      if(err_status===0||err_status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(err_status===403){
        this.router.navigate([''])
        this.accessoriesService.alertShow("Unauthorized","danger")
      }
    }

    this.showStatusReport=!this.showStatusReport
  }


  async pullPaymentStatus(){
    let month:number=Number(this.MonthYear.value['month-year'].split('-')[1])
    let year:number=Number(this.MonthYear.value['month-year'].split('-')[0])
    
    let result:CommonOutput=await this.irdaService.pullPaymentStatus(month,year)
    if(result.result==RESULT.SUCCESS){
      this.accessoriesService.alertShow(`Data related to payment status is successfully fetched to the database. Use 'Payment Status' to check`,"success")
    }
    else{
      let err_status=result.output.status
      let error_block=result.output.error.output
      if(err_status===0||err_status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(err_status===403){
        this.router.navigate([''])
        this.accessoriesService.alertShow("Unauthorized","danger")
      }
    }
  }

  async pullClaimStatus(){
    let month:number=Number(this.MonthYear.value['month-year'].split('-')[1])
    let year:number=Number(this.MonthYear.value['month-year'].split('-')[0])
    
    let result:CommonOutput=await this.irdaService.pullClaimStatus(month,year)
    if(result.result==RESULT.SUCCESS){
      this.accessoriesService.alertShow(`Data related to claim status is successfully fetched to the database. Use 'Claim Status' to check`,"success")
    }
    else{
      let err_status=result.output.status
      let error_block=result.output.error.output
      if(err_status===0||err_status>=500){
        this.router.navigate(['internalservererror'])
      }
      else if(err_status===403){
        this.router.navigate([''])
        this.accessoriesService.alertShow("Unauthorized","danger")
      }
    }

  }
}
