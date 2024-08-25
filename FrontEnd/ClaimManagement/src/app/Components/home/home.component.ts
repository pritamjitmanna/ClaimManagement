import { Component } from '@angular/core';
import { globalModules } from '../../global_module';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [globalModules],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  title="Claim Management"

  constructor(private router:Router){}

  fetchClaim(claimId:string){
    this.router.navigate(['claim',claimId])
  }
}
