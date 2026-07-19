import { CommonModule } from "@angular/common";
import { signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { RouterModule } from "@angular/router";
import { BehaviorSubject } from "rxjs";
import { NotificationModel } from "./Models/notification-model";


export const globalModules=[
    RouterModule,
    FormsModule,
    CommonModule,
    
]

export const globalVariables={
    isAuthenticated:new BehaviorSubject<boolean>(false),
    token:"",
    username:new BehaviorSubject<string>(""),
    role:new BehaviorSubject<Array<string>>([]),
    surveyorRespectiveFlag:new BehaviorSubject<boolean>(false),
    profileSet:new BehaviorSubject<boolean>(false),
    userId:new BehaviorSubject<string>(""),
    notifications:signal<NotificationModel[]>([])
}