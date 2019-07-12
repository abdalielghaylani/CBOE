import { EventEmitter } from '@angular/core';
import * as X2JS from 'x2js';
import { ISearchCriteriaItem } from './registry-base.types';
import { IFormGroup, IForm, ICoeForm, ICoeFormMode, IFormElement } from '../../../common';
import {
  CBatch, CBatchList, CComponent, CComponentList,
  IAddInList, IBatchList, IComponentList, IIdentifierList, IProjectList, IPropertyList, IValidationRuleList,
  IRegNumber, IStructureData
} from '../../common';
import { CSystemSettings } from '../../../redux';

export enum DrawingType {
  Chemical = 0,
  Unknown = 1,
  NoStructure = 2,
  NonChemicalContent = 3
}

export interface IViewGroup {
  id: string;
  data: ICoeForm[];
}

export interface IViewGroupContainer {
  id: string;
  title: string;
  viewGroups: CViewGroup[];
  subArray: any[];
  subIndex: number;
}

export class CEntryInfo {
  dataSource: string;
  bindingExpression: string;
  searchCriteriaItem?: ISearchCriteriaItem;
}

export class CBoundObject {
  obj: any;
  property?: string;
}

export class CViewGroupColumns {
  baseTableColumns: any[] = [];
  batchTableColumns: any[] = [];
}

export class CViewGroup implements IViewGroup {
  public id: string;
  public title: string;
  constructor(public data: ICoeForm[], private disabledControls: any[]) {
    this.update();
  }

  private update() {
    if (this.data.length === 1) {
      this.title = this.data[0].title;
      if (this.title) {
        this.id = this.title.toLowerCase().replace(/\s/g, '_');
      }
    }
  }

  private canAppend(f: ICoeForm): boolean {
    return this.data.length === 0 || !f.title || this.title === f.title;
  }

  private getFormElementContainer(f: ICoeForm, mode: string): ICoeFormMode {
    return mode === 'add' ? f.addMode : mode === 'edit' ? f.editMode : mode === 'view' ? f.viewMode : f.layoutInfo;
  }

  private setItemValue(item: any, property: string, value: any) {
    if (value) {
      item[property] = value;
    }
  }

  private getEditorType(fe: IFormElement): string {
    return fe.displayInfo.type.indexOf('COEDatePicker') > 0 ? 'dxDateBox' : 'dxTextBox';
  }

  private getDataField(fe: IFormElement): string {
    return fe.Id ? fe.Id : fe._name.replace(/\s/g, '');
  }

  private checkEditorType(fe: IFormElement, item: any) {
    let type = fe.displayInfo.type;
    let readOnly = type.endsWith('ReadOnly');
    if (type.endsWith('COEDropDownList')) {
      let pickListDomain = fe.configInfo ? fe.configInfo.fieldConfig.PickListDomain : undefined;
      if (pickListDomain) {
        if (pickListDomain.__text) {
          pickListDomain = pickListDomain.__text;
        }
        if (String(Math.floor(Number(pickListDomain))) === pickListDomain) {
          item.template = 'dropDownTemplate';
          item.editorOptions = {
            pickListDomain: +pickListDomain
          };
        }
      }
      if (!item.template && fe.configInfo && fe.configInfo.fieldConfig.dropDownItemsSelect) {
        item.template = 'dropDownTemplate';
        item.editorOptions = {
          dropDownItemsSelect: fe.configInfo.fieldConfig.dropDownItemsSelect
        };
        if (fe.searchCriteriaItem && fe.searchCriteriaItem._fieldid) {
          item.editorOptions.fieldid = fe.searchCriteriaItem._fieldid + '_' + (fe.searchCriteriaItem._id ? fe.searchCriteriaItem._id : '0');
        }
      }
    } else if (type.indexOf('COEStateControl') > 0) {
      item.template = 'dropDownTemplate';
      item.editorOptions = {
        dataSource: (fe.configInfo.fieldConfig as any).States.State,
        valueExpr: '_value',
        displayExpr: '_text'
      };
    } else if (type.endsWith('COEWebGridUltra') && fe.bindingExpression.indexOf('IdentifierList') >= 0) {
      item.template = 'idListTemplate';
      item.editorOptions = {
        idListType: fe.Id.startsWith('S') ? 'S' : fe.Id.startsWith('C') ? 'C' : fe.Id.startsWith('B') ? 'B' : 'R',
        dataField: item.dataField
      };
      if (fe.configInfo && fe.configInfo.fieldConfig) {
        item.editorOptions.fieldConfig = fe.configInfo.fieldConfig;
      }
      item.colSpan = 2;
    } else if (type.endsWith('COEStructureQuery')) {
      item.template = 'structureQueryTemplate';
      item.colSpan = 5;
    } else if (type.indexOf('COEChemDraw') > 0) {
      item.template = readOnly ? 'structureImageTemplate' : 'structureTemplate';
      if (!readOnly) {
        item.colSpan = 5;
      }
    } else if (type.indexOf('COELink') > 0) {
      item.template = 'urlTemplate';
      item.editorOptions = {
        label: fe.label,
        config: fe
      };
      if (!!fe.clientEvents) {
        item.label = { visible: false };
      }
    } else if (type.indexOf('COEDatePicker') > 0) {
      item.template = 'dateTemplate';
    } else if (fe.bindingExpression.endsWith('ProjectList')) {
      item.template = 'projectsTemplate';
      item.editorOptions = {
        projectType: fe.Id.startsWith('B') ? 'B' : 'R'
      };
    } else if (fe.displayInfo.type.endsWith('COEFragments')) {
      item.template = 'fragmentsTemplate';
      item.colSpan = 5;
    } else if (fe.displayInfo.type.endsWith('COECheckBox') ||
      fe.displayInfo.type.endsWith('COECheckBoxReadOnly')) {
      item.template = 'checkBoxTemplate';
    } else if (fe.displayInfo.type.endsWith('COEListUpload') ||
      fe.displayInfo.type.endsWith('COEListUploadReadOnly')) {
      item.template = 'fileUploaderTemplate';
    }
    if (!item.template) {
      item.template = 'textTemplate';
    }
    if (!item.editorOptions) {
      item.editorOptions = {};
    }
    item.editorOptions.readOnly = readOnly;
    item.editorOptions.configInfo = fe.configInfo;
  }

  private setDefaultValue(fe: IFormElement, item: any, displayMode?: string) {
    let type = fe.displayInfo.type;
    if (type.endsWith('COEDropDownList')) {
      if (fe.defaultValue) {
        item.editorOptions.defaultValue = fe.defaultValue;
        item.editorOptions.displayMode = displayMode;
      }
    } else if (type.endsWith('COEDatePicker')) {
      let defaultDate = fe.configInfo ? fe.configInfo.fieldConfig.DefaultDate : undefined;
      if (defaultDate) {
        item.editorOptions.defaultValue = defaultDate;
      } else if (fe.defaultValue) {
        item.editorOptions.defaultValue = fe.defaultValue;
      }
    } else if (type.endsWith('COETextArea')) {
      if (fe.defaultValue) {
        item.editorOptions.defaultValue = fe.defaultValue;
      }
    } else if (type.endsWith('COETextBox')) {
      if (fe.defaultValue) {
        item.editorOptions.defaultValue = fe.defaultValue;
      }
    } else if (type.endsWith('COENumericTextBox')) {
      if (fe.defaultValue) {
        item.editorOptions.defaultValue = fe.defaultValue;
      }
    }
  }

  private checkValidationRules(fe: IFormElement, item: any) {
    let ruleList: IValidationRuleList = fe.validationRuleList;
    if (ruleList && ruleList.validationRule) {
      if (!item.editorOptions) {
        item.editorOptions = {};
      }
      let required = false;
      ruleList.validationRule.forEach(r => {
        if (r._validationRuleName === 'requiredField') {
          required = true;
        }
      });
      if (required) {
        item.validationRules = [{
          type: 'required'
        }];
      }
      item.editorOptions.customRules = ruleList;
    }
  }

  private isIdText(fe: IFormElement): boolean {
    let bindingExpression = fe.bindingExpression.toLowerCase();
    return !fe.label && (bindingExpression === 'id' || bindingExpression.endsWith('regnum')) && fe.displayInfo.type.endsWith('COETextBoxReadOnly');
  }

  private isVisible(fe: IFormElement, displayMode: string, formId: string): boolean {
    if (displayMode === 'query') {
      return !this.disabledControls.find(dc => dc.id && dc.id === fe.Id && dc.formId === Number(formId))
        && fe.displayInfo && fe.displayInfo.visible === 'true' && fe._name && fe.configInfo
        && !fe.displayInfo.type.endsWith('COELabel')
        && !fe.displayInfo.type.endsWith('RegStatusControl')
        && !fe.displayInfo.type.endsWith('COEChemDrawToolbar')
        && !this.isIdText(fe);
    } else {
      return !this.disabledControls.find(dc => dc.id && dc.id === fe.Id)
        && fe.displayInfo && fe.displayInfo.visible === 'true' && fe._name && fe.configInfo
        && !fe.displayInfo.type.endsWith('COELabel')
        && !fe.displayInfo.type.endsWith('RegStatusControl')
        && !fe.displayInfo.type.endsWith('COEChemDrawToolbar')
        && !this.isIdText(fe);
    }
  }

  private fixColSpans(items: any[]): any[] {
    let fixed: any[] = [];
    let colSpans = 0;
    let maxSpans = 5;
    items.forEach(i => {
      let remaining = maxSpans - (colSpans % maxSpans);
      let itemSpan = i.colSpan ? i.colSpan : 1;
      if (itemSpan > remaining) {
        fixed.push({ itemType: 'empty', colSpan: remaining });
        colSpans += remaining;
      }
      colSpans += itemSpan;
      fixed.push(i);
    });
    return fixed;
  }

  private static getForm(config: IFormGroup, displayMode: string): IForm {
    let form: IForm = undefined;
    if (config) {
      if (displayMode === 'query') {
        if (config.queryForms && config.queryForms.queryForm && config.queryForms.queryForm.length > 0) {
          form = config.queryForms.queryForm[0];
        }
      } else if (displayMode === 'list') {
        if (config.listForms && config.listForms.listForm && config.listForms.listForm.length > 0) {
          form = config.listForms.listForm[0];
        }
      } else if (config.detailsForms && config.detailsForms.detailsForm && config.detailsForms.detailsForm.length > 0) {
        form = config.detailsForms.detailsForm[0];
      }
    }
    return form;
  }

  private static getFilteredDisabledControls(temporary: boolean, displayMode: string, disabledControls: any[]): any[] {
    let pageId;
    switch (displayMode) {
      case 'add':
        pageId = 'SUBMITMIXTURE';
        break;
      case 'view':
      case 'edit':
        pageId = temporary ? 'REVIEWREGISTERMIXTURE' : 'VIEWMIXTURE';
        break;
      case 'query':
      case 'list':
        pageId = 'CHEMBIOVIZSEARCH';
        break;
    }

    return pageId ? disabledControls.filter(dc => dc.pageId === pageId) : [];
  }

  private static sortAndFilterForms(forms: ICoeForm[]): ICoeForm[] {
    // The form array sometimes is not sorted property.
    // Registry should go first.
    // For now, doc-manager and inventory integration forms are removed.
    let sorted: ICoeForm[] = [];
    forms.forEach(f => {
      let dataSource = f._dataSourceId ? f._dataSourceId.toLowerCase() : '';
      if (dataSource.startsWith('mixture')) {
        sorted.push(f);
      }
    });
    let fragmentsFound = false;
    forms.forEach(f => {
      let dataSource = f._dataSourceId ? f._dataSourceId.toLowerCase() : '';
      if (!dataSource.startsWith('mixture') && !dataSource.startsWith('docmgr') && !dataSource.startsWith('inv')) {
        if (dataSource.startsWith('fragments')) {
          if (fragmentsFound) {
            return;
          }
          fragmentsFound = true;
        }
        sorted.push(f);
      }
    });
    return sorted;
  }

  public static viewGroupsChanged(source: CViewGroup[], target: CViewGroup[]): boolean {
    let changed = source.length !== target.length;
    if (!changed) {
      let index = 0;
      source.forEach(vg => {
        if (vg.id !== target[index].id) {
          changed = true;
        }
        ++index;
      });
    }
    return changed;
  }

  public static getViewGroups(temporary: boolean, config: IFormGroup, displayMode: string, disabledControls: any[]): CViewGroup[] {
    let viewGroups: CViewGroup[] = [];
    let form: IForm = this.getForm(config, displayMode);
    if (form) {
      let disabledControlsFiltered = this.getFilteredDisabledControls(temporary, displayMode, disabledControls);
      let coeForms = this.sortAndFilterForms(form.coeForms.coeForm);
      coeForms.forEach(f => {
        if (f.formDisplay.visible === 'true') {
          if (viewGroups.length === 0) {
            viewGroups.push(new CViewGroup([], disabledControlsFiltered));
          }
          let viewGroup = viewGroups[viewGroups.length - 1];
          if (!viewGroup.append(f)) {
            viewGroups.push(new CViewGroup([f], disabledControlsFiltered));
          }
        }
      });
    }
    let viewGroupsFiltered: CViewGroup[] = [];
    viewGroups.forEach(vg => {
      if (displayMode === 'list' || vg.getItems(displayMode).length > 0) {
        viewGroupsFiltered.push(vg);
      }
    });
    return viewGroupsFiltered;
  }

  public static getColumns(temporary: boolean, config: IFormGroup, disabledControls: any[], pickListDomain: any[],
    systemSettings: CSystemSettings): CViewGroupColumns {
    const displayMode = 'list';
    const viewGroups: CViewGroup[] = CViewGroup.getViewGroups(temporary, config, displayMode, disabledControls);
    let viewGroupColumns = new CViewGroupColumns();
    if (viewGroups.length > 0) {
      viewGroupColumns = viewGroups[0].getColumns(displayMode, pickListDomain);
      let statusColumn = viewGroupColumns.baseTableColumns.find(c => c.dataField === 'STATUSID' || c.dataField === 'Approved');
      if (statusColumn) {
        if (temporary) {
          statusColumn.visible = systemSettings.isApprovalsEnabled;
        } else {
          statusColumn.visible = systemSettings.isLockingEnabled;
        }
      }
    }
    return viewGroupColumns;
  }

  public getEntryInfo(displayMode: string, id: string): CEntryInfo {
    let entryInfo = new CEntryInfo();
    this.data.forEach(f => {
      let formElementContainer = this.getFormElementContainer(f, displayMode);
      if (formElementContainer && formElementContainer.formElement) {
        formElementContainer.formElement.forEach(fe => {
          if (this.getDataField(fe) === id) {
            entryInfo.dataSource = f._dataSourceId;
            entryInfo.bindingExpression = fe.bindingExpression;
            if (fe.searchCriteriaItem) {
              entryInfo.searchCriteriaItem = fe.searchCriteriaItem;
            }
          }
        });
      }
    });
    return entryInfo;
  }

  public append(f: ICoeForm): boolean {
    let canAppend = this.canAppend(f);
    if (canAppend) {
      this.data.push(f);
      this.update();
    }
    return canAppend;
  }

  public getItems(displayMode: string): any[] {
    let items = [];
    this.data.forEach(f => {
      let formElementContainer = this.getFormElementContainer(f, displayMode);
      if (formElementContainer && formElementContainer.formElement) {
        formElementContainer.formElement.forEach((fe) => {
          if (this.isVisible(fe, displayMode, f._id)) {
            let item: any = {};
            if (fe.label) {
              this.setItemValue(item, 'label', { text: fe.label });
            } else {
              this.setItemValue(item, 'label', { visible: false });
            }
            this.setItemValue(item, 'editorType', this.getEditorType(fe));
            this.setItemValue(item, 'dataField', this.getDataField(fe));
            // discard duplicate formElements
            let duplicateIndex = items.findIndex(i => i.dataField === item.dataField);
            if (duplicateIndex === -1) {
              this.checkEditorType(fe, item);
              this.setDefaultValue(fe, item, displayMode);
              this.checkValidationRules(fe, item);
              items.push(item);
            }
          }
        });
      }
    });
    return this.fixColSpans(items);
  }

  public getColumns(displayMode: string, pickListDomain: any[]): CViewGroupColumns {
    let regColumns = [];
    let batchColumns = [];
    this.data.forEach(f => {
      let formElementContainer = this.getFormElementContainer(f, displayMode);
      if (formElementContainer && formElementContainer.formElement) {
        formElementContainer.formElement.forEach(fe => {
          let fieldConfig = fe.configInfo ? fe.configInfo.fieldConfig : undefined;
          if (fieldConfig && fieldConfig.tables) {
            let index = 0;
            fieldConfig.tables.table.forEach(tb => {
              const columns = index === 0 ? regColumns : batchColumns;
              if (tb.Columns && tb.Columns.Column) {
                tb.Columns.Column.forEach(c => {
                  const column: any = {
                    dataField: (c.formElement) ? c.formElement._name : c._name,
                    caption: (c.headerText && typeof c.headerText === 'string') ? c.headerText : c._name,
                    visible: !c._hidden || c._hidden.toLowerCase() !== 'true'
                  };
                  if (c.formElement && c.formElement.configInfo && c.formElement.configInfo.fieldConfig &&
                    c.formElement.configInfo.fieldConfig.PickListDomain) {
                    let currentPickListDomain = pickListDomain.find(d => d.ID.toString()
                      === c.formElement.configInfo.fieldConfig.PickListDomain.toString());
                    if (currentPickListDomain && currentPickListDomain.data) {
                      column.lookup = {
                        dataSource: currentPickListDomain.data,
                        displayExpr: 'value',
                        valueExpr: 'key'
                      };
                    }
                  }
                  if (c._name === 'STATUSCOLUMN' || c._name === 'STATUSID') {
                    column.width = 70;
                    column.allowEditing = false;
                    column.allowFiltering = false;
                    column.allowSorting = false;
                    column.cellTemplate = 'statusTemplate';
                    columns.push(column);
                  } else if (c._name === 'Structure' || c._name === 'STRUCTUREAGGREGATION') {
                    column.width = (c.width) ? c.width : '140';
                    column.allowEditing = false;
                    column.allowFiltering = false;
                    column.allowSorting = false;
                    column.cellTemplate = 'structureImageColumnTemplate';
                    columns.push(column);
                  } else if (c._name === 'REG_COMMENTS' || c._name === 'NOTEBOOK_TEXT' || c._name === 'PURITY_COMMENTS'
                    || c._name === 'BATCH_COMMENT' || c._name === 'STORAGE_REQ_AND_WARNINGS') {
                    if (!c._childTableName) {
                      column.width = 150;
                      column.dataType = 'string';
                      columns.push(column);
                    }
                  } else if (c._name === 'Marked') {
                    column.dataField = 'Marked';
                    column.visible = true;
                    column.width = 70;
                    column.allowEditing = false;
                    column.allowFiltering = false;
                    column.allowSorting = false;
                    column.cellTemplate = 'markedTemplate';
                    column.headerCellTemplate = 'markedHeaderTemplate';
                    columns.push(column);
                  } else if (!c._childTableName) {
                    column.visible = column.visible && c._name !== 'Review Record';
                    if (c.formElement && c.formElement.displayInfo && c.formElement.displayInfo.type) {
                      if (c.formElement.displayInfo.type.indexOf('COEDatePicker') > 0) {
                        column.dataType = 'date';
                      } else if (c.formElement.displayInfo.type.indexOf('COECheckBoxReadOnly') > 0) {
                        column.dataType = 'boolean';
                        column.cellTemplate = 'checkBoxTemplate';
                      } else if ((c.formElement.displayInfo.type.indexOf('COELink') > 0) && (index === 0)) {
                        column.cellTemplate = 'urlTemplate';
                      }
                    }
                    columns.push(column);
                  }
                });
              }
              index++;
            });
          }
        });
      }
    });
    let viewGroupColumns = new CViewGroupColumns();
    viewGroupColumns.baseTableColumns = regColumns;
    viewGroupColumns.batchTableColumns = batchColumns;
    return viewGroupColumns;
  }

  public getSearchItems(): ISearchCriteriaItem[] {
    let items: ISearchCriteriaItem[] = [];
    this.data.forEach(f => {
      let formElementContainer = this.getFormElementContainer(f, 'query');
      if (formElementContainer && formElementContainer.formElement) {
        formElementContainer.formElement.forEach(fe => {
          if (this.isVisible(fe, 'query', f._id) && fe.searchCriteriaItem) {
            items.push(JSON.parse(JSON.stringify(fe.searchCriteriaItem)));
          }
        });
      }
    });
    return items;
  }
}

export class CViewGroupContainer implements IViewGroupContainer {
  constructor(
    public id: string,
    public title: string,
    public viewGroups: CViewGroup[] = [],
    public subArray: any[] = null,
    public subIndex = 0) {
  }

  private getGroupedItems(displayMode: string): any[] {
    let items = [];
    this.viewGroups.forEach(vg => {
      items.push({
        itemType: 'group',
        caption: vg.title,
        cssClass: 'tan',
        colCount: 5,
        items: vg.getItems(displayMode)
      });
    });
    return items;
  }

  public static createSimpleViewGroupContainers(viewGroups: CViewGroup[]) {
    return viewGroups.map(vg => new CViewGroupContainer(vg.id, vg.title, [vg]));
  }

  public getEntryInfo(displayMode: string, id: string): CEntryInfo {
    let entryInfo = new CEntryInfo();
    this.viewGroups.forEach(vg => {
      let i = vg.getEntryInfo(displayMode, id);
      if (i.bindingExpression != null || i.dataSource != null || i.searchCriteriaItem != null) {
        entryInfo = i;
      }
    });
    return entryInfo;
  }

  public getItems(displayMode: string): any[] {
    return this.viewGroups.length === 1 ? this.viewGroups[0].getItems(displayMode) : this.getGroupedItems(displayMode);
  }

  public getSearchItems(): ISearchCriteriaItem[] {
    let items = [];
    this.viewGroups.forEach(vg => {
      items.concat(vg.getSearchItems());
    });
    return items;
  }
}

export class CRegistryRecord {
  _SameBatchesIdentity?: string; // True
  _ActiveRLS?: string; // Off
  _IsEditable?: string; // True
  _IsRegistryDeleteable?: string; // True
  ID?: string; // 921
  DateCreated?: string; // 2017-01-15 08:21:50 PM
  DateLastModified?: string; //  2017-01-15 08:38:18 PM
  PersonCreated?: string; // 61
  PersonApproved?: string; // number
  StructureAggregation?: string; // VmpD...
  StatusID?: string; // 3
  PropertyList?: IPropertyList;
  RegNumber?: IRegNumber;
  IdentifierList?: IIdentifierList;
  ProjectList?: IProjectList;
  ComponentList: IComponentList = new CComponentList();
  BatchList: IBatchList = new CBatchList();
  AddIns?: IAddInList;
  constructor() {
    this.ComponentList.Component.push(new CComponent());
    this.BatchList.Batch.push(new CBatch());
  }

  private static getFormElementContainer(f: ICoeForm, mode: string): ICoeFormMode {
    return mode === 'add' ? f.addMode : mode === 'edit' ? f.editMode : f.viewMode;
  }

  private static getDataField(fe: IFormElement): string {
    return fe.Id ? fe.Id : fe._name.replace(/\s/g, '');
  }

  private static fixBindingExpression(expression: string): string {
    if (expression === 'ID') {
      expression = 'BatchID';
    }
    if (expression.endsWith('.RegNumber.RegNum')) {
      expression = expression.replace('.RegNumber.RegNum', '.RegNumber.RegNumber');
    }
    return expression.replace('.Sequence.ID', '.SequenceID')
      .replace('.Structure.ID', '.Structure.StructureID')
      .replace('.Structure.UseNormalizedStructure', '.Structure.UseNormalization')
      .replace('.Structure.Value', '.Structure')
      .replace('.Structure.Formula', '.Structure.Structure._formula')
      .replace('.Structure.MolWeight', '.Structure.Structure._molWeight');
  }

  /**
   * Flattens the specified property of the specified object into a string value
   * @param object The object to use
   * @param property The name of the property to serialize
   */
  private static serializeValue(object: any, property: string) {
    let valueObject = object[property];
    if (valueObject && typeof valueObject === 'object') {
      // If the value is object, convert it to a string value.
      if (valueObject.viewModel) {
        // If this is a form-item, use toString to convert.
        object[property] = object[property].toString();
      } else if (valueObject.Structure) {
        // If this is an IStructureData object, use its serializeValue method.
        let structureData: IStructureData = valueObject;
        let structureFormItem: any = structureData.Structure.__text;
        if (typeof structureFormItem === 'object' && structureFormItem.viewModel) {
          object[property] = structureFormItem.serializeValue(structureFormItem.toString());
        }
      }
    }
  }

  public static createFromPlainObj(obj: IRegistryRecord): CRegistryRecord {
    let created = new CRegistryRecord();
    for (let k in obj) {
      if (obj.hasOwnProperty(k)) {
        created[k] = obj[k];
      }
    }
    return created;
  }

  public static findBoundObject(viewModel: any, bindingExpression: string, createMissing: boolean = false): CBoundObject {
    let foundObject = new CBoundObject();
    bindingExpression = this.fixBindingExpression(bindingExpression);
    let objectNames = bindingExpression.split('.');
    let nextObject = viewModel;
    let index = 0;
    objectNames.forEach(n => {
      ++index;
      if (nextObject) {
        let m = n.match(/PropertyList\[@Name='(.*)'/);
        if (m && m.length > 1) {
          let propertyList = nextObject.PropertyList as IPropertyList;
          if (propertyList != null && propertyList.Property != null) {
            let p = propertyList.Property.filter(pt => pt._name === m[1]);
            if (createMissing && p.length === 0) {
              propertyList.Property.push({ _name: m[1] });
              p = propertyList.Property.filter(pt => pt._name === m[1]);
            }
            if (p.length > 0) {
              foundObject.obj = p[0];
              foundObject.property = '__text';
            }
          }
        } else if (index === objectNames.length) {
          foundObject.obj = nextObject;
          foundObject.property = n;
        } else {
          if (createMissing && !nextObject[n]) {
            nextObject[n] = {};
          }
          nextObject = nextObject[n];
        }
      }
    });
    return foundObject;
  }

  public getDataSource(dataSource: string, subIndex: number): any {
    dataSource = dataSource.toLowerCase();
    return dataSource.indexOf('fragmentscsla') >= 0 ? this.BatchList.Batch[subIndex].BatchComponentList.BatchComponent[0]
      : dataSource.indexOf('component') >= 0 ? this.ComponentList.Component[subIndex]
        : dataSource.indexOf('batch') >= 0 || dataSource.startsWith('fragments') ? this.BatchList.Batch[subIndex]
          : this;
  }

  public setEntryValue(viewConfig: CViewGroupContainer, displayMode: string, id: string, entryValue, serialize: boolean = false) {
    let entryInfo = viewConfig.getEntryInfo(displayMode, id);
    if (entryInfo.dataSource && entryInfo.bindingExpression) {
      let dataSource = this.getDataSource(entryInfo.dataSource, viewConfig.subIndex);
      let foundObject = CRegistryRecord.findBoundObject(dataSource, entryInfo.bindingExpression, true);
      if (foundObject && foundObject.property) {
        if (serialize || (foundObject.obj[foundObject.property] &&
          typeof (foundObject.obj[foundObject.property]) === 'object' && displayMode === 'add')) {
          CRegistryRecord.serializeValue(foundObject.obj, foundObject.property);
        } else {
          foundObject.obj[foundObject.property] = entryValue;
        }
      }
    }
  }

  /**
  * Flattens all data into string
  * @param displayMode The current display-mode
  * @param idList The list of ID's to update
  * @param viewModel The view-model to flatten
  */
  public serializeFormData(viewConfig: CViewGroupContainer[], displayMode: string, idList: string[]) {
    viewConfig.forEach(vgc => {
      idList.forEach(id => {
        this.setEntryValue(vgc, displayMode, id, null, true);
      });
    });
  }
}

export interface IRegistryRecord extends CRegistryRecord {
}

export interface ISearchCriteriaValue {
  _negate: string;
  __text?: string;
}

export interface ISearchCriteriaItem {
  _id: string;
  _tableid: string;
  _fieldid: string;
  _modifier?: string;
  _aggregateFunctionName?: string;
  _searchLookupByID?: string;
  structureCriteria?: any;
  StructureCriteria?: any;
  molweightCriteria?: any;
  MolWeightCriteria?: any;
}

export class CSearchCriteria {
  constructor(private searchCriteriaItem: ISearchCriteriaItem[] = []) {
  }

  private static get x2jsTool() {
    return new X2JS.default({
      arrayAccessFormPaths: [
        'searchCriteria.searchCriteriaItem',
      ]
    });
  }

  private getSearchCriteriaItemObj(item: ISearchCriteriaItem): ISearchCriteriaValue {
    let objectProp = Object.getOwnPropertyNames(item).find(n => typeof item[n] === 'object');
    return item[objectProp] as ISearchCriteriaValue;
  }

  private getQueryEntryValue(searchCriteriaItem: ISearchCriteriaItem): string {
    let value;
    let searchCriteriaItemObj: any = this.getSearchCriteriaItemObj(searchCriteriaItem);
    if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeStructureCriteria) {
      value = searchCriteriaItemObj.CSCartridgeStructureCriteria;
    } else if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeMolWeightCriteria && searchCriteriaItemObj.CSCartridgeMolWeightCriteria.__text) {
      value = searchCriteriaItemObj.CSCartridgeMolWeightCriteria.__text;
    } else if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeFormulaCriteria && searchCriteriaItemObj.CSCartridgeFormulaCriteria.__text) {
      value = searchCriteriaItemObj.CSCartridgeFormulaCriteria.__text;
    } else if (searchCriteriaItemObj.__text) {
      value = searchCriteriaItemObj.__text;
    } else if (searchCriteriaItem.structureCriteria || searchCriteriaItem.StructureCriteria) {
      value = searchCriteriaItem.structureCriteria ? searchCriteriaItem.structureCriteria : searchCriteriaItem.StructureCriteria;
    }
    return value;
  }

  private setQueryEntryValue(searchCriteriaItem: ISearchCriteriaItem, entryValue, serialize: boolean = false) {
    let searchCriteriaItemObj: any = this.getSearchCriteriaItemObj(searchCriteriaItem);
    if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeStructureCriteria) {
      for (let k in entryValue) {
        if (entryValue.hasOwnProperty(k)) {
          searchCriteriaItemObj.CSCartridgeStructureCriteria[k] = entryValue[k];
        }
      }
    } else if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeMolWeightCriteria && searchCriteriaItemObj.CSCartridgeMolWeightCriteria.__text) {
      searchCriteriaItemObj.CSCartridgeMolWeightCriteria.__text = entryValue;
    } else if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeFormulaCriteria && searchCriteriaItemObj.CSCartridgeFormulaCriteria.__text) {
      searchCriteriaItemObj.CSCartridgeFormulaCriteria.__text = entryValue;
    } else if (typeof entryValue !== 'object') {
      searchCriteriaItemObj.__text = entryValue;
    }
  }

  public static getConfiguredItems(viewConfig: CViewGroup[]): ISearchCriteriaItem[] {
    let searchItems: ISearchCriteriaItem[] = [];
    viewConfig.forEach(vg => {
      let items = vg.getSearchItems();
      items.forEach(i => {
        searchItems.push(i);
      });
    });
    return searchItems;
  }

  public static deserialize(viewConfig: CViewGroup[], xml: string): CSearchCriteria {
    // This is to compensate bug in the server implementation.
    // Sometimes the XML contents are not properly encoded.
    xml = xml.replace('<=', '&lt;-').replace('< ', '&lt; ');
    let query: any = this.x2jsTool.xml2js(xml);
    let queryItems: ISearchCriteriaItem[] = query && query.searchCriteria ? query.searchCriteria.searchCriteriaItem : [];
    let searchItems = this.getConfiguredItems(viewConfig);
    queryItems.forEach(qi => {
      let i = searchItems.findIndex(si => si._id === qi._id);
      if (i >= 0) {
        searchItems[i] = qi;
      }
    });
    return new CSearchCriteria(searchItems);
  }

  public serialize(): string {
    const items: ISearchCriteriaItem[] = [];
    this.searchCriteriaItem.forEach(i => {
      const value: any = this.getQueryEntryValue(i);
      if (value) {
        if (i.structureCriteria || i.StructureCriteria) {
          const structureCriteria = i.structureCriteria ? i.structureCriteria : i.StructureCriteria;
          if (structureCriteria.CSCartridgeStructureCriteria == null) {
            // The system requires CSCartridgeStructureCriteria, but it's not present.
            // This might be due to legacy search criteria XML.
            // Fix this by moving all properties (except _negate) to CSCartridgeStructureCriteria.
            // Note that this condition is not met when search criteria is restored from saved query.
            const cartridgeCriteria = {};
            for (let k in structureCriteria) {
              if (structureCriteria.hasOwnProperty(k) && k !== '_negate') {
                cartridgeCriteria[k] = structureCriteria[k];
                delete structureCriteria[k];
              }
            }
            structureCriteria.CSCartridgeStructureCriteria = cartridgeCriteria;
          }
          const structureData = structureCriteria.CSCartridgeStructureCriteria.__text;
          if (typeof structureData === 'object' && structureData.viewModel) {
            structureCriteria.CSCartridgeStructureCriteria.__text = structureData.toString();
          }
          if (structureCriteria.CSCartridgeStructureCriteria.__text) {
            items.push(i);
          }
        } else if (i.molweightCriteria || i.MolWeightCriteria) {
          items.push(this.getMolWeightCriteria(i, value));
        } else {
          items.push(i);
        }
      }
    });
    return CSearchCriteria.x2jsTool.js2xml({ searchCriteria: new CSearchCriteria(items) })
      .replace('<searchCriteria>', `<?xml version="1.0" encoding="UTF-8"?><searchCriteria xmlns="COE.SearchCriteria">`);
  }

  public getMolWeightCriteria(c: any, entryValue: any) {
    let searchCriteria: any = {
      searchCriteriaItem: {
        _fieldid: c._fieldid,
        _id: c._id,
        _tableid: c._tableid,
        _searchLookupByID: c._searchLookupByID,
        _aggregateFunctionName: c._aggregateFunctionName
      }
    };

    let molWeightCriteria: any = {
      CSCartridgeMolWeightCriteria: {
        _negate: c.molweightCriteria ? c.molweightCriteria._negate : c.MolWeightCriteria._negate,
        _operator: c.molweightCriteria ? c.molweightCriteria._operator : c.MolWeightCriteria._operator,
        __text: entryValue
      }
    };
    if (entryValue) {
      const criteria = molWeightCriteria.CSCartridgeMolWeightCriteria;
      entryValue = entryValue.trim().toUpperCase();
      if ((entryValue.startsWith(`"`) && entryValue.endsWith(`"`))
        || (entryValue.startsWith(`'`) && entryValue.endsWith(`'`))) {
        entryValue = entryValue.substring(1, entryValue.length - 2).trim();
      }
      criteria._max = 1e20;
      const match = entryValue.match(/^(.*\d+)( *- *)(\+?\d+.*)$/);
      const values = match && match.length === 4 ? [`>${match[1]}`, `<${match[3]}`] : entryValue.split(' AND ');
      this.setMinMaxValues(criteria, values[0]);
      this.setMinMaxValues(criteria, values[1]);
    }
    searchCriteria.searchCriteriaItem.molWeightCriteria = molWeightCriteria;
    return searchCriteria.searchCriteriaItem;
  }

  private setMinMaxValues(criteria: any, text: string) {
    if (text) {
      text = text.trim();
      const match = text.match(/^([<>]?)(.*)$/);
      if (match.length === 3) {
        const type: string = !match[1] ? 'both' : match[1] === '<' ? 'max' : 'min';
        text = match[2] ? match[2].trim() : undefined;
        if (text && text[0] === '+') {
          text = text.substring(1).trim();
        }
        let value = Number(text);
        if (type === 'both' && (isNaN(value) || value < 0)) {
          criteria._max = -criteria._max;
        } else {
          let min = value;
          let max = value;
          if (type === 'both') {
            let toleranceLevel = 1;
            if (value > 0) {
              let expIndex = text.toLowerCase().indexOf('e');
              if (expIndex > 0) {
                let digits = text.substring(0, expIndex).replace('.', '').replace('+', '').match(/^(0*)([^0]\d*)$/)[2];
                toleranceLevel = digits.length - Math.floor(Math.log10(value));
              } else {
                let decimalPointIndex = text.indexOf('.');
                if (decimalPointIndex >= 0) {
                  toleranceLevel = text.length - decimalPointIndex;
                }
              }
            }
            let tolerance = 5 * Math.pow(10, -Math.min(toleranceLevel, 3));
            min -= tolerance;
            max += tolerance;
          }
          if (type !== 'max') {
            criteria._min = Math.max(criteria._min ? criteria._min : 0, min);
          }
          if (type !== 'min') {
            criteria._max = Math.min(criteria._max, max);
          }
        }
      }
    }
  }

  public getQueryFormData(viewConfig: CViewGroupContainer, displayMode: string, idList: string[]): any {
    let formData: any = {};
    idList.forEach(id => {
      let entryInfo = viewConfig.getEntryInfo(displayMode, id);
      if (entryInfo.searchCriteriaItem) {
        this.searchCriteriaItem.forEach(i => {
          if (entryInfo.searchCriteriaItem._id === i._id) {
            formData[id] = this.getQueryEntryValue(i);
          }
        });
      }
    });
    return formData;
  }

  public updateFromQueryFormData(formData: any, viewConfig: CViewGroupContainer, displayMode: string, idList: string[]) {
    idList.forEach(id => {
      let entryInfo = viewConfig.getEntryInfo(displayMode, id);
      if (entryInfo.searchCriteriaItem) {
        this.searchCriteriaItem.forEach(i => {
          if (entryInfo.searchCriteriaItem._id === i._id) {
            this.setQueryEntryValue(i, formData[id]);
          }
        });
      }
    });
  }
}

export class CStructureQueryOptions {
  _hitAnyChargeHetero: string = 'YES';
  _reactionCenter: string = 'YES';
  _hitAnyChargeCarbon: string = 'YES';
  _permitExtraneousFragments: string = 'NO';
  _permitExtraneousFragmentsIfRXN: string = 'NO';
  _fragmentsOverlap: string = 'NO';
  _tautometer: string = 'NO';
  _doubleBondStereo: string = 'YES';
  _simThreshold: string = '100';
  _fullSearch: string = 'NO';
  _identity: string = 'NO';
  _similar: string = 'NO';
  _tetrahedralStereo: string = 'SAME';
  _relativeTetStereo: string = 'YES';
  __text: any;
}

export interface IStructureQueryOptions extends CStructureQueryOptions {
}
