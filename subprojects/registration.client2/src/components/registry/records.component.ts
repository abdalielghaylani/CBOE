import {
  Component,
  Input, ViewChild,
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
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
  selector: 'reg-records',
  template: `
    <div class="container">
      <h4 data-testid="configuration-heading" id="qa-configuration-heading">
        <span *ngIf="temporary">Temporary</span> Registration Records
      </h4>

      <dx-data-grid [columns]=gridColumns [dataSource]=records.rows [paging]='{pageSize: 10}' 
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
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @Input() temporary: boolean;
  @select(s => s.session.lookups) lookups$: Observable<any>;
  @select(s => s.registry.tempRecords) tempRecords$: Observable<IRecords>;
  @select(s => s.registry.records) regRecords$: Observable<IRecords>;
  private records$: Observable<IRecords>;
  private recordsSubscription: Subscription;
  private lookupsSubscription: Subscription;
  private lookups: any;
  private records: IRecords;
  private gridColumns: any[];

  constructor(private router: Router, private ngRedux: NgRedux<IAppState>, private registryActions: RegistryActions) { 
    this.records = { temporary: this.temporary, rows: [], gridColumns: [] };
    this.gridColumns = [];
  }

  ngOnInit() {
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
    if (this.recordsSubscription) {
      this.recordsSubscription.unsubscribe();
    }
  }

  retrieveContents(lookups: any) {
    this.lookups = lookups;
    this.registryActions.openRecords(this.temporary);
    this.records$ = this.ngRedux.select(['registry', this.temporary ? 'tempRecords' : 'records']);
    this.recordsSubscription = this.records$.subscribe(d => { this.updateContents(d); });
  }

  updateContents(records: IRecords) {
    this.records = records;
    this.gridColumns = records.gridColumns.map(s => this.updateGridColumn(s));
    if (this.grid && this.grid.instance) {
      this.grid.instance.refresh();
    }
  }

  updateGridColumn(gridColumn) {
    if (gridColumn.lookup) {
      gridColumn.lookup = { dataSource: this.lookups.users, displayExpr: 'USERID', valueExpr: 'PERSONID' };
    }
    return gridColumn;
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
