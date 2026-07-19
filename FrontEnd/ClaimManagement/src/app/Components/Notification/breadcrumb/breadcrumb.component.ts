import { Component, Input } from '@angular/core';
import { NotificationModel } from '../../../Models/notification-model';
import { globalModules, globalVariables } from '../../../global_module';

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [globalModules],
  templateUrl: './breadcrumb.component.html',
  styleUrl: './breadcrumb.component.css'
})
export class BreadcrumbComponent {
  notifications:NotificationModel[]=[];
  count: number = 0;
  isOpen = false;

  constructor(){
    this.notifications=globalVariables.notifications();
    this.count=this.notifications.length;
  }

  toggleDropdown(): void {
    this.isOpen = !this.isOpen;
  }
}
