import { Component, input } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-rate-card',
  imports: [DecimalPipe],
  templateUrl: './rate-card.component.html',
})
export class RateCardComponent {
  readonly label = input('');
  readonly buy = input(0);
  readonly sell = input(0);
  readonly change = input(0);
}
