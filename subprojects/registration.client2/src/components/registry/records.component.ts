import {
  Component,
  Input,
  Output,
  OnInit,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RecordsActions } from '../../actions';

@Component({
  selector: 'reg-records',
  template: `
    <div class="container">
      <h4 data-testid="configuration-heading" id="qa-configuration-heading">
        <span *ngIf="temporary">Temporary</span> Registration Records
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
export class RegRecords implements OnInit {
  @Input() temporary: boolean = false;

  constructor(private router: Router, private recordsActions: RecordsActions) { }
  
  ngOnInit() {
    this.recordsActions.openRecords(this.temporary);
  }

  addRecord() {
    this.router.navigate(['new-record']);
  }
};
