import {
  Component,
  Input,
  Output,
  OnInit,
  OnDestroy,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { select, NgRedux } from 'ng2-redux';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { RegistryActions } from '../../actions';
import { IAppState, IRecords } from '../../store';

@Component({
  selector: 'reg-records',
  template: `
    <div class="container">
      <h4 data-testid="configuration-heading" id="qa-configuration-heading">
        <span *ngIf="records.temporary">Temporary</span> Registration Records
      </h4>

      <dx-data-grid [columns]=records.gridColumns [dataSource]=records.rows [paging]='{pageSize: 10}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [5, 10, 20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' rowAlternationEnabled=true
        (onContentReady)='onContentReady($event)'
        (onCellPrepared)='onCellPrepared($event)'
        (onInitNewRow)='onInitNewRow($event)'
        (onEditingStart)='onEditingStart($event)'
        (onRowRemoving)='onRowRemoving($event)'>
        <dxo-editing mode="row" [allowUpdating]="true" [allowDeleting]="records.temporary" [allowAdding]="false"></dxo-editing>
        <div *dxTemplate="let data of 'cellTemplate'">
          <reg-structure-image [src]="data.value"></reg-structure-image>
        </div>
      </dx-data-grid>
    </div>
  `,
  styles: [require('./records.css')],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecords implements OnInit, OnDestroy {
  @Input() title: string;
  @Input() records: IRecords;
  @select(s => s.configuration.lookups) lookups$: Observable<any>;
  @select(s => s.session.token) token$: Observable<string>;
  private refreshSubscription: Subscription;

  constructor(private router: Router, private ngRedux: NgRedux<IAppState>, private registryActions: RegistryActions) { }

  ngOnInit() {
    this.refreshSubscription = this.token$.subscribe(s => { if (s) { this.updateContents(); } });
  }

  ngOnDestroy() {
    this.refreshSubscription.unsubscribe();
  }

  updateContents() {
    this.registryActions.openRecords(this.records.temporary);
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

  onInitNewRow(e) {
    e.cancel = true;
    this.registryActions.retrieveRecord(this.records.temporary, -1);
  }

  onEditingStart(e) {
    e.cancel = true;
    let id = e.data[Object.keys(e.data)[0]];
    this.registryActions.retrieveRecord(this.records.temporary, id);
  }

  onRowRemoving(e) {
  }
};
