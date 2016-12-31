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
    <div class="flex">
      <h4>Registration Records</h4>
      <dx-data-grid [columns]='gridColumns' [dataSource]='records' [paging]='{pageSize: 10}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [5, 10, 20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' (onRowRemoving)='deleteRecord($event)'
        (onInitNewRow)='addRecord()' (onEditingStart)='editRecord($event)' rowAlternationEnabled=true,
        [editing]='{ mode: form, allowUpdating: true, allowDeleting: true, allowAdding: true }'>
      </dx-data-grid>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegCounter {
  @Input() counter: ICounter;
  @Output() increment = new EventEmitter<void>();
  @Output() decrement = new EventEmitter<void>();
};
