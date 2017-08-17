import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgRedux, select } from '@angular-redux/store';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { IFormGroup, prepareFormGroupData, FormGroupType, getExceptionMessage, notify, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { RecordDetailActions, IAppState, ILookupData } from '../../redux';
import { HttpService } from '../../services';
import { CRegistryRecord, CViewGroup, CFragment } from './base';

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
  private datasource: any[];
  private loadingVisible: boolean = false;
  private currentRecord: { ID: number, REGNUMBER: string };
  @Input() parentHeight: number;
  @Output() onClose = new EventEmitter<any>();
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
    width: 150,
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
  private buttonVisibility = { duplicate: true, addBatch: true, useComponent: true, useStructure: true };
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
  }

  loadData(e) {
    if (e) {
      this.gridHeight = this.parentHeight - 80;
      let settings = this.ngRedux.getState().session.lookups.systemSettings;
      this.buttonVisibility.addBatch = settings.filter(s => s.name === 'EnableAddBatchButton')[0].value === 'True' ? true : false;
      this.buttonVisibility.duplicate = settings.filter(s => s.name === 'EnableDuplicateButton')[0].value === 'True' ? true : false;
      this.buttonVisibility.useComponent = settings.filter(s => s.name === 'EnableUseComponentButton')[0].value === 'True' ? true : false;
      this.buttonVisibility.useStructure = settings.filter(s => s.name === 'EnableUseStructureButton')[0].value === 'True' ? true : false;
      this.datasource = e;
      this.columns = this.columns.map(s => this.updateGridColumn(s));
      this.actions.clearDuplicateRecord();
      this.changeDetector.markForCheck();
    }
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

  dismissAlert() {
    this.gridHeight = this.parentHeight - 20;
  }

  createDuplicateRecord(action: string, regNum: string) {
    this.loadingVisible = true;
    this.actions.createDuplicate(
      this.ngRedux.getState().registry.previousRecordDetail,
      action, regNum);
  }

  cancelDuplicateResolution(e) {
    this.loadingVisible = false;
    this.onClose.emit(e);
  }

};
