import { Component, Input } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-rate-card',
  imports: [DecimalPipe],
  templateUrl: './rate-card.component.html',
})
export class RateCardComponent {
  @Input() label = '';
  @Input() buy = 0;
  @Input() sell = 0;
  @Input() change = 0;
}
