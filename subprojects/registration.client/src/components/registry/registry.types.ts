import { apiUrlPrefix } from '../../configuration';
import {
  IShareableObject, FormGroupType, SubFormType, IFormGroup, ICoeForm, IFormElement, IFormContainer
} from '../../common';
import { IAppState, IRecordsData, IRecords, ILookupData } from '../../redux';
import {
  IIdentifier, IIdentifierList, IProjectList, IFragment, IComponent,
  IBatchComponentFragment, IBatch, IRegistryRecord, IProperty, IPropertyList
} from './base';

export enum RegistryStatus {
  NotSet,
  Submitted,
  Approved,
  Registered,
  Locked
}

/**
 * Build the form group contents for batch compound fragments
 */
function buildBatchCompoundFragmentGroup(container: IFormContainer): any {
  let groupItem = {
    itemType: 'group',
    caption: undefined,
    items: []
  };
  let lookups = getLookups(container);
  let coeForm = getCoeFormById(container.formGroup, SubFormType.BatchComponentFragmentsForm);
  if (coeForm) {
    // if (coeForm.title) {
    //   groupItem.caption = coeForm.title as string;
    // }
    groupItem.items = [{
      dataField: 'batchComponentFragmentList',
      label: { text: 'Fragments' },
      colSpan: 1,
      template: 'fragmentsTemplate'
    }];
  }
  return groupItem;
}

function onCellPrepared(e) {
  if (e.rowType === 'data' && e.column.command === 'edit') {
    let isEditing = e.row.isEditing;
    let $links = e.cellElement.find('.dx-link');
    $links.text('');
    if (isEditing) {
      $links.filter('.dx-link-save').addClass('dx-icon-save');
      $links.filter('.dx-link-cancel').addClass('dx-icon-revert');
    } else {
      $links.filter('.dx-link-edit').addClass('dx-icon-edit');
      $links.filter('.dx-link-delete').addClass('dx-icon-trash');
    }
  }
}

function getPicklist(domainId: number, formElement: IFormElement, container: IFormContainer) {
  let lookups = getLookups(container);
  let pickListDomains = lookups ? lookups.pickListDomains : [];
  let placeholder = 'Select ' + (formElement ? formElement.label : '...');
  let pickList = {
    dataSource: [],
    displayExpr: 'NAME',
    valueExpr: 'ID',
    placeholder: placeholder,
    showClearButton: true
  };
  let pickListDomain = pickListDomains.find(d => d.ID === domainId);
  if (pickListDomain != null) {
    pickList.dataSource = pickListDomain.data;
    pickList.valueExpr = pickListDomain.EXT_ID_COL;
    pickList.displayExpr = pickListDomain.EXT_DISPLAY_COL;
  }
  return pickList;
}

function getPropertyColumn(p: IProperty, formElements: IFormElement[], container: IFormContainer): any {
  let formElement = formElements.find(fe => fe._name === p._name);
  if (formElement != null && formElement.displayInfo.visible === 'false') {
    return null;
  }
  let column: any = {
    dataField: p._name,
    dataType: p._type === 'DATE' ? 'date' : 'string'
  };
  if (column.dataType === 'date') {
    column.editorType = 'dxDateBox';
  }
  let label: any = {};
  if (formElement != null) {
    label.text = formElement.label;
    column.label = label;
  }
  if (p._type === 'PICKLISTDOMAIN') {
    column.dataType = 'number';
    column.editorType = 'dxSelectBox';
    column.editorOptions = getPicklist(+p._pickListDomainID, formElement, container);
  }
  // if (p._precision > 100) {
  //   column.colSpan = 2;
  //   column.editorType = 'dxTextArea';
  // }
  return column;
}

function getLookups(container: IFormContainer): ILookupData {
  let state: IAppState = container.ngRedux.getState();
  return state.session ? state.session.lookups : undefined;
}

function getPropertyValue(p: IProperty): any {
  let value = undefined;
  let textValue = p.__text;
  if (textValue) {
    textValue = textValue.trim();
  }
  switch (p._type) {
    case 'DATE':
      if (textValue) {
        value = new Date(textValue);
      }
      break;
    case 'PICKLISTDOMAIN':
      value = +textValue;
      break;
    default:
      value = textValue;
      break;
  }
  return value;
}

function getCoeFormById(formGroup: IFormGroup, coeFormId: number) {
  let coeForm = formGroup ? formGroup.detailsForms.detailsForm[0].coeForms.coeForm.find(f => +f._id === coeFormId) : undefined;
  return coeForm != null ? coeForm : null;
}

/**
 * Adds the custom properties to the given view-model object, and
 * adds the property column description into the given column array. 
 */
function buildPropertyList(vm: any, columns: any[], propertyList: IPropertyList, coeForm: ICoeForm, container: IFormContainer) {
  if (propertyList) {
    let formElements = coeForm ? coeForm.editMode.formElement : [];
    propertyList.Property.forEach(p => {
      let propertyName = p._name;
      if (propertyName) {
        vm[propertyName as string] = getPropertyValue(p);
        let column = getPropertyColumn(p, formElements, container);
        if (column) {
          columns.push(column);
        }
      }
    });
  }
}

export class CRecords implements IRecords {
  filterRow: { visible: boolean } = { visible: true };
  constructor(public temporary: boolean, public data: IRecordsData, public gridColumns: any[] = []) {
  }
  setRecordData(d: IRecordsData) {
    if (d.startIndex === 0) {
      this.data = d;
    } else {
      this.data.totalCount = d.totalCount;
      this.data.rows = this.data.rows.concat(d.rows);
    }
  }
  getFetchedRows() {
    return this.data.rows;
  }
}

export interface IResponseData {
  id?: number;
  regNumber?: string;
  message?: string;
  data?: any;
}

export interface ITemplateData extends IShareableObject {
  id?: number;
  dateCreated?: Date;
  username?: string;
  data?: string;
}

export class CTemplateData implements ITemplateData {
  constructor(
    public name: string = '',
    public description?: string,
    public isPublic?: boolean,
    public id?: number,
    dateCreated?:
      Date, username?: string,
    date?: string) { }
}
