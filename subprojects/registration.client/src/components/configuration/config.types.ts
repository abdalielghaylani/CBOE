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
export const CONFIG_PROPERTIES_VALIDATION_FORM_COLUMNS = [{
  dataField: 'name',
  label: { text: 'Property Name' },
  dataType: 'string',
  editorType: 'dxTextBox',
  disabled: true,
}, {
  dataField: 'type',
  label: { text: 'Property Type' },
  dataType: 'string',
  editorType: 'dxTextBox',
  disabled: true,
}];
export const PROPERTIES_VALIDATION_GRID_COLUMNS = [{
  dataField: 'name', width: 100
},
{ dataField: 'min', width: 80 },
{ dataField: 'max', width: 80 },
{ dataField: 'maxLength', width: 80 },
{ dataField: 'error' },
{ dataField: 'defaultValue', width: 80 },
{ dataField: 'parameters', cellTemplate: 'parameterTemplate' }];
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
  editorType: 'dxSelectBox',
  disabled: true,
}, {
  dataField: 'name',
  label: { text: 'Name' },
  dataType: 'string',
  editorType: 'dxTextBox',
  disabled: true,
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
  editorType: 'dxSelectBox',
  disabled: true,
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

export class CConfigPropertiesFormData {
  name: string;
  groupName: string;
  groupLabel: string;
  friendlyName: string;
  type: string;
  precision: string;
  validationRules: any[];
};
export class CPropertiesValidationFormData {
  name: string;
  type: string;
}
export class CConfigProperties {
  columns: any;
  formColumns: any;
  formData: CConfigPropertiesFormData;
  window: CWindow;
  formValidationColumns: any;
  validationGridColumns: any;
  constructor(state: IAppState) {
    this.window = { title: 'Manage Data Properties', viewIndex: 'list' };
    this.columns = CONFIG_PROPERTIES_COLUMNS;
    this.formValidationColumns = CONFIG_PROPERTIES_VALIDATION_FORM_COLUMNS;
    this.validationGridColumns = PROPERTIES_VALIDATION_GRID_COLUMNS;
    this.formColumns = CONFIG_PROPERTIES_FORMS;
    this.formData = new CConfigPropertiesFormData();
    this.formColumns[0].editorOptions = { dataSource: [], valueExpr: 'groupName', displayExpr: 'groupLabel' };
    this.formColumns[0].editorOptions.dataSource = getLookups(state).propertyGroups;
    this.columns[5].lookup = { dataSource: [], valueExpr: 'ID', displayExpr: 'DESCRIPTION' };
    this.columns[5].lookup.dataSource = getLookups(state).pickListDomains;
    this.formColumns[5].editorOptions = { dataSource: [], valueExpr: 'ID', displayExpr: 'DESCRIPTION' };
    this.formColumns[5].editorOptions.dataSource = getLookups(state).pickListDomains;
  }
  clearFormData() {
    this.formData = new CConfigPropertiesFormData();
  }
  addEditProperty(w: string, d?: any) {
    if (w === 'add') {
      this.formColumns[0].disabled = false;
      this.formColumns[1].disabled = false;
      this.formColumns[3].disabled = false;
      this.window = { title: 'Add New Property', viewIndex: w };
      this.formColumns[5].visible = false;
      this.formColumns[4].label.visible = false;
      this.formColumns[4].editorOptions.visible = false;
    }
    if (w === 'edit') {
      this.formColumns[0].disabled = true;
      this.formColumns[1].disabled = true;
      this.formColumns[3].disabled = true;
      this.window = { title: 'Edit Property', viewIndex: w };
      this.formData = d;
      this.showHideDataFields(d.type, this.formColumns, this.formData);
    }
  }

  showHideDataFields(type: string, vm: any, data: any, component?: any, refresh?: boolean) {
    if (type === 'PICKLISTDOMAIN') {
      vm[5].visible = true;
      vm[4].label.visible = false;
      vm[4].editorOptions.visible = false;
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
      if (refresh) { component._refresh(); }
    }
  }
}

export interface CWindow {
  title: string;
  viewIndex: string; // = 'list' || 'add' || 'edit' || 'validation';
}

export interface ISettingData {
  groupName: string;
  groupLabel: string;
  name: string;
  controlType: string;
  value: string;
  description: string;
  pikclistDatabaseName: string;
  allowedValues: string;
  processorClass: string;
  isHidden?: boolean;
}

export class CSystemSettings {
  constructor(private systemSettings: ISettingData[]) {
  }

  private getSetting(groupLabel: string, settingName: string): ISettingData {
    let settings = this.systemSettings.filter(s => s.groupLabel === groupLabel && s.name === settingName);
    return settings && settings.length === 1 ? settings[0] : null;
  }

  private getRegSetting(settingName: string): ISettingData {
    return this.getSetting('Registration', settingName);    
  }

  public get isApprovalsEnabled(): boolean {
    let setting = this.getRegSetting('ApprovalsEnabled');
    return setting && setting.value && setting.value.toLowerCase() === 'true';
  }
}
