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
  private selectedRowsKeys: any[] = [];
  private selectedFileType: string = 'SDFFlatFileUncorrelated';
  private filesType: any[] = [ { displayExpr: 'SDF Flat', valueExpr: 'SDFFlatFileUncorrelated' }, { displayExpr: 'SDF Nested', valueExpr: 'SDFNested' } ];
  private exportTemplates: any[] = [];
  private currentExportTemplate = 0;
  private columns: any[] = [
    { dataField: 'tableName', caption: 'Table', groupIndex: 0 },
    { dataField: 'tableId', visible: false },
    { dataField: 'fieldId', visible: false },
    { dataField: 'indexType', visible: false },
    { dataField: 'mimeType', visible: false },
    { dataField: 'fieldName', caption: 'Field Name' },
    { dataField: 'key', visible: false }
  ];

  constructor(
    private http: HttpService,
    private ngRedux: NgRedux<IAppState>,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {
    this.dataSource = this.createCustomStore(this);
    this.getExportTemplates(); 
  }

  ngOnDestroy() {
  
  }

  protected showForm(e) {
    this.formVisible = true;
  }

  protected getExportTemplates() {
    let url = `${apiUrlPrefix}/exportTemplates${this.temporary ? '?temp=true' : ''}`;
    this.http.get(url).toPromise().then(result => {
      result.json().forEach(t => {
        this.exportTemplates.push({ ID: t.ID, Name: t.Name });
      });
    })
    .catch(error => {
      let message = getExceptionMessage(`The export tempaltes were not retrieved properly due to a problem`, error);
    });
  }

  protected exportTemplateValueChanged(e) {
    this.currentExportTemplate = e.value;
    this.dataSource = this.createCustomStore(this);
  }

  protected export(e) {
    if (this.selectedRowsKeys.length > 0) {
      let url = `${apiUrlPrefix}hitlists/${this.hitListId}/export/${this.selectedFileType}${this.temporary ? '?temp=true' : ''}`;
      
      let data: any[] = [];
      this.selectedRowsKeys.forEach(key => {
        let field = this.rows.find(r => r.key === key);
        if (field) {
          data.push({
            tableId: field.tableId,
            fieldId: field.fieldId,
            visible: true,
            indexType: field.indexType,
            mimeType: field.mimeType,
            alias: field.fieldName });
        }
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
    this.selectedRowsKeys = e.component.getSelectedRowKeys();
  }

  protected isVisible(e) {
    return e.visible;
  }

  onContentReady(e) {
    e.component.selectRows(this.selectedRowsKeys);
  }

  fileTypeValueChanged(e) {
    this.selectedFileType = e.value;
  }

  private createCustomStore(parent: RegSearchExport): CustomStore {
    let tableName = 'hitlists/resultsCriteria';
    let params = '';
    if (this.temporary) { params += `${params ? '&' : '?'}temp=true`; }
    if (this.currentExportTemplate > 0) { params += `${params ? '&' : '?'}templateId=${this.currentExportTemplate}`; }
    let apiUrlBase = `${apiUrlPrefix}${tableName}${params}`;
    return new CustomStore({
      key: 'key',
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        parent.http.get(apiUrlBase)
          .toPromise()
          .then(result => {
            let rows = result.json();
            parent.rows = rows; 
            parent.selectedRowsKeys = [];
            parent.rows.forEach(row => {
              if (row.visible) {
                parent.selectedRowsKeys.push(row.key); 
              }
            });       
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
