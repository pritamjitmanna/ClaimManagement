import { Component } from '@angular/core';
import { globalModules } from '../../global_module';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [globalModules],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

}
