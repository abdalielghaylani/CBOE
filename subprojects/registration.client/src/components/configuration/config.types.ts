
export const PROJECTS_COLUMNS = [
  {
    dataField: 'PROJECTID',
    visible: false
  },
  {
    dataField: 'NAME',
    caption: 'Name'
  },
  {
    dataField: 'DESCRIPTION',
    caption: 'Description'
  },
  {
    dataField: 'ACTIVE',
    caption: 'Is Active?',
    cellTemplate: 'activeTemplate',
    editCellTemplate: 'editActiveTemplate'
  },
  {
    dataField: 'TYPE',
    caption: 'Type',
    cellTemplate: 'projectTypeTemplate',
    editCellTemplate: 'editProjectTypeTemplate'
  }
];
export const PICKLIST_COLUMNS = [
  {
    dataField: 'ID',
    visible: false
  },
  {
    dataField: 'DESCRIPTION',
    caption: 'Description'
  },
  {
    dataField: 'PICKLISTVALUE',
    caption: 'Picklist value'
  },
  {
    dataField: 'ACTIVE',
    caption: 'Is Active?',
    cellTemplate: 'activeTemplate',
    editCellTemplate: 'editActiveTemplate'
  },
  {
    dataField: 'SORTORDER',
    caption: 'Sort Order',
    dataType: 'number'
  }];
export class CConfigTable {
  columns: any;
  constructor(tableId: string) {
    switch (tableId) {
      case 'VW_PROJECT':
        this.columns = PROJECTS_COLUMNS;
        break;
      case 'VW_PICKLIST':
        this.columns = PICKLIST_COLUMNS;
        break;
    }
  }
}
