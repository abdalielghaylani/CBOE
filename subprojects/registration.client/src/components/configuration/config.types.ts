import { IAppState } from '../../store';
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
    caption: 'Description',
    editCellTemplate: 'editPickListDomainTemplate'
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

export const PICKLISTDOMAIN_COLUMNS = [{ dataField: 'DESCRIPTION', caption: 'Description' },
{ dataField: 'EXT_TABLE', caption: 'External Table' },
{ dataField: 'EXT_ID_COL', caption: 'External Column Id' },
{ dataField: 'EXT_DISPLAY_COL', caption: 'External Column Display' },
{ dataField: 'EXT_SQL_FILTER', caption: 'Additional SQL Filter' },
{ dataField: 'EXT_SQL_SORTORDER', caption: 'Additional SQL SortOrder' },
{
  dataField: 'LOCKED', caption: 'Is Locked?',
  cellTemplate: 'activeTemplate',
  allowEditing: false
}];

export class CConfigTable {
  columns: any[];
  pickListDomains: any[];
  projectType = [{ key: 'A', name: 'All' }, { key: 'R', name: 'Registry' }, { key: 'B', name: 'Batch' }];
  isDeleteEnabled: boolean = true;
  constructor(tableId: string, state: IAppState) {
    switch (tableId) {
      case 'VW_PROJECT':
        this.columns = PROJECTS_COLUMNS;
        break;
      case 'VW_PICKLIST':
        this.columns = PICKLIST_COLUMNS;
        this.isDeleteEnabled = false;
        break;
      case 'VW_PICKLISTDOMAIN':
        this.columns = PICKLISTDOMAIN_COLUMNS;
        this.isDeleteEnabled = false;
        break;
    }
    this.pickListDomains = getLookups(state).pickListDomains;
  }
}

export const CONFIG_FORMS_COLUMNS = [
  {
    dataField: 'group',
    allowFiltering: false,
    allowSorting: false,
    allowEditing: false,
    groupIndex: 0
  },
  {
    dataField: 'name',
    allowEditing: false
  },
  {
    dataField: 'controlType',
    cellTemplate: 'controlTypeTemplate',
    editCellTemplate: 'editControlTypeTemplate'
  },
  {
    dataField: 'cssClass',
    editCellTemplate: 'editCssClassTemplate'
  },
  { dataField: 'visible' }
];

export class CConfigForms {
  columns: any;
  cssClassItems = ['Std20x40', 'Std50x40', 'Std50x80', 'Std75x40', 'Std100x40', 'Std100x80'];
  constructor() {
    this.columns = CONFIG_FORMS_COLUMNS;
  }
}

function getLookups(state: IAppState): any {
  return state.session ? state.session.lookups : undefined;
}
export const CONFIG_PROPERTIES_COLUMNS = [
  {
    dataField: 'groupLabel',
    caption: 'Properties -',
    groupIndex: 0,
    allowEditing: false
  },
  { dataField: 'name' },
  {
    dataField: 'friendlyName',
    caption: 'Label'
  }, {
    caption: 'Type', dataField: 'type',
    lookup: { dataSource: ['BOOLEAN', 'DATE', 'FLOAT', 'INTEGER', 'NUMBER', 'PICKLISTDOMAIN', 'TEXT', 'URL'] }
  },
  {
    caption: 'Length',
    dataField: 'precision',
    width: 80
  }, {
    dataField: 'pickListDomainId',
    caption: 'PickList Domain'
  }, {
    dataField: 'sortOrder',
    caption: 'SortOrder',
    width: 80
  },
  {
    caption: 'Validation Rule',
    cellTemplate: 'propertyValidationRuleTemplate',
    allowEditing: false,
    width: 100
  }];

export const CONFIG_PROPERTIES_FORMS = [{
  dataField: 'groupName',
  label: { text: 'Group Name' },
  editorOptions: {
    items: ['Batch', 'Base Fragment', 'Batch Component', 'Compound', 'Registry']
  },
  editorType: 'dxSelectBox',
  visible: false,
}, {
  dataField: 'name',
  label: { text: 'Name' },
  dataType: 'string',
  editorType: 'dxTextBox',
}, {
  dataField: 'friendlyName',
  label: { text: 'Label' },
  dataType: 'string',
  editorType: 'dxTextBox',
}, {
  dataField: 'type',
  label: { text: 'Type' },
  editorOptions: {
    items: ['BOOLEAN', 'DATE', 'FLOAT', 'INTEGER', 'NUMBER', 'PICKLISTDOMAIN', 'TEXT', 'URL']
  },
  editorType: 'dxSelectBox'
}, {
  dataField: 'precision',
  label: { text: 'Length', visible: false },
  editorOptions: { showSpinButtons: true, showClearButton: true, visible: false },
  dataType: 'Number',
  editorType: 'dxNumberBox'
}, {
  dataField: 'pickListDomainId',
  label: { text: 'PickList Domain' },
  visible: false,
  editorType: 'dxSelectBox'
}];

export class CCONFIGPROPERTIESFORMSDATA {
  name: string;
  groupName: string;
  friendlyName: string;
  type: string;
  precision: string;
  pickListDomainId: string;
};

export class CConfigProperties {
  columns: any;
  formColumns: any;
  formData: CCONFIGPROPERTIESFORMSDATA;
  window: CWindow;
  constructor(state: IAppState) {
    this.window = { title: 'Manage Data Properties', viewIndex: 'list' };
    this.columns = CONFIG_PROPERTIES_COLUMNS;
    this.formColumns = CONFIG_PROPERTIES_FORMS;
    this.formData = new CCONFIGPROPERTIESFORMSDATA();
    this.columns[5].lookup = { dataSource: [], valueExpr: 'ID', displayExpr: 'DESCRIPTION' };
    this.columns[5].lookup.dataSource = getLookups(state).pickListDomains;
    this.formColumns[5].editorOptions = { dataSource: [], valueExpr: 'ID', displayExpr: 'DESCRIPTION' };
    this.formColumns[5].editorOptions.dataSource = getLookups(state).pickListDomains;
  }
  clearFormData() {
    this.formData = new CCONFIGPROPERTIESFORMSDATA();
  }
  addEditProperty(w: string, d?: any) {
    if (w === 'add') {
      this.formColumns[0].visible = true;
      this.window = { title: 'Add New Property', viewIndex: w };
      this.formColumns[5].visible = false;
      this.formColumns[4].label.visible = false;
      this.formColumns[4].editorOptions.visible = false;
    }
    if (w === 'edit') {
      this.formColumns[0].visible = false;
      this.window = { title: 'Edit Data Properties (' + d.groupLabel + ':-' + d.name + ')', viewIndex: w };
      this.formData = d;
      this.showHideDataFields(d.type, this.formColumns, this.formData);
    }
  }

  showHideDataFields(type: string, vm: any, data: any, component?: any, refresh?: boolean) {
    if (type === 'PICKLISTDOMAIN') {
      vm[5].visible = true;
      vm[4].label.visible = false;
      vm[4].editorOptions.visible = false;
      data[4] = undefined;
      if (refresh) { component._refresh(); }
    }
    if (type === 'TEXT' || type === 'NUMBER') {
      if (type === 'TEXT') {
        vm[4].label.text = 'Length';
      } else {
        vm[4].label.text = 'Precision';
      }
      vm[4].label.visible = true;
      vm[4].editorOptions.visible = true;
      vm[5].visible = false;
      data[5] = undefined;
      if (refresh) { component._refresh(); }
    }
  }
}

export interface CWindow {
  title: string;
  viewIndex: string; // = 'list' || 'add' || 'edit' || 'validation';
}
