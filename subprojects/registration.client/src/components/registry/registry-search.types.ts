import { IAppState } from '../../store';
import { FormGroupType, CFormGroup, CFormElement } from '../../common';
import * as X2JS from 'x2js';

export class CRegSearchVM {
  data: any = {};
  sdfFile?: File;
  title?: String;
  columns: any[] = [];
  searchTypeValue?: String;
  searchTypeItems: any[] = ['Substructure', 'Full Structure', 'Exact', 'Similarity'];
  constructor(state: IAppState) {
    let coeForm = getCoeFormById(state, 0);
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

function getCoeFormById(state: IAppState, coeFormId: number) {
  let filtered = state.configuration.formGroups.SearchPermanent ?
    state.configuration.formGroups.SearchPermanent.queryForms.queryForm[0].coeForms.coeForm.filter(f => +f._id === coeFormId) : undefined;
  return filtered && filtered.length > 0 ? filtered[0] : null;
}

export class CStructureSearchVM {
  data: any = {};
  columns: any[] = [];
  constructor(state: IAppState) {
    let coeForm = getCoeFormById(state, 4);
    if (coeForm) {
      buildPropertyList(this, coeForm.layoutInfo.formElement, state, 4);
    }
  }
}

export class CComponentSearchVM {
  data: any = {};
  columns: any[] = [];
  title: String;
  constructor(state: IAppState) {
    let coeForm = getCoeFormById(state, 1);
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
  constructor(state: IAppState) {
    let coeForm = getCoeFormById(state, 2);
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
  constructor(state: IAppState) {
    this.registrySearchVM = new CRegSearchVM(state);
    this.structureSearchVM = new CStructureSearchVM(state);
    this.componentSearchVM = new CComponentSearchVM(state);
    this.batchSearchVM = new CBatchSearchVM(state);
  }
}

export const HITLIST_GRID_COLUMNS = [{
  dataField: 'name',
  dataType: 'String',
  cellTemplate: 'saveCellTemplate'
}, {
  dataField: 'description',
  dataType: 'String'
}, {
  dataField: 'isPublic',
  width: '60px'
}, {
  caption: '# Hits',
  dataField: 'numberOfHits',
  dataType: Number,
  allowEditing: false,
  width: '60px'
}, {
  dataField: 'dateCreated',
  caption: 'Date Created',
  dataType: 'date',
  format: 'shortDateShortTime',
  allowEditing: false
}, {
  dataField: 'hitlistType',
  caption: 'Queries',
  lookup: { dataSource: [{ id: 'TEMP', name: 'Recent' }, { id: 'SAVED', name: 'Saved' }], valueExpr: 'id', displayExpr: 'name' },
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
