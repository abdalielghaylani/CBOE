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
import { IShareableObject, CShareableObject, IFormGroup, prepareFormGroupData, getExceptionMessage, 
  notify, notifyError, notifyException, notifySuccess } from '../../../../common';
import { IAppState } from '../../../../redux';
import DxForm from 'devextreme/ui/form';
import { RequestOptionsArgs, RequestOptions, ResponseContentType } from '@angular/http';

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
  private isAllowUpdatingEnabled: boolean = false;
  private isAddTemplateEnable: boolean = false;
  private isAddTemplateButtonEnabled: boolean = true;
  private isEditDeleteTemplateButtonEnabled: boolean = false;
  private columns: any[] = [
    { dataField: 'tableName', caption: 'Table', groupIndex: 0 },
    { dataField: 'tableId', visible: false },
    { dataField: 'fieldId', visible: false },
    { dataField: 'indexType', visible: false },
    { dataField: 'mimeType', visible: false },
    { dataField: 'fieldName', caption: 'Field Name' },
    { dataField: 'key', visible: false }
  ];
  private updatedFields: any[] = [];
  private saveTemplateItems = [{
    dataField: 'name',
    label: { text: 'Template Name' },
    dataType: 'string',
    editorType: 'dxTextBox',
    editorOptions: { width: '200px' },
    validationRules: [{ type: 'required', message: 'Name is required' }]
  }, {
    dataField: 'description',
    label: { text: 'Template Description' },
    dataType: 'string',
    editorType: 'dxTextArea',
    editorOptions: { width: '200px' }
  }, {
    dataField: 'isPublic',
    label: { text: 'Is Public' },
    dataType: 'boolean',
    editorType: 'dxCheckBox'
  }];
  private saveTemplateData: IShareableObject = new CShareableObject('', '', false);
  private saveTemplateForm: DxForm;

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
    this.isAllowUpdatingEnabled = true;  
  }

  protected showForm(e) {
    this.formVisible = true;
  }

  private resetTemplateButtons() {
    this.isAddTemplateEnable = false;
    this.isAddTemplateButtonEnabled = true;
    this.isEditDeleteTemplateButtonEnabled = false;
    this.changeDetector.markForCheck();
  }

  private onSaveTemplateFormInit(e) {
    this.saveTemplateForm = e.component as DxForm;
  }

  private cancelSaveTemplate(e) {
    this.resetTemplateButtons();
    if (this.currentExportTemplate > 0) {      
      this.isEditDeleteTemplateButtonEnabled = true;
      this.changeDetector.markForCheck();
    }
  }

  toolbarPreparing(data: any) {
    let indexSaveButton = data.toolbarOptions.items.indexOf(data.toolbarOptions.items.find(function (item) {
      return item.name === 'saveButton';
    }));
    if (indexSaveButton !== -1) {
      data.toolbarOptions.items.splice(indexSaveButton, 1);
    }
    let indexRevertButton = data.toolbarOptions.items.indexOf(data.toolbarOptions.items.find(function (item) {
      return item.name === 'revertButton';
    }));
    if (indexRevertButton !== -1) {
      data.toolbarOptions.items.splice(indexRevertButton, 1);
    }
  }

  protected exportTemplateValueChanged(e) {
    if (this.currentExportTemplate !== e.value) {
      this.resetTemplateButtons();
      if (e.value > 0) {      
        this.isEditDeleteTemplateButtonEnabled = true;
        this.changeDetector.markForCheck();
      }
    }
    this.currentExportTemplate = e.value;
    this.dataSource = this.createCustomStore(this);
  }

  protected cancel(e) {
    this.isAllowUpdatingEnabled = false;
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

  private enableTemplateForm() {
    this.saveTemplateData.name = '';
    this.saveTemplateData.description = '';
    this.saveTemplateData.isPublic = false;
    this.isAllowUpdatingEnabled = true;
    this.isAddTemplateEnable = true;
    this.isAddTemplateButtonEnabled = false;
    this.isEditDeleteTemplateButtonEnabled = false;
    this.changeDetector.markForCheck();
  }

  protected addTemplate() {
    this.enableTemplateForm();
    if (this.currentExportTemplate > 0) {
      this.currentExportTemplate = 0;
    }
  }

  protected editTemplate() {
    let template = this.exportTemplates.find(r => r.ID === this.currentExportTemplate);
    if (template) {
      this.saveTemplateData.name = template.Name;
      this.saveTemplateData.description = template.Description;
      this.saveTemplateData.isPublic = template.IsPublic;
      this.isAllowUpdatingEnabled = true;
      this.isAddTemplateEnable = true;
      this.isAddTemplateButtonEnabled = false;
      this.isEditDeleteTemplateButtonEnabled = false;
      this.changeDetector.markForCheck();
    }
  }

  protected deleteTemplate() {
    let url = `${apiUrlPrefix}exportTemplates/${this.currentExportTemplate}`;
    this.http.delete(url)
      .toPromise()
      .then((result => {
        this.currentExportTemplate = 0;
        this.exportTemplates = [];
        this.changeDetector.markForCheck();
        this.getExportTemplates(); 
        this.createCustomStore(this);
        notifySuccess(`The template was deleted successfully!`, 5000);
      }).bind(this))
      .catch(error => {
        let message = getExceptionMessage(`The template ${this.currentExportTemplate} was not deleted due to a problem`, error);
      });
  }

  protected getExportTemplates() {
    let url = `${apiUrlPrefix}/exportTemplates${this.temporary ? '?temp=true' : ''}`;
    this.http.get(url).toPromise().then(result => {
      result.json().forEach(t => {
        this.exportTemplates.push({ ID: t.ID, Name: t.Name, Description: t.Description, IsPublic: t.IsPublic });
        this.changeDetector.markForCheck();
      });
    })
    .catch(error => {
      let message = getExceptionMessage(`The export tempaltes were not retrieved properly due to a problem`, error);
    });
  }

  private saveTemplate(e) {
    this.grid.instance.saveEditData();
    let resultsCriteria: any[] = [];
    this.rows.forEach(row => {
      let updatedRow = this.updatedFields.find(r => r.key === `${row.fieldId}${row.fieldName.toLowerCase()}`);
      resultsCriteria.push({
        tableId: row.tableId,
        fieldId: row.fieldId,
        visible: this.selectedRowsKeys.find(s => s === `${row.fieldId}${row.fieldName.toLowerCase()}`) !== undefined,
        indexType: row.indexType,
        mimeType: row.mimeType,
        alias: (updatedRow) ? updatedRow.name.fieldName : row.fieldName });
    });

    let data = {
      templateName: this.saveTemplateData.name,
      templateDescription: this.saveTemplateData.description,
      isPublic: this.saveTemplateData.isPublic,
      resultsCriteriaTableData: resultsCriteria
    };

    let tableName = 'hitlists/saveExportTemplates';
    let params = `?id=${this.currentExportTemplate}`;
    if (this.temporary) { params += `&temp=true`; }
    let apiUrlBase = `${apiUrlPrefix}${tableName}${params}`;
    this.http.post(apiUrlBase, data)
    .toPromise()
    .then(result => {
      this.currentExportTemplate = result.json().id;
      this.exportTemplates = [];
      this.isAllowUpdatingEnabled = false;
      this.changeDetector.markForCheck();
      this.resetTemplateButtons();
      if (this.currentExportTemplate > 0) {      
        this.isEditDeleteTemplateButtonEnabled = true;
        this.changeDetector.markForCheck();
      }
      this.getExportTemplates(); 
      this.changeDetector.markForCheck();
      this.createCustomStore(this);
      notifySuccess(`The template was saved successfully!`, 5000);
    })
    .catch(error => {
      let message = getExceptionMessage(`The template ${this.currentExportTemplate} was not saved due to a problem`, error);
    });
  }
  
  protected export(e) {
    this.isAllowUpdatingEnabled = false;
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

      let options = new RequestOptions({ responseType: ResponseContentType.ArrayBuffer });
      this.http.post(url, data, options).toPromise().then(res => {
        let filename = res.headers.get('x-filename');
        let contentType = res.headers.get('content-type');
        let linkElement = document.createElement('a');
        try {
          let type = `${contentType}; charset=UTF-8`;
          let blob = new Blob([res.arrayBuffer()], { type: type });         
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

  private createCustomStore(parent: RegSearchExport): CustomStore {
    return new CustomStore({
      key: 'key',
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        let tableName = 'hitlists/resultsCriteria';
        let params = '';
        if (parent.temporary) { params += `${params ? '&' : '?'}temp=true`; }
        if (parent.currentExportTemplate > 0) { params += `${params ? '&' : '?'}templateId=${parent.currentExportTemplate}`; }
        let apiUrlBase = `${apiUrlPrefix}${tableName}${params}`;
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
        },
      update: function (key, values) {
        parent.updatedFields.push({ key: key, name: values });
        parent.isAllowUpdatingEnabled = false;
        return null;
      }
    });
  }
};
