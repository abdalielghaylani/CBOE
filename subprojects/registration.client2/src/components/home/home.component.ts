import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'reg-home',
  template: `
    <div class="container">
      <h4 data-testid="configuration-heading" id="qa-configuration-heading">
        Registration Records
      </h4>

      <dx-data-grid [columns]='gridColumns' [dataSource]=[] [paging]='{pageSize: 10}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [5, 10, 20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' (onRowRemoving)='deleteRecord($event)'
        (onInitNewRow)='addRecord()' (onEditingStart)='editRecord($event)' rowAlternationEnabled=true,
        [editing]='{ mode: form, allowUpdating: true, allowDeleting: true, allowAdding: true }'>
      </dx-data-grid>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegHome {
  constructor(private router: Router) { }
  
  addRecord() {
    this.router.navigate(['home/new']);
  }
};
