import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { ICounter } from '../../store';

@Component({
  selector: 'reg-counter',
  template: `
    <div class="flex">
      <reg-button
        className="bg-black col-2"
        (onClick)="decrement.emit()"
        testid="counter-decrementButton">
        -
      </reg-button>

      <div 
        data-testid="counter-result"
        class="flex-auto flex-center center h1">
        {{ counter.counter }}
      </div>

      <reg-button className="col-2"
        (onClick)="increment.emit()"
        testid="counter-incrementButton">
        +
      </reg-button>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegCounter {
  @Input() counter: ICounter;
  @Output() increment = new EventEmitter<void>();
  @Output() decrement = new EventEmitter<void>();
};
