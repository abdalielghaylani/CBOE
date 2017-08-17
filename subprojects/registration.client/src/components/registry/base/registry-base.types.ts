import { EventEmitter } from '@angular/core';
import * as X2JS from 'x2js';
import { ISearchCriteriaItem } from './registry-base.types';
import { IFormGroup, IForm, ICoeForm, ICoeFormMode, IFormElement } from '../../../common';
import {
  CBatch, CBatchList, CComponent, CComponentList,
  IAddInList, IBatchList, IComponentList, IIdentifierList, IProjectList, IPropertyList, IValidationRuleList,
  IRegNumber, IStructureData
} from '../../common';

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

export class CEntryInfo {
  dataSource: string;
  bindingExpression: string;
  searchCriteriaItem?: ISearchCriteriaItem;
}

export class CBoundObject {
  obj: any;
  property?: string;
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
    return this.data.length === 0 || !f.title || this.title === f.title || f.title === 'Fragment Information';
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
      item.colSpan = 2;
    } else if (type.endsWith('COEStructureQuery')) {
      item.template = 'structureQueryTemplate';
      item.colSpan = 5;
    } else if (type.indexOf('COEChemDraw') > 0) {
      item.template = readOnly ? 'structureImageTemplate' : 'structureTemplate';
      if (!readOnly) {
        item.colSpan = 5;
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
    }
    if (!item.template) {
      item.template = 'textTemplate';
    }
    if (!item.editorOptions) {
      item.editorOptions = {};
    }
    item.editorOptions.readOnly = readOnly;
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

  private isVisible(fe: IFormElement): boolean {
    return !this.disabledControls.find(dc => dc.id && dc.id === fe.Id)
      && fe.displayInfo && fe.displayInfo.visible === 'true' && fe._name && fe.configInfo
      && !fe.displayInfo.type.endsWith('COELabel')
      && !fe.displayInfo.type.endsWith('RegStatusControl')
      && !fe.displayInfo.type.endsWith('COEChemDrawToolbar')
      && !this.isIdText(fe);
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

  private removeDuplicate(items: any[], item) {
    let duplicateIndex = items.findIndex(i => i.dataField === item.dataField);
    if (duplicateIndex > 0) {
      items.splice(duplicateIndex, 1);
    }
  }

  private static getForm(config: IFormGroup, displayMode: string): IForm {
    let form: IForm = undefined;
    if (config) {
      if (displayMode === 'query') {
        if (config.queryForms && config.queryForms.queryForm && config.queryForms.queryForm.length > 0) {
          form = config.queryForms.queryForm[0];
        }
      } else if (config.detailsForms && config.detailsForms.detailsForm && config.detailsForms.detailsForm.length > 0) {
        form = config.detailsForms.detailsForm[0];
      }
    }
    return form;
  }

  private static getFilteredDisabledControls(displayMode: string, disabledControls: any[]): any[] {
    let pageId: string = displayMode === 'add' ? 'SUBMITMIXTURE'
      : displayMode === 'view' ? 'VIEWMIXTURE'
        : displayMode === 'edit' ? 'REVIEWREGISTERMIXTURE'
          : displayMode === 'query' ? 'CHEMBIOVIZSEARCH'
            : undefined;
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

  public static getViewGroups(config: IFormGroup, displayMode: string, disabledControls: any[]): CViewGroup[] {
    let viewGroups: CViewGroup[] = [];
    let viewGroupsFiltered: CViewGroup[] = [];
    let form: IForm = this.getForm(config, displayMode);
    if (form) {
      let disabledControlsFiltered = this.getFilteredDisabledControls(displayMode, disabledControls);
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
        viewGroupsFiltered = [];
        viewGroups.forEach(vg => {
          if (vg.getItems(displayMode).length > 0) {
            viewGroupsFiltered.push(vg);
          }
        });
      });
    }
    return viewGroupsFiltered;
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
        formElementContainer.formElement.forEach(fe => {
          if (this.isVisible(fe)) {
            let item: any = {};
            if (fe.label) {
              this.setItemValue(item, 'label', { text: fe.label });
            } else {
              this.setItemValue(item, 'label', { visible: false });
            }
            this.setItemValue(item, 'editorType', this.getEditorType(fe));
            this.setItemValue(item, 'dataField', this.getDataField(fe));
            this.checkEditorType(fe, item);
            this.checkValidationRules(fe, item);
            this.removeDuplicate(items, item);
            items.push(item);
          }
        });
      }
    });
    return this.fixColSpans(items);
  }

  public getSearchItems(): ISearchCriteriaItem[] {
    let items: ISearchCriteriaItem[] = [];
    this.data.forEach(f => {
      let formElementContainer = this.getFormElementContainer(f, 'query');
      if (formElementContainer && formElementContainer.formElement) {
        formElementContainer.formElement.forEach(fe => {
          if (this.isVisible(fe) && fe.searchCriteriaItem) {
            items.push(JSON.parse(JSON.stringify(fe.searchCriteriaItem)));
          }
        });
      }
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
    if (expression.endsWith('.RegNumber.RegNum')) {
      expression = expression.replace('.RegNumber.RegNum', '.RegNumber.RegNumber');
    }
    return expression.replace('.Sequence.ID', '.SequenceID')
      .replace('.Structure.ID', '.Structure.StructureID')
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
          let p = propertyList.Property.filter(p => p._name === m[1]);
          if (createMissing && p.length === 0) {
            propertyList.Property.push({ _name: m[1] });
            p = propertyList.Property.filter(p => p._name === m[1]);
          }
          if (p.length > 0) {
            foundObject.obj = p[0];
            foundObject.property = '__text';
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

  public getDataSource(dataSource: string): any {
    dataSource = dataSource.toLowerCase();
    return dataSource.indexOf('fragmentscsla') >= 0 ? this.BatchList.Batch[0].BatchComponentList.BatchComponent[0]
      : dataSource.indexOf('component') >= 0 ? this.ComponentList.Component[0]
        : dataSource.indexOf('batch') >= 0 || dataSource.startsWith('fragments') ? this.BatchList.Batch[0]
          : this;
  }

  public setEntryValue(viewConfig: CViewGroup, displayMode: string, id: string, entryValue, serialize: boolean = false) {
    let entryInfo = viewConfig.getEntryInfo(displayMode, id);
    if (entryInfo.dataSource && entryInfo.bindingExpression) {
      let dataSource = this.getDataSource(entryInfo.dataSource);
      let foundObject = CRegistryRecord.findBoundObject(dataSource, entryInfo.bindingExpression, true);
      if (foundObject.property) {
        if (serialize) {
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
  public serializeFormData(viewConfig: CViewGroup[], displayMode: string, idList: string[]) {
    viewConfig.forEach(vg => {
      idList.forEach(id => {
        this.setEntryValue(vg, displayMode, id, null, true);
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
    if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeStructureCriteria && searchCriteriaItemObj.CSCartridgeStructureCriteria.__text) {
      value = searchCriteriaItemObj.CSCartridgeStructureCriteria.__text;
    } else if (searchCriteriaItemObj.__text) {
      value = searchCriteriaItemObj.__text;
    }
    return value;
  }

  private setQueryEntryValue(searchCriteriaItem: ISearchCriteriaItem, entryValue, serialize: boolean = false) {
    let searchCriteriaItemObj: any = this.getSearchCriteriaItemObj(searchCriteriaItem);
    if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeStructureCriteria && searchCriteriaItemObj.CSCartridgeStructureCriteria.__text) {
      searchCriteriaItemObj.CSCartridgeStructureCriteria.__text = entryValue;
    } else {
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
    let items: ISearchCriteriaItem[] = [];
    this.searchCriteriaItem.forEach(i => {
      let value: any = this.getQueryEntryValue(i);
      if (value) {
        if (typeof value === 'object' && value.viewModel) {
          this.setQueryEntryValue(i, value.toString());
        }
        items.push(i);
      }
    });
    return CSearchCriteria.x2jsTool.js2xml({ searchCriteria: new CSearchCriteria(items) })
      .replace('<searchCriteria>', `<?xml version="1.0" encoding="UTF-8"?><searchCriteria xmlns="COE.SearchCriteria">`);
  }

  public getQueryFormData(viewConfig: CViewGroup, displayMode: string, idList: string[]): any {
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

  public updateFromQueryFormData(formData: any, viewConfig: CViewGroup, displayMode: string, idList: string[]) {
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
