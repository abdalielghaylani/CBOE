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
import { CViewGroup, CViewGroupColumns } from './base';
import { FormGroupType, prepareFormGroupData, IFormGroup, getExceptionMessage, notify, notifyError, notifySuccess, notifyException } from '../../common';
import * as regSearchTypes from './registry-search.types';
import { RegistryStatus, IRegMarkedPopupModel, IResponseData } from './registry.types';
import CustomStore from 'devextreme/data/custom_store';
import { fetchLimit, printAndExportLimit, apiUrlPrefix, invWideWindowParams } from '../../configuration';
import { HttpService } from '../../services';
import { RegRecordSearch } from './record-search.component';
import { PrivilegeUtils } from '../../common';
import { RegistryActions, RegistrySearchActions, IRecordListData, CRecordListData } from '../../redux';
import { IAppState, CRecordsData, ISearchRecords, ILookupData, IQueryData, CSystemSettings, HitlistType } from '../../redux';
import { RegInvContainerHandler } from './inventory-container-handler/inventory-container-handler';
import * as dxDialog from 'devextreme/ui/dialog';
import { forEach } from '@angular/router/src/utils/collection';
import { CStructureImagePrintService } from '../common/structure-image-print.service';

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
  @Input() hitListId: number;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;
  @select(s => !!s.session.token) loggedIn$: Observable<boolean>;
  @select(s => s.session.isLoading) isLoading$: Observable<any>;
  private responseData$: Observable<IResponseData>;
  private viewGroupsColumns: CViewGroupColumns;
  private lookupsSubscription: Subscription;
  private responseSubscription: Subscription;
  private hitlistSubscription: Subscription;
  private isLoadingSubscription: Subscription;
  private lookups: ILookupData;
  private popupVisible: boolean = false;
  private rowSelected: boolean = false;
  private selectedRows: any[] = [];
  private markedRecords: any[] = [];
  private tempResultRows: any[];
  private hitlistVM: regSearchTypes.CQueryManagementVM = new regSearchTypes.CQueryManagementVM(this.ngRedux.getState());
  private hitlistData$: Observable<ISearchRecords>;
  private bulkRecordData$: Observable<any[]>;
  private isMarkedQuery: boolean;
  private loadIndicatorVisible: boolean = false;
  private gridHeight: string;
  private currentIndex: number = 0;
  private sortCriteria: string;
  private structureImageApiPrefix: string = `${apiUrlPrefix}StructureImage/`;
  private idField;
  private regMarkedModel: IRegMarkedPopupModel = { description: '', option: 'None', isVisible: false };
  private defaultPrintStructureImage = require('../common/assets/no-structure.png');
  private approvedIcon = require('../common/assets/approved.png');
  private notApprovedIcon = require('../common/assets/notapproved.png');
  private isPrintAndExportAvailable: boolean = false;
  private clearSearchForm: boolean = false;
  private dataStore: CustomStore;
  private recordsTotalCount: number = 0;
  private refreshHitList: boolean = false;
  private totalSearchableCount: number = 0;
  private isTotalSearchableCountUpdated: boolean = false;

  constructor(
    private router: Router,
    private http: HttpService,
    private ngRedux: NgRedux<IAppState>,
    private registryActions: RegistryActions,
    private actions: RegistrySearchActions,
    private elementRef: ElementRef,
    private imageService: CStructureImagePrintService,
    private changeDetector: ChangeDetectorRef) {
  }

  ngOnInit() {
    // Synchronize the hit-list ID with the global cache.
    if (this.hitListId !== 0) {
      this.listData = new CRecordListData(this.hitListId);
    } else {
      this.hitListId = this.listData.hitListId;
    }
    this.responseData$ = this.ngRedux.select(['registry', 'responseData']);
    if (!this.responseSubscription) {
      this.responseSubscription = this.responseData$.subscribe(d => { this.deleteRecordStatus(d); });
    }
    this.idField = this.temporary ? 'TEMPBATCHID' : 'REGID';
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
    this.isLoadingSubscription = this.isLoading$.subscribe(d => { this.setProgressBarVisibility(d); });
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
    if (this.hitlistSubscription) {
      this.hitlistSubscription.unsubscribe();
    }
    if (this.responseSubscription) {
      this.registryActions.clearResponse();
      this.responseSubscription.unsubscribe();
    }
    if (this.isLoadingSubscription) {
      this.isLoadingSubscription.unsubscribe();
    }
  }

  private get listData(): IRecordListData {
    const state = this.ngRedux.getState();
    return this.temporary ? state.registry.tempListData : state.registry.regListData;
  }

  private set listData(data: IRecordListData) {
    this.registryActions.updateListData(this.temporary, data);
  }

  updateHitListId(id: number) {
    if (this.hitListId !== id) {
      this.hitListId = id;
      this.listData = new CRecordListData(id);
    }
  }

  bulkRegisterStatus(val) {
    if (val) {
      this.router.navigate([`records/bulkreg`]);
    }
  }

  setProgressBarVisibility(e) {
    this.loadIndicatorVisible = e;
    this.changeDetector.markForCheck();
  }

  deleteRecordStatus(e: IResponseData) {
    if (e) {
      if (e.data.status) {
        notifySuccess(e.message, 5000);
      } else {
        notifyError(e.message, 5000);
      }
      this.currentIndex = 0;
      this.setProgressBarVisibility(false);
      this.grid.instance.refresh();
      this.registryActions.clearResponse();
    }
  }

  setTotalSearchableCount() {
    this.http.get(`${apiUrlPrefix}records/databaseRecordCount?temp=${this.temporary}`).toPromise()
      .then((res => {
        this.totalSearchableCount = res.json();
        this.isTotalSearchableCountUpdated = true;
      }).bind(this))
      .catch(error => {
        notifyException(`The search failed due to a problem`, error, 5000);
      });
  }

  // Trigger data retrieval for the view to show.
  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
    if (this.loggedIn$) {
      this.loadIndicatorVisible = true;
    }

    let formGroupType = this.temporary ? FormGroupType.SearchTemporary : FormGroupType.SearchPermanent;
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    let formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
    this.viewGroupsColumns = this.lookups
      ? CViewGroup.getColumns(this.temporary, formGroup, this.lookups.disabledControls, new CSystemSettings(this.lookups.systemSettings))
      : new CViewGroupColumns();

    if (this.restore) {
      this.restoreHitlist();
    } else {
      this.loadData();
    }
  }

  restoreHitlist() {
    if ((this.restore) && (this.hitListId > 0)) {
      this.dataStore = this.createCustomStore(this);
    }
  }

  getGridHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  loadData() {
    this.dataStore = this.createCustomStore(this);
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

  private createCustomStore(ref: RegRecords) {
    return new CustomStore({
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        if (ref.rowSelected) {
          ref.isPrintAndExportAvailable = (ref.selectedRows.length <= printAndExportLimit && ref.selectedRows.length > 0);
          deferred.resolve(ref.selectedRows, { totalCount: ref.selectedRows.length });
        } else {
          let sortCriteria;
          if (loadOptions.sort != null) {
            sortCriteria = (loadOptions.sort[0].desc === false) ? loadOptions.sort[0].selector : loadOptions.sort[0].selector + ' DESC';
            ref.sortCriteria = sortCriteria;
          }
          let url = `${apiUrlPrefix}${ref.temporary ? 'temp-' : ''}records`;
          let params = '';
          if (loadOptions.skip) { params += `?skip=${loadOptions.skip}`; }
          let take = loadOptions.take != null ? loadOptions.take : fetchLimit;
          if (take) { params += `${params ? '&' : '?'}count=${take}`; }
          if (sortCriteria) { params += `${params ? '&' : '?'}sort=${sortCriteria}`; }
          if (ref.hitListId) { params += `${params ? '&' : '?'}hitListId=${ref.hitListId}`; }
          params += `&highlightSubStructures=${ref.ngRedux.getState().registrysearch.highLightSubstructure}`;
          url += params;
          ref.http.get(url)
            .toPromise()
            .then(result => {
              let response = result.json();
              ref.recordsTotalCount = response.totalCount;
              ref.updateHitListId(response.hitlistId);
              ref.setProgressBarVisibility(false);
              ref.isPrintAndExportAvailable = (response.totalCount <= printAndExportLimit && response.totalCount > 0);
              deferred.resolve(response.rows, { totalCount: response.totalCount });
            })
            .catch(error => {
              let message = getExceptionMessage(`The records were not retrieved properly due to a problem`, error);
              deferred.reject(message);
            });
        }
        if (!ref.isTotalSearchableCountUpdated) {
          ref.setTotalSearchableCount();
        }
        return deferred.promise();
      }
    });
  }

  private onDocumentClick(event: any) {
    const target = event.target || event.srcElement;
    if (target.title === 'Full Screen') {
      let fullScreenMode = target.className === 'fa fa-compress fa-stack-1x white';
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

  onSearch(hitListId) {
    this.loadIndicatorVisible = true;
    this.rowSelected = false;
    this.currentIndex = 0;
    this.updateHitListId(hitListId);
    this.grid.instance.refresh();
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
        if (PrivilegeUtils.hasDeletePrivilege(this.temporary, this.lookups.userPrivileges)) {
          let $deleteIcon = $links.filter('.dx-link-delete');
          $deleteIcon.addClass('dx-icon-trash');
          $deleteIcon.attr({ 'data-toggle': 'tootip', 'title': 'Delete' });
        }
      }
    }
    if (e.rowType === 'data' && e.column.dataField === 'Mol Formula') {
      let fieldData = e.value;
      if (fieldData) {
        e.cellElement.html(fieldData);
      }
    }
    if (e.rowType === 'header' && e.column.allowSorting) {
      let $header = e.cellElement.find('.dx-datagrid-text-content');
      $header.attr({ 'style': 'text-decoration:underline;' });
    }
  }

  onBatchCellPrepared(e) {
    if (e.rowType === 'data' && e.column.command === 'edit') {
      let isEditing = e.row.isEditing;
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      let $editIcon = $links.filter('.dx-link-edit');
      $editIcon.addClass('fa fa-flask fa-2');
      $editIcon.attr({ 'data-toggle': 'tootip', 'title': 'Create Container' });
    }
  }

  onContentReady(e) {
    e.component.element().find('.dx-datagrid-group-closed').attr('title', 'Show Batch Details');
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
    let id = e.data[this.idField];
    this.router.navigate([`records/${this.temporary ? 'temp' : ''}/${id}`]);
  }

  onEditingBatchGridStart(e) {
    e.cancel = true;
    let invContainerHandler = new RegInvContainerHandler();
    let systemSettings = new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings);
    systemSettings.isInventoryUseFullContainerForm
      ? invContainerHandler.openContainerPopup((systemSettings.invNewContainerURL + `&vRegBatchID=` +
        e.data.BATCHID + `&RefreshOpenerLocation=false`), null)
      : invContainerHandler.openContainerPopup((systemSettings.invSendToInventoryURL + `?RegIDList=` +
        e.data.REGID + `&OpenAsModalFrame=true`), invWideWindowParams);
  }

  onRowRemoving(e) {
    this.loadIndicatorVisible = true;
    let ids = e.data[this.idField];
    this.registryActions.deleteRecord(this.temporary, { data: [{ id: ids }] });
    e.cancel = true;
    if (this.grid.instance.getSelectedRowKeys().length > 0) {
      let key = this.grid.instance.getSelectedRowKeys().find(r => r[this.idField] === ids);
      if (key) {
        this.grid.instance.deselectRows(key);
      }
    }
    this.isTotalSearchableCountUpdated = false;
  }

  private manageQueries() {
    this.actions.openHitlists(this.temporary);
    this.currentIndex = 1;
  }

  private newQuery() {
    this.currentIndex = 2;
    if (this.clearSearchForm) {
      // Due to CDD activation issues, we can clear the search form only after showing the search form first.
      this.clearSearchForm = false;
      this.searchForm.clear();
    }
  }

  private saveQuery(isMarked: boolean) {
    this.isMarkedQuery = isMarked;
    this.hitlistVM.saveQueryVM.clear();
    this.popupVisible = true;
  }

  private retrieveAll() {
    this.updateHitListId(0);
    this.clearSearchForm = true;
    this.rowSelected = false;
    let keys = this.grid.instance.getSelectedRowKeys();
    this.grid.instance.deselectRows(keys);
    this.grid.instance.refresh();
  }

  private saveHitlist() {
    if (this.hitlistVM.saveQueryVM.data.name && this.hitlistVM.saveQueryVM.data.description) {
      if (this.isMarkedQuery === true) {
        let selectedhitIds = [];
        (this.temporary) ? this.selectedRows.forEach(v => { selectedhitIds.push(v.TEMPBATCHID); })
          : this.selectedRows.forEach(v => { selectedhitIds.push(v.REGID); });

        let url: string = `${apiUrlPrefix}hitlists/mark/${this.temporary}`;
        this.http.post(url, {
          name: this.hitlistVM.saveQueryVM.data.name,
          description: this.hitlistVM.saveQueryVM.data.description,
          isPublic: this.hitlistVM.saveQueryVM.data.isPublic,
          hitlistType: HitlistType.MARKED,
          markedHitIds: selectedhitIds
        })
          .toPromise()
          .then(result => {
            notifySuccess('Marked records saved successfully!', 5000);
          })
          .catch(error => {
            notifyException(`Saving the marked records failed due to a problem`, error, 5000);
          });

        this.hitlistVM.saveQueryVM.clear();
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
    if (e.hitlistId) {
      this.rowSelected = false;
      this.loadIndicatorVisible = true;
      this.updateHitListId(e.hitlistId);
      this.grid.instance.refresh();
    }
  }

  private restoreClicked(queryData: IQueryData) {
    this.currentIndex = 2;
    this.searchForm.restore(queryData);
  }

  private showMarked() {
    this.markedRecords = [];
    if (this.selectedRows && this.selectedRows.length > 0 && !this.rowSelected) {
      this.rowSelected = true;
      this.selectedRows.map(r => { this.markedRecords.push(r[this.idField]); });
      this.grid.instance.refresh();
      this.isPrintAndExportAvailable = true;
    }
  }

  registerMarkedStart(e) {
    let records: string[] = [];
    this.selectedRows.forEach(v => { records.push(v.TEMPBATCHID.toString()); });
    this.registryActions.bulkRegister(
      {
        description: e.description,
        duplicateAction: e.option,
        records: records
      });
    this.regMarkedModel.isVisible = false;
    this.registryActions.clearBulkRrgisterStatus();
    this.router.navigate([`records/bulkreg`]);
  }

  private deleteMarked() {
    if (this.selectedRows && this.selectedRows.length > 0) {
      let dialogResult = dxDialog.confirm(
        `Are you sure you want to delete these Registry Records?`,
        'Confirm Delete');
      dialogResult.done(result => {
        if (result) {
          let ids = [];
          this.loadIndicatorVisible = true;
          this.selectedRows.map(r => { ids.push({ id: r[this.idField] }); });
          this.registryActions.deleteRecord(this.temporary, { data: ids });
          this.selectedRows = [];
          this.rowSelected = false;
          let keys = this.grid.instance.getSelectedRowKeys();
          this.grid.instance.deselectRows(keys);
          this.isTotalSearchableCountUpdated = false;
        }
      });
    }
  }

  private registerMarked() {
    let maxRecordAllowed = this.lookups.systemSettings.find(v => v.name === 'MaxRegisterMarked').value;
    if ((this.selectedRows ? this.selectedRows.length : 0) <= Number(maxRecordAllowed)) {
      this.regMarkedModel.isVisible = true;
    } else {
      notify(`You are trying to register more than ` + maxRecordAllowed
        + ` records, which is not allowed. Please unmark some records and try again.`,
        `warning`, 5000);
    }
  }

  private showSearchResults() {
    this.markedRecords = [];
    if (this.rowSelected) {
      this.rowSelected = false;
      this.isPrintAndExportAvailable = false || this.recordsTotalCount <= printAndExportLimit;
      this.grid.instance.refresh();
    }
  }

  private printRecords() {
    this.loadIndicatorVisible = true;
    let url = `${apiUrlPrefix}hitlists/${this.hitListId}/print`;
    let params = '';
    if (this.temporary) { params += '?temp=true'; }
    params += `${params ? '&' : '?'}count=${printAndExportLimit}`;
    if (this.sortCriteria) { params += `&sort=${this.sortCriteria}`; }
    params += `&highlightSubStructures=${this.ngRedux.getState().registrysearch.highLightSubstructure}`;

    let data: number[];
    if (this.rowSelected) {
      data = this.selectedRows.map(r => r[this.idField]);
    }
    url += params;
    this.http.post(url, data).toPromise()
      .then(res => {
        let rows = res.json().rows;
        let structureColumnNamae = this.temporary ? 'Structure' : 'STRUCTUREAGGREGATION';
        this.imageService.generateMultipleImages(rows.map(r => r[structureColumnNamae])).subscribe(
          (values: Array<string>) => {
            let printContents: string;
            printContents = '<table width="100%" height="auto"><tr>';
            this.viewGroupsColumns.baseTableColumns.forEach(c => {
              if (c.visible) {
                printContents += `<td>${c.caption}</td>`;
              }
            });
            this.viewGroupsColumns.batchTableColumns.forEach(c => {
              if (c.visible) {
                printContents += `<td>${c.caption}</td>`;
              }
            });
            printContents += '</tr>';
            rows.forEach(row => {
              printContents += '<tr>';
              this.viewGroupsColumns.baseTableColumns.forEach(c => {
                if (c.visible) {
                  let field = row[c.dataField];
                  if (c.caption === 'Approved') {
                    printContents += `<td rowspan=${row.BatchDataSource.length}>
                    <img src="${(field === RegistryStatus.Approved) ? this.approvedIcon : this.notApprovedIcon}" /></td>`;
                  } else if (c.dataField === 'Structure' || c.dataField === 'STRUCTUREAGGREGATION') {
                    let structureImage = this.imageService.getImage(field);
                    printContents += `<td rowspan=${row.BatchDataSource.length}><img src="${structureImage ?
                      structureImage : this.defaultPrintStructureImage}" /></td>`;
                  } else if (c.dataType && c.dataType === 'date') {
                    let date = new Date(field);
                    printContents += `<td rowspan=${row.BatchDataSource.length}>
                    ${(field) ? `${date.getMonth() + 1}/${date.getDate()}/${date.getFullYear()}` : ''}</td>`;
                  } else if (c.dataType && c.dataType === 'boolean') {
                    printContents += `<td rowspan=${row.BatchDataSource.length}><div class="center">
                    <i class="fa fa-${(field === 'T') ? 'check-' : ''}square-o"></i></div></td>`;
                  } else {
                    printContents += `<td rowspan=${row.BatchDataSource.length}>${(field) ? field : ''}</td>`;
                  }
                }
              });
              let rowIndex = 0;
              row.BatchDataSource.forEach(batchRow => {
                if (rowIndex > 0) {
                  printContents += '<tr>';
                }
                this.viewGroupsColumns.batchTableColumns.forEach(c => {
                  if (c.visible) {
                    let field = batchRow[c.dataField];
                    if (c.dataType && c.dataType === 'date') {
                      let date = new Date(field);
                      printContents += `<td>${(field) ? date.toDateString() : ''}</td>`;
                    } else if (c.dataType && c.dataType === 'boolean') {
                      printContents += `<td><div class="center">
                      <i class="fa fa-${(field === 'T') ? 'check-' : ''}square-o"></i></div></td>`;
                    } else {
                      printContents += `<td >${(field) ? field : ''}</td>`;
                    }
                  }
                });
                printContents += '</tr>';
              });
            });

            let popupWin;
            popupWin = window.open('', '_blank', 'top=0,left=0,height=500,width=auto');
            popupWin.document.open();
            popupWin.document.write(`<html>
              <head>
                <title>Print table</title>
                <link rel="stylesheet" href="/node_modules/font-awesome/css/font-awesome.min.css">
                <style>
                table, tr, td 
                {
                    border:solid 1px #f0f0f0;
                    font-size: 12px;
                    font-family: 'Helvetica Neue', 'Segoe UI', Helvetica, Verdana, sans-serif;
                    border-spacing: 0px;
                }
                img {
                  max-width: 100px;
                  margin-left: auto;
                  margin-right: auto;
                  display: block;
                }
                .center {
                  text-align: center;
                }
                .green {
                  color: #58a618 !important;
                }
                .red {
                  color: #b71234 !important;
                }
                </style>
              </head>
              <body onload="window.print(); window.close()">${printContents}</body>
              </html>`);
            popupWin.document.close();
            this.setProgressBarVisibility(false);
          });
      })
      .catch(error => {
        this.setProgressBarVisibility(false);
      });

  }

  private get isSessionValid(): boolean {
    return this.ngRedux.getState().session.lookups != null;
  }

  private isApproved(data): boolean {
    return data.value === RegistryStatus.Approved;
  }

  private get approvalsEnabled(): boolean {
    return this.temporary && this.isSessionValid && new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings).isApprovalsEnabled;
  }

  private get approveMarkedEnabled(): boolean {
    return this.temporary && this.selectedRows && this.selectedRows.length > 0
      && this.approvalsEnabled
      && PrivilegeUtils.hasApprovalPrivilege(this.lookups.userPrivileges);
  }

  // set delete marked button visibility
  private get deleteMarkedEnabled(): boolean {
    if (this.selectedRows && this.selectedRows.length > 0) {
      return PrivilegeUtils.hasDeletePrivilege(this.temporary, this.lookups.userPrivileges);
    }
    return false;
  }

  // set create container button visibility
  private get createContainersEnabled(): boolean {
    return ((!this.temporary && this.isSessionValid
      && new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings).isInventoryIntegrationEnabled
      && new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings).isSendToInventoryEnabled)
      && PrivilegeUtils.hasCreateContainerPrivilege(this.lookups.userPrivileges)) ? true : false;
  }

  private get filterRowEnabled(): boolean {
    return false;
  }


  private get getTotalRecordsCount(): Number {
    return this.recordsTotalCount;
  }

  // set bulk register button visibility
  private get registerMarkedEnabled(): boolean {
    return this.selectedRows && this.selectedRows.length > 0 && this.temporary && PrivilegeUtils.hasRegisterPrivilege(this.lookups.userPrivileges);
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
      this.loadIndicatorVisible = false;
      return;
    }
    // Otherwise, shift ids
    let id = ids.shift();
    let row = this.selectedRows.find(r => r[this.idField] === id);
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
      let dialogResult = dxDialog.confirm(
        `Are you sure you want to approve the marked registries?`,
        'Confirm Approval');
      dialogResult.done(res => {
        if (res) {
          let ids: number[] = this.selectedRows.map(r => r[this.idField]);
          let succeeded: number[] = [];
          let failed: number[] = [];
          this.loadIndicatorVisible = true;
          this.approveRows(ids, failed, succeeded);
        }
      });
    }
  }

  lodingCompleted() {
    this.loadIndicatorVisible = false;
  }

  createBulkContainers() {
    let regInvContainer = new RegInvContainerHandler();
    let systemSettings = new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings);
    regInvContainer.openContainerPopup((systemSettings.invSendToInventoryURL + `?RegIDList=` +
      this.selectedRows.map(({ REGID }) => REGID).join() + `&OpenAsModalFrame=false`), invWideWindowParams);
  }

};
