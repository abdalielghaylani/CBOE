import { IAppState } from '../../store';
import { notify, notifyError, notifySuccess } from '../../common';
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
export const CONFIG_PROPERTIES_VALIDATION_FORM_COLUMNS = {
  grdColumns: [{
    dataField: 'name', width: 100
  },
  { dataField: 'min', width: 80 },
  { dataField: 'max', width: 80 },
  { dataField: 'maxLength', width: 80 },
  { dataField: 'error' },
  { dataField: 'defaultValue', width: 80 },
  { dataField: 'parameters', cellTemplate: 'parameterTemplate' }],
  property: [
    {
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
    }
  ],
  editColumn: [{
    dataField: 'name',
    label: { text: 'Type' },
    dataType: 'string',
    editorType: 'dxSelectBox',
    editorOptions: {
      items: ['requiredField', 'textLength', 'wordListEnumeration', 'custom', 'notEmptyStructure', 'notEmptyStructureAndNoText']
    }, isRequired: true
  }, {
    dataField: 'error',
    label: { text: 'Error' },
    dataType: 'string',
    editorType: 'dxTextBox'
  }, {
    dataField: 'defaultValue',
    label: { text: 'Default Value TEXT' },
    dataType: 'string',
    editorType: 'dxTextBox',
    visible: false,
    isRequired: true
  }, {
    dataField: 'clientScript',
    label: { text: 'Client Script' },
    dataType: 'string',
    editorType: 'dxTextArea',
    visible: false,
    isRequired: true
  }]
};

export const CONFIG_PROPERTIES_FORMS = [{
  dataField: 'groupName',
  label: { text: 'Group Name' },
  editorType: 'dxSelectBox',
  validationRules: [{ group: 'always', type: 'required', message: 'Group name required' }],
  disabled: true,
}, {
  dataField: 'name',
  label: { text: 'Name' },
  dataType: 'string',
  editorType: 'dxTextBox',
  validationRules: [{ group: 'always', type: 'required', message: 'Name required' },
  {
    group: 'always', type: 'pattern', pattern: '^[a-zA-Z_@#][a-zA-Z0-9_\\$@#]{0,28}[a-zA-Z0-9_\\$@#]$',
    message: 'Invalid property name: use only alpha-numeric and underscore characters (no spaces, 30 characters max)'
  }
  ],
  disabled: true,
}, {
  dataField: 'friendlyName',
  label: { text: 'Label' },
  dataType: 'string',
  validationRules: [{ group: 'always', type: 'required', message: 'Friendly name required' },
  {
    group: 'always', type: 'pattern',
    pattern: /^[a-zA-Z0-9.\-_,;:\?!\[\]\{\}\(\)][a-zA-Z0-9\s.\-_,;:\?!\[\]\{\}\(\)]{0,28}[a-zA-Z0-9.\-_,;:\?!\[\]\{\}\(\)]$/,
    message: 'Invalid label text: some punctuation characters not allowed (30 characters max)'
  }],
  editorType: 'dxTextBox',
}, {
  dataField: 'type',
  label: { text: 'Type' },
  editorOptions: {
    items: ['BOOLEAN', 'DATE', 'FLOAT', 'INTEGER', 'NUMBER', 'PICKLISTDOMAIN', 'TEXT', 'URL']
  },
  editorType: 'dxSelectBox',
  validationRules: [{ group: 'always', type: 'required', message: 'Type required' }],
  disabled: true,
}, {
  dataField: 'precision',
  label: { text: 'Length', visible: false },
  editorOptions: { showSpinButtons: true, showClearButton: true, visible: false },
  validationRules: [{ group: 'length', type: 'required', message: 'Length required' },
  { group: 'length', type: 'pattern', pattern: '^[0-9]+$', message: 'Length should be a positive number with out decimal' }],
  validationGroup: 'precision',
  dataType: 'Number',
  editorType: 'dxNumberBox'
}, {
  dataField: 'pickListDomainId',
  label: { text: 'PickList Domain' },
  visible: false,
  validationRules: [{ group: 'picklist', type: 'required', message: 'PickList Domain required' }],
  validationGroup: 'picklist',
  editorType: 'dxSelectBox'
}, {
  dataField: 'precision',
  label: { text: 'Precision', visible: false },
  editorOptions: { showClearButton: true, visible: false },
  validationRules: [{
    group: 'precision', type: 'required', message: 'Precision required'
  },
  { group: 'precision', type: 'pattern', pattern: '^[0-9]+\.[0-9]+$', message: 'Precision required ( Eg:5.2 )' },
  ],
  validationGroup: 'number',
  dataType: 'Number',
  editorType: 'dxNumberBox'
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
  min: number;
  max: number;
  maxLength: number;
  error: string;
  defaultValue: string;
  parameters: any = [];
  clientScript: string;
  validWord: string;
}

export class CPropertiesValidationFormDataModel {
  name: string;
  min: number;
  max: number;
  maxLength: number;
  error: string;
  defaultValue: string;
  parameters: any = [];
}

export class CConfigProperties {
  columns: any;
  formColumns: any;
  formData: CConfigPropertiesFormData;
  formDataValidation: CPropertiesValidationFormData;
  window: CWindow;
  addRuleVisible: boolean = false;
  formValidationColumns: any;
  validationGridColumns: any;
  constructor(state: IAppState) {
    this.window = { title: 'Manage Data Properties', viewIndex: 'list' };
    this.columns = this.buildPropertiesColumnConfig(state);
    this.formValidationColumns = CONFIG_PROPERTIES_VALIDATION_FORM_COLUMNS;
    this.formColumns = CONFIG_PROPERTIES_FORMS;
    this.formData = new CConfigPropertiesFormData();
    this.formDataValidation = new CPropertiesValidationFormData();
    this.formColumns[0].editorOptions = { dataSource: [], valueExpr: 'groupName', displayExpr: 'groupLabel' };
    this.formColumns[0].editorOptions.dataSource = getLookups(state).propertyGroups;
    this.formColumns[5].editorOptions = { dataSource: [], valueExpr: 'ID', displayExpr: 'DESCRIPTION' };
    this.formColumns[5].editorOptions.dataSource = getLookups(state).pickListDomains;
  }

  addParams(n: string, v: string) {
    if (isBlank(v)) {
      notify(`Please enter the parameter value!`, 'warning', 5000);
    } else {
      if (n === 'min' || n === 'max') {
        if (this.formDataValidation.parameters.find(myObj => myObj.name === 'min' && n === 'min') ||
          (this.formDataValidation.parameters.find(myObj => myObj.name === 'max' && n === 'max'))) {
          notifyError(`Parameter already added! Delete the existing to add new`, 5000);
        } else {
          this.formDataValidation.parameters.push({ name: n, value: v });
        }
      }
      if (n === 'validWord') {
        if (this.formDataValidation.parameters.find(myObj => myObj.validWord === v)) {
          notifyError(`Parameter already added!`, 5000);
        } else {
          this.formDataValidation.parameters.push({ name: n, value: v });
        }
      }
    }
  }

  combuteValidation(broken: any) {
    if (!this.filterRule(broken, 'always')) {
      return false;
    }
    switch (this.formData.type) {
      case 'PICKLISTDOMAIN':
        return this.filterRule(broken, 'picklist');
      case 'TEXT':
        return this.filterRule(broken, 'length');
      case 'NUMBER':
        return this.filterRule(broken, 'precision');
      case 'BOOLEAN':
      case 'DATE':
      case 'FLOAT':
      case 'INTEGER':
      case 'URL':
        return true;
    }
    return true;
  }

  filterRule(broken: any, param: string) {
    let val = broken.filter(function (obj) {
      return obj.group === param;
    });
    return val.length > 0 ? false : true;
  }

  isValidRule(): boolean {
    if (!this.formDataValidation.name) {
      notify(`Type required!`, 'warning', 5000);
      return false;
    }
    if (this.formDataValidation.name === 'requiredField'
      && (isBlank(this.formDataValidation.defaultValue))) {
      notify(`Default value required!`, 'warning', 5000);
      return false;
    }
    if (this.formDataValidation.name === 'textLength' && this.formDataValidation.parameters.length !== 2) {
      notify(`Min, Max parameters required!`, 'warning', 5000);
      return false;
    }
    if (this.formDataValidation.name === 'wordListEnumeration' && this.formDataValidation.parameters.length <= 0) {
      notify(`Atleast one parameter required!`, 'warning', 5000);
      return false;
    }
    if (this.formDataValidation.name === 'custom' && isBlank(this.formDataValidation.clientScript)) {
      notify(`Client Script required!`, 'warning', 5000);
      return false;
    }
    return true;
  }

  addValidationRule(e) {
    this.addRuleVisible = true;
  }

  onParameterItemDeleted(e) {
    this.formDataValidation.parameters.splice(e.itemIndex, 1);
  }

  clearFormDataValidations() {
    this.formDataValidation = new CPropertiesValidationFormData();
    this.addRuleVisible = false;
  }

  clearFormData() {
    this.formData = new CConfigPropertiesFormData();
  }

  addEditProperty(w: string, d?: any) {
    if (w === 'add') {
      this.formColumns[0].disabled = false;
      this.formColumns[1].disabled = false;
      this.formColumns[2].disabled = false;
      this.formColumns[3].disabled = false;
      this.window = { title: 'Add New Property', viewIndex: w };
    }
    if (w === 'edit') {
      this.formColumns[0].disabled = true;
      this.formColumns[1].disabled = true;
      this.formColumns[2].disabled = true;
      this.formColumns[3].disabled = true;
      this.window = { title: 'Edit Property', viewIndex: w };
      this.formData = d;
      this.showHideDataFields({ dataField: 'type', value: this.formData.type });
    }
  }

  onValidationTypeChanged(e) {
    if (e.dataField === 'name') {
      this.formValidationColumns.editColumn[2].visible = false;
      this.formValidationColumns.editColumn[3].visible = false;
      this.formDataValidation.parameters = [];
      switch (e.value) {
        case 'requiredField':
          this.formValidationColumns.editColumn[2].visible = true;
          e.component._refresh();
          break;
        case 'textLength':
        case 'wordListEnumeration':
        case 'notEmptyStructure':
        case 'notEmptyStructureAndNoText':
          e.component._refresh();
          break;
        case 'custom':
          this.formValidationColumns.editColumn[3].visible = true;
          e.component._refresh();
          break;
      }
    }
  }

  showHideDataFields(e) {
    if (e.dataField === 'type') {
      this.formColumns[5].visible = false;
      this.formColumns[4].label.visible = false;
      this.formColumns[4].editorOptions.visible = false;
      this.formColumns[6].label.visible = false;
      this.formColumns[6].editorOptions.visible = false;
      switch (e.value) {
        case 'PICKLISTDOMAIN':
          this.formColumns[5].visible = true;
          this.formColumns[4].label.visible = false;
          this.formColumns[4].editorOptions.visible = false;
          if (e.component) { e.component._refresh(); }
          break;
        case 'TEXT':
          this.formColumns[4].label.visible = true;
          this.formColumns[4].editorOptions.visible = true;
          this.formColumns[5].visible = false;
          this.formColumns[6].label.visible = false;
          this.formColumns[6].editorOptions.visible = false;
          if (e.component) { e.component._refresh(); }
          break;
        case 'NUMBER':
          this.formColumns[6].label.visible = true;
          this.formColumns[6].editorOptions.visible = true;
          this.formColumns[4].label.visible = false;
          this.formColumns[4].editorOptions.visible = false;
          this.formColumns[5].visible = false;
          if (e.component) { e.component._refresh(); }
          break;
        case 'BOOLEAN':
        case 'DATE':
        case 'FLOAT':
        case 'INTEGER':
        case 'URL':
          if (e.component) { e.component._refresh(); }
          break;
      }
    }
  }

  private buildPropertiesColumnConfig(state: IAppState): any[] {
    return [{
      dataField: 'groupLabel',
      caption: 'Properties',
      groupIndex: 0,
      allowEditing: false
    }, {
      dataField: 'name',
      allowSorting: false
    }, {
      dataField: 'friendlyName',
      caption: 'Label',
      allowSorting: false
    }, {
      caption: 'Type', dataField: 'type',
      allowSorting: false,
      lookup: { dataSource: ['BOOLEAN', 'DATE', 'FLOAT', 'INTEGER', 'NUMBER', 'PICKLISTDOMAIN', 'TEXT', 'URL'] },
      width: 120
    }, {
      caption: 'Length',
      dataField: 'precision',
      allowSorting: false,
      width: 80
    }, {
      dataField: 'pickListDomainId',
      caption: 'PickList Domain',
      allowSorting: false,
      width: 120,
      lookup: { dataSource: getLookups(state).pickListDomains, valueExpr: 'ID', displayExpr: 'DESCRIPTION' }
    }, {
      dataField: 'sortOrder',
      caption: 'Sort Order',
      allowFiltering: false,
      allowSorting: false,
      cellTemplate: 'sortOrderTemplate',
      width: 80
    }, {
      caption: 'Validation Rule',
      cellTemplate: 'propertyValidationRuleTemplate',
      allowEditing: false,
      allowSorting: false,
      width: 100
    }];
  }
}

function isBlank(str) {
  return (!str || /^\s*$/.test(str));
}
export const CONFIG_ADDIN_COLUMNS = {
  grdColumn: [{
    dataField: 'name',
    caption: 'Current Addins',
    width: 200
  }, {
    caption: 'AddIn,Assembly',
    cellTemplate: 'addInTemplate'
  }, {
    dataField: 'events',
    caption: 'Event List (Event Name = Event Handler)',
    cellTemplate: 'eventTemplate'
  }],
  editColumn: {
    addIn: [{
      dataField: 'name',
      label: { text: 'Friendly Name' },
      validationRules: [{ group: 'always', type: 'required', message: 'Friendly Name required' }],
      editorType: 'dxTextBox'
    },
    {
      dataField: 'assembly',
      label: { text: 'Assembly' },
      validationRules: [{ group: 'always', type: 'required', message: 'Name required' }],
      editorType: 'dxSelectBox'
    },
    {
      dataField: 'className',
      label: { text: 'Class' },
      validationRules: [{ group: 'always', type: 'required', message: 'Class name required' }],
      editorType: 'dxSelectBox'
    }],
    addInInfo: [{
      dataField: 'name',
      label: { text: 'Friendly Name' },
      editorType: 'dxTextBox',
      editorOptions: { disabled: true }
    }, {
      dataField: 'addinName',
      label: { text: 'Name' },
      editorType: 'dxTextBox',
      editorOptions: { disabled: true }
    },
    {
      dataField: 'assembly',
      label: { text: 'Assembly' },
      editorType: 'dxTextBox',
      editorOptions: { disabled: true }
    },
    {
      dataField: 'className',
      label: { text: 'Class' },
      editorType: 'dxTextBox',
      editorOptions: { disabled: true }
    },
    {
      dataField: 'enable',
      colSpan: 2,
      label: { text: 'Enabled' },
      editorType: 'dxCheckBox',
    },
    {
      dataField: 'required',
      label: { text: 'Required' },
      editorType: 'dxCheckBox',
      editorOptions: { disabled: true }
    }],
    events: [{
      dataField: 'eventName',
      caption: 'Event',
      editorType: 'dxSelectBox',
      dataType: String,
      width: 200
    }, {
      dataField: 'eventHandler',
      caption: 'Handler',
      editorType: 'dxSelectBox',
      dataType: String,
      width: 200
    }],
    config: [{
      dataField: 'Configuration',
      label: { visible: false },
      editorType: 'dxTextArea',
      editorOptions: { height: 150 }
    }],
  }
};

export class CAddInModel {
  addinName: string;
  className: string;
  Configuration: string;
  events: any = [];
}

export class CConfigAddIn {
  columns: any;
  editRow: any;
  window: CWindow;
  configView: string;
  configVal: string;
  addinAssemblies: any;
  currentEvents: any;
  constructor(state: IAppState) {
    this.columns = CONFIG_ADDIN_COLUMNS;
    this.window = { title: 'Manage Addins', viewIndex: 'list' };
    this.addinAssemblies = getLookups(state).addinAssemblies;
    this.columns.editColumn.addIn[1].editorOptions = { items: [this.addinAssemblies[0].name], value: this.addinAssemblies[0].name };
    this.columns.editColumn.addIn[2].editorOptions = { dataSource: this.addinAssemblies[0].classes, valueExpr: 'name', displayExpr: 'name' };
  }
  addEditProperty(op, e) {
    if (op === 'edit') {
      this.window = { title: 'Edit Addins', viewIndex: 'edit' };
      this.editRow = e.data;
      this.configView = 'view';
      this.configVal = this.editRow.Configuration;
      this.currentEvents = this.addinAssemblies[0].classes.filter
        (s => s.name === e.data.addinName.replace('CambridgeSoft.COE.Registration.Services.RegistrationAddins.', ''));
      this.columns.editColumn.events[1].lookup = { dataSource: this.currentEvents[0].eventHandlers, valueExpr: 'name', displayExpr: 'name' };
    }
    if (op === 'add') {
      this.editRow = new CAddInModel();
      this.columns.editColumn.events[1].lookup = {};
      this.window = { title: 'Add Addins', viewIndex: 'add' };
      this.configView = 'edit';
    }
    this.columns.editColumn.events[0].lookup = {
      dataSource: ['Loaded', 'Inserting', 'Inserted', 'Updating', 'Updated', 'Registering', 'UpdatingPerm', 'Saving', 'Saved', 'PropertyChanged']
    };
  }
  saveConfig() {
    this.configView = 'view';
  }
  editConfig() {
    this.configView = 'edit';
    this.configVal = this.editRow.Configuration;
  }
  cancelEditConfig(config) {
    this.configView = 'view';
    this.editRow.Configuration = this.configVal;
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

  private isSettingTrue(settingName: string): boolean {
    let setting = this.getRegSetting(settingName);
    return setting && setting.value && setting.value.toLowerCase() === 'true';
  }

  public get isApprovalsEnabled(): boolean {
    return this.isSettingTrue('ApprovalsEnabled');
  }

  public get isSubmissionTemplateEnabled(): boolean {
    return this.isSettingTrue('EnableSubmissionTemplates');
  }
}
