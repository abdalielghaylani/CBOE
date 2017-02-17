import { IAppState } from '../../store';
import { FormGroupType, CFormGroup, CFormElement } from '../../common';
export const REGISTRY_SEARCH_DESC_LIST = [{
  dataField: 'RegNumber',
  label: { text: 'Registry Number' },
  dataType: 'string',
}, {
  dataField: 'STARTDATECREATED',
  label: { text: 'Start Date Created' },
  dataType: 'string',
  editorType: 'dxDateBox',
}, {
  dataField: 'ENDDATECREATED',
  label: { text: 'End Date Created' },
  dataType: 'string',
  editorType: 'dxDateBox',
}, {
  dataField: 'SeqNumber',
  label: { text: 'Sequence Number' },
  dataType: 'string',
}, {
  dataField: 'MolWeight',
  label: { text: 'MW' },
  dataType: 'string',
}, {
  dataField: 'Formula',
  label: { text: 'MF' },
  dataType: 'string',
}, {
  dataField: 'IdentifierValue',
  label: { text: 'Value' },
  dataType: 'string',
}, {
  dataField: 'REG_COMMENTS',
  label: { text: 'Registry Comments' },
  dataType: 'string',
  editorType: 'dxTextArea',
}, {
  dataField: 'SDF_upload',
  label: { text: 'SDF Search' },
  dataType: 'string',
  editorType: 'dxFileUploader',
}
];

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
  personCreated?: Number;
  regNumber?: String;
  startdateCreated?: Date;
  enddateCreated?: Date;
  createdByList?: CPersonCreatedList;
  seqNumber?: String;
  prefixKey?: Number;
  prefixList?: CPrefixList;
  rgistryProjectID?: Number;
  projectList?: CProjectList;
  mw?: String;
  mf?: String;
  regIdentifier?: Number;
  regidentifierList?: CRegIdentifierList;
  identifierValue?: String;
  registryComments?: String;
  sdfFile?: File;
  columns: any[] = [];
  searchTypeItems: any[] = ['Substructure', 'Full Structure', 'Exact', 'Similarity'];
  constructor(m: CRegSearch, state: IAppState, propertyList?: CPropertyList) {
    this.columns.push(REGISTRY_SEARCH_DESC_LIST);
    if (REGISTRY_SEARCH_DESC_LIST) {
      REGISTRY_SEARCH_DESC_LIST.forEach(p => {
        this.columns.push(p);
      });
    }
    if (propertyList) {
      propertyList.Property.forEach(p => {
        let propertyName = p._name;
        if (propertyName) {
          this[propertyName as string] = getPropertyValue(p);
          this.columns.push(getPropertyColumn(p));
        }
      });
    }
    this.columns.splice(0, 0, {
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

    this.columns.splice(4, 0, {
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

    this.columns.splice(7, 0, {
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

    this.columns.splice(10, 0, {
      dataField: 'IDENTIFIERTYPE',
      label: { text: 'Registry Identifier' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ?
          state.session.lookups.identifierTypes.filter(i => ((i.TYPE === 'R' || i.TYPE === 'A') && i.ACTIVE === 'T')) : [],
        displayExpr: 'NAME',
        valueExpr: 'ID',
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
  constructor(m: CStructureSearch, state: IAppState, propertyList?: CPropertyList) {
    this.columns.push(STRUCTURE_SEARCH_DESC_LIST);
    if (STRUCTURE_SEARCH_DESC_LIST) {
      STRUCTURE_SEARCH_DESC_LIST.forEach(p => {
        this.columns.push(p);
      });
    }
    if (propertyList) {
      propertyList.Property.forEach(p => {
        let propertyName = p._name;
        if (propertyName) {
          this[propertyName as string] = getPropertyValue(p);
          this.columns.push(getPropertyColumn(p));
        }
      });
    }
  }
}

export const COMPONENT_SEARCH_DESC_LIST = [{
  dataField: 'COMPONENTID',
  label: { text: 'Component ID' },
  dataType: 'string'
}, {
  dataField: 'IdentifierValue',
  label: { text: 'Value' },
  dataType: 'string'
}, {
  dataField: 'CHEM_NAME_AUTOGEN',
  label: { text: 'Structure Name' },
  dataType: 'string'
}, {
  dataField: 'CMP_COMMENTS',
  label: { text: 'Component Comments' },
  dataType: 'string',
  editorType: 'dxTextArea'
}, {
  dataField: 'STRUCTURE_COMMENTS_TXT',
  label: { text: 'Stereochemistry Comments' },
  dataType: 'string',
  editorType: 'dxTextArea'
}
];
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
  constructor(m: CComponentSearch, state: IAppState, propertyList?: CPropertyList) {
    this.columns.push(COMPONENT_SEARCH_DESC_LIST);
    if (COMPONENT_SEARCH_DESC_LIST) {
      COMPONENT_SEARCH_DESC_LIST.forEach(p => {
        this.columns.push(p);
      });
    }
    if (propertyList) {
      propertyList.Property.forEach(p => {
        let propertyName = p._name;
        if (propertyName) {
          this[propertyName as string] = getPropertyValue(p);
          this.columns.push(getPropertyColumn(p));
        }
      });
    }

    this.columns.splice(2, 0, {
      dataField: 'IDENTIFIERTYPE',
      label: { text: 'Component Identifier' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ?
          state.session.lookups.identifierTypes.filter(i => ((i.TYPE === 'C' || i.TYPE === 'A') && i.ACTIVE === 'T')) : [],
        displayExpr: 'NAME',
        valueExpr: 'ID',
      }
    });

  }
}

export class CComponentIdentifier {
  Key?: Number; // 381
  Name?: String;
}

export class CComponentIdentifierList {
  Project: CComponentIdentifier[] = [];
}

export const BATCH_SEARCH_DESC_LIST = [{
  dataField: 'BatchID',
  label: { text: 'BatchID' },
  dataType: 'string'
}, {
  dataField: 'FULLREGNUMBER',
  label: { text: 'Full Registry Number' },
  dataType: 'string',
  editorType: 'dxTextArea'
}, {
  dataField: 'START_DATECREATED',
  label: { text: 'Start Date Created' },
  dataType: 'string',
  editorType: 'dxDateBox',
}, {
  dataField: 'END_DATECREATED',
  label: { text: 'End Date Created' },
  dataType: 'string',
  editorType: 'dxDateBox',
}, {
  dataField: 'START_CREATION_DATE',
  label: { text: 'Start Synthesis Date' },
  dataType: 'string',
  editorType: 'dxDateBox',
}, {
  dataField: 'END_CREATION_DATE',
  label: { text: 'End Synthesis Date' },
  dataType: 'string',
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
}, {
  dataField: 'FORMULA_WEIGHT',
  label: { text: 'Formula Weight' },
  dataType: 'string'
}, {
  dataField: 'NotebookReference',
  label: { text: 'Notebook Reference' },
  dataType: 'string'
}
];

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
  constructor(m: CBatchSearch, state: IAppState, propertyList?: CPropertyList) {
    this.columns.push(BATCH_SEARCH_DESC_LIST);
    if (BATCH_SEARCH_DESC_LIST) {
      BATCH_SEARCH_DESC_LIST.forEach(p => {
        this.columns.push(p);
      });
    }
    if (propertyList) {
      propertyList.Property.forEach(p => {
        let propertyName = p._name;
        if (propertyName) {
          this[propertyName as string] = getPropertyValue(p);
          this.columns.push(getPropertyColumn(p));
        }
      });
    }

    this.columns.splice(2, 0, {
      dataField: 'IDENTIFIERTYPE',
      label: { text: 'Component Identifier' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ?
          state.session.lookups.identifierTypes.filter(i => ((i.TYPE === 'C' || i.TYPE === 'A') && i.ACTIVE === 'T')) : [],
        displayExpr: 'NAME',
        valueExpr: 'ID',
      }
    });

    this.columns.splice(5, 0, {
      dataField: 'BATCH_PROJECT',
      label: { text: 'Batch Project Name' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ?
          state.session.lookups.projects.filter(i => ((i.TYPE === 'B' || i.TYPE === 'A') && (i.ACTIVE === 'T' || i.ACTIVE === 'F'))) : [],
        valueExpr: 'PROJECTID',
        displayExpr: 'NAME'
      }
    });

    this.columns.splice(7, 0, {
      dataField: 'PERSONCREATED',
      label: { text: 'Created By' },
      dataType: 'string',
      editorType: 'dxSelectBox',
      editorOptions: {
        dataSource: state && state.session.lookups ? state.session.lookups.users : [],
        valueExpr: 'PERSONID',
        displayExpr: 'USERID'
      }
    });

    this.columns.splice(8, 0, {
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

function getPropertyColumn(p: CProperty): any {
  let column: any = {
    dataField: p._name.toLowerCase(),
    dataType: p._type === 'DATE' ? 'date' : 'string'
  };
  return column;
}

export class CQueryManagementVM {
  latestQueriesList?: CQueries[];
  savedQueriesList?: CQueries[];
  constructor() {
    // mock data to be deleted
    this.latestQueriesList = [{ ID: 1, Name: 'TEMP 17022017001' }, { ID: 2, Name: 'TEMP 17022017002' }, { ID: 3, Name: 'TEMP 17022017003 ' }];
    this.savedQueriesList = [{ ID: 1, Name: 'SAVED 17022017001' }, { ID: 1, Name: 'SAVED 17022017002' }, { ID: 1, Name: 'SAVED 17022017003' }];
    // mock data to be deleted

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

export class CManageHitlistVM {
  editColumns: any[] = [];
  items?: CQueries[];
  hitlistValue?: String;
  hitlistEdit?: boolean;
  hitlistRestore?: boolean;
  hitlistDelete?: boolean;
  unionHitlist: boolean;
  substractHitlist: boolean;
  intersectHitlist: boolean;
  replaceHitlist: boolean;
  constructor(m: CQueryManagementVM) {
    this.editColumns.push(HITLIST_EDIT_DESC_LIST);
    if (HITLIST_EDIT_DESC_LIST) {
      HITLIST_EDIT_DESC_LIST.forEach(p => {
        this.editColumns.push(p);
      });
    }
    this.items = m.latestQueriesList.concat(m.savedQueriesList);
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

export class CSearchFormVM {
  ID?: Number;
  registrySearch?: CRegSearch;
  structureSearch?: CStructureSearch;
  componentSearch?: CComponentSearch;
  batchSearch?: CBatchSearch;
  registrySearchVM?: CRegSearchVM;
  structureSearchVM?: CStructureSearchVM;
  componentSearchVM?: CComponentSearchVM;
  batchSearchVM?: CBatchSearchVM;
  queryManagementVM?: CQueryManagementVM;
  hitlistVM?: CManageHitlistVM;
  preferenceVM?: CPreferenceVM;
  constructor(state: IAppState) {
    this.registrySearch = new CRegSearch();
    this.structureSearch = new CStructureSearch();
    this.componentSearch = new CComponentSearch();
    this.batchSearch = new CBatchSearch();
    this.registrySearchVM = new CRegSearchVM(this.registrySearch, state);
    this.structureSearchVM = new CStructureSearchVM(this.structureSearch, state);
    this.componentSearchVM = new CComponentSearchVM(this.componentSearch, state);
    this.batchSearchVM = new CBatchSearchVM(this.batchSearch, state);
    this.queryManagementVM = new CQueryManagementVM();
    this.hitlistVM = new CManageHitlistVM(this.queryManagementVM);
    this.preferenceVM = new CPreferenceVM();
  }
}
