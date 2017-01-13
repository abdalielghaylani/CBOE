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
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' rowAlternationEnabled=true
        (onRowRemoving)='deleteRecord($event)'
        (onInitNewRow)='addRecord()' 
        (onEditingStart)='editRecord($event)' 
        (onContentReady)='onContentReady($event)'
        (onCellPrepared)='onCellPrepared($event)'>
        <dxo-editing mode="form" [allowUpdating]="true" [allowDeleting]="registry.temporary" [allowAdding]="true"></dxo-editing>
      </dx-data-grid>
    </div>
  `,
  styles: [require('./records.css')],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecords implements OnInit {
  @Input() title: string;
  @Input() registry: IRegistry;

  constructor(private router: Router, private registryActions: RegistryActions) { }

  ngOnInit() {
    this.registryActions.openRecords(this.registry.temporary);
  }

  onContentReady(e) {
    e.component.columnOption('command:edit', {
      visibleIndex: -1,
      width: 80
    });
  }

  onCellPrepared(e) {
    if (e.rowType === 'data' && e.column.command === 'edit') {
      let isEditing = e.row.isEditing;
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      if (isEditing) {
        $links.filter('.dx-link-save').addClass('dx-icon-save');
        $links.filter('.dx-link-cancel').addClass('dx-icon-revert');
      } else {
        $links.filter('.dx-link-edit').addClass('dx-icon-edit');
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
      }
    }
  }

  addRecord() {
    this.router.navigate(['new-record']);
  }
};
