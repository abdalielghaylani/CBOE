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
import {
  IShareableObject, CShareableObject, IFormGroup, prepareFormGroupData, getExceptionMessage,
  notify, notifyError, notifyException, notifySuccess
} from '../../../../common';
import { IAppState } from '../../../../redux';
import DxForm from 'devextreme/ui/form';
import { RequestOptionsArgs, RequestOptions, ResponseContentType } from '@angular/http';
import * as FileSaver from 'file-saver';
import * as dxDialog from 'devextreme/ui/dialog';

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
  @Input() recordsCount: number;
  private rows: any[] = [];
  private dataSource: CustomStore;
  private formVisible: boolean = false;
  private selectedFileType: string = 'SDFFlatFileUncorrelated';
  private filesType: any[] = [{ displayExpr: 'SDF Flat', valueExpr: 'SDFFlatFileUncorrelated' }, { displayExpr: 'SDF Nested', valueExpr: 'SDFNested' }];
  private exportTemplates: any[] = [];
  private currentExportTemplate = 0;
  private isAllowUpdatingEnabled: boolean = false;
  private isAddTemplateEnable: boolean = false;
  private isAddTemplateButtonEnabled: boolean = true;
  private isEditDeleteTemplateButtonEnabled: boolean = false;
  private columns: any[] = [
    { dataField: 'tableName', caption: 'Table', groupIndex: 0, groupCellTemplate: 'tableNameGroupCellTemplate' },
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
  private loadIndicatorVisible: boolean = false;
  private customSelectionFlag: boolean = false;
  private groupSelected: any[] = [];
  private defaultSelectedRowsKeys: any[] = [];

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
    this.isAllowUpdatingEnabled = false;
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

  protected isSelected(groupName) {
    let group = this.groupSelected.find(g => g.id === groupName[0]);
    if (group) {
      return group.selected;
    }
    return false;
  }

  protected setGroupedSelected() {
    this.rows.forEach(r => {
      if (this.groupSelected.length === 0) {
        this.groupSelected.push({ id: r.tableName, selected: r.visible });
      } else {
        let group = this.groupSelected.find(g => g.id === r.tableName);
        if (group) {
          if (!r.visible) {
            group.selected = false;
          }
        } else {
          this.groupSelected.push({ id: r.tableName, selected: r.visible });
        }
      }
    });
  }

  onGroupSelectionChanged(e) {
    let tableName = e.element[0].getAttribute('id');
    let group = this.grid.instance.getDataSource().items().find(r => r.key === tableName);
    let keys = [];
    if (group.items) {
      keys = group.items.map(r => r.key);
    } else {
      keys = group.collapsedItems.map(r => r.key);
    }
    if (e.value) {
      this.grid.instance.getSelectedRowKeys().map(r => {
        let isIn = keys.indexOf(r);
        if (isIn < 0) { 
          keys.push(r); 
        } 
      });
      this.grid.instance.selectRows(keys);
      let groupSelected = this.groupSelected.find(g => g.id === tableName);
      if (groupSelected) {
        groupSelected.selected = true;
      }
    } else {
      this.grid.instance.deselectRows(keys);
      let groupSelected = this.groupSelected.find(g => g.id === tableName);
      if (groupSelected) {
        groupSelected.selected = false;
      }
    }
  }

  protected isVisible(e) {
    return e.visible;
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
    let keys = this.grid.instance.getSelectedRowKeys();
    if (keys.length > 0) {
      this.grid.instance.deselectRows(keys);
    }
    
    if (this.defaultSelectedRowsKeys.length > 0) {
      this.grid.instance.selectRows(this.defaultSelectedRowsKeys);
    }
    this.isAddTemplateEnable = true;
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
    let dialogResult = dxDialog.confirm(
      `Are you sure you want to continue?`,
      'Confirm Delete template');
    dialogResult.done(r => {
      if (r) {
        let url = `${apiUrlPrefix}exportTemplates/${this.currentExportTemplate}`;
        this.http.delete(url)
          .toPromise()
          .then((result => {
            this.currentExportTemplate = 0;
            this.exportTemplates = [];
            this.changeDetector.markForCheck();
            this.resetTemplateButtons();
            this.getExportTemplates();
            this.createCustomStore(this);
            notifySuccess(`The template was deleted successfully!`, 5000);
          }).bind(this))
          .catch(error => {
            let message = getExceptionMessage(`The template ${this.currentExportTemplate} was not deleted due to a problem`, error);
          });
      }
    }
    );
  }

  protected getExportTemplates() {
    let url = `${apiUrlPrefix}/exportTemplates${this.temporary ? '?temp=true' : ''}`;
    this.http.get(url).toPromise().then(result => {
      result.json().forEach(t => {
        this.exportTemplates.push({ ID: t.ID, Name: t.Name, Description: t.Description, IsPublic: t.IsPublic });
        this.changeDetector.markForCheck();
      });
    }).catch(error => {
      let message = getExceptionMessage(`The export templates were not retrieved properly due to a problem`, error);
    });
  }

  private saveTemplate(e) {
    if (this.grid.instance.getSelectedRowKeys().length === 0) {
      notify('Select at least one table criteria before saving the template', 'warning', 5000);
      return;
    }
    let validationResult: any = this.saveTemplateForm.validate();
    if (validationResult.isValid) {
      this.updatedFields = [];
      this.grid.instance.saveEditData();
      let resultsCriteria: any[] = [];
      this.rows.forEach(row => {
        let updatedRow = this.updatedFields.find(r => r.key === `${row.fieldId}${row.fieldName.toLowerCase()}`);
        resultsCriteria.push({
          tableId: row.tableId,
          fieldId: row.fieldId,
          visible: this.grid.instance.getSelectedRowKeys().find(s => s === `${row.fieldId}${row.fieldName.toLowerCase()}`) !== undefined,
          indexType: row.indexType,
          mimeType: row.mimeType,
          alias: (updatedRow) ? updatedRow.name.fieldName : row.fieldName
        });
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
          this.resetTemplateButtons();
          if (this.currentExportTemplate > 0) {
            this.isEditDeleteTemplateButtonEnabled = true;
          }
          this.getExportTemplates();
          this.changeDetector.markForCheck();
          this.createCustomStore(this);
          this.grid.instance.refresh();
          this.isAddTemplateEnable = false;
          notifySuccess(`The template was saved successfully!`, 5000);
        })
        .catch(error => {
          let message = getExceptionMessage(`The template ${this.currentExportTemplate} was not saved due to a problem`, error);
        });
    }
  }

  protected export(e) {
    if (this.isAllowUpdatingEnabled) {
      notify(`Save or update the template before export!`, 'warning', 5000);
    } else {
      if (this.grid.instance.getSelectedRowKeys().length > 0) {
        let dialogResult = dxDialog.confirm(
          `Save ${this.recordsCount} records as sdf`,
          'Exporting...');
        dialogResult.done(result => {
          if (result) {
            this.loadIndicatorVisible = true;
            let url = `${apiUrlPrefix}hitlists/${this.hitListId}/export/${this.selectedFileType}${this.temporary ? '?temp=true' : ''}`;
            let data = {
              resultsCriteriaTables: []
            };
            this.grid.instance.getSelectedRowKeys().forEach(key => {
              let field = this.rows.find(r => r.key === key);
              if (field) {
                data.resultsCriteriaTables.push({
                  tableId: field.tableId,
                  fieldId: field.fieldId,
                  visible: true,
                  indexType: field.indexType,
                  mimeType: field.mimeType,
                  alias: field.fieldName
                });
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
                FileSaver.saveAs(blob, filename);
              } catch (ex) {

              }
              this.clearLoadIndicator();
              notifySuccess(`The file was exported correctly`, 5000);
            })
              .catch(error => {
                this.clearLoadIndicator();
                notifyException(`The submission data was not posted properly due to a problem`, error, 5000);
              });
          }
          this.formVisible = false;
          this.changeDetector.markForCheck();
        });
      } else {
        notify(`At least one field is required!`, 'warning', 5000);
      }
    }
  }

  private createCustomStore(parent: RegSearchExport): CustomStore {
    return new CustomStore({
      key: 'key',
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        let tableName = 'hitlists/resultsCriteria';
        let params = parent.temporary ? `?temp=true` : ``;
        if (parent.currentExportTemplate > 0) { params += `${params ? '&' : '?'}templateId=${parent.currentExportTemplate}`; }
        let apiUrlBase = `${apiUrlPrefix}${tableName}${params}`;
        parent.http.get(apiUrlBase)
          .toPromise()
          .then(result => {
            let rows = result.json();
            parent.rows = rows;
            let selectedRowsKeys: any[] = [];
            parent.rows.forEach(row => {
              if (row.visible) {
                selectedRowsKeys.push(row.key);
              }
            });
            if (selectedRowsKeys.length > 0) {
              parent.grid.instance.selectRows(selectedRowsKeys);
            }
            if (selectedRowsKeys.length > 0 && parent.currentExportTemplate === 0) {
              parent.defaultSelectedRowsKeys = selectedRowsKeys.slice();
            }
            parent.setGroupedSelected();
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

  clearLoadIndicator() {
    this.loadIndicatorVisible = false;
    this.changeDetector.markForCheck();
  }
};
