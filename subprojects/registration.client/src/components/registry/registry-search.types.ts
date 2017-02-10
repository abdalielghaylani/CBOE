export const REGISTRY_SEARCH_DESC_LIST = [{
  dataField: 'PersonCreated',
  label: { text: 'Created By' },
  dataType: 'string',
  editorType: 'dxSelectBox',
}, {
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
  dataField: 'PREFIX',
  label: { text: 'Prefix' },
  dataType: 'string',
  editorType: 'dxSelectBox',
}, {
  dataField: 'REGISTRY_PROJECT',
  label: { text: 'Registry Project Name' },
  dataType: 'string',
  editorType: 'dxSelectBox',
}, {
  dataField: 'MolWeight',
  label: { text: 'MW' },
  dataType: 'string',
}, {
  dataField: 'Formula',
  label: { text: 'MF' },
  dataType: 'string',
}, {
  dataField: 'IDENTIFIERTYPE',
  label: { text: 'Registry Identifier' },
  dataType: 'string',
  editorType: 'dxSelectBox',
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
  constructor(propertyList?: CPropertyList) {
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
  }
}

export class CRegIdentifierList {
  Project: CRegIdentifier[] = [];
}

export class CRegIdentifier {
  ID?: Number; // 381
  Name?: string;
}

export class CPersonCreatedList {
  Project: CPersonCreated[] = [];
}

export class CPersonCreated {
  ID?: Number; // 381
  Created?: string;
}

export class CPrefixList {
  Project: CPrefix[] = [];
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

export class CStructureSearchVM {
  structureComments?: String;
  columns: any[] = [];
  constructor(propertyList?: CPropertyList) {
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
  dataField: 'IDENTIFIERTYPE',
  label: { text: 'Component Identifier' },
  dataType: 'string',
  editorType: 'dxSelectBox'
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

export class CCompoundSearchVM {
  componentID?: String;
  identifierType?: Number;
  IdentifierValue?: String;
  componentidentifierList?: CComponentIdentifierList;
  structureName: String;
  cmpComments: String;
  structureComments: String;
  columns: any[] = [];
  constructor(propertyList?: CPropertyList) {
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
  dataField: 'BATCH_PROJECT',
  label: { text: 'Batch Project Name' },
  dataType: 'string',
  editorType: 'dxSelectBox',
}, {
  dataField: 'PERSONCREATED',
  label: { text: 'Created By' },
  dataType: 'string',
  editorType: 'dxSelectBox',
}, {
  dataField: 'SCIENTIST_ID',
  label: { text: 'Scientist' },
  dataType: 'string',
  editorType: 'dxSelectBox',
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
  dataField: 'AMOUNT_UNITS',
  label: { text: 'Units' },
  dataType: 'string',
  editorType: 'dxSelectBox'
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
  constructor(propertyList?: CPropertyList) {
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
  latestQueriesList?: CQueriesList;
  savedQueriesList?: CQueriesList;
}

export class CQueries {
  ID?: Number; // 381
  Name?: String;
}

export class CQueriesList {
  Items: CQueries[] = [];
}

export class CRegistrySearchVM {
  registrySearch?: CRegSearchVM;
  structureSearch?: CStructureSearchVM;
  componentSearch?: CCompoundSearchVM;
  batchSearch?: CBatchSearchVM;
}
