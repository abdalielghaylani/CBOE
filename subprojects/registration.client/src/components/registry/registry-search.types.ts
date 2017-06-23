import { IAppState } from '../../store';
import { IShareableObject, CShareableObject, FormGroupType, CFormGroup, CFormElement } from '../../common';
import * as X2JS from 'x2js';

export interface ISearchCriteriaItem {
  fieldid: string;
  tableid: string;
}

export class CRegSearchVM {
  data: any = {};
  sdfFile?: File;
  title?: String;
  columns: any[] = [];
  searchTypeValue?: String;
  searchTypeItems: any[] = ['Substructure', 'Full Structure', 'Exact', 'Similarity'];
  constructor(temporary: boolean, state: IAppState) {
    let coeForm = getCoeFormById(temporary, state, 0);
    if (coeForm) {
      if (coeForm.title) {
        this.title = coeForm.title;
      }
      buildPropertyList(this, coeForm.layoutInfo.formElement, state, 0);
    }
  }
}

function buildPropertyList(vm: any, formElement: any, state: IAppState, coeFormId: Number) {
  if (formElement) {
    formElement.forEach(p => {
      let propertyName = p._name;
      if (propertyName) {
        let column = getPropertyColumn(p, coeFormId, state);
        if (column) {
          vm.columns.push(column);
          vm.data[column.dataField] = undefined;
        }
      }
    });
  }
}

function getPropertyColumn(p: any, coeFormId: Number, state: IAppState): any {
  if (p && p.displayInfo.visible === 'false') {
    return null;
  }
  let column: any = {
    dataField: p._name,
    dataType: p._type === 'DATE' ? 'date' : 'string',
    validationRules: [],
    coeType: p.displayInfo.type.replace('CambridgeSoft.COE.Framework.Controls.COEFormGenerator.', ''),
    searchCriteriaItem: p.searchCriteriaItem
  };
  if (p.label) {
    column.label = { text: p.label };
  }
  switch (column.coeType) {
    case 'COEDropDownList':
      column.dataType = 'number';
      column.editorType = 'dxSelectBox';
      column.editorOptions = getPicklist(p, state, coeFormId);
      break;
    case 'COEListUpload':
      column.editorType = 'dxFileUploader';
      break;
    case 'COETextArea':
      column.editorType = 'dxTextArea';
      break;
    case 'COEDatePicker':
      column.editorType = 'dxDateBox';
      break;
    case 'COEStructureQuery':
    case 'COEStateControl':
      column.editorType = 'dxTextArea';
      column.visible = false;
      break;
    default:
      if (typeof (p.searchCriteriaItem.numericalCriteria) === 'object') {
        column.validationRules.push({ type: 'numeric' });
      }
      break;
  }
  return column;
}

function getPicklist(p: any, state: IAppState, coeFormId: Number) {
  let lookups = getLookups(state);
  let placeholder = 'Select ' + (p.label ? p.label : '...');
  let pickList = {
    dataSource: [],
    displayExpr: 'NAME',
    valueExpr: 'ID',
    placeholder: placeholder,
    showClearButton: true
  };
  if (p._name === 'PREFIX') {
    pickList.dataSource = lookups ? lookups.sequences.filter(i => (i.TYPE === 'R' || i.TYPE === 'A')) : [];
    pickList.valueExpr = 'SEQUENCEID';
    pickList.displayExpr = 'PREFIX';
  } else if (p._name === 'BATCH_PROJECT') {
    pickList.dataSource = lookups ? lookups.projects.filter(i => (i.TYPE === 'B' || i.TYPE === 'A') && (i.ACTIVE === 'T' || i.ACTIVE === 'F')) : [];
  } else if (p._name === 'REGISTRY_PROJECT') {
    pickList.dataSource = lookups ? lookups.projects.filter(i => (i.TYPE === 'R' || i.TYPE === 'A') && (i.ACTIVE === 'T' || i.ACTIVE === 'F')) : [];
  } else if (p._name === 'IDENTIFIERTYPE' && p.Id === 'IDENTIFIERTYPETextBox') {
    if (coeFormId === 0) {
      pickList.dataSource = lookups ? lookups.identifierTypes.filter(i => (i.TYPE === 'R' || i.TYPE === 'A') && i.ACTIVE === 'T') : [];
    } else if (coeFormId === 4) {
      pickList.dataSource = lookups ? lookups.identifierTypes.filter(i => (i.TYPE === 'S' || i.TYPE === 'A') && i.ACTIVE === 'T') : [];
    } else if (coeFormId === 1) {
      pickList.dataSource = lookups ? lookups.identifierTypes.filter(i => (i.TYPE === 'C' || i.TYPE === 'A') && i.ACTIVE === 'T') : [];
    } else if (coeFormId === 2) {
      pickList.dataSource = lookups ? lookups.identifierTypes.filter(i => (i.TYPE === 'B' || i.TYPE === 'A') && i.ACTIVE === 'T') : [];
    }
  } else if (p.configInfo.fieldConfig.PickListDomain) {
    let pickListDomains = lookups ? lookups.pickListDomains : [];
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
  }
  return pickList;
}

function getCoeFormById(temporary: boolean, state: IAppState, coeFormId: number) {
  let formGroupType = temporary ? FormGroupType.SearchTemporary : FormGroupType.SearchPermanent;
  let filtered = state.configuration && state.configuration.formGroups && state.configuration.formGroups[FormGroupType[formGroupType]] ?
    state.configuration.formGroups[FormGroupType[formGroupType]].queryForms.queryForm[0].coeForms.coeForm.filter(f => +f._id === coeFormId) : undefined;
  return filtered && filtered.length > 0 ? filtered[0] : null;
}

export class CStructureSearchVM {
  data: any = {};
  columns: any[] = [];
  constructor(temporary: boolean, state: IAppState) {
    let coeForm = getCoeFormById(temporary, state, 4);
    if (coeForm) {
      buildPropertyList(this, coeForm.layoutInfo.formElement, state, 4);
    }
  }
}

export class CComponentSearchVM {
  data: any = {};
  columns: any[] = [];
  title: String;
  constructor(temporary: boolean, state: IAppState) {
    let coeForm = getCoeFormById(temporary, state, 1);
    if (coeForm) {
      if (coeForm.title) {
        this.title = coeForm.title;
      }
      buildPropertyList(this, coeForm.layoutInfo.formElement, state, 1);
    }
  }
}

export class CBatchSearchVM {
  data: any = {};
  columns: any[] = [];
  title: string;
  constructor(temporary: boolean, state: IAppState) {
    let coeForm = getCoeFormById(temporary, state, 2);
    if (coeForm) {
      if (coeForm.title) {
        this.title = coeForm.title;
      }
      buildPropertyList(this, coeForm.layoutInfo.formElement, state, 2);
    }
  }
}

function getLookups(state: IAppState): any {
  return state.session ? state.session.lookups : undefined;
}

export class CSearchFormVM {
  ID?: Number;
  registrySearchVM?: CRegSearchVM;
  structureSearchVM?: CStructureSearchVM;
  componentSearchVM?: CComponentSearchVM;
  batchSearchVM?: CBatchSearchVM;
  constructor(temporary: boolean, state: IAppState) {
    this.registrySearchVM = new CRegSearchVM(temporary, state);
    this.structureSearchVM = new CStructureSearchVM(temporary, state);
    this.componentSearchVM = new CComponentSearchVM(temporary, state);
    this.batchSearchVM = new CBatchSearchVM(temporary, state);
  }
}

export const HITLIST_GRID_COLUMNS = [{
  dataField: 'name',
  dataType: 'string',
  formItem: { colSpan: 2 }
}, {
  dataField: 'description',
  dataType: 'string',
  formItem: { colSpan: 2 }
}, {
  dataField: 'isPublic',
  dataType: 'boolean',
  width: '60px'
}, {
  caption: '# Hits',
  dataField: 'numberOfHits',
  dataType: Number,
  allowEditing: false,
  width: '60px',
  formItem: { visible: false }
}, {
  dataField: 'dateCreated',
  caption: 'Date Created',
  dataType: 'date',
  format: 'shortDateShortTime',
  allowEditing: false,
  formItem: { visible: false }
}, {
  caption: 'Queries',
  groupIndex: 0,
  allowEditing: false,
  formItem: { visible: false },
  calculateCellValue: function(d) { return d.hitlistType === 'TEMP' ? 'Recent' : 'Saved'; }
},
{
  caption: 'Commands',
  cellTemplate: 'commandCellTemplate',
  width: '200px',
  allowEditing: false,
  formItem: { visible: false }
}
];

export class CQueryManagementVM {
  gridColumns?: any[];
  queriesList?: CQueries[];
  advancedRestoreType?: string;
  saveQueryVM?: CSaveQuery;
  constructor(state: IAppState) {
    this.queriesList = state.registrysearch.hitlist.rows;
    this.gridColumns = HITLIST_GRID_COLUMNS;
    this.advancedRestoreType = 'intersect';
    this.saveQueryVM = new CSaveQuery();
  }

  getRestoreDataSource() {
    return [{
      key: 'intersect',
      value: 'Intersect with current list'
    }, {
      key: 'union',
      value: 'Union with current list'
    }, {
      key: 'subtract',
      value: 'Subtract from current list'
    }];
  }

}

export class CQueries {
  ID?: Number; // 381
  Name?: String;
}

export class CSaveQuery {
  editColumns: any[] = [{
    dataField: 'name',
    label: { text: 'Name' },
    dataType: 'string',
    editorType: 'dxTextBox',
  }, {
    dataField: 'description',
    label: { text: 'Description' },
    dataType: 'string',
    editorType: 'dxTextArea',
  }, {
    dataField: 'isPublic',
    label: { text: 'Is Public' },
    dataType: 'boolean',
    editorType: 'dxCheckBox',
  }];
  data?: IShareableObject;
  constructor() {
    this.clear();
  }
  clear() {
    this.data = new CShareableObject('', '', false);
  }
}
