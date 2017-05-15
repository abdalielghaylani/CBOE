import { IAppState } from '../../store';
import { FormGroupType, CFormGroup, CFormElement } from '../../common';

export class CRegSearch {
  ID?: Number; // 921
  PersonCreated?: String;
  RegNumber?: String;
  STARTDATECREATED?: String;
  ENDDATECREATED?: String;
  SeqNumber?: String;
  PREFIX?: String;
  REGISTRY_PROJECT?: String;
  MolWeight?: String;
  MF?: String;
  IDENTIFIERTYPE?: String;
  IdentifierValue?: String;
  REG_COMMENTS?: String;
  SDF_upload?: String;
  PropertyList?: CPropertyList;
  ProjectList?: CProjectList;
  searchTypeValue?: String;
}

export class CRegSearchVM {
  sdfFile?: File;
  title?: String;
  columns: any[] = [];
  structureData: any;
  searchTypeItems: any[] = ['Substructure', 'Full Structure', 'Exact', 'Similarity'];
  constructor(m: CRegSearch, state: IAppState) {
    let coeForm = getCoeFormById(state, 0);
    if (coeForm) {
      if (coeForm.title) {
        this.title = coeForm.title;
      }
      buildPropertyList(this, coeForm.layoutInfo.formElement, state);
    }
  }
}

function buildPropertyList(vm: any, formElement: any, state: IAppState) {
  if (formElement) {
    formElement.forEach(p => {
      let propertyName = p._name;
      if (propertyName) {
        let column = getPropertyColumn(p, [], state);
        if (column) {
          vm.columns.push(column);
        }
      }
    });
  }
}

function getPropertyColumn(p: any, formElements: CFormElement[], state: IAppState): any {
  if (p && p.displayInfo.visible === 'false') {
    return null;
  }
  let column: any = {
    dataField: p._name,
    dataType: p._type === 'DATE' ? 'date' : 'string'
  };
  if (p.label) {
    column.label = { text: p.label };
  }
  if (p.displayInfo.type === 'CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList') {
    column.dataType = 'number';
    column.editorType = 'dxSelectBox';
    column.editorOptions = getPicklist(p, state);
  }
  return column;
}

function getPicklist(p: any, state: IAppState) {
  let lookups = getLookups(state);
  let pickListDomains = lookups ? lookups.pickListDomains : [];
  let placeholder = 'Select ' + (p.label ? p.label : '...');
  let pickList = {
    dataSource: [],
    displayExpr: 'NAME',
    valueExpr: 'ID',
    placeholder: placeholder,
    showClearButton: true
  };
  let filtered = pickListDomains.filter(
    d => d.ID === Number(p.configInfo.fieldConfig.PickListDomain));
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

function getCoeFormById(state: IAppState, coeFormId: number) {
  let filtered = state.configuration.formGroups.SearchPermanent ?
    state.configuration.formGroups.SearchPermanent.queryForms.queryForm[0].coeForms.coeForm.filter(f => +f._id === coeFormId) : undefined;
  return filtered && filtered.length > 0 ? filtered[0] : null;
}

export const TEMP_SEARCH_DESC_LIST = [{
  dataField: 'TempBatchID',
  label: { text: 'TempBatchID' },
  dataType: 'string',
}, {
  dataField: 'STARTDATECREATED',
  label: { text: 'Start Date Created' },
  dataType: 'date',
  editorType: 'dxDateBox',
}, {
  dataField: 'ENDDATECREATED',
  label: { text: 'End Date Created' },
  dataType: 'date',
  editorType: 'dxDateBox',
}, {
  dataField: 'MolWeight',
  label: { text: 'MW' },
  dataType: 'string',
}, {
  dataField: 'Formula',
  label: { text: 'MF' },
  dataType: 'string',
}, {
  dataField: 'NotebookReference',
  label: { text: 'Notebook Reference' },
  dataType: 'string'
}, {
  dataField: 'START_CREATION_DATE',
  label: { text: 'Start Synthesis Date' },
  dataType: 'date',
  editorType: 'dxDateBox',
}, {
  dataField: 'END_CREATION_DATE',
  label: { text: 'End Synthesis Date' },
  dataType: 'date',
  editorType: 'dxDateBox',
}, {
  dataField: 'AMOUNT',
  label: { text: 'Amount' },
  dataType: 'string'
}, {
  dataField: 'APPEARANCE',
  label: { text: 'Appearance' },
  dataType: 'string'
}, {
  dataField: 'PURITY',
  label: { text: 'Purity' },
  dataType: 'string'
}, {
  dataField: 'PURITY_COMMENTS',
  label: { text: 'Purity Comments' },
  dataType: 'string'
}, {
  dataField: 'SAMPLEID',
  label: { text: 'Sample ID' },
  dataType: 'string'
}, {
  dataField: 'SOLUBILITY',
  label: { text: 'Solubility' },
  dataType: 'string'
}, {
  dataField: 'BATCH_COMMENT',
  label: { text: 'Batch Comments' },
  dataType: 'string',
  editorType: 'dxTextArea'
}, {
  dataField: 'STORAGE_REQ_AND_WARNINGS',
  label: { text: 'Storage Requirements Warnings' },
  dataType: 'string',
  editorType: 'dxTextArea'
}
];

export class CTemporarySearch {
  ID?: Number;
  TempBatchID?: String;
  STARTDATECREATED?: Date;
  ENDDATECREATED?: Date;
  MolWeight?: String;
  MF?: String;
  SCIENTIST_ID?: String;
  PREFIX?: String;
  REGISTRY_PROJECT?: String;
  NotebookReference?: String;
  START_CREATION_DATE?: Date;
  END_CREATION_DATE?: Date;
  AMOUNT?: String;
  AMOUNT_UNITS?: String;
  APPEARANCE: String;
  PURITY: String;
  PURITY_COMMENTS: String;
  SAMPLEID: String;
  SOLUBILITY: String;
  BATCH_COMMENT: String;
  STORAGE_REQ_AND_WARNINGS: String;
  searchTypeValue?: String;
}

export class CTemporarySearchVM {
  columns: any[] = [];
  searchTypeItems: any[] = ['Substructure', 'Full Structure', 'Exact', 'Similarity'];
  constructor(m: CTemporarySearch, state: IAppState, propertyList?: CPropertyList) {
    this.columns.push(TEMP_SEARCH_DESC_LIST);
    if (TEMP_SEARCH_DESC_LIST) {
      TEMP_SEARCH_DESC_LIST.forEach(p => {
        this.columns.push(p);
      });
    }

    this.columns.splice(4, 0, {
      dataField: 'PersonCreated',
      label: { text: 'Created By' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ? state.session.lookups.users : [],
        valueExpr: 'PERSONID',
        displayExpr: 'USERID'
      }
    });

    this.columns.splice(7, 0, {
      dataField: 'SCIENTIST_ID',
      label: { text: 'Scientist' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ? state.session.lookups.users : [],
        valueExpr: 'PERSONID',
        displayExpr: 'USERID'
      }
    });

    this.columns.splice(8, 0, {
      dataField: 'PREFIX',
      label: { text: 'Prefix' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ?
          state.session.lookups.sequences.filter(i => (i.TYPE === 'R' || i.TYPE === 'A')) : [],
        valueExpr: 'SEQUENCEID',
        displayExpr: 'PREFIX'
      }
    });

    this.columns.splice(9, 0, {
      dataField: 'REGISTRY_PROJECT',
      label: { text: 'Registry Project Name' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ?
          state.session.lookups.projects.filter(i => ((i.TYPE === 'R' || i.TYPE === 'A') && (i.ACTIVE === 'T' || i.ACTIVE === 'F'))) : [],
        valueExpr: 'PROJECTID',
        displayExpr: 'NAME'
      }
    });

    this.columns.splice(12, 0, {
      dataField: 'AMOUNT_UNITS',
      label: { text: 'Units' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ?
          state.session.lookups.units : [],
        valueExpr: 'ID',
        displayExpr: 'UNIT'
      }
    });
  }
}

export class CRegIdentifierList {
  Identifier: CRegIdentifier[] = [];
}

export class CRegIdentifier {
  ID?: Number; // 381
  Name?: string;
}

export class CPersonCreatedList {
  Created: CPersonCreated[] = [];
}

export class CPersonCreated {
  ID?: Number; // 381
  Created?: string;
}

export class CPrefixList {
  Prefix: CPrefix[] = [];
}

export class CPrefix {
  KEY?: Number; // 381
  VALUE?: string;
}

export class CProject {
  ID?: Number; // 381
  ProjectID?: CProjectID;
}

export class CProjectList {
  Project: CProject[] = [];
}

export class CProjectID {
  _Description?: String; // Hedione Process Optimization
  _Name?: String; // Hedione Process Optimization
  _Active?: String; // T
  __text?: Number; // 2
}


export class CProperty {
  _name: String; // REG_COMMENTS
  _friendlyName: String; // REG_COMMENTS
  _type: String; // TEXT
  _precision?: Number; // 200
  _sortOrder?: Number; // 0
  validationRuleList?: CValidationRuleList;
  __text?: String;
}

export class CPropertyList {
  Property: CProperty[] = [];
}

export class CValidationRule {
  _validationRuleName: String; // textLength
  _errorMessage: String; // The property value can have between 0 and 200 characters
  params?: CParamList;
}

export class CValidationRuleList {
  validationRule: CValidationRule[] = [];
}

export class CParamList {
  param?: CParam[] = [];
}

export class CParam {
  _name: String; // min
  _value: String; // 0
}

export const STRUCTURE_SEARCH_DESC_LIST = [{
  dataField: 'StructureComments',
  label: { text: 'Structure Comments' },
  dataType: 'string',
  editorType: 'dxTextArea',
}];
export class CStructureSearch {
  StructureComments?: String;
}
export class CStructureSearchVM {
  structureComments?: String;
  columns: any[] = [];
  constructor(m: CStructureSearch, state: IAppState) {
    let coeForm = getCoeFormById(state, 4);
    if (coeForm) {
      buildPropertyList(this, coeForm.layoutInfo.formElement, state);
    }
  }
}

export class CComponentSearch {
  COMPONENTID: String;
  IDENTIFIERTYPE: String;
  IdentifierValue: String;
  CHEM_NAME_AUTOGEN: String;
  CMP_COMMENTS: String;
  STRUCTURE_COMMENTS_TXT: String;
}
export class CComponentSearchVM {
  componentID?: String;
  identifierType?: Number;
  IdentifierValue?: String;
  componentidentifierList?: CComponentIdentifierList;
  structureName: String;
  cmpComments: String;
  structureComments: String;
  columns: any[] = [];
  title: String;
  constructor(m: CComponentSearch, state: IAppState) {
    let coeForm = getCoeFormById(state, 1);
    if (coeForm) {
      if (coeForm.title) {
        this.title = coeForm.title;
      }
      buildPropertyList(this, coeForm.layoutInfo.formElement, state);
    }
  }
}

export class CComponentIdentifier {
  Key?: Number; // 381
  Name?: String;
}

export class CComponentIdentifierList {
  Project: CComponentIdentifier[] = [];
}

export class CBatchSearch {
  BatchID: String;
  FULLREGNUMBER: String;
  START_DATECREATED: String;
  END_DATECREATED: String;
  BATCH_PROJECT: String;
  PERSONCREATED: String;
  SCIENTIST_ID: String;
  START_CREATION_DATE: String;
  END_CREATION_DATE: String;
  AMOUNT: String;
  AMOUNT_UNITS: String;
  APPEARANCE: String;
  PURITY: String;
  PURITY_COMMENTS: String;
  SAMPLEID: String;
  SOLUBILITY: String;
  BATCH_COMMENT: String;
  STORAGE_REQ_AND_WARNINGS: String;
  FORMULA_WEIGHT: String;
  NotebookReference: String;
}

export class CBatchSearchVM {
  batchID: String;
  fullRegistryNumber: String;
  startdateCreated?: Date;
  enddateCreated?: Date;
  batchProjectID?: Number;
  projectList?: CProjectList;
  personCreated?: Number;
  createdByList?: CPersonCreatedList;
  scientistID?: Number;
  scientistList?: CPersonCreatedList;
  startSynthesisDate?: Date;
  endSynthesisDate?: Date;
  amount?: String;
  units?: Number;
  unitList?: CUnitList;
  appearance?: String;
  purity?: String;
  purityComments?: String;
  sampleID?: String;
  solubility?: String;
  batchComments?: String;
  storageRequirementsWarnings?: String;
  formulaWeight?: String;
  noteBookReference?: Number;
  noteBookReferenceList?: CNoteBookReferenceList;
  columns: any[] = [];
  title: string;
  constructor(m: CBatchSearch, state: IAppState) {
    let coeForm = getCoeFormById(state, 2);
    if (coeForm) {
      if (coeForm.title) {
        this.title = coeForm.title;
      }
      buildPropertyList(this, coeForm.layoutInfo.formElement, state);
    }
  }
}

export class CUnit {
  ID?: Number; // 381
  Name?: String;
}

export class CUnitList {
  Name: CUnit[] = [];
}

export class CNoteBookReference {
  ID?: Number; // 381
  Value?: String;
}

export class CNoteBookReferenceList {
  Name: CNoteBookReference[] = [];
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
  }
  return value;
}

export const HITLIST_GRID_COLUMNS = [{
  dataField: 'Name',
  dataType: 'String',
  cellTemplate: 'saveCellTemplate'
}, {
  dataField: 'Description',
  dataType: 'String'
}, {
  dataField: 'IsPublic',
  width: '60px'
}, {
  caption: '# Hits',
  dataField: 'NumberOfHits',
  dataType: Number,
  allowEditing: false,
  width: '60px'
}, {
  dataField: 'DateCreated._date',
  caption: 'Date Created',
  dataType: 'date',
  allowEditing: false
}, {
  dataField: 'HistlistType',
  caption: 'Queries',
  lookup: { dataSource: [{ id: 0, name: 'Recent' }, { id: 1, name: 'Saved' }], valueExpr: 'id', displayExpr: 'name' },
  groupIndex: 0,
  allowEditing: false
},
{
  caption: 'Restore',
  cellTemplate: 'restoreCellTemplate',
  width: '200px'
}
];

export class CQueryManagementVM {
  gridColumns?: any[];
  queriesList?: CQueries[];
  advancedRestoreType?: number;
  isCurrentHitlist?: boolean;
  saveQueryVM?: CSaveQuery;
  constructor(state: IAppState) {
    this.queriesList = state.registrysearch.hitlist.rows;
    this.gridColumns = HITLIST_GRID_COLUMNS;
    this.advancedRestoreType = 0;
    this.isCurrentHitlist = false;
    this.saveQueryVM = new CSaveQuery();
  }

  getRestoreDataSource() {
    return [{
      key: 0,
      value: 'Subtract from entire list'
    }, {
      key: 2,
      value: 'Subtract from current list',
      disabled: !this.isCurrentHitlist
    }, {
      key: 1,
      value: 'Intersect with current list',
      disabled: !this.isCurrentHitlist
    }, {
      key: 3,
      value: 'Union with current list',
      disabled: !this.isCurrentHitlist
    }];
  }

}

export class CQueries {
  ID?: Number; // 381
  Name?: String;
}

export const HITLIST_EDIT_DESC_LIST = [{
  dataField: 'Name',
  label: { text: 'Name' },
  dataType: 'string',
  editorType: 'dxTextBox',
}, {
  dataField: 'Description',
  label: { text: 'Description' },
  dataType: 'string',
  editorType: 'dxTextArea',
}, {
  dataField: 'IsPublic',
  label: { text: 'Is Public' },
  dataType: 'boolean',
  editorType: 'dxCheckBox',
}
];

export class CSaveData {
  Name?: string; // 921
  Description?: String;
  IsPublic?: boolean;
}

export class CSaveQuery {
  editColumns: any[] = [];
  data?: CSaveData;
  constructor() {
    this.editColumns.push(HITLIST_EDIT_DESC_LIST);
    if (HITLIST_EDIT_DESC_LIST) {
      HITLIST_EDIT_DESC_LIST.forEach(p => {
        this.editColumns.push(p);
      });
    }
    this.data = new CSaveData();
  }
  clear() {
    this.data.Name = '';
    this.data.Description = '';
    this.data.IsPublic = false;
  }
}

export const PREFERENCES_LIST = [{
  dataField: 'HitsCount',
  label: { text: 'Hits per page' },
  dataType: 'Number',
  editorType: 'dxTextBox',
}, {
  dataField: 'FilterChildData',
  text: 'Filter child data by search',
  dataType: 'boolean',
  editorType: 'dxCheckBox',
}, {
  dataField: 'HighlightSubStructure',
  text: 'Highlight sub-structures',
  dataType: 'boolean',
  editorType: 'dxCheckBox',
}
];

export class CPreferenceVM {
  columns: any[] = [];
  constructor() {
    this.columns.push(PREFERENCES_LIST);
    if (PREFERENCES_LIST) {
      PREFERENCES_LIST.forEach(p => {
        this.columns.push(p);
      });
    }
  }
}

function getLookups(state: IAppState): any {
  return state.session ? state.session.lookups : undefined;
}

export class CSearchFormVM {
  ID?: Number;
  registrySearch?: CRegSearch;
  structureSearch?: CStructureSearch;
  componentSearch?: CComponentSearch;
  temporarySearch?: CTemporarySearch;
  batchSearch?: CBatchSearch;
  registrySearchVM?: CRegSearchVM;
  temporarySearchVM?: CTemporarySearchVM;
  structureSearchVM?: CStructureSearchVM;
  componentSearchVM?: CComponentSearchVM;
  batchSearchVM?: CBatchSearchVM;
  preferenceVM?: CPreferenceVM;
  constructor(state: IAppState) {
    this.registrySearch = new CRegSearch();
    this.structureSearch = new CStructureSearch();
    this.componentSearch = new CComponentSearch();
    this.batchSearch = new CBatchSearch();
    this.temporarySearch = new CTemporarySearch();
    this.registrySearchVM = new CRegSearchVM(this.registrySearch, state);
    this.temporarySearchVM = new CTemporarySearchVM(this.temporarySearch, state);
    this.structureSearchVM = new CStructureSearchVM(this.structureSearch, state);
    this.componentSearchVM = new CComponentSearchVM(this.componentSearch, state);
    this.batchSearchVM = new CBatchSearchVM(this.batchSearch, state);
    this.preferenceVM = new CPreferenceVM();
  }
}
