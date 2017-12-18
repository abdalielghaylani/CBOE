import * as X2JS from 'x2js';
import { IShareableObject, CShareableObject, FormGroupType, IFormGroup, IFormElement } from '../../common';
import { IAppState, ILookupData } from '../../redux';

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
  groupCellTemplate: 'groupCellTemplate',
  allowEditing: false,
  formItem: { visible: false },
  calculateCellValue: function (d) { return d.hitlistType === 'TEMP' ? 'Recent' : 'Saved'; }
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
