import {
  Component,
  Input,
  Output,
  OnInit,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RegistryActions } from '../../actions';
import { IRegistry } from '../../store';

@Component({
  selector: 'reg-records',
  template: `
    <div class="container">
      <h4 data-testid="configuration-heading" id="qa-configuration-heading">
        <span *ngIf="registry.temporary">Temporary</span> Registration Records
      </h4>

      <dx-data-grid [dataSource]=this.registry.rows [paging]='{pageSize: 10}' 
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
  @Input() title: string;
  @Input() registry: IRegistry;

  constructor(private router: Router, private registryActions: RegistryActions) { }
  
  ngOnInit() {
    this.registryActions.openRecords(this.registry.temporary);
  }

  addRecord() {
    this.router.navigate(['new-record']);
  }
};
