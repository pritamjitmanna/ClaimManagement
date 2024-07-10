import { Component } from '@angular/core';
import { globalModules } from '../../global_module';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [globalModules],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  title="Claim Management"
}
