import { EventEmitter } from '@angular/core';
import { ISearchCriteriaItem, getSearchCriteriaValue } from './registry-search-base.types';
import { IFormGroup, IForm, ICoeForm, ICoeFormMode, IFormElement } from '../../../common';

export interface IViewControl {
  activated: boolean;
  editMode: boolean;
  viewModel: any;
  viewConfig: any;
  valueUpdated: EventEmitter<any>;
}

export interface IFormItemTemplate extends IViewControl {
  deserializeValue(value: any): any;
  serializeValue(value: any): any;
}

export interface IViewGroup {
  id: string;
  data: ICoeForm[];
}

class CEntryInfo {
  dataSource: string;
  bindingExpression: string;
  searchCriteriaItem?: ISearchCriteriaItem;
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

  private getEntryInfo(displayMode: string, id: string): CEntryInfo {
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

  private getDataSource(dataSource: string, viewModel: IRegistryRecord): any {
    dataSource = dataSource.toLowerCase();
    let data = dataSource.indexOf('component') >= 0 ? viewModel.ComponentList.Component[0]
      : dataSource.indexOf('batch') >= 0 ? viewModel.BatchList.Batch[0]
        : viewModel;
  }

  private getQueryEntryValue(searchCriteriaItem: ISearchCriteriaItem): string {
    let value;
    let searchCriteriaValue: any = getSearchCriteriaValue(searchCriteriaItem);
    if (searchCriteriaValue && searchCriteriaValue.CSCartridgeStructureCriteria && searchCriteriaValue.CSCartridgeStructureCriteria.__text) {
      value = searchCriteriaValue.CSCartridgeStructureCriteria.__text;
    } else if (searchCriteriaValue.__text) {
      value = searchCriteriaValue.__text;
    }
    return value;
  }

  private getEntryValue(displayMode: string, id: string, viewModel: IRegistryRecord): any {
    let entryInfo = this.getEntryInfo(displayMode, id);
    return displayMode === 'query'
      ? this.getQueryEntryValue(entryInfo.searchCriteriaItem)
      : this.parseEntryValue(entryInfo.bindingExpression, this.getDataSource(entryInfo.dataSource, viewModel));
  }

  private serializeValue(object: any, property: string) {
    let textObject = object[property];
    if (textObject && typeof textObject === 'object' && textObject.viewModel) {
      object[property] = object[property].toString();
    }
  }

  /**
   * Parses the binding expression, and sets the bound object value
   * For non-query form-group data only
   * @param bindingExpression The path expression of the bound object
   * @param viewModel The view-model object that contains the object to set the value
   * @param value The value to set
   * @param serialize The indicator whether or not the value must be rendered into string value
   */
  private parseAndSetEntryValue(bindingExpression: string, viewModel: any, value, serialize: boolean = false) {
    bindingExpression = this.fixBindingExpression(bindingExpression);
    let objectNames = bindingExpression.split('.');
    let nextObject = viewModel;
    objectNames.forEach(n => {
      if (nextObject) {
        let m = n.match(/PropertyList\[@Name='(.*)'/);
        if (m && m.length > 1) {
          let propertyList = nextObject.PropertyList as IPropertyList;
          let p = propertyList.Property.filter(p => p._name === m[1]);
          if (p) {
            if (serialize) {
              this.serializeValue(p[0], '__text');
            } else {
              p[0].__text = value;
            }
          } else {
            propertyList.Property.push({ _name: m[1], __text: value });
          }
          nextObject = p ? p[0].__text : undefined;
        } else {
          if (n === objectNames[objectNames.length - 1]) {
            if (serialize) {
              this.serializeValue(nextObject, n);
            } else {
              nextObject[n] = value;
            }
          } else {
            if (!nextObject[n]) {
              nextObject[n] = {};
            } else {
              nextObject = nextObject[n];
            }
          }
        }
      }
    });
  }

  private setEntryValue(displayMode: string, id: string, viewModel: IRegistryRecord, entryValue, serialize: boolean = false) {
    let entryInfo = this.getEntryInfo(displayMode, id);
    if (displayMode === 'query') {
      this.parseAndSetEntryValue(entryInfo.bindingExpression, viewModel, entryInfo.searchCriteriaItem, serialize);
    } else {
      let dataSource = this.getDataSource(entryInfo.dataSource, viewModel);
      this.parseAndSetEntryValue(entryInfo.bindingExpression, dataSource, entryValue, serialize);
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
          : undefined;
    return pageId ? disabledControls.filter(dc => dc.pageId === pageId) : [];
  }

  private static sortForms(forms: ICoeForm[]): ICoeForm[] {
    // The form aray sometimes is not sorted property.
    // Registry should go first.
    // For now, doc-manager and inventory integration forms are removed.
    let sorted: ICoeForm[] = [];
    forms.forEach(f => {
      let dataSource = f._dataSourceId ? f._dataSourceId.toLowerCase() : '';
      if (dataSource.startsWith('mixture')) {
        sorted.push(f);
      }
    });
    forms.forEach(f => {
      let dataSource = f._dataSourceId ? f._dataSourceId.toLowerCase() : '';
      if (!dataSource.startsWith('mixture') && !dataSource.startsWith('docmgr') && !dataSource.startsWith('inv')) {
        sorted.push(f);
      }
    });
    return sorted;
  }

  public static getViewGroups(config: IFormGroup, displayMode: string, disabledControls: any[]): CViewGroup[] {
    let viewGroups: CViewGroup[] = [];
    let viewGroupsFiltered: CViewGroup[] = [];
    let form: IForm = this.getForm(config, displayMode);
    if (form) {
      let disabledControlsFiltered = this.getFilteredDisabledControls(displayMode, disabledControls);
      let coeForms = this.sortForms(form.coeForms.coeForm);
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
            this.removeDuplicate(items, item);
            items.push(item);
          }
        });
      }
    });
    return this.fixColSpans(items);
  }

  public getFormData(displayMode: string, idList: string[], viewModel: IRegistryRecord): any {
    let formData: any = {};
    idList.forEach(id => {
      formData[id] = this.getEntryValue(displayMode, id, viewModel);
    });
    return formData;
  }

  public readFormData(displayMode: string, idList: string[], viewModel: IRegistryRecord, formData: any) {
    idList.forEach(id => {
      this.setEntryValue(displayMode, id, viewModel, formData[id]);
    });
  }

  public serializeFormData(displayMode: string, idList: string[], viewModel: IRegistryRecord) {
    idList.forEach(id => {
      this.setEntryValue(displayMode, id, viewModel, null, true);
    });
  }
}

export interface IParam {
  _name: string; // min
  _value: string; // 0
}

export class CParamList {
  param?: IParam[];
}

export interface IParamList extends CParamList {
}

export interface IValidationRule {
  _validationRuleName: string; // textLength
  _errorMessage: string; // The property value can have between 0 and 200 characters
  params?: IParamList;
}

export class CValidationRuleList {
  validationRule: IValidationRule[] = [];
}

export interface IValidationRuleList extends CValidationRuleList {
}

export interface IProperty {
  _name: string; // REG_COMMENTS
  _friendlyName?: string; // REG_COMMENTS
  _type?: string; // TEXT
  _precision?: string; // 200
  _sortOrder?: string; // 0
  _pickListDomainID?: string; // number
  _pickListDisplayValue?: string;
  validationRuleList?: IValidationRuleList;
  __text?: string;
}

export interface IPropertyList {
  Property: IProperty[];
}

export interface IRegNumber {
  RegID?: string; // 1826
  SequenceNumber?: string; // 877
  RegNumber?: string; // RN-000877
  SequenceID?: string; // 4
}

export interface IIdentifierID {
  _Description?: string;
  _Name?: string;
  _Active?: string; // T
  __text?: string; // 4
}

export interface IIdentifier {
  OrderIndex?: string; // 2621
  IdentifierID?: IIdentifierID;
  InputText?: string; // Dehydrohedione (DHH)
}

export class CIdentifierList {
  Identifier: IIdentifier[] = [];
}

export interface IIdentifierList extends CIdentifierList {
}

export interface IProjectID {
  _Description?: string; // Hedione Process Optimization
  _Name?: string; // Hedione Process Optimization
  _Active?: string; // T
  __text?: string; // 2
}

export interface IProject {
  ID?: string; // 381
  ProjectID?: IProjectID;
}

export class CProjectList {
  Project: IProject[] = [];
}

export interface IProjectList extends CProjectList {
}

export interface IChemicalStructure {
  _molWeight?: string; // 224.2961
  _formula?: string; // C13H20O3
  __text?: string; // VmpD...
}

export class IStructureData {
  StructureID?: string; // 1821
  StructureFormat?: string;
  Structure?: IChemicalStructure;
  NormalizedStructure?: string; // VmpD...
  UseNormalization?: string; // F
  DrawingType?: string; // 0
  CanPropogateStructureEdits?: string; // True
  PropertyList?: IPropertyList;
  IdentifierList?: IIdentifierList;
}

export interface IBaseFragment {
  Structure: IStructureData;
}

export class CFragment {
  FragmentID?: string; // number
  ComponentFragmentID?: string;
  Code?: string;
  FragmentType?: string;
  Description?: string;
  Structure?: string;
  DateCreated?: string; // date
  DateLastModified?: string; // date
}

export interface IFragment extends CFragment {
}

export class CFragmentList {
  Fragment: IFragment[] = [new CFragment()];
}

export interface IFragmentList extends CFragmentList {
}

export class CCompound {
  CompoundID?: string; // 901
  DateCreated?: string; // 2017-01-15 08:21:51 pm
  PersonCreated?: string; // 61
  PersonApproved?: string; // 61
  PersonRegistered?: string; // 61
  DateLastModified?: string; // 2017-01-15 08:38:18 pm
  Tag?: string; // P1
  PropertyList?: IPropertyList;
  RegNumber?: IRegNumber;
  CanPropogateComponentEdits?: string; // True
  FragmentList: IFragmentList = new CFragmentList();
  IdentifierList?: IIdentifierList;
}

export interface ICompound extends CCompound {
}

export class CComponent {
  ID?: string;
  ComponentIndex?: string; // -901
  Compound: ICompound = new CCompound();
}

export interface IComponent extends CComponent {
}

export class CComponentList {
  Component: IComponent[] = [];
}

export interface IComponentList extends CComponentList {
}

export interface IBatchComponentFragment {
  ID?: string; // number
  FragmentID?: string; // number
  ComponentFragmentID?: string; // number
  Equivalents?: string; // number
  OrderIndex?: string; // number
}

export class CBatchComponentFragmentList {
  BatchComponentFragment: IBatchComponentFragment[] = [];
}

export interface IBatchComponentFragmentList extends CBatchComponentFragmentList {
}

export interface IBatchComponent {
  ID?: string; // 1721
  BatchID?: string; // 1741
  CompoundID?: string; // 901
  MixtureComponentID?: string; // 901
  ComponentIndex?: string; // -901
  ComponentRegNum?: string; // C000885
  PropertyList?: IPropertyList;
  BatchComponentFragmentList?: IBatchComponentFragmentList;
}

export class CBatchComponentList {
  BatchComponent: IBatchComponent[] = [];
}

export interface IBatchComponentList extends CBatchComponentList {
}

export interface IPerson {
  _displayName?: string; // PMORIEUX
  __text?: string; // 61
}

export class CBatch {
  BatchID?: string; // 1741
  BatchNumber?: string; // 1
  FullRegNumber?: string; // RN-000877-001
  DateCreated?: string; // 2017-01-15
  PersonCreated?: IPerson;
  PersonApproved?: IPerson;
  PersonRegistered?: IPerson;
  DateLastModified?: string; // 2017-01-15
  StatusID?: string; // 3
  IsBatchEditable?: string; // True
  ProjectList?: IProjectList;
  IdentifierList?: IIdentifierList;
  PropertyList?: IPropertyList;
  BatchComponentList?: IBatchComponentList;
}

export interface IBatch extends CBatch {
}

export class CBatchList {
  Batch: IBatch[] = [];
}

export interface IBatchList extends CBatchList {
}

export interface IEvent {
  _eventName?: string; // Inserting
  _eventHandler?: string; // OnInsertHandler
}

export interface IAddIn {
  _assembly?: string; // CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc
  _class?: string; // CambridgeSoft.COE.Registration.Services.RegistrationAddins.NormalizedStructureAddIn
  _friendlyName?: string; // Structure Normalization
  _required?: string; // no
  _enabled?: string; // no
  Event?: IEvent[];
  AddInConfiguration?: any;
  // <AddInConfiguration>
  //   <ScriptFile>C:\Program Files\CambridgeSoft\ChemOfficeEnterprise12.1.0.0\Registration\PythonScripts\parentscript.py</ScriptFile>
  //   <!--Commented <PythonWebServiceURL> to bypass soap
  // <PythonWebServiceURL>http://localhost/PyEngine/Service.asmx</PythonWebServiceURL> -->
  //   <StructuresIdsToAvoid>-2|-3|</StructuresIdsToAvoid>
  // </AddInConfiguration>
}

export class CAddInList {
  AddIn: IAddIn[] = [];
}

export interface IAddInList extends CAddInList {
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
}

export interface IRegistryRecord extends CRegistryRecord {
}
