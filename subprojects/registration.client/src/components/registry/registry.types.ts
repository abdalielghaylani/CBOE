import { IAppState } from '../../store';
import { FormGroupType, CFormGroup, CFormElement } from '../../common';

export class FragmentData {
  FragmentID: number;
  Code: number;
  FragmentType: string;
  Description: string;
  Structure: string;
  DateCreated: Date;
  DateLastModified: Date;
}

export const FRAGMENT_DESC_LIST = [{
  dataField: 'FragmentID',
  dataType: 'number',
  editorOptions: { disabled: true }
}, {
  dataField: 'Code',
  dataType: 'number',
  editorOptions: { disabled: true }
}, {
  dataField: 'FragmentType',
  dataType: 'string',
  editorOptions: { disabled: true }
}, {
  dataField: 'Structure',
  dataType: 'string',
  editorOptions: { disabled: true }
}, {
  dataField: 'DateCreated',
  dataType: 'date',
  editorType: 'dxDateBox',
}, {
  dataField: 'DateLastModified',
  label: 'Last Modified Date',
  dataType: 'date',
  editorType: 'dxDateBox',
}];

export class CParam {
  _name: String; // min
  _value: String; // 0
}

export class CParamList {
  param?: CParam[] = [];
}

export class CValidationRule {
  _validationRuleName: String; // textLength
  _errorMessage: String; // The property value can have between 0 and 200 characters
  params?: CParamList;
}

export class CValidationRuleList {
  validationRule: CValidationRule[] = [];
}

export class CProperty {
  _name: String; // REG_COMMENTS
  _friendlyName: String; // REG_COMMENTS
  _type: String; // TEXT
  _precision?: Number; // 200
  _sortOrder?: Number; // 0
  _pickListDomainID?: Number;
  _pickListDisplayValue?: String;
  validationRuleList?: CValidationRuleList;
  __text?: String;
}

export class CPropertyList {
  Property: CProperty[] = [];
}

export class CRegNumber {
  RegID?: Number; // 1826
  SequenceNumber?: Number; // 877
  RegNumber?: String; // RN-000877
  SequenceID?: Number; // 4
}

export class CIdentifierID {
  _Description?: String;
  _Name?: String;
  _Active?: String; // T
  __text?: String; // 4
}

export class CIdentifier {
  ID?: Number; // 2621
  IdentifierID?: CIdentifierID;
  InputText?: String; // Dehydrohedione (DHH)
}

export class CIdentifierVM {
  id?: Number;
  inputText?: String;
  constructor(m: CIdentifier) {
    this.id = m.IdentifierID ? +m.IdentifierID.__text : undefined;
    this.inputText = m.InputText;
  }
}

export class CIdentifierList {
  Identifier: CIdentifier[] = [];
}

export class CProjectID {
  _Description?: String; // Hedione Process Optimization
  _Name?: String; // Hedione Process Optimization
  _Active?: String; // T
  __text?: Number; // 2
}

export class CProject {
  ID?: Number; // 381
  ProjectID?: CProjectID;
}

export class CProjectList {
  Project: CProject[] = [];
}

export class CChemicalStructure {
  _molWeight?: Number; // 224.2961
  _formula?: String; // C13H20O3
  __text?: String; // VmpD...
}

export class CStructureData {
  StructureID?: Number; // 1821
  StructureFormat?: String;
  Structure?: CChemicalStructure;
  NormalizedStructure?: String; // VmpD...
  UseNormalization?: String; // F
  DrawingType?: Number; // 0
  CanPropogateStructureEdits?: String; // True
  PropertyList?: CPropertyList;
  IdentifierList?: CIdentifierList;
}

export class CBaseFragment {
  Structure: CStructureData;
}

export class CFragment {
  // Incomplete
}

export class CFragmentList {
  Fragment: CFragment[] = [new CFragment()];
}

export class CCompound {
  CompoundID?: Number; // 901
  DateCreated?: String; // 2017-01-15 08:21:51 pm
  PersonCreated?: Number; // 61
  PersonApproved?: Number; // 61
  PersonRegistered?: Number; // 61
  DateLastModified?: String; // 2017-01-15 08:38:18 pm
  Tag?: String; // P1
  PropertyList?: CPropertyList;
  RegNumber?: CRegNumber;
  CanPropogateComponentEdits?: String; // True
  FragmentList: CFragmentList = new CFragmentList();
  IdentifierList?: CIdentifierList;
}

export class CComponent {
  ID?: Number;
  ComponentIndex?: Number; // -901
  Compound: CCompound = new CCompound();
}

export class CComponentVM {
  id?: Number;
  componentIndex?: Number;
  compoundId?: Number;
  dateCreated?: Date;
  dateLastModified?: Date;
  personCreated?: Number;
  personApproved?: Number;
  regNumber?: String;
  identifierList?: CIdentifierVM[];
  columns: any[] = [];
  constructor(m: CComponent, state: IAppState) {
    this.id = m.ID;
    this.componentIndex = m.ComponentIndex;
    this.compoundId = m.Compound.CompoundID;
    this.dateCreated = m.Compound.DateCreated ? new Date(m.Compound.DateCreated) : undefined;
    this.dateLastModified = m.Compound.DateLastModified ? new Date(m.Compound.DateLastModified) : undefined;
    this.personCreated = m.Compound.PersonCreated;
    this.personApproved = m.Compound.PersonApproved;
    this.regNumber = m.Compound.RegNumber ? m.Compound.RegNumber.RegNumber : undefined;
    this.identifierList = m.Compound.IdentifierList ? m.Compound.IdentifierList.Identifier.map(i => new CIdentifierVM(i)) : undefined;
    // TODO: The built-in property view should also come from configuration
    this.columns.push({
      dataField: 'identifierList',
      label: { text: 'Component Identifiers' },
      colSpan: 2,
      template: function (d, itemElement) {
        (jQuery('<div>')
          .appendTo(itemElement) as any)
          .dxDataGrid({
            dataSource: d.editorOptions.value ? d.editorOptions.value : [],
            columns: [{
              dataField: 'id',
              caption: 'Identifier',
              editorType: 'dxSelectBox',
              lookup: {
                dataSource: state && state.session.lookups ? state.session.lookups.identifierTypes.filter(i => i.TYPE === 'C' && i.ACTIVE === 'T') : [],
                displayExpr: 'NAME',
                valueExpr: 'ID',
                placeholder: 'Select Identifier'
              }
            }, {
              dataField: 'inputText',
              caption: 'Value'
            }],
            editing: {
              mode: 'row',
              allowUpdating: true,
              allowDeleting: true,
              allowAdding: true
            },
            onCellPrepared: function (e) {
              onCellPrepared(e);
            }
          });
      },
    });    
    buildPropertyList(this, m.Compound.PropertyList, FormGroupType.SubmitMixture, 1001, state);
  }
}

export class CComponentList {
  Component: CComponent[] = [new CComponent()];
}

export class CBatchComponentFragmentList {
  // Incompoete
}

export class CBatchComponent {
  ID?: Number; // 1721
  BatchID?: Number; // 1741
  CompoundID?: Number; // 901
  MixtureComponentID?: Number; // 901
  ComponentIndex?: Number; // -901
  ComponentRegNum?: String; // C000885
  PropertyList?: CPropertyList;
  BatchComponentFragmentList?: CBatchComponentFragmentList;
}

export class CBatchComponentList {
  BatchComponent: CBatchComponent[] = [];
}

export class CPerson {
  _displayName?: String; // PMORIEUX
  __text?: String; // 61
}

export class CBatch {
  BatchID?: Number; // 1741
  BatchNumber?: Number; // 1
  FullRegNumber?: String; // RN-000877-001
  DateCreated?: String; // 2017-01-15
  PersonCreated?: CPerson;
  PersonApproved?: CPerson;
  PersonRegistered?: CPerson;
  DateLastModified?: String; // 2017-01-15
  StatusID?: Number; // 3
  IsBatchEditable?: String; // True
  ProjectList?: CProjectList;
  IdentifierList?: CIdentifierList;
  PropertyList?: CPropertyList;
  BatchComponentList?: CBatchComponentList;
}

export class CBatchVM {
  id?: Number;
  batchNumber?: Number;
  fullRegNumber?: String;
  dateCreated?: Date;
  dateLastModified?: Date;
  personCreated?: Number;
  personApproved?: Number;
  personRegistered?: Number;
  statusId?: Number;
  identifierList?: CIdentifierVM[] = [];
  projectList?: number[] = [];
  columns?: any[] = [];
  constructor(m: CBatch, state: IAppState) {
    this.id = m.BatchID;
    this.batchNumber = m.BatchNumber;
    this.fullRegNumber = m.FullRegNumber;
    this.dateCreated = m.DateCreated ? new Date(m.DateCreated) : undefined;
    this.dateLastModified = m.DateLastModified ? new Date(m.DateLastModified) : undefined;
    this.personCreated = m.PersonCreated ? +m.PersonCreated.__text : undefined;
    this.personApproved = m.PersonApproved ? +m.PersonApproved.__text : undefined;
    this.personRegistered = m.PersonRegistered ? +m.PersonRegistered.__text : undefined;
    this.statusId = m.StatusID;
    this.identifierList = m.IdentifierList ? m.IdentifierList.Identifier.map(i => new CIdentifierVM(i)) : undefined;
    this.columns.push({
      dataField: 'id',
      dataType: 'number',
      label: { text: 'Batch ID' },
      editorOptions: { disabled: true }
    });
    this.columns.push({
      dataField: 'dateCreated',
      dataType: 'date',
      label: { text: 'Date Created' },
      editorOptions: { disabled: true }
    });
    this.columns.push({
      dataField: 'dateLastModified',
      dataType: 'date',
      label: { text: 'Last Modification Date' },
      editorOptions: { disabled: true }
    });
    buildPropertyList(this, m.PropertyList, FormGroupType.SubmitMixture, 1002, state);
    if (m.ProjectList) {
      this.projectList = [];
      m.ProjectList.Project.forEach(p => this.projectList.push(+p.ProjectID));
    }
  }
}

export class CBatchList {
  Batch: CBatch[] = [];
}

export class CEvent {
  _eventName?: String; // Inserting
  _eventHandler?: String; // OnInsertHandler
}

export class CAddIn {
  _assembly?: String; // CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc
  _class?: String; // CambridgeSoft.COE.Registration.Services.RegistrationAddins.NormalizedStructureAddIn
  _friendlyName?: String; // Structure Normalization
  _required?: String; // no
  _enabled?: String; // no
  Event?: CEvent[];
  AddInConfiguration?: any;
  // <AddInConfiguration>
  //   <ScriptFile>C:\Program Files\CambridgeSoft\ChemOfficeEnterprise12.1.0.0\Registration\PythonScripts\parentscript.py</ScriptFile>
  //   <!--Commented <PythonWebServiceURL> to bypass soap
  // <PythonWebServiceURL>http://localhost/PyEngine/Service.asmx</PythonWebServiceURL> -->
  //   <StructuresIdsToAvoid>-2|-3|</StructuresIdsToAvoid>
  // </AddInConfiguration>
}

export class CAddInList {
  AddIn: CAddIn[] = [];
}

export class CRegistryRecord {
  _SameBatchesIdentity?: String; // True
  _ActiveRLS?: String; // Off
  _IsEditable?: String; // True
  _IsRegistryDeleteable?: String; // True
  ID?: Number; // 921
  DateCreated?: String; // 2017-01-15 08:21:50 PM
  DateLastModified?: String; //  2017-01-15 08:38:18 PM
  PersonCreated?: Number; // 61
  PersonApproved?: Number; //
  StructureAggregation?: String; // VmpD...
  StatusID?: Number; // 3
  PropertyList?: CPropertyList;
  RegNumber?: CRegNumber;
  IdentifierList?: CIdentifierList;
  ProjectList?: CProjectList;
  ComponentList: CComponentList = new CComponentList();
  BatchList: CBatchList = new CBatchList();
  AddIns?: CAddInList;
  constructor() {
    this.ComponentList.Component.push(new CComponent());
    this.BatchList.Batch.push(new CBatch());
  }
}

export class CRegistryRecordVM {
  sameBatchesIdentity?: Boolean;
  activeRLS?: String;
  isEditable?: Boolean;
  isRegistryDeleteable?: Boolean;
  id?: Number;
  dateCreated?: Date;
  dateLastModified?: Date;
  personCreated?: Number;
  personApproved?: Number;
  structureAggregation?: String;
  statusId?: Number;
  regNumber?: String;
  identifierList?: CIdentifierVM[] = [];
  projectList?: number[] = [];
  componentList?: CComponentVM[] = [];
  batchList: CBatchVM[] = [];
  columns: any[] = [];
  constructor(m: CRegistryRecord, state: IAppState) {
    this.sameBatchesIdentity = m._SameBatchesIdentity ? m._SameBatchesIdentity === 'True' : undefined;
    this.activeRLS = m._ActiveRLS;
    this.isEditable = m._IsEditable ? m._IsEditable === 'True' : undefined;
    this.isRegistryDeleteable = m._IsRegistryDeleteable ? m._IsRegistryDeleteable === 'True' : undefined;
    this.id = m.ID;
    this.dateCreated = m.DateCreated ? new Date(m.DateCreated) : undefined;
    this.dateLastModified = m.DateLastModified ? new Date(m.DateLastModified) : undefined;
    this.personCreated = m.PersonCreated;
    this.personApproved = m.PersonApproved;
    this.structureAggregation = m.StructureAggregation;
    this.statusId = m.StatusID;
    this.regNumber = m.RegNumber ? m.RegNumber.RegNumber : undefined;
    this.identifierList = m.IdentifierList ? m.IdentifierList.Identifier.map(i => new CIdentifierVM(i)) : undefined;
    if (m.ProjectList) {
      this.projectList = [];
      m.ProjectList.Project.forEach(p => this.projectList.push(+p.ProjectID));
    }
    this.columns.push({
      dataField: 'projectList',
      label: { text: 'Projects' },
      colSpan: 2,
      template: function (d, itemElement) {
        (jQuery('<div>')
          .appendTo(itemElement) as any)
          .dxTagBox({
            value: d.editorOptions.value ? d.editorOptions.value : [],
            valueExpr: 'PROJECTID',
            displayExpr: 'NAME',
            dataSource: state && state.session.lookups ? state.session.lookups.projects : [],
            onValueChanged: function (e) {
              d.component.option('formData.' + d.dataField, e.value);
            }
          });
      }
    });
    this.columns.push({
      dataField: 'identifierList',
      label: { text: 'Registry Identifiers' },
      colSpan: 2,
      template: function (d, itemElement) {
        (jQuery('<div>')
          .appendTo(itemElement) as any)
          .dxDataGrid({
            dataSource: d.editorOptions.value ? d.editorOptions.value : [],
            columns: [{
              dataField: 'id',
              caption: 'Identifier',
              editorType: 'dxSelectBox',
              lookup: {
                dataSource: state && state.session.lookups ? state.session.lookups.identifierTypes.filter(i => i.TYPE === 'R' && i.ACTIVE === 'T') : [],
                displayExpr: 'NAME',
                valueExpr: 'ID',
                placeholder: 'Select Identifier'
              }
            }, {
              dataField: 'inputText',
              caption: 'Value'
            }],
            editing: {
              mode: 'row',
              allowUpdating: true,
              allowDeleting: true,
              allowAdding: true
            },
            onCellPrepared: function (e) {
              onCellPrepared(e);
            }
          });
      },
    });
    buildPropertyList(this, m.PropertyList, FormGroupType.SubmitMixture, 1000, state);
    if (m.ComponentList) {
      this.componentList = m.ComponentList.Component.map(c => new CComponentVM(c, state));
    }
    if (m.BatchList) {
      this.batchList = m.BatchList.Batch.map(b => new CBatchVM(b, state));
    }
  }
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

function getPicklist(domainId: number, formElement: CFormElement, state: IAppState) {
  let pickListDomains = state.session.lookups.pickListDomains;
  let placeholder = 'Select ' + (formElement ? formElement.label : '...');
  let pickList = {
    dataSource: [],
    displayExpr: 'NAME',
    valueExpr: 'ID',
    placeholder: placeholder,
    showClearButton: true
  };
  let filtered = pickListDomains.filter(d => d.ID === domainId);
  if (filtered.length === 1) {
    let pickListDomain = filtered[0];
    let extTable = pickListDomain.EXT_TABLE ? pickListDomain.EXT_TABLE.toUpperCase() : null;
    // TODO: Should support external tables
    if (extTable && extTable.indexOf('REGDB.') === 0) {
      let lookup = extTable.replace('REGDB.', '');
      // TODO: Should support all internal tables geneticall
      if (lookup === 'VW_UNIT') {
        pickList.dataSource = state && state.session.lookups ? state.session.lookups.units : [];
      } else if (lookup === 'VW_PEOPLE') {
        pickList.dataSource = state && state.session.lookups ? state.session.lookups.users : [];
      } else if (lookup === 'VW_NOTEBOOKS') {
        pickList.dataSource = state && state.session.lookups ? state.session.lookups.notebooks : [];
      } else {
        pickList.dataSource = [];
      }
      // TODO: Should apply filter and sort order
      // EXT_SQL_FILTER: Where active='T'
      // EXT_SQL_SORTORDER: ORDER BY SORTORDER ASC
      pickList.displayExpr = pickListDomain.EXT_DISPLAY_COL;
      pickList.valueExpr = pickListDomain.EXT_ID_COL;
    }
  }
  switch (domainId) {
    case 3:
      pickList.dataSource = state && state.session.lookups ? state.session.lookups.users : [];
      pickList.displayExpr = 'USERID';
      pickList.valueExpr = 'PERSONID';
      break;
  }
  return pickList;
}

function getPropertyColumn(p: CProperty, formElements: CFormElement[], state: IAppState): any {
  let filtered = formElements.filter(fe => fe._name === p._name);
  let formElement = filtered && filtered.length > 0 ? filtered[0] : null;
  if (formElement && formElement.displayInfo.visible === 'false') {
    return null;
  }
  let column: any = {
    dataField: p._name,
    dataType: p._type === 'DATE' ? 'date' : 'string'
  };
  let label: any = {};
  if (formElement) {
    label.text = formElement.label;
    column.label = label;
  }
  if (p._type === 'PICKLISTDOMAIN') {
    column.dataType = 'number';
    column.editorType = 'dxSelectBox';
    column.editorOptions = getPicklist(+p._pickListDomainID, formElement, state);
  }
  // if (p._precision > 100) {
  //   column.colSpan = 2;
  //   column.editorType = 'dxTextArea';
  // }
  return column;
}

function getPropertyValue(p: CProperty): any {
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

function buildPropertyList(vm: any, propertyList: CPropertyList, formGroupType: FormGroupType, coeFormId: number, state: IAppState) {
  if (propertyList) {
    let formGroup = state ? state.configuration.formGroups[FormGroupType[formGroupType]] as CFormGroup : null;
    let formElements = formGroup ? formGroup.detailsForms.detailsForm[0].coeForms.coeForm.filter(f => +f._id === coeFormId)[0].editMode.formElement : [];
    propertyList.Property.forEach(p => {
      let propertyName = p._name;
      if (propertyName) {
        vm[propertyName as string] = getPropertyValue(p);
        let column = getPropertyColumn(p, formElements, state);
        if (column) {
          vm.columns.push(column);
        }
      }
    });
  }
}

