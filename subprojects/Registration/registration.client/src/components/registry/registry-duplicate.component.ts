import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgRedux, select } from '@angular-redux/store';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { Observable, Subscription } from 'rxjs';
import { IFormGroup, prepareFormGroupData, FormGroupType, getExceptionMessage, notify, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { RecordDetailActions, IAppState, ILookupData } from '../../redux';
import { HttpService } from '../../services';
import { CRegistryRecord, CViewGroup } from './base';
import { CFragment } from '../common';
import { DxiCenterComponent } from 'devextreme-angular/ui/nested/center-dxi';

@Component({
  selector: 'reg-duplicate-record',
  template: require('./registry-duplicate.component.html'),
  styles: [require('./records.css')],
  // host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDuplicateRecord implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  private gridHeight: number;
  private duplicateData$: Observable<any[]>;
  private recordsSubscription: Subscription;
  private duplicateRecoreCount;
  private currentRecord: { ID: number, REGNUMBER: string };
  private duplicateActions = [];
  private duplicateButtonVisibility: boolean = false;
  @Input() parentHeight: number;
  @Output() onClose = new EventEmitter<any>();
  @Input() sourceRecordIsTemporary: boolean;
  private dataStore: CustomStore;
  private fetchLimit = 20;

  private columns = [{
    cellTemplate: 'commandCellTemplate',
    width: 80
  }, {
    dataField: 'REGNUMBER',
    caption: 'Reg Number'
  }, {
    dataField: 'NAME',
    caption: 'Name'
  }, {
    dataField: 'CREATED',
    dataType: 'date',
    caption: 'Created'
  }, {
    dataField: 'MODIFIED',
    dataType: 'date',
    caption: 'Modified'
  }, {
    dataField: 'CREATOR',
    caption: 'Created By'
  }, {
    dataField: 'STRUCTURE',
    caption: 'Structure',
    cellTemplate: 'cellTemplate',
    width: 220,
    alignment: 'center',
    allowFiltering: false,
    allowSorting: false,
  }, {
    dataField: 'STATUS',
    caption: 'Status',
    width: 70,
  }, {
    dataField: 'APPROVED',
    caption: 'Approved',
    width: 80
  }, {
    dataField: 'REGNUMBER',
    caption: 'Resolution options',
    cellTemplate: 'resolutionOptionTemplate',
    allowFiltering: false,
    allowSorting: false,
    width: 150
  }];

  private editRowIndex: number = -1;
  constructor(
    private ngRedux: NgRedux<IAppState>,
    private router: Router,
    private http: HttpService,
    private actions: RecordDetailActions,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {
    this.duplicateData$ = this.ngRedux.select(['registry', 'duplicateRecords']);
    this.recordsSubscription = this.duplicateData$.subscribe((value: number[]) => this.loadData(value));
  }

  ngOnDestroy() {
    if (this.recordsSubscription) {
      this.recordsSubscription.unsubscribe();
    }
  }

  edit(e) {
    this.currentRecord = { ID: e.data.ID, REGNUMBER: e.data.REGNUMBER };
    this.editRowIndex = e.rowIndex;
  }

  cancel(e) {
    this.editRowIndex = -1;
  }

  isEditRowVisible(e, view) {
    if (view === 'data') {
      if (this.editRowIndex === e) {
        return true;
      } else { return false; }
    } else if (view === 'edit') {
      if (this.editRowIndex === e) {
        return false;
      } else { return true; }
    }
    return false;
  }

  loadData(e) {
    if (e) {
      this.gridHeight = this.parentHeight - 80;
      this.duplicateRecoreCount = e.TotalDuplicateCount;
      this.dataStore = this.createCustomStore(this);
      // this.duplicateActions = e.DuplicateActions;
      let settings = this.ngRedux.getState().session.lookups.systemSettings;
      this.duplicateButtonVisibility = settings.filter(s => s.name === 'EnableDuplicateButton')[0].value === 'True' ? true : false;
      this.columns = this.columns.map(s => this.updateGridColumn(s));
      this.actions.clearDuplicateRecord();
      this.changeDetector.markForCheck();
    }
  }

  setButtonVisibility(buttonType: string, regNumber: string) {
    let enabled: boolean = false;
    if (this.duplicateActions) {
      switch (buttonType) {
        case 'AddBatch':
          enabled = this.duplicateActions.filter(s => s.REGNUMBER === regNumber)[0].canAddBatch;
          break;
        case 'UseComponent':
          enabled = this.duplicateActions.filter(s => s.REGNUMBER === regNumber)[0].canUseCompound;
          break;
        case 'UseStructure':
          enabled = this.duplicateActions.filter(s => s.REGNUMBER === regNumber)[0].canUseStructure;
          break;
      }
    }
    return enabled;
  }

  updateGridColumn(gridColumn) {
    if (gridColumn.dataField === 'CREATOR') {
      gridColumn.lookup = {
        dataSource: this.ngRedux.getState().session.lookups.users,
        displayExpr: 'USERID',
        valueExpr: 'PERSONID'
      };
    }
    return gridColumn;
  }

  onToolbarPreparing(e) {
    e.toolbarOptions.items.unshift({
      location: 'before',
      template: 'toolbarContents'
    });
  }

  private getGridHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  private get addBatchButtonTitle(): string {
    return this.sourceRecordIsTemporary ? 'Add Batch' : 'Move Batches';
  }

  dismissAlert() {
    this.gridHeight = this.parentHeight - 20;
  }

  createDuplicateRecord(action: string, regNum: string) {
    this.actions.createDuplicate(
      this.ngRedux.getState().registry.previousRecordDetail,
      action, regNum);
    this.onClose.emit(action);
  }

  cancelDuplicateResolution(e) {
    this.onClose.emit('cancel');
  }

  private createCustomStore(ref: any) {
    return new CustomStore({
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        /*if (loadOptions.skip <= 5) {
          alert(loadOptions.take+3);
          alert(loadOptions.skip);
          deferred.resolve(ref.datasource.DuplicateRecords, { totalCount: ref.datasource.TotalDuplicatesCountIdentified });
        } else {
          alert(loadOptions.take);
          alert(loadOptions.skip);
        }*/
        if (loadOptions.take) {
          let sortCriteria, sortOrder;
          if (loadOptions.sort != null) {
            ref.sortOrder = (loadOptions.sort[0].desc === false) ? 'ASC' : 'DESC';
            ref.sortCriteria = sortCriteria;
          }
          let url = `${apiUrlPrefix}get-duplicate-records`;
          let params = '';
          if (loadOptions.skip) { params += `?skip=${loadOptions.skip}`; }
          let take = loadOptions.take != null ? loadOptions.take : this.fetchLimit;
          let data = JSON.parse(sessionStorage.getItem('registerRecordData'));
          data.skip = loadOptions.skip;
          data.count = loadOptions.take;
          data.sort = ref.sortCriteria ? ref.sortCriteria : 'REGNUMBER';
          data.sortOrder = ref.sortOrder;
          ref.http.post(url, data)
            .toPromise()
            .then(result => {
              let response = result.json();
              // ref.recordsTotalCount = response.totalCount;
              // ref.noDataText = ref.recordsTotalCount === 0 ? 'Search returned no hit!' : '';
              // ref.setProgressBarVisibility(false);
              // ref.duplicateActions = Array.prototype.push.apply(ref.duplicateActions, response.data.DuplicateActions);
              Array.prototype.push.apply(ref.duplicateActions, response.data.DuplicateActions);
              deferred.resolve(response.data.DuplicateRecords
                , { totalCount: ref.duplicateRecoreCount });
            })
            .catch(error => {
              let message = getExceptionMessage(`The records were not retrieved properly due to a problem`, error);
              deferred.reject(message);
            });
        }
        return deferred.promise();
      }
    });
  }

}
