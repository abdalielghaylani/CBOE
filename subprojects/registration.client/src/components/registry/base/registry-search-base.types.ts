import { EventEmitter } from '@angular/core';
import { IViewGroup } from './registry-base.types';
import { ILookupData } from '../../../redux';
import { ICoeForm, ICoeFormMode, IFormElement } from '../../../common';
import { IPropertyList } from './registry-base.types';

export interface ITabularData {
  data: any;
  columns: any[];
}

class CEntryInfo {
  dataSource: string;
  bindingExpression: string;
  searchCriteriaItem: any;
}

export class CViewGroup implements IViewGroup {
  public id: string;
  public title: string;
  public searchCriteria: any;
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
    }else if (type.indexOf('COEChemDraw') > 0) {
      item.template = readOnly ? 'structureImageTemplate' : 'structureTemplate';
      if (!readOnly) {
        item.colSpan = 5;
      }
    } else if (type.indexOf('COEDatePicker') > 0) {
      item.template = 'dateTemplate';
    } else if (fe.bindingExpression.endsWith('ProjectList')) {
      item.template = 'projectsTemplate';
    } else if (fe.displayInfo.type.endsWith('COEFragments')) {
      item.template = 'fragmentsTemplate';
      item.colSpan = 5;
    }
    if (!item.template) {
      item.editorType = 'dxTextBox';
    }
    if (!item.editorOptions) {
      item.editorOptions = {};
    }
    item.editorOptions.readOnly = readOnly;
  }
  
  private checkOtherDropdown(fe: IFormElement, item: any, lookups: ILookupData) {
    if (fe.displayInfo.type.endsWith('COEDropDownList') &&
      (fe.Id.indexOf('PROJECTDropDownListPerm') >= 0 || fe.Id.indexOf('IDENTIFIERTYPETextBox') >= 0)) {
      item.editorType = 'dxSelectBox';
      let criteria: string;
      if (fe.Id.indexOf('PROJECTDropDownListPerm') >= 0) {
        criteria = this.getSelectCriteria(fe, 'lookups.projects');
      } else if (fe.Id.indexOf('IDENTIFIERTYPETextBox') >= 0) {
        criteria = this.getSelectCriteria(fe, 'lookups.identifierTypes');
      }
      item.editorOptions = {
        dataSource: criteria ? (eval)(criteria) : '',
        displayExpr: 'NAME',
        valueExpr: 'ID',
        placeholder: 'Select'
      };
    }
  }

  private getSelectCriteria(fe: IFormElement, type: string) {
    if (fe.configInfo.fieldConfig.dropDownItemsSelect.toUpperCase().indexOf('WHERE') >= 0) {
      return type + '.filter(i=>' + fe.configInfo.fieldConfig.dropDownItemsSelect.
        split(/WHERE/gi)[1].replace(/OR/gi, '||').
        replace(/AND/gi, '&&').replace(/TYPE/gi, 'i.TYPE').replace(/=/gi, '===').replace(/ACTIVE/gi, 'i.ACTIVE') + ')';
    } else {
      return type;
    }
  }

  private getEntryInfo(displayMode: string, id: string): CEntryInfo {
    let entryInfo = new CEntryInfo();
    this.data.forEach(f => {
      if (f.layoutInfo && f.layoutInfo.formElement) {
        f.layoutInfo.formElement.forEach(fe => {
          if ((fe.Id && fe.Id === id) || (!fe.Id && fe._name.replace(/\s/g, '') === id)) {
            entryInfo.dataSource = f._dataSourceId;
            entryInfo.bindingExpression = fe.bindingExpression;
            entryInfo.searchCriteriaItem = fe.searchCriteriaItem;
          }
        });
      }
    });
    return entryInfo;
  }

  private fixBindingExpression(expression: string): string {
    return expression.replace('.Sequence.ID', '.SequenceID')
      .replace('.Structure.Value', '.Structure.Structure.__text')
      .replace('.Structure.Formula', '.Structure.Structure._formula')
      .replace('.Structure.MolWeight', '.Structure.Structure._molWeight');
  }

  private parseEntryValue(bindingExpression: string, viewModel: any) {
    bindingExpression = this.fixBindingExpression(bindingExpression);
    let objectNames = bindingExpression.split('.');
    let nextObject = viewModel;
    objectNames.forEach(n => {
      if (nextObject) {
        let m = n.match(/PropertyList\[@Name='(.*)'/);
        if (m && m.length > 1) {
          let propertyList = nextObject.PropertyList as IPropertyList;
          let p = propertyList.Property.filter(p => p._name === m[1]);
          nextObject = p ? p[0].__text : undefined;
        } else {
          nextObject = nextObject[n];
        }
      }
    });
    return nextObject;
  }

  private getEntryValue(displayMode: string, id: string, viewModel: any): any {
    let entryInfo = this.getEntryInfo(displayMode, id);
    let dataSource = entryInfo.dataSource ? entryInfo.dataSource.toLowerCase() : '';
    return dataSource.indexOf('component') >= 0 ? this.parseEntryValue(entryInfo.bindingExpression, viewModel.ComponentList.Component[0])
      : dataSource.indexOf('batch') >= 0 ? this.parseEntryValue(entryInfo.bindingExpression, viewModel.BatchList.Batch[0])
        : this.parseEntryValue(entryInfo.bindingExpression, viewModel);
  }

  private setEntryValue(displayMode: string, id: string, viewModel: any, entryValue, serialize: boolean = false) {
    let entryInfo = this.getEntryInfo(displayMode, id);
    // let dataSource = entryInfo.dataSource ? entryInfo.dataSource.toLowerCase() : '';
    // dataSource.indexOf('component') >= 0 ?
    //  this.parseAndSetEntryValue(entryInfo.searchCriteriaItem, entryInfo.bindingExpression, viewModel.ComponentList.Component[0], entryValue, serialize)
    // : dataSource.indexOf('batch') >= 0 ?
    //    this.parseAndSetEntryValue(entryInfo.searchCriteriaItem, entryInfo.bindingExpression, viewModel.BatchList.Batch[0], entryValue, serialize)
    //    : 
    this.parseAndSetEntryValue(entryInfo.searchCriteriaItem, entryInfo.bindingExpression, viewModel, entryValue, serialize);
  }

  private parseAndSetEntryValue(searchCriteriaItem: any, bindingExpression: string, viewModel: any, value, serialize: boolean = false) {
    bindingExpression = this.fixBindingExpression(bindingExpression);
    let objectNames = bindingExpression.split('.');
    let nextObject = viewModel;
    if (value) {
      for (const prop in searchCriteriaItem) {
        if (typeof searchCriteriaItem[prop] === 'object') {
          searchCriteriaItem[prop].__text = value;
        }
      }
      nextObject[objectNames[0]] = searchCriteriaItem;
    }
  }

  private serializeValue(object: any, property: string) {
    let textObject = object[property];
    if (textObject && typeof textObject === 'object' && textObject.viewModel) {
      object[property] = object[property].toString();
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
            this.setItemValue(item, 'searchCriteriaItem', fe.searchCriteriaItem);
            if (fe.label) {
              this.setItemValue(item, 'label', { text: fe.label });
            } else {
              this.setItemValue(item, 'label', { visible: false });
            }
            this.setItemValue(item, 'editorType', this.getEditorType(fe));
            this.setItemValue(item, 'dataField', this.getDataField(fe));
            this.checkEditorType(fe, item);
            // this.checkOtherDropdown(fe, item, lookups);
            this.removeDuplicate(items, item);
            items.push(item);
          }
        });
      }
    });
    return this.fixColSpans(items);
  }

  private removeDuplicate(items: any[], item) {
    let duplicateIndex = items.findIndex(i => i.dataField === item.dataField);
    if (duplicateIndex > 0) {
      items.splice(duplicateIndex, 1);
    }
  }

  public getFormData(displayMode: string, idList: string[], viewModel: any): any {
    let formData: any = {};
    idList.forEach(id => {
      formData[id] = this.getEntryValue(displayMode, id, viewModel);
    });
    return formData;
  }

  public readFormData(displayMode: string, idList: string[], viewModel: any, formData: any) {
    idList.forEach(id => {
      this.setEntryValue(displayMode, id, viewModel, formData[id]);
    });
  }
}
