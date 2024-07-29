import { EventEmitter, Injectable } from "@angular/core";


@Injectable({
    providedIn:'root'
})
export class AccessoriesService{

    alertEmitter=new EventEmitter<{
        message:string,
        alertType:string,
    }>()

    alertShow(message:string,alertType:string){
        this.alertEmitter.emit({
            message,alertType
        })
    }
}