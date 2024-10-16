import { EventEmitter, Injectable, Output } from "@angular/core";


@Injectable({
    providedIn:'root'
})
export class AccessoriesService{

    alertEmitter=new EventEmitter<{
        message:string;
        alertType:string
    }>()

    surveyorEstimatedLossEmitter=new EventEmitter<number>();

    alertShow(message:string,alertType:string){
        this.alertEmitter.emit({
            message,alertType
        })
    }

    emitEstimatedLossValue(value:number){
        this.surveyorEstimatedLossEmitter.emit(value)
    }
}