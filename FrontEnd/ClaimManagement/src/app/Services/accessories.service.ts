import { EventEmitter, Injectable, Output } from "@angular/core";


@Injectable({
    providedIn:'root'
})
export class AccessoriesService{

    alertEmitter=new EventEmitter<{
        message:string;
        alertType:string
    }>()

    alertShow(message:string,alertType:string){
        console.log(message,alertType)
        this.alertEmitter.emit({
            message,alertType
        })
    }
}