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
import { getExceptionMessage, notify, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { ICustomTableData, IConfiguration, IAppState } from '../../redux';
import { HttpService } from '../../services';

declare var jQuery: any;

@Component({
  selector: 'reg-duplicate-record',
  template: require('./registry-duplicate.component.html'),
  styles: [require('./records.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDuplicateRecord implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  private gridHeight: string;
  private duplicateData$: Observable<any[]>;
  private recordsSubscription: Subscription;
  private datasource: any[];
  @Output() onClose = new EventEmitter<any>();
  private columns = [{
    dataField: 'REGNUMBER',
    caption: 'Reg Number'
  }, {
    dataField: 'NAME',
    caption: 'Name'
  }, {
    dataField: 'CREATED',
    dataType: 'date',
    caption: 'Created',
  }, {
    dataField: 'MODIFIED',
    dataType: 'date',
    caption: 'Modified',
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
    caption: 'Status'
  }, {
    dataField: 'APPROVED',
    caption: 'Approved'
  }];

  constructor(
    private ngRedux: NgRedux<IAppState>,
    private router: Router,
    private http: HttpService,
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

  loadData(e) {
    if (e) {
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

  onToolbarPreparing(e) {
    e.toolbarOptions.items.unshift({
      location: 'before',
      template: 'toolbarContents'
    });
  }
  private getGridHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  private onResize(event: any) {
    this.gridHeight = this.getGridHeight();
    this.grid.height = this.getGridHeight();
    this.grid.instance.repaint();
  }

  private onDocumentClick(event: any) {
    if (event.srcElement.title === 'Full Screen') {
      let fullScreenMode = event.srcElement.className === 'fa fa-compress fa-stack-1x white';
      this.gridHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
      this.grid.height = this.gridHeight;
      this.grid.instance.repaint();
    }
  }

  cancelDuplicateResolution(e) {
    this.onClose.emit(e);
  }

};