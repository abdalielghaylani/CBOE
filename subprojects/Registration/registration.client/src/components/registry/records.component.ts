import {
  Component,
  Input, ViewChild,
  Output,
  OnInit,
  OnDestroy,
  EventEmitter,
  ChangeDetectorRef, ChangeDetectionStrategy, ElementRef,
  Directive, HostListener, NgZone
} from '@angular/core';
import { select, NgRedux } from '@angular-redux/store';
import { Router, NavigationEnd } from '@angular/router';
import { Observable ,  Subscription } from 'rxjs';
import { EmptyObservable } from 'rxjs/Observable/EmptyObservable';
import 'rxjs/add/operator/filter';
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
import { RegistryActions, RegistrySearchActions, IHitlistData } from '../../redux';
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
  private viewGroupsColumns: CViewGroupColumns = new CViewGroupColumns();
  private lookupsSubscription: Subscription;
  private isLoadingSubscription: Subscription;
  private routerSubscription: Subscription;
  private lookups: ILookupData;
  private popupVisible: boolean = false;
  private marksShown: boolean = false;
  private refinedRows: any[] = [];
  private refinedTotalRecordsCount: number = 0;
  private tempResultRows: any[];
  private hitlistVM: regSearchTypes.CQueryManagementVM = new regSearchTypes.CQueryManagementVM(this.ngRedux.getState());
  private hitlistData$: Observable<ISearchRecords>;
  private bulkRecordData$: Observable<any[]>;
  private isMarkedQuery: boolean;
  private loadIndicatorVisible: boolean = false;
  private gridHeight: string;
  private currentIndex: number = 0;
  private sortCriteria: string;
  private idField;
  private regMarkedModel: IRegMarkedPopupModel = { description: '', option: 'None', isVisible: false };
  private defaultPrintStructureImage = require('../common/assets/no-structure.png');
  private approvedIcon = require('../common/assets/approved.png');
  private notApprovedIcon = require('../common/assets/notapproved.png');
  private checkedIcon = require('../common/assets/checked-checkbox-100.png');
  private uncheckedIcon = require('../common/assets/unchecked-checkbox-100.png');
  private isPrintAndExportAvailable: boolean = false;
  private dataStore: CustomStore;
  private recordsTotalCount: number = 0;
  private refreshHitList: boolean = false;
  private totalSearchableCount: number = 0;
  private markedHitListId: number = 0;
  private markedHitCount: number = 0;
  private queryFormShown: boolean = false;
  private updateQueryForm: boolean = true;
  private isRefine = false;
  private markedHitsMax: number = 0;
  private structureDataVisible: boolean = false;
  private structureData: string;
  private noDataText: string;
  private url: string;
  private isQueryManagementVisible: boolean = true;

  constructor(
    private router: Router,
    private http: HttpService,
    private ngRedux: NgRedux<IAppState>,
    private registryActions: RegistryActions,
    private actions: RegistrySearchActions,
    private elementRef: ElementRef,
    private imageService: CStructureImagePrintService,
    private changeDetector: ChangeDetectorRef,
    private ngZone: NgZone) {
    this.routerSubscription = router.events.filter(e => e instanceof NavigationEnd).subscribe(value => {
      const match = (value as NavigationEnd).url.match(/\/hits\/(\d+)$/);
      if (match && match.length === 2 && this.hitListId > 0 && !this.marksShown && this.url !== (value as NavigationEnd).url) {
        this.url = (value as NavigationEnd).url;
        this.hitListId = +match[1];
        this.loadIndicatorVisible = true;
        this.currentIndex = 0;
        this.grid.instance.refresh();
      }
    });
  }

  ngOnInit() {
    this.url = this.router.url;
    this.queryFormShown = false;
    this.updateQueryForm = true;
    this.idField = this.temporary ? 'TEMPBATCHID' : 'REGID';
    this.marksShown = this.isPrintAndExportAvailable = this.router.url.endsWith('/marked');
    this.getMarkedHitList();
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
    this.isLoadingSubscription = this.isLoading$.subscribe(d => { this.setProgressBarVisibility(d); });

    window.NewRegWindowHandle = window.NewRegWindowHandle || {};
    window.NewRegWindowHandle.closePrintPage = this.closePrintPage.bind(this);
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
    if (this.isLoadingSubscription) {
      this.isLoadingSubscription.unsubscribe();
    }
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
    window.NewRegWindowHandle.closePrintPage = null;
  }

  updateHitListId(id: number) {
    if (this.hitListId !== id) {
      this.hitListId = id;
    }
  }

  bulkRegisterStatus(val) {
    if (val) {
      this.router.navigate([`records/bulkreg`]);
    }
  }

  getAbsoluteUrl(value) {
    if (value && value.indexOf('http') < 0 && value.substring(0, 2) !== '//') {
      value = `//${value}`;
    }
    return value;
  }

  setProgressBarVisibility(visible: boolean) {
    this.loadIndicatorVisible = visible;
    this.changeDetector.markForCheck();
  }

  setTotalSearchableCount() {
    this.http.get(`${apiUrlPrefix}records/databaseRecordCount?temp=${this.temporary}`).toPromise()
      .then((res => {
        this.totalSearchableCount = res.json();
      }).bind(this))
      .catch(error => {
        // notifyException(`The total searchable count query failed due to a problem`, error, 5000);
      });
  }

  getMarkedHitList(retry: boolean = true) {
    const url = `${apiUrlPrefix}hitlists/markedHitList?temp=${this.temporary}`;
    this.http.get(url).toPromise()
      .then((res => {
        let markedHitList = res.json();
        this.markedHitListId = markedHitList.hitlistId;
        this.markedHitCount = markedHitList.numberOfHits;
        if (this.marksShown) {
          if (this.markedHitCount === 0) {
            this.navigateToHits(this.hitListId);
          } else {
            this.loadData();
          }
        }
        this.changeDetector.markForCheck();
      }).bind(this))
      .catch((error => {
        if (retry) {
          // If case it fails, try again once to compensate occasional server failures.
          this.getMarkedHitList(false);
        } else {
          if (this.marksShown && this.markedHitListId === 0) {
            // In case when we need to show marked and this keeps failing, navigate back to the unmarked list.
            // Otherwise, the page won't show anything.
            this.showSearchResults();
          }
          notifyException(`The marked hit-list query failed due to a problem`, error, 5000);
        }
      }).bind(this));
  }

  // Trigger data retrieval for the view to show.
  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
    if (this.loggedIn$) {
      this.loadIndicatorVisible = true;
    }

    let systemSettings = new CSystemSettings(this.lookups.systemSettings);
    let formGroupType = this.temporary ? FormGroupType.SearchTemporary : FormGroupType.SearchPermanent;
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    let formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
    this.viewGroupsColumns = this.lookups
      ? CViewGroup.getColumns(this.temporary, formGroup, this.lookups.disabledControls, this.lookups.pickListDomains, systemSettings)
      : new CViewGroupColumns();
    this.markedHitsMax = systemSettings.markedHitsMax;
    this.isQueryManagementVisible = systemSettings.allowQueryManagement;
    if (this.restore) {
      this.restoreHitlist();
    } else {
      this.loadData();
    }
  }

  restoreHitlist() {
    if (this.restore && (this.hitListId > 0)) {
      this.dataStore = this.createCustomStore(this);
      this.updateQueryForm = true;
    }
  }

  getGridHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  loadData() {
    if (this.marksShown && this.markedHitListId === 0) {
      return;
    }
    this.setTotalSearchableCount();
    this.dataStore = this.createCustomStore(this);
    this.actions.openHitlists(this.temporary);
    this.hitlistData$ = this.ngRedux.select(['registrysearch', 'hitlist']);
    this.gridHeight = this.getGridHeight();
  }

  private createCustomStore(ref: RegRecords) {
    const systemSettings = new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings);
    return new CustomStore({
      key: ref.idField,
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        if (ref.isRefine) {
          let refinedRows = ref.refinedRows;
          ref.recordsTotalCount = ref.refinedTotalRecordsCount;
          ref.noDataText = ref.recordsTotalCount === 0 ? 'Search returned no hit!' : '';
          ref.refinedRows = [];
          ref.refinedTotalRecordsCount = 0;
          ref.isRefine = false;
          ref.isPrintAndExportAvailable = (ref.recordsTotalCount <= printAndExportLimit && ref.recordsTotalCount > 0);
          deferred.resolve(refinedRows, { totalCount: ref.recordsTotalCount });
          ref.setProgressBarVisibility(false);
        } else {
          if (loadOptions.take) {
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
            if (ref.sortCriteria) { params += `${params ? '&' : '?'}sort=${ref.sortCriteria}`; }
            if (ref.marksShown) {
              params += `${params ? '&' : '?'}hitListId=${ref.markedHitListId}`;
            } else if (ref.hitListId) {
              params += `${params ? '&' : '?'}hitListId=${ref.hitListId}`;
            }
            params += `&highlightSubStructures=${ref.ngRedux.getState().registrysearch.highLightSubstructure}`;
            url += params;
            ref.http.get(url)
              .toPromise()
              .then(result => {
                let response = result.json();
                ref.recordsTotalCount = response.totalCount;
                ref.noDataText = ref.recordsTotalCount === 0 ? 'Search returned no hit!' : '';
                if (response.hitlistId !== ref.markedHitListId) {
                  ref.updateHitListId(response.hitlistId);
                }
                ref.setProgressBarVisibility(false);
                ref.isPrintAndExportAvailable = (response.totalCount <= printAndExportLimit && response.totalCount > 0);
                deferred.resolve(response.rows, { totalCount: response.totalCount });
              })
              .catch(error => {
                let message = getExceptionMessage(`The records were not retrieved properly due to a problem`, error);
                deferred.reject(message);
              });
          }
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

  navigateToHits(hitListId: number) {
    this.router.navigate([`records/${this.temporary ? 'temp' : ''}${hitListId > 0 ? '/hits/' + hitListId : ''}`]);
  }

  onSearch(hitListId) {
    this.navigateToHits(hitListId);
  }

  onRefine(result) {
    this.setTotalSearchableCount();
    this.marksShown = false;
    this.currentIndex = 0;
    this.refinedRows = result.rows;
    this.refinedTotalRecordsCount = result.totalCount;
    this.navigateToHits(result.hitlistId);
  }

  clearMarked() {
    this.loadIndicatorVisible = true;
    let url = `${apiUrlPrefix}hitlists/unMarkhit/all${this.temporary ? '?temp=true' : ''}`;
    this.http.put(url, undefined).toPromise()
      .then(result => {
        if (this.marksShown) {
          this.showSearchResults();
        } else {
          let markedHitList = result.json();
          this.markedHitListId = markedHitList.hitlistId;
          this.markedHitCount = markedHitList.numberOfHits;
          this.grid.instance.refresh();
          this.changeDetector.markForCheck();
          this.setProgressBarVisibility(false);
        }
      })
      .catch(error => {
        notifyException(`Clearing all marks failed due to a problem`, error, 5000);
        this.setProgressBarVisibility(false);
      });
  }

  prepareAllMarked(e) {
    e.element.on('click', ((e2) => {
      this.setProgressBarVisibility(true);
      let params = this.temporary ? '?temp=true' : '';
      params += `${params ? '&' : '?'}hitlistId=${this.hitListId}`;
      if (this.sortCriteria) { params += `&sort=${this.sortCriteria}`; }
      let url = `${apiUrlPrefix}hitlists/markhit/all${params}`;
      this.http.put(url, undefined).toPromise()
        .then(result => {
          let markedHitList = result.json();
          this.markedHitListId = markedHitList.hitlistId;
          this.markedHitCount = markedHitList.numberOfHits;
          this.grid.instance.refresh();
          this.changeDetector.markForCheck();
          this.setProgressBarVisibility(false);
        })
        .catch(error => {
          notifyException(`Marking all records failed due to a problem`, error, 5000);
          this.setProgressBarVisibility(false);
        });
      e2.stopPropagation();
    }).bind(this));
  }

  prepareMarked(e) {
    e.element.on('click', ((e2) => {
      if (e2.currentTarget && e2.currentTarget.firstChild) {
        const value = e2.currentTarget.firstChild.value;
        if (value !== 'true' && value !== '1') {
          if (this.markedHitCount >= this.markedHitsMax) {
            e2.stopPropagation();
          }
        }
      }
    }).bind(this));
  }

  selectMarked(event, data) {
    const idField = this.temporary ? 'TEMPBATCHID' : 'MIXTUREID';
    const recordId = data.row.data[idField];
    const url = `${apiUrlPrefix}hitlists/${event.value ? 'markhit' : 'unMarkhit'}/${recordId}${this.temporary ? '?temp=true' : ''}`;
    this.http.put(url, undefined)
      .toPromise()
      .then((result => {
        let markedHitList = result.json();
        this.markedHitListId = markedHitList.hitlistId;
        this.markedHitCount = markedHitList.numberOfHits;
        data.row.data.Marked = event.value ? 1 : 0;
        this.changeDetector.markForCheck();
      }).bind(this))
      .catch(error => {
        notifyException(`${event.value ? 'Marking' : 'Un-marking'} the selected record failed due to a problem`, error, 5000);
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
        // For Edit
        let $editIcon = $links.filter('.dx-link-edit');
        $editIcon.addClass('fa fa-info-circle');
        $editIcon.attr({ 'data-toggle': 'tootip', 'title': 'Detail view' });
        // For Delete icon
        if (PrivilegeUtils.hasDeletePrivilege(this.temporary, this.lookups.userPrivileges) &&
          !(this.approvalsEnabled && this.isApproved({ value: e.data.STATUSID }))) {
          let $deleteIcon = $links.filter('.dx-link-delete');
          $deleteIcon.addClass('dx-icon-trash');
          $deleteIcon.attr({ 'data-toggle': 'tootip', 'title': 'Delete' });
        }
      }
    }
    if (e.rowType === 'data' && e.column.dataField === 'Marked') {
      let $checkbox = e.cellElement.find('.dx-checkbox');
      let attrId = this.temporary ? 'TEMPBATCHID' : 'MIXTUREID';
      $checkbox.attr({ attrId: this.temporary ? e.data.TEMPBATCHID : e.data.MIXTUREID });
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
    this.setProgressBarVisibility(true);
    let id = Number(e.data[this.idField]);
    let url = `${apiUrlPrefix}${this.temporary ? 'temp-' : ''}records/${id}`;
    this.http.delete(url).toPromise()
      .then(res => {
        this.currentIndex = 0;
        this.setProgressBarVisibility(false);
        this.getMarkedHitList();
        this.grid.instance.refresh();
        notifySuccess(`The record was deleted successfully!`, 5000);
        this.setTotalSearchableCount();
        this.newQuery();
      })
      .catch(error => {
        notifyException(`The record was not deleted due to a problem`, error, 5000);
        this.setProgressBarVisibility(false);
      });
    e.cancel = true;
  }

  private manageQueries() {
    this.actions.openHitlists(this.temporary);
    this.currentIndex = 1;
  }

  private newQuery() {
    this.showQueryForm();
    if (this.queryFormShown) {
      this.searchForm.clear();
    }
  }

  private saveQuery(isMarked: boolean) {
    this.isMarkedQuery = isMarked;
    this.hitlistVM.saveQueryVM.clear();
    this.popupVisible = true;
  }

  private saveHitlist() {
    if (this.hitlistVM.saveQueryVM.data.name && this.hitlistVM.saveQueryVM.data.description) {
      if (this.isMarkedQuery === true) {
        let url: string = `${apiUrlPrefix}hitlists/mark/${this.temporary}`;
        this.http.post(url, {
          name: this.hitlistVM.saveQueryVM.data.name,
          description: this.hitlistVM.saveQueryVM.data.description,
          isPublic: this.hitlistVM.saveQueryVM.data.isPublic,
          hitlistType: HitlistType.SAVED
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
    if (this.updateQueryForm) {
      if (id > 0) {
        let url = `${apiUrlPrefix}hitlists/${id}/query${this.temporary ? '?temp=true' : ''}`;
        this.http.get(url).toPromise()
          .then(res => {
            let queryData = res.json() as IQueryData;
            this.searchForm.restore(queryData);
          })
          .catch(error => {
            notifyException(`Restoring the previous query failed due to a problem`, error, 5000);
          });
      } else if (this.queryFormShown) {
        this.searchForm.clear();
      }
      this.updateQueryForm = false;
    }
    this.showQueryForm();
  }

  private refineQuery(id: Number) {
    this.showQueryForm();
    this.isRefine = true;
    this.searchForm.clear();
  }

  private cancelSaveQuery() {
    this.popupVisible = false;
  }

  private headerClicked(e) {
    this.currentIndex = 0;
    this.isRefine = false;
    if (e.hitlistId) {
      this.navigateToHits(e.hitlistId);
    }
  }

  private showQueryForm() {
    this.setTotalSearchableCount();
    this.currentIndex = 2;
    this.queryFormShown = true;
  }

  private restoreClicked(queryData: IQueryData) {
    this.showQueryForm();
    this.searchForm.restore(queryData);
    this.updateQueryForm = false;
  }

  private showMarked() {
    this.router.navigate([`${this.router.url}/marked`]);
  }

  registerMarkedStart(e) {
    this.registryActions.bulkRegister(
      {
        description: e.description,
        duplicateAction: e.option,
        records: []
      });
    this.regMarkedModel.isVisible = false;
    this.ngRedux.dispatch(RegistryActions.bulkRegisterRecordSuccessAction([]));
    this.router.navigate([`records/bulkreg`]);
  }

  private deleteMarked() {
    if (this.markedHitCount > 0) {
      let dialogResult = dxDialog.confirm(
        `Are you sure you want to delete these Registry Records?`,
        'Confirm Delete');
      dialogResult.done(result => {
        if (result) {
          this.loadIndicatorVisible = true;
          let url = `${apiUrlPrefix}hitlists/markedHitList/delete${this.temporary ? '?temp=true' : ''}`;
          this.http.put(url, undefined)
            .toPromise()
            .then(res => {
              let responseData = res.json();
              if (responseData.data.status) {
                notifySuccess(responseData.message, 5000);
              } else {
                notifyError(responseData.message, 5000);
              }
              if (responseData.data.status && this.marksShown) {
                this.showSearchResults();
              } else {
                this.setProgressBarVisibility(false);
                this.getMarkedHitList();
                this.grid.instance.refresh();
                this.setTotalSearchableCount();
              }
            })
            .catch(error => {
              notifyException(`Delete records failed due to a problem`, error, 5000);
              this.setProgressBarVisibility(false);
            });
        }
      });
    }
  }

  private registerMarked() {
    let maxRecordAllowed = this.lookups.systemSettings.find(v => v.name === 'MaxRegisterMarked').value;
    if (this.markedHitCount <= Number(maxRecordAllowed)) {
      this.regMarkedModel.isVisible = true;
    } else {
      notify(`You are trying to register more than ` + maxRecordAllowed
        + ` records, which is not allowed. Please unmark some records and try again.`,
        `warning`, 5000);
    }
  }

  private showAllRecords() {
    this.router.navigate([`records${this.temporary ? '/temp' : ''}`]);
  }

  /**
   * This must be called only when the list is in marks-shown mode.
   * In this mode, the URL ends with /marked, and navigating to URL without /marked would show the desired list.
   */
  private showSearchResults() {
    this.router.navigate([this.router.url.replace('/marked', '')]);
  }

  closePrintPage() {
    this.ngZone.run(() => this.setProgressBarVisibility(false));
  }

  private printRecords() {
    this.loadIndicatorVisible = true;
    let url = `${apiUrlPrefix}hitlists/${this.marksShown ? this.markedHitListId : this.hitListId}/print`;
    let params = '';
    if (this.temporary) { params += '?temp=true'; }
    params += `${params ? '&' : '?'}count=${printAndExportLimit}`;
    if (this.sortCriteria) { params += `&sort=${this.sortCriteria}`; }
    params += `&highlightSubStructures=${this.ngRedux.getState().registrysearch.highLightSubstructure}`;

    url += params;
    this.http.post(url, undefined).toPromise()
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
                  } else if (c.caption === 'Marked') {
                    printContents += `<td rowspan=${row.BatchDataSource.length}>
                    <img style="height:12px" src="${(field === 0) ? this.uncheckedIcon : this.checkedIcon}" /></td>`;
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
                    if (c.lookup && c.lookup.dataSource) {
                      let option = c.lookup.dataSource.find(d => d.key.toString() === field);
                      if (option) {
                        field = option.value;
                      }
                    }
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
                      if (c.lookup && c.lookup.dataSource) {
                        let option = c.lookup.dataSource.find(d => d.key.toString() === field);
                        if (option) {
                          field = option.value;
                        }
                      }
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
                    padding: 4px;
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
            popupWin.onbeforeunload = function () {
              this.opener.NewRegWindowHandle.closePrintPage();
            };
            popupWin.document.close();
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
    return this.temporary && this.markedHitCount > 0
      && this.approvalsEnabled
      && PrivilegeUtils.hasApprovalPrivilege(this.lookups.userPrivileges);
  }

  // set delete marked button visibility
  private get deleteMarkedEnabled(): boolean {
    if (this.markedHitCount > 0) {
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
    return this.markedHitCount > 0 && this.temporary && PrivilegeUtils.hasRegisterPrivilege(this.lookups.userPrivileges);
  }

  private approveMarked() {
    if (this.markedHitCount > 0) {
      let dialogResult = dxDialog.confirm(
        `Are you sure you want to approve the marked registries?`,
        'Confirm Approval');
      dialogResult.done(res => {
        if (res) {
          this.loadIndicatorVisible = true;
          let url = `${apiUrlPrefix}hitlists/markedHitList/approve${this.temporary ? '?temp=true' : ''}`;
          this.http.put(url, undefined)
            .toPromise()
            .then(result => {
              let responseData = result.json();
              if (responseData.data.status) {
                notifySuccess(responseData.message, 5000);
              } else {
                notifyError(responseData.message, 5000);
              }
              this.setProgressBarVisibility(false);
              this.getMarkedHitList();
              this.grid.instance.refresh();
            })
            .catch(error => {
              notifyException(`Approve records failed due to a problem`, error, 5000);
              this.setProgressBarVisibility(false);
            });
        }
      });
    }
  }

  createBulkContainers() {
    let url = `${apiUrlPrefix}hitlists/getRegNumberList?hitlistId=${this.markedHitListId}`;
    this.http.get(url).toPromise()
      .then(res => {
        let selectedRows = res.json();
        let regInvContainer = new RegInvContainerHandler();
        let systemSettings = new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings);
        regInvContainer.openContainerPopup((systemSettings.invSendToInventoryURL + `?RegIDList=` +
          selectedRows + `&OpenAsModalFrame=false`), invWideWindowParams);
      })
      .catch(error => {
        notifyException(`Getting reg number list failed due to a problem`, error, 5000);
      });
  }

  private structureImageClicked(e: string) {
    if (e) {
      this.structureData = e;
      this.structureDataVisible = true;
    }
  }
}

