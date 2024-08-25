import { Component, OnInit } from "@angular/core";
import { AccessoriesService } from "../../Services/accessories.service";



@Component({
    selector:'app-alert',
    standalone:true,
    providers:[],
    imports:[],
    template:`
        @if(toShow){
            <div class="alert alert-{{alertType}}" >
                <span class="alert-message">{{ message }}</span>
                <button type="button" class="close-btn" (click)="close()">×</button>
            </div>
        }
    `,
    styleUrl:'./alert.component.css'
})
export class AlertComponent implements OnInit{
    public message:string=""
    public alertType:string=""
    public toShow:boolean=false

    constructor(private accessoriesService:AccessoriesService){}

    ngOnInit(): void {
        this.accessoriesService.alertEmitter.subscribe({
            next:(info:{message:string,alertType:string})=>{
                this.toShow=true
                this.message=info.message
                this.alertType=info.alertType
            },
            error:(err:any)=>{
                console.log(err)
            }
        })
    }

    public close(){
        this.toShow=false
    }
}