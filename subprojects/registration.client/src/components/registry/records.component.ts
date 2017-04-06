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
import { RegistryActions, RegistrySearchActions } from '../../actions';
import { IAppState, IRecords, ISearchRecords } from '../../store';
import { DxDataGridComponent } from 'devextreme-angular';
import { notify, notifyError, notifySuccess } from '../../common';
import * as regSearchTypes from './registry-search.types';

@Component({
  selector: 'reg-records',
  template: require('./records.component.html'),
  styles: [require('./records.css')],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecords implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @Input() temporary: boolean;
  @Input() restore: boolean;
  @select(s => s.session.lookups) lookups$: Observable<any>;
  @select(s => !!s.session.token) loggedIn$: Observable<boolean>;
  private records$: Observable<IRecords>;
  private lookupsSubscription: Subscription;
  private recordsSubscription: Subscription;
  private hitlistSubscription: Subscription;
  private lookups: any;
  private records: IRecords;
  private popupVisible: boolean = false;
  private rowSelected: boolean = false;
  private selectedRows: any[] = [];
  private tempResultRows: any[];
  private hitlistVM: regSearchTypes.CQueryManagementVM = new regSearchTypes.CQueryManagementVM(this.ngRedux.getState());
  private hitlistData$: Observable<ISearchRecords>;
  private isMarkedQuery: boolean;
  private loadIndicatorVisible: boolean = false;

  constructor(
    private router: Router,
    private ngRedux: NgRedux<IAppState>,
    private registryActions: RegistryActions,
    private actions: RegistrySearchActions,
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
    if (this.hitlistSubscription) {
      this.hitlistSubscription.unsubscribe();
    }
  }

  loadData() {
    this.hitlistVM = new regSearchTypes.CQueryManagementVM(this.ngRedux.getState());
    this.changeDetector.markForCheck();
  }

  // Trigger data retrieval for the view to show.
  // Select the view model from redux store, and listen to it.
  retrieveContents(lookups: any) {
    if (this.loggedIn$) {
      this.loadIndicatorVisible = true;
    }
    this.lookups = lookups;
    if (!this.restore) {
      this.registryActions.openRecords(this.temporary);
    }
    this.records$ = this.ngRedux.select(['registry', this.temporary ? 'tempRecords' : 'records']);
    this.recordsSubscription = this.records$.subscribe(d => { this.updateContents(d); });
    this.actions.openHitlists();
    this.hitlistData$ = this.ngRedux.select(['registrysearch', 'hitlist']);
    this.hitlistSubscription = this.hitlistData$.subscribe(() => { this.loadData(); });
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

  onRowPrepared(e) {
    if (e.rowType === 'data') {
      this.loadIndicatorVisible = false;
    }
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
    e.toolbarOptions.items.unshift({
      location: 'before',
      template: 'content'
    });
  }

  onSelectionChanged(e) {
    this.selectedRows = e.selectedRowKeys;
  }

  onSearchClick(e) {
    e.cancel = true;
    this.router.navigate([`search/${this.temporary ? 'temp' : ''}`]);
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

  manageQueries() {
    this.actions.openHitlists();
  }

  newQuery() {
    this.router.navigate([`search/${this.temporary ? 'temp' : ''}`]);
  }

  saveQuery(isMarked: boolean) {
    this.isMarkedQuery = isMarked;
    this.hitlistVM.saveQueryVM.clear();
    this.popupVisible = true;
  }
  saveHitlist() {
    if (this.hitlistVM.saveQueryVM.data.Name && this.hitlistVM.saveQueryVM.data.Description) {
      if (this.isMarkedQuery === true) {
        this.hitlistVM.saveQueryVM.clear();
        notifySuccess('Marked records saved successfully!', 5000);
      } else {
        this.hitlistVM.saveQueryVM.clear();
        notifySuccess('Query saved successfully!', 5000);
      }
    } else {
      notifyError('Name and Description is required!', 5000);
    }
  }

  editQuery(id: Number) {
    this.router.navigate([`search/${id}`]);
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

  onShown() {
    setTimeout(() => {
      this.loadIndicatorVisible = false;
    }, 3000);
  }

  onHidden() {
  }

  showLoadPanel() {
    this.loadIndicatorVisible = true;
  }
};
