import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef, NgZone
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
import { CStructureImagePrintService } from '../common/structure-image-print.service';

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
  private bulkDataIsLoaded: boolean = false;
  private defaultPrintStructureImage = require('../common/assets/no-structure.png');
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
    private elementRef: ElementRef,
    private ngZone: NgZone,
    private imageService: CStructureImagePrintService,
  ) { }

  ngOnInit() {
    this.bulkRecordData$ = this.ngRedux.select(['registry', 'bulkRegisterRecords']);
    this.recordsSubscription = this.bulkRecordData$.subscribe((value: number[]) => this.loadData(value));
    
    window.NewRegWindowHandle = window.NewRegWindowHandle || {};
    window.NewRegWindowHandle.closePrintPage = this.closePrintPage.bind(this);
  }

  ngOnDestroy() {
    if (this.recordsSubscription) {
      this.recordsSubscription.unsubscribe();
    }

    window.NewRegWindowHandle.closePrintPage = null;
  }

  setProgressBarVisibility(e) {
    this.loadIndicatorVisible = e;
    this.changeDetector.markForCheck();
  }

  closePrintPage() {
    this.ngZone.run(() => this.setProgressBarVisibility(false));
  }

  loadData(e) {
    if (e) {
      this.datasource = e;
      this.columns = this.columns.map(s => this.updateGridColumn(s));
      this.loadIndicatorVisible = false;
      this.bulkDataIsLoaded = true;
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

  private printRecords() {
    this.loadIndicatorVisible = true;
    let rows = this.datasource;
    this.imageService.generateMultipleImages(rows.map(r => r.structure)).subscribe(
      (values: Array<string>) => {
        let printContents: string;
        printContents = '<table width="100%" height="auto"><tr>';
        this.columns.forEach(c => {
          printContents += `<td>${c.caption}</td>`;
        });
        printContents += '</tr>';
        rows.forEach(row => {
          printContents += '<tr>';
          this.columns.forEach(c => {
            let field = row[c.dataField];
            if (c.caption === 'STRUCTURE') {
              let structureImage = this.imageService.getImage(field);
              printContents += `<td><img src="${structureImage ? structureImage : this.defaultPrintStructureImage}" /></td>`;
            } else {
              printContents += `<td>${(field) ? field : ''}</td>`;
            }
          });
          printContents += '</tr>';
        });

        let popupWin;
        popupWin = window.open('', '_blank', 'top=0,left=0,height=500,width=auto');
        popupWin.document.open();
        popupWin.document.write(`<html>
          <head>
            <title>Print table</title>
            <style>
              table, tr, td {
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
            </style>
          </head>
          <body onload="window.print(); window.close()">${printContents}</body>
        </html>`);
        popupWin.onbeforeunload = function() { 
          this.opener.NewRegWindowHandle.closePrintPage();
        };
        popupWin.document.close();
      });
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
