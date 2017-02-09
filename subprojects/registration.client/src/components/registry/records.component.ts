import {
  Component,
  Input, ViewChild,
  Output,
  OnInit,
  OnDestroy,
  EventEmitter,
  ChangeDetectorRef, ChangeDetectionStrategy,
} from '@angular/core';
import { select, NgRedux } from '@angular-redux/store';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { RegistryActions } from '../../actions';
import { IAppState, IRecords } from '../../store';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
  selector: 'reg-records',
  template: `
    <div class="container-fluid border-light background-white pb2">
      <reg-page-header testid="configuration-heading" id="qa-configuration-heading">
            <span *ngIf="records.temporary">Temporary</span> Registration Records
      </reg-page-header>
      <dx-data-grid [columns]=records.gridColumns [dataSource]=records.rows [paging]='{pageSize: 10}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [5, 10, 20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' rowAlternationEnabled=true
        (onToolbarPreparing)="onToolbarPreparing($event)"
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
  private records$: Observable<IRecords>;
  private lookupsSubscription: Subscription;
  private recordsSubscription: Subscription;
  private lookups: any;
  private records: IRecords;

  constructor(
    private router: Router,
    private ngRedux: NgRedux<IAppState>,
    private registryActions: RegistryActions,
    private changeDetector: ChangeDetectorRef) { 
    this.records = { temporary: this.temporary, rows: [], gridColumns: [] };
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

  // Trigger data retrieval for the view to show.
  // Select the view model from redux store, and listen to it.
  retrieveContents(lookups: any) {
    this.lookups = lookups;
    this.registryActions.openRecords(this.temporary);
    this.records$ = this.ngRedux.select(['registry', this.temporary ? 'tempRecords' : 'records']);
    this.recordsSubscription = this.records$.subscribe(d => { this.updateContents(d); });
  }

  updateContents(records: IRecords) {
    if (this.temporary !== records.temporary) {
      return;
    }
    this.records.temporary = records.temporary;
    this.records.rows = records.rows;
    this.records.gridColumns = records.gridColumns.map(s => this.updateGridColumn(s));
    this.changeDetector.markForCheck();
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

  onToolbarPreparing(e) {
    if (!this.records.temporary) {
      e.toolbarOptions.items.unshift({
        location: 'after',
        widget: 'dxButton',
        options: {
          icon: 'find',
          text: 'Advanced Search',
          onClick: this.onSearchClick.bind(this)
        }
      });
    }
  }

  onSearchClick(e) {
    e.cancel = true;
    this.router.navigate(['records/search']);
  }

  onInitNewRow(e) {
    e.cancel = true;
    this.router.navigate(['records/new']);
  }

  onEditingStart(e) {
    e.cancel = true;
    let id = e.data[Object.keys(e.data)[0]];
    this.router.navigate([`records/${this.temporary ? 'temp' : ''}/${id}`]);
  }

  onRowRemoving(e) {
  }
};
