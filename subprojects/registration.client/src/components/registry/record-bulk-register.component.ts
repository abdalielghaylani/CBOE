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
import { IAppState, ILookupData } from '../../redux';
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
  @Input() parentHeight: number;
  @Output() onClose = new EventEmitter<any>();
  private columns = [{
    dataField: 'LOGID',
    caption: 'LogId'
  }, {
    dataField: 'TEMPID',
    caption: 'ID',
    cellTemplate: 'recorrdViewTemplate',
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
    caption: 'REGNUMBER'
  }];

  private editRowIndex: number = -1;
  constructor(
    private ngRedux: NgRedux<IAppState>,
    private router: Router,
    private http: HttpService,
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
      this.gridHeight = this.parentHeight - 80;
      this.datasource = e;
      this.columns = this.columns.map(s => this.updateGridColumn(s));
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
    this.router.navigate([`records${data.key.regNumber ? '' : '/temp'}/${data.value}`]);
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

  private cancel(e) {
    this.onClose.emit(e);
  }


};
