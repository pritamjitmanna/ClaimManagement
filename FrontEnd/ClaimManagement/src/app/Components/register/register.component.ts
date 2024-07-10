import { Component } from '@angular/core';
import { globalModules } from '../../global_module';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [globalModules],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

}
