import { IAppState } from '../../store';
import { apiUrlPrefix } from '../../configuration';
import {
  IShareableObject, FormGroupType, SubFormType, CFormGroup, CCoeForm, CFormElement, IFormContainer
} from '../../common';
import { IRecordsData, IRecords } from '../../store';

export enum RegistryStatus {
  NotSet,
  Submitted,
  Approved,
  Registered,
  Locked
}

export class FragmentData {
  FragmentID: number;
  Code: string;
  FragmentType: string;
  Description: string;
  Structure: string;
  DateCreated: Date;
  DateLastModified: Date;
}

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
  FragmentID?: Number;
  ComponentFragmentID?: Number;
  Code?: String;
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
  fragmentList?: CFragment[];
  batchComponentFragmentList?: CBatchComponentFragmentVM[] = [];
  columns: any[] = [];
  constructor(m: CComponent, container: IFormContainer) {
    let lookups = getLookups(container);
    this.id = m.ID;
    this.componentIndex = m.ComponentIndex;
    this.compoundId = m.Compound.CompoundID;
    this.dateCreated = m.Compound.DateCreated ? new Date(m.Compound.DateCreated) : undefined;
    this.dateLastModified = m.Compound.DateLastModified ? new Date(m.Compound.DateLastModified) : undefined;
    this.personCreated = m.Compound.PersonCreated;
    this.personApproved = m.Compound.PersonApproved;
    this.regNumber = m.Compound.RegNumber ? m.Compound.RegNumber.RegNumber : undefined;
    this.identifierList = m.Compound.IdentifierList ? m.Compound.IdentifierList.Identifier.map(i => new CIdentifierVM(i)) : undefined;
    this.fragmentList = m.Compound.FragmentList.Fragment;
    // TODO: The built-in property view should also come from configuration
    this.columns.push({
      dataField: 'identifierList',
      label: { text: 'Component Identifiers' },
      colSpan: 2,
      template: function (d, itemElement) {
        (jQuery('<div>')
          .appendTo(itemElement) as any)
          .dxDataGrid({
            disabled: !container.editMode,
            dataSource: d.editorOptions.value ? d.editorOptions.value : [],
            columns: [{
              dataField: 'id',
              caption: 'Identifier',
              editorType: 'dxSelectBox',
              lookup: {
                dataSource: lookups ? lookups.identifierTypes.filter(i => i.TYPE === 'C' && i.ACTIVE === 'T') : [],
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
    this.columns = [{
      itemType: 'group',
      caption: undefined,
      items: []
    }];
    let coeForm = getCoeFormById(container.formGroup, SubFormType.CompoundCustomProperties);
    if (coeForm) {
      let groupItem = this.columns[0];
      buildPropertyList(this, groupItem.items, m.Compound.PropertyList, coeForm, container);
      // if (coeForm.title) {
      //   groupItem.caption = coeForm.title as string;
      // }
    }
  }
}

export class CComponentList {
  Component: CComponent[] = [new CComponent()];
}

export class CBatchComponentFragment {
  ID?: Number;
  FragmentID?: Number;
  ComponentFragmentID?: Number;
  Equivalents?: Number;
  OrderIndex?: Number;
}

export class CBatchComponentFragmentVM {
  batch: CBatchVM;
  component: CComponentVM;
  fragment: any;
  equivalents?: Number;
  fragmentId?: Number;
  fragmentTypeId?: Number;
  code?: String;
  structure?: String;
  description?: String;
  molWeight?: Number;
  formula?: String;
  columns?: any[] = [];
  constructor(
    m: CBatchComponentFragment,
    batch: CBatchVM,
    components: CComponentVM[],
    container: IFormContainer) {
    let lookups = getLookups(container);
    this.batch = batch;
    this.component = components.length === 1
      ? components[0]
      : components.find(cvm => !cvm.fragmentList.find(f => +f.ComponentFragmentID === +m.ComponentFragmentID));
    this.fragment = lookups ? lookups.fragments.find(f => +f.FRAGMENTID === +m.FragmentID) : {};
    this.fragmentId = this.fragment.FRAGMENTID;
    this.fragmentTypeId = this.fragment.FRAGMENTTYPEID;
    this.code = this.fragment.CODE;
    this.structure = this.fragment.STRUCTURE;
    this.description = this.fragment.DESCRIPTION;
    this.molWeight = this.fragment.MOLWEIGHT;
    this.formula = this.fragment.FORMULA;
    this.equivalents = +m.Equivalents;
  }
}

export class CBatchComponentFragmentList {
  BatchComponentFragment: CBatchComponentFragment[] = [];
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
  batchComponentFragmentList?: CBatchComponentFragmentVM[] = [];
  columns?: any[] = [];
  constructor(m: CBatch, container: IFormContainer) {
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
    this.columns = [{
      itemType: 'group',
      caption: undefined,
      colCount: 'auto',
      colCountByScreen: {
        lg: 3,
        md: 2,
        sm: 1,
        xs: 1
      },
      items: []
    }];
    let groupItem = this.columns[0];
    groupItem.items.push({
      dataField: 'id',
      dataType: 'number',
      label: { text: 'Batch ID' },
      editorOptions: { disabled: true }
    });
    groupItem.items.push({
      dataField: 'dateCreated',
      dataType: 'date',
      label: { text: 'Date Created' },
      editorOptions: { disabled: true }
    });
    groupItem.items.push({
      dataField: 'dateLastModified',
      dataType: 'date',
      label: { text: 'Last Modification Date' },
      editorOptions: { disabled: true }
    });
    buildPropertyList(this, groupItem.items, m.PropertyList, getCoeFormById(container.formGroup, SubFormType.BatchCustomProperties), container);
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
  constructor(m: CRegistryRecord, container: IFormContainer) {
    let lookups = getLookups(container);
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
            disabled: !container.editMode,
            value: d.editorOptions.value ? d.editorOptions.value : [],
            valueExpr: 'PROJECTID',
            displayExpr: 'NAME',
            dataSource: lookups ? lookups.projects : [],
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
            disabled: !container.editMode,
            dataSource: d.editorOptions.value ? d.editorOptions.value : [],
            columns: [{
              dataField: 'id',
              caption: 'Identifier',
              editorType: 'dxSelectBox',
              lookup: {
                dataSource: lookups ? lookups.identifierTypes.filter(i => i.TYPE === 'R' && i.ACTIVE === 'T') : [],
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
            },
            onContentReady: function (e) {
              let grid = e.component;
              if (grid.totalCount() === 0) {
                grid.option('height', 100);
              } else {
                grid.option('height', 'auto');                
              }
            }
          });
      },
    });
    let batchComponentFragmentList: CBatchComponentFragmentVM[] = [];
    buildPropertyList(this, this.columns, m.PropertyList, getCoeFormById(container.formGroup, SubFormType.RegistryCustomProperties), container);
    if (m.ComponentList) {
      this.componentList = m.ComponentList.Component.map(c => new CComponentVM(c, container));
    }
    if (m.BatchList) {
      this.batchList = m.BatchList.Batch.map(b => new CBatchVM(b, container));
      // Build the batch component fragment list
      // A batch component fragment is associated with a batch
      // and points to component fragment, and fragment id.
      // From this one should extract:
      // 1. Batch ID
      // 2. Component
      // 3. Fragment ID and all other fragment data from Fragments tables
      // 4. Equivalents
      m.BatchList.Batch.forEach(b => {
        if (batchComponentFragmentList && b.BatchComponentList) {
          b.BatchComponentList.BatchComponent.forEach(bc => {
            if (bc.BatchComponentFragmentList) {
              bc.BatchComponentFragmentList.BatchComponentFragment.forEach(bcf =>
                batchComponentFragmentList.push(
                  new CBatchComponentFragmentVM(
                    bcf,
                    this.batchList.find(bvm => bvm.id === b.BatchID),
                    this.componentList, container)
                )
              );
            }
          });
        }
      });
    }

    // Depending on the SBI setting of SBI, add the list to component or batch
    if (m._SameBatchesIdentity && m._SameBatchesIdentity === 'True') {
      this.componentList.forEach(cvm => {
        cvm.batchComponentFragmentList = batchComponentFragmentList.filter(bcf => bcf.component === cvm);
        cvm.columns.push(buildBatchCompoundFragmentGroup(container));
      });
    } else {
      this.batchList.forEach(bvm => {
        bvm.batchComponentFragmentList = batchComponentFragmentList.filter(bcf => bcf.batch === bvm);
        bvm.columns.push(buildBatchCompoundFragmentGroup(container));
      });
    }
  }
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
      template: function (d, itemElement) {
        (jQuery('<div />')
          .appendTo(itemElement) as any)
          .dxDataGrid({
            disabled: !container.editMode,
            dataSource: d.editorOptions.value ? d.editorOptions.value : [],
            columns: [{
              dataField: 'fragmentTypeId',
              caption: 'Type',
              editorType: 'dxSelectBox',
              lookup: {
                dataSource: lookups ? lookups.fragmentTypes : [],
                displayExpr: 'DESCRIPTION',
                valueExpr: 'ID'
              },
              width: 60
            }, {
              dataField: 'equivalents',
              caption: 'Equivalent',
              width: 80
            }, {
              dataField: 'code',
              caption: 'Code',
              width: 50
            }, {
              dataField: 'structure',
              caption: 'Structure',
              allowEditing: false,
              allowFiltering: false,
              allowSorting: false,
              width: 150,
              cellTemplate: function (c, o) {
                jQuery(`<img src="${apiUrlPrefix}StructureImage/${o.data.structure}" />`).appendTo(c);
              }
            }, {
              dataField: 'description',
              caption: 'Description',
              allowEditing: false
            }, {
              dataField: 'molWeight',
              caption: 'MW',
              allowEditing: false
            }, {
              dataField: 'formula',
              caption: 'MF',
              allowEditing: false
            }],
            editing: {
              mode: 'row',
              allowUpdating: true,
              allowDeleting: true,
              allowAdding: true
            },
            onCellPrepared: function (e) {
              onCellPrepared(e);
            },
            onContentReady: function (e) {
              let grid = e.component;
              if (grid.totalCount() === 0) {
                grid.option('height', 100);
              } else {
                grid.option('height', 'auto');                
              }
            }
          });
      }
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

function getPicklist(domainId: number, formElement: CFormElement, container: IFormContainer) {
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
  let filtered = pickListDomains.filter(d => d.ID === domainId);
  if (filtered.length === 1) {
    let pickListDomain = filtered[0];
    let extTable = pickListDomain.EXT_TABLE ? pickListDomain.EXT_TABLE.toUpperCase() : null;
    // TODO: Should support external tables
    if (extTable && extTable.indexOf('REGDB.') === 0) {
      let lookup = extTable.replace('REGDB.', '');
      // TODO: Should support all internal tables geneticall
      if (lookup === 'VW_UNIT') {
        pickList.dataSource = lookups ? lookups.units : [];
      } else if (lookup === 'VW_PEOPLE') {
        pickList.dataSource = lookups ? lookups.users : [];
      } else if (lookup === 'VW_NOTEBOOKS') {
        pickList.dataSource = lookups ? lookups.notebooks : [];
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
  return pickList;
}

function getPropertyColumn(p: CProperty, formElements: CFormElement[], container: IFormContainer): any {
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
    column.editorOptions = getPicklist(+p._pickListDomainID, formElement, container);
  }
  // if (p._precision > 100) {
  //   column.colSpan = 2;
  //   column.editorType = 'dxTextArea';
  // }
  return column;
}

function getLookups(container: IFormContainer): any {
  let state: IAppState = container.ngRedux.getState();
  return state.session ? state.session.lookups : undefined;
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

function getCoeFormById(formGroup: CFormGroup, coeFormId: number) {
  let filtered = formGroup ? formGroup.detailsForms.detailsForm[0].coeForms.coeForm.filter(f => +f._id === coeFormId) : undefined;
  return filtered && filtered.length > 0 ? filtered[0] : null;
}

/**
 * Adds the custom properties to the given view-model object, and
 * adds the property column description into the given column array. 
 */
function buildPropertyList(vm: any, columns: any[], propertyList: CPropertyList, coeForm: CCoeForm, container: IFormContainer) {
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

export interface IRecordSaveData {
  temporary: boolean;
  id: number;
  recordDoc: Document;
  saveToPermanent: boolean;
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
