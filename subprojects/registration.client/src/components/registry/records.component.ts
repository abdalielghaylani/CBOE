import {
  Component,
  Input, ViewChild,
  Output,
  OnInit,
  OnDestroy,
  EventEmitter,
  ChangeDetectorRef, ChangeDetectionStrategy, ElementRef,
  Directive, HostListener
} from '@angular/core';
import { select, NgRedux } from '@angular-redux/store';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { EmptyObservable } from 'rxjs/Observable/EmptyObservable';
import { Subscription } from 'rxjs/Subscription';
import 'rxjs/add/operator/toPromise';
import { DxDataGridComponent } from 'devextreme-angular';
import { notify, notifyError, notifySuccess } from '../../common';
import * as regSearchTypes from './registry-search.types';
import { CRecords, RegistryStatus } from './registry.types';
import CustomStore from 'devextreme/data/custom_store';
import { fetchLimit, apiUrlPrefix } from '../../configuration';
import { HttpService } from '../../services';
import { RegRecordSearch } from './record-search.component';
import { PrivilegeUtils } from '../../common';
import { RegistryActions, RegistrySearchActions } from '../../redux';
import { IAppState, CRecordsData, IRecords, ISearchRecords, ILookupData, IQueryData, CSystemSettings } from '../../redux';

declare var jQuery: any;

@Component({
  selector: 'reg-records',
  template: require('./records.component.html'),
  styles: [require('./records.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecords implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @ViewChild(RegRecordSearch) searchForm: RegRecordSearch;
  @Input() temporary: boolean;
  @Input() restore: boolean;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;
  @select(s => !!s.session.token) loggedIn$: Observable<boolean>;
  private records$: Observable<IRecords>;
  private lookupsSubscription: Subscription;
  private recordsSubscription: Subscription;
  private hitlistSubscription: Subscription;
  private lookups: ILookupData;
  private popupVisible: boolean = false;
  private rowSelected: boolean = false;
  private selectedRows: any[] = [];
  private tempResultRows: any[];
  private hitlistVM: regSearchTypes.CQueryManagementVM = new regSearchTypes.CQueryManagementVM(this.ngRedux.getState());
  private hitlistData$: Observable<ISearchRecords>;
  private isMarkedQuery: boolean;
  private loadIndicatorVisible: boolean = false;
  private records: CRecords;
  private gridHeight: string;
  private currentIndex: number = 0;
  private dataStore: CustomStore;
  private sortCriteria: string;
  private structureImageApiPrefix: string = `${apiUrlPrefix}StructureImage/`;

  constructor(
    private router: Router,
    private http: HttpService,
    private ngRedux: NgRedux<IAppState>,
    private registryActions: RegistryActions,
    private actions: RegistrySearchActions,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef) {
    this.records = new CRecords(this.temporary, new CRecordsData(this.temporary));
    this.createCustomStore(this);
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

  // Trigger data retrieval for the view to show.
  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
    if (this.loggedIn$) {
      this.loadIndicatorVisible = true;
    }
    if (this.restore) {
      this.restoreHitlist();
    } else {
      this.loadData();
    }
  }

  restoreHitlist() {
  }

  getGridHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  loadData() {
    this.registryActions.openRecords({
      temporary: this.temporary,
      skip: this.records.data.rows.length,
      take: fetchLimit,
      sort: this.sortCriteria
    });
    this.records$ = this.ngRedux.select(['registry', this.temporary ? 'tempRecords' : 'records']);
    if (!this.recordsSubscription) {
      this.recordsSubscription = this.records$.subscribe(d => { this.openRegistryRecords(d); });
    }
    this.actions.openHitlists(this.temporary);
    this.hitlistData$ = this.ngRedux.select(['registrysearch', 'hitlist']);
    if (this.hitlistSubscription) {
      this.hitlistSubscription = this.hitlistData$.subscribe(() => {
        this.hitlistVM = new regSearchTypes.CQueryManagementVM(this.ngRedux.getState());
        this.changeDetector.markForCheck();
      });
    }
    this.gridHeight = this.getGridHeight();
  }

  openRegistryRecords(records: IRecords) {
    this.loadIndicatorVisible = false;
    if (records.data.startIndex === 0) {
      this.records.temporary = records.temporary;
      if (records.data.rows.length < records.data.totalCount) {
        this.records.filterRow.visible = false;
      }
      this.records.setRecordData(records.data);
      this.records.gridColumns = records.gridColumns.map(s => this.updateGridColumn(s));
    } else if (this.records.data.rows.length > 0 && this.records.data.rows.length === records.data.startIndex) {
      this.records.setRecordData(records.data);
    }
    this.createCustomStore(this);
    if (this.currentIndex !== 0) {
      this.currentIndex = 0;
    }
    this.changeDetector.markForCheck();
  }

  createCustomStore(ref: RegRecords) {
    ref.dataStore = new CustomStore({
      load: function (loadOptions) {
        if (loadOptions.sort !== null) {
          let sortCriteria = !loadOptions.sort[0].desc ? loadOptions.sort[0].selector : loadOptions.sort[0].selector + ' DESC';
          if (ref.sortCriteria !== sortCriteria && ref.records.data.rows.length !== ref.records.data.totalCount) {
            ref.sortCriteria = sortCriteria;
            ref.updateContents();
          }
        }
        if (ref.records.data.rows.length === ref.records.data.totalCount) {
          ref.records.filterRow.visible = true;
        }
        return ref.records.getFetchedRows();
      }
    });
  }

  updateGridColumn(gridColumn) {
    if (gridColumn.lookup) {
      gridColumn.lookup = {
        dataSource: this.lookups.users,
        displayExpr: 'USERID',
        valueExpr: 'PERSONID'
      };
    } else if (gridColumn.cellTemplate === 'statusTemplate') {
      let systemSettings = new CSystemSettings(this.lookups.systemSettings);
      gridColumn.visible = systemSettings.isApprovalsEnabled;
    }
    return gridColumn;
  }

  private onDocumentClick(event: any) {
    if (event.srcElement.title === 'Full Screen') {
      let fullScreenMode = event.srcElement.className === 'fa fa-compress fa-stack-1x white';
      this.gridHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
      this.grid.height = this.gridHeight;
      this.grid.instance.repaint();
    }
  }

  onResize(event: any) {
    this.gridHeight = this.getGridHeight();
    this.grid.height = this.getGridHeight();
    this.grid.instance.repaint();
  }

  onInitialized(e) {
    if (!e.component.columnOption('command:edit', 'visibleIndex')) {
      e.component.columnOption('command:edit', {
        visibleIndex: -1,
        width: 80
      });
    }
  }

  updateContents() {
    this.registryActions.openRecords({
      temporary: this.temporary,
      skip: this.records.data.rows.length,
      take: fetchLimit,
      sort: this.sortCriteria
    });
  }

  onRowPrepared(e) {
    if (!this.rowSelected && e.rowType === 'data'
      && this.records.data.rows.length < this.records.data.totalCount
      && e.rowIndex === this.records.data.rows.length - 1) {
      this.updateContents();
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
        // For Edit
        let $editIcon = $links.filter('.dx-link-edit');
        $editIcon.addClass('fa fa-info-circle');
        $editIcon.attr({ 'data-toggle': 'tootip', 'title': 'Detail view' });
        // For Delete icon
        if (PrivilegeUtils.hasDeleteRecordPrivilege(this.temporary, this.lookups.userPrivileges)) {
          let $deleteIcon = $links.filter('.dx-link-delete');
          $deleteIcon.addClass('dx-icon-trash');
          $deleteIcon.attr({ 'data-toggle': 'tootip', 'title': 'Delete' });
        }
      }
    }
  }

  onToolbarPreparing(e) {
    e.toolbarOptions.items.unshift({
      location: 'before',
      template: 'toolbarContents'
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

  onRowRemoving(e) {
    // TODO: Should use redux
    let deferred = jQuery.Deferred();
    let id = e.data[Object.keys(e.data)[0]];
    let url = `${apiUrlPrefix}${this.temporary ? 'temp-' : ''}records/${id}`;
    this.http.delete(url)
      .toPromise()
      .then(result => {
        notifySuccess(`The record (ID: ${id}) was deleted successfully!`, 5000);
        deferred.resolve(false);
      })
      .catch(error => deferred.resolve(true));
    e.cancel = deferred.promise();
  }

  private manageQueries() {
    this.actions.openHitlists(this.temporary);
    this.currentIndex = 1;
  }

  private newQuery() {
    this.currentIndex = 2;
  }

  private saveQuery(isMarked: boolean) {
    this.isMarkedQuery = isMarked;
    this.hitlistVM.saveQueryVM.clear();
    this.popupVisible = true;
  }

  private retrieveAll() {
    this.registryActions.openRecords({ temporary: this.temporary });
  }

  private saveHitlist() {
    if (this.hitlistVM.saveQueryVM.data.name && this.hitlistVM.saveQueryVM.data.description) {
      if (this.isMarkedQuery === true) {
        this.hitlistVM.saveQueryVM.clear();
        notifySuccess('Marked records saved successfully!', 5000);
      } else {
        this.hitlistVM.saveQueryVM.clear();
        notifySuccess('Query saved successfully!', 5000);
      }
      this.popupVisible = false;
    } else {
      notifyError('Name and Description is required!', 5000);
    }
  }

  private editQuery(id: Number) {
    // TODO: It should show the search page and populate the contents with hitlist query.
    // this.router.navigate([`search/${id}`]);
    this.currentIndex = 2;
  }

  private cancelSaveQuery() {
    this.popupVisible = false;
  }

  private headerClicked(e) {
    this.currentIndex = 0;
  }

  private restoreClicked(queryData: IQueryData) {
    this.currentIndex = 2;
    this.searchForm.restore(queryData);
  }

  private showMarked() {
    if (this.selectedRows && this.selectedRows.length > 0 && !this.rowSelected) {
      this.rowSelected = true;
      this.tempResultRows = this.records.data.rows;
      this.records.data.rows = this.selectedRows;
      this.grid.instance.refresh();
    }
  }

  private deleteRows(ids: number[], failed: number[], succeeded: number[]) {
    if (ids.length === 0) {
      // If id list is empty, change deleted rows and refresh grid.
      // Remove from this.records.data.rows
      // Remove from this.selectedRows
      // Also display a message about success/failure.
      this.records.data.rows = this.records.data.rows.filter(r => !succeeded.find(s => s === r.ID));
      this.selectedRows = this.selectedRows.filter(r => !succeeded.find(s => s === r.ID));
      if (failed.length === 0) {
        notifySuccess(`All marked records were deleted successfully!`, 5000);
      } else if (succeeded.length === 0) {
        notifySuccess(`Deleting failed for all marked records!`, 5000);
      } else {
        notify(`Deleting records failed: ${failed.join(', ')}`, `warning`, 5000);
      }
      this.grid.instance.repaint();
      return;
    }
    // Otherwise, shift ids
    let id = ids.shift();
    let url = `${apiUrlPrefix}${this.temporary ? 'temp-' : ''}records/${id}`;
    // Delete first id
    this.http.delete(url)
      .catch(error => {
        failed.push(id);
        this.deleteRows(ids, failed, succeeded);
        throw error;
      })
      .subscribe(r => {
        if (r !== null) {
          // Upon successful deletion, recursively call deleteRows
          succeeded.push(id);
          this.deleteRows(ids, failed, succeeded);
        }
      });
  }

  private deleteMarked() {
    if (this.selectedRows && this.selectedRows.length > 0) {
      let ids: number[] = this.selectedRows.map(r => r[Object.keys(r)[0]]);
      let succeeded: number[] = [];
      let failed: number[] = [];
      this.deleteRows(ids, failed, succeeded);
    }
  }

  private showSearchResults() {
    if (this.rowSelected) {
      this.rowSelected = false;
      this.selectedRows = this.records.data.rows;
      this.records.data.rows = this.tempResultRows;
      this.tempResultRows = [];
      this.grid.instance.refresh();
    }
  }

  private printRecords() {
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

  private isApproved(data): boolean {
    return data.value === RegistryStatus.Approved;
  }

  private get approvalsEnabled(): boolean {
    return this.temporary && new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings).isApprovalsEnabled;
  }

  private get approveMarkedEnabled(): boolean {
    return this.temporary && this.selectedRows && this.selectedRows.length > 0 && this.approvalsEnabled;
  }

  // set delete marked button visibility
  private get deleteMarkedEnabled(): boolean {
    if (this.selectedRows && this.selectedRows.length > 0) {
      return PrivilegeUtils.hasDeleteRecordPrivilege(this.temporary, this.lookups.userPrivileges);
    }
    return false;
  }

  private approveRows(ids: number[], failed: number[], succeeded: number[]) {
    if (ids.length === 0) {
      // If id list is empty, change deleted rows and refresh grid.
      // Remove from this.records.data.rows
      // Remove from this.selectedRows
      // Also display a message about success/failure.
      if (failed.length === 0) {
        notifySuccess(`All marked records were approved successfully!`, 5000);
      } else if (succeeded.length === 0) {
        notifySuccess(`Approving failed for all marked records!`, 5000);
      } else {
        notify(`Approving for some records failed: ${failed.join(', ')}`, `warning`, 5000);
      }
      this.grid.instance.refresh();
      return;
    }
    // Otherwise, shift ids
    let id = ids.shift();
    let row = this.records.data.rows.find(r => r.ID === id);
    if (row && row.STATUSID === RegistryStatus.Submitted) {
      let url = `${apiUrlPrefix}${this.temporary ? 'temp-' : ''}records/${id}/${RegistryStatus.Approved}`;
      // Approve first id
      this.http.put(url, undefined)
        .catch(error => {
          failed.push(id);
          this.approveRows(ids, failed, succeeded);
          throw error;
        })
        .subscribe(r => {
          if (r !== null) {
            // Upon successful deletion, recursively call approveRows
            row.STATUSID = RegistryStatus.Approved;
            succeeded.push(id);
            this.approveRows(ids, failed, succeeded);
          }
        });
    } else {
      this.approveRows(ids, failed, succeeded);
    }
  }

  private approveMarked() {
    if (this.selectedRows && this.selectedRows.length > 0) {
      let ids: number[] = this.selectedRows.map(r => r[Object.keys(r)[0]]);
      let succeeded: number[] = [];
      let failed: number[] = [];
      this.approveRows(ids, failed, succeeded);
    }
  }
};
