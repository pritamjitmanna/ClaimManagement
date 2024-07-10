import { LowerCasePipe } from "@angular/common";
import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";
import { globalModules } from "../../global_module";


@Component({
    selector:'app-claims',
    standalone:true,
    imports:[globalModules],
    templateUrl:'./claims.component.html',
    styleUrl:'./claims.component.css'
})
export class ClaimsComponent{
    value:string="dasd";

}
