import { Component } from '@angular/core';
import { globalModules } from '../../global_module';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [globalModules],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {

}
