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
import { IAppState, ILookupData, RegistryActions } from '../../redux';
import { HttpService } from '../../services';
import { CRegistryRecord, CViewGroup } from './base';
import { CFragment } from '../common';

@Component({
  selector: 'reg-bulk-register-record',
  template: require('./record-bulk-register.component.html'),
  styles: [require('./records.css')],
  // host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBulkRegisterRecord implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  private gridHeight: number;
  private bulkRecordData$: Observable<any[]>;
  private recordsSubscription: Subscription;
  private datasource: any[];
  private currentRecord: { ID: number, RegNumber: string, temporary: boolean } = { ID: 0, RegNumber: '', temporary: false };
  private loadIndicatorVisible: boolean = true;
  private columns = [{
    dataField: 'LOGID',
    caption: 'LogId'
  }, {
    dataField: 'TEMPID',
    caption: 'TempID',
    cellTemplate: 'tempViewTemplate',
  }, {
    dataField: 'DESCRIPTION',
    caption: 'Description'
  }, {
    dataField: 'structure',
    caption: 'STRUCTURE',
    cellTemplate: 'cellTemplate',
    width: 220,
    allowFiltering: false,
    allowSorting: false,
  }, {
    dataField: 'COMMENTS',
    caption: 'Comments'
  }, {
    dataField: 'ACTION',
    caption: 'Action'
  }, {
    dataField: 'USERID',
    caption: 'Submitted By'
  }, {
    dataField: 'REGNUMBER',
    caption: 'REGNUMBER',
    cellTemplate: 'recorrdViewTemplate',
  }];

  constructor(
    private ngRedux: NgRedux<IAppState>,
    private router: Router,
    private http: HttpService,
    private registryActions: RegistryActions,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {
    this.bulkRecordData$ = this.ngRedux.select(['registry', 'bulkRegisterRecords']);
    this.recordsSubscription = this.bulkRecordData$.subscribe((value: number[]) => this.loadData(value));
  }

  ngOnDestroy() {
    if (this.recordsSubscription) {
      this.recordsSubscription.unsubscribe();
    }
  }

  loadData(e) {
    if (e) {
      this.datasource = e;
      this.columns = this.columns.map(s => this.updateGridColumn(s));
      this.loadIndicatorVisible = false;
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

  reviewRecord(data) {
    let temporary = (data.key.REGNUMBER ? false : true);
    this.router.navigate([`records/${temporary ? 'temp' : ''}/bulkreg/${data.key.TEMPID}`]);
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

  private cancel(e) {
    this.registryActions.clearBulkRrgisterStatus();
    this.router.navigate([`records/temp`]);
  }

};
