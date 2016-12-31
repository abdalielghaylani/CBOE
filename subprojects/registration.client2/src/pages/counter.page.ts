import { Component, Inject, ApplicationRef } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { CounterActions } from '../actions';
import { RegContainer, RegCounter } from '../components';
import { ICounter } from '../store';

@Component({
  selector: 'counter-page',
  providers: [ CounterActions ],
  template: `
    <reg-container testid="counter">
      <h2 data-testid="counter-heading" id="qa-counter-heading"
        class="center caps">
        Counter
      </h2>

      <reg-counter
        [counter]="counter$ | async"
        (increment)="actions.increment()"
        (decrement)="actions.decrement()">
      </reg-counter>
    </reg-container>
  `
})
export class RegCounterPage {
  @select() private counter$: Observable<ICounter>;
  constructor(private actions: CounterActions) {}
}
