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
import * as regSearchTypes from './registry-search.types';

@Component({
  selector: 'reg-records',
  template: `
  <div class="container-fluid border-light background-white pb2">
      <reg-page-header testid="configuration-heading" id="qa-configuration-heading">
            <span *ngIf="records.temporary">Temporary</span> Registration Records
      </reg-page-header>
      <dx-popup
        [width]="300"
        [height]="250"
        [showTitle]="true"
        title="Save Query"
        [dragEnabled]="false"
        [closeOnOutsideClick]="true"
        [(visible)]="popupVisible">
        <div *dxTemplate="let data of 'content'">
         <dx-form id="editHitlistForm" labelLocation="left" minColWidth=300 [colCount]="1" [items]="regsearch.editColumns"></dx-form>
          <div style="margin-top: 15px">
          <button class="mr1 btn btn-primary">Save</button>
          <button class="mr1 btn btn-primary" (click)="cancelSaveQuery()">Cancel</button>
          </div>
        </div>
      </dx-popup>
      <dx-data-grid id="grdRecords" [columns]=records.gridColumns [dataSource]=records.rows [paging]='{pageSize: 10}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [5, 10, 20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' rowAlternationEnabled=true
        (onToolbarPreparing)="onToolbarPreparing($event)"
        (onSelectionChanged)="onSelectionChanged($event)"
        (onContentReady)='onContentReady($event)'
        (onCellPrepared)='onCellPrepared($event)'
        (onInitNewRow)='onInitNewRow($event)'
        (onEditingStart)='onEditingStart($event)'
        (onRowRemoving)='onRowRemoving($event)'
        [hoverStateEnabled]="true"
        [selectedRowKeys]="[]" >
        <dxo-editing mode="row" [allowUpdating]="true" [allowDeleting]="records.temporary" [allowAdding]="false"></dxo-editing>
        <dxo-selection mode="multiple"></dxo-selection>
        <div *dxTemplate="let data of 'content'">
        <div class="btn-group btn-group-sm">
          <button class="btn btn-group btn-group-sm rose text-relaxed"  data-original-title="New Query" (click)="newQuery()">
          <i class="fa fa-list-alt"></i>New Query</button>
        </div>
        <div class="btn-group btn-group-sm">
          <button class="btn btn-group btn-group-sm rose text-relaxed" (click)="saveQuery()" data-original-title="Save Query">
          <i class="fa fa-floppy-o"></i>Save Query</button>
        </div>
        <div class="btn-group btn-group-sm">
          <button class="btn btn-group btn-group-sm rose text-relaxed" (click)="editQuery(1)" data-original-title="Save Query">
          <i class="fa fa-pencil-square-o"></i>Edit Current Query</button>
        </div>
        <div class="btn-group btn-group-sm">
          <button class="btn btn-group btn-group-sm rose text-relaxed"  data-original-title="Print" (click)="printRecords()"  >
          <i class="fa fa-print"></i>Print</button>
        </div>
        <div class="btn-group btn-group-sm" *ngIf="!rowSelected">
          <button type="button" class="btn dropdown-toggle rose text-relaxed" data-original-title="Marked" (click)="showMarked()">
          <i class="fa fa-filter"></i>Show Marked 
          </button>
        </div>
         <div class="btn-group btn-group-sm" *ngIf="rowSelected">
          <button type="button" class="btn dropdown-toggle rose text-relaxed" data-original-title="Marked" (click)="showSearchResults()">
          <i class="fa fa-level-up"></i>Show Search Results 
          </button>
        </div>
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
  private popupVisible: boolean = false;
  private rowSelected: boolean = false;
  private selectedRows: any[];
  private tempResultRows: any[];
  private regsearch: regSearchTypes.CSaveQuery = new regSearchTypes.CSaveQuery();

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
        location: 'before',
        template: 'content'
      });
    }
  }

  onSelectionChanged(e) {
    this.selectedRows = e.selectedRowKeys;
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

  newQuery() {
    this.router.navigate(['records/search']);
  }

  saveQuery() {
    this.popupVisible = true;
  }

  editQuery(id: Number) {
    this.router.navigate([`records/search/${id}`]);
  }

  cancelSaveQuery() {
    this.popupVisible = false;
  }

  showMarked() {
    if (this.selectedRows) {
      this.rowSelected = true;
      this.tempResultRows = this.records.rows;
      this.records.rows = this.selectedRows;
    }
  }

  showSearchResults() {
    this.rowSelected = false;
    this.records.rows = this.tempResultRows;
    this.selectedRows = [];
    this.tempResultRows = [];
  }

  printRecords() {
    let printContents, popupWin;
    printContents = document.getElementById('grdRecords').innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(`<html>
        <head>
          <title>Print table</title>
          <style>
          table, th, td 
          {
              border:solid 1px #f0f0f0;
              font:15px Verdana;
          }
          .dx-header-row {
              font-weight:bold;
          }
          img {
            height:120px;
          }
          .dx-datagrid-header-panel,.dx-datagrid-filter-row,.dx-datagrid-pager,
          .dx-command-edit,.dx-command-select{
            display:none;
          }
          </style>
        </head>
    <body onload="window.print();window.close()">${printContents}</body>
      </html>`);
    popupWin.document.close();
  }
};
