import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ActionResultPanelComponent} from "../shared/components/action-result-panel/action-result-panel.component";

@Component({
  selector: 'cmp-home',
  standalone: true,
  imports: [CommonModule, ActionResultPanelComponent],
  templateUrl: './home.component.html'
})
export class HomeComponent {

}
