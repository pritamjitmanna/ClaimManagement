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
    // Send surveyor Id and details to update claim component
    surveyorDetailsEmitter=new EventEmitter<{
        surveyorUserId:string;
        toShow:string
    }>();

    alertShow(message:string,alertType:string){
        this.alertEmitter.emit({
            message,alertType
        })
    }

    emitEstimatedLossValue(value:number){
        this.surveyorEstimatedLossEmitter.emit(value)
    }
}