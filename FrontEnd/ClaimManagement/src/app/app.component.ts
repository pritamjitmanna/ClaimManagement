import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './Components/navbar/navbar.component';
import { LoadingComponent } from './Components/loading/loading.component';
import { AlertComponent } from './Components/alert/alert.component';
import { SurveyorslistComponent } from "./Components/surveyorslist/surveyorslist.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, LoadingComponent, AlertComponent, SurveyorslistComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'ClaimManagement';
}
