import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { select, NgRedux } from '@angular-redux/store';
import { Router, ActivatedRoute } from '@angular/router';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { apiUrlPrefix } from '../../../../configuration';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { HttpService } from '../../../../services';
import { getExceptionMessage, notify, notifyError, notifyException, notifySuccess } from '../../../../common';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-search-export',
  template: require('./search-export.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchExport implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid;
  @Input() temporary: boolean;
  @Input() hitListId: number;
  private rows: any[] = [];
  private dataSource: CustomStore;
  private formVisible: boolean = false;
  private selectedRows: any[] = [];
  private selectedFileType: string = 'SDFNested';
  private filesType: any[] = [ { displayExpr: 'SDF Nested', valueExpr: 'SDFNested' }, { displayExpr: 'SDF Flat', valueExpr: 'SDFFlatFileUncorrelated' } ];
  private columns: any[] = [
    { dataField: 'tableName', caption: 'Table', groupIndex: 0 },
    { dataField: 'tableId', visible: false },
    { dataField: 'fieldId', visible: false },
    { dataField: 'indexType', visible: false },
    { dataField: 'mimeType', visible: false },
    { dataField: 'fieldName', caption: 'Field Name' }
  ];

  constructor(
    private http: HttpService,
    private ngRedux: NgRedux<IAppState>,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {
    this.dataSource = this.createCustomStore(this);
  }

  ngOnDestroy() {
  
  }

  protected showForm(e) {
    this.formVisible = true;
  }

  protected export(e) {
    if (this.selectedRows.length > 0) {
      let url = `${apiUrlPrefix}hitlists/${this.hitListId}/export/${this.selectedFileType}${this.temporary ? '?temp=true' : ''}`;
      let data = this.selectedRows.map(r => {
        return {
          tableId: r.tableId,
          fieldId: r.fieldId,
          visible: true,
          indexType: r.indexType,
          mimeType: r.mimeType,
          alias: r.fieldName
        };
      });
      this.http.post(url, data).toPromise().then(res => {
        let filename = res.headers.get('x-filename');
        let contentType = res.headers.get('content-type');
        let linkElement = document.createElement('a');
        try {
          let blob = new Blob([res.arrayBuffer()], { type: contentType });
          let urlFile = window.URL.createObjectURL(blob);
          linkElement.setAttribute('href', urlFile);
          linkElement.setAttribute('download', filename);
          let clickEvent = new MouseEvent('click', {
            'view': window,
            'bubbles': true,
            'cancelable': false
          });
          linkElement.dispatchEvent(clickEvent);
        } catch (ex) {
          
        }
        notifySuccess(`The file was exported correctly`, 5000);
      })
      .catch(error => {
        notifyException(`The submission data was not posted properly due to a problem`, error, 5000);
      });
      this.formVisible = false;
    } else {
      notify(`At least one field is required!`, 'warning', 5000);
    }
  }

  protected cancel(e) {
    this.formVisible = false;    
  }

  onSelectionChanged(e) {
    this.selectedRows = e.selectedRowKeys;
  }
  protected isVisible(e) {
    return e.visible;
  }

  onContentReady(e) {
    // e.component.selectRows(this.rows.filter(this.isVisible).map(r => r.fieldId));
  }

  fileTypeValueChanged(e) {
    this.selectedFileType = e.value;
  }

  private createCustomStore(parent: RegSearchExport): CustomStore {
    let tableName = 'hitlists/resultsCriteria';
    let apiUrlBase = `${apiUrlPrefix}${tableName}${this.temporary ? '?temp=true' : ''}`;
    return new CustomStore({
      // key: 'fieldId',
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        parent.http.get(apiUrlBase)
          .toPromise()
          .then(result => {
            let rows = result.json();
            deferred.resolve(rows, { totalCount: rows.length });
          })
          .catch(error => {
            let message = getExceptionMessage(`The records of ${tableName} were not retrieved properly due to a problem`, error);
            deferred.reject(message);
          });
        return deferred.promise();
      }
    });
  }
};
