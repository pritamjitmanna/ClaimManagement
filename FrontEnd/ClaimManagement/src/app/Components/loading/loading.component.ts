import { Component } from '@angular/core';
import { AccessoriesService } from '../../Services/accessories.service';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [],
  providers:[AccessoriesService],
  template: `
    <div class="spinner-container">
        <div class="spinner"></div>
      </div>
   `,
  styleUrl: './loading.component.css'
})
export class LoadingComponent {
  
}
