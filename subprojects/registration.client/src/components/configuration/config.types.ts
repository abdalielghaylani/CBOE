import { IAppState, ILookupData, ICustomTableData, IValidationRuleData } from '../../redux';
import { notify, notifyError, notifySuccess } from '../../common';
import { CValidator } from '../common';

export class CConfigTable {
  columns: any[];
  pickListDomains: any[];
  window: IWindow;
  editMode: string = 'form';
  tableName: string;
  formData: any = {};
  formColumns: any[];
  editFormOptions = {
    showRequiredMark: true,
    requiredMark: '*',
    items: []
  };
  projectType = [{ key: 'A', name: 'All' }, { key: 'R', name: 'Registry' }, { key: 'B', name: 'Batch' }];
  identifierType = [
    { key: 'A', name: 'All' },
    { key: 'R', name: 'Registry' },
    { key: 'B', name: 'Batch' },
    { key: 'C', name: 'Compound' },
    { key: 'S', name: 'Base Fragment' }
  ];
  sequenceType = [{ key: 'A', name: 'All' }, { key: 'R', name: 'Registry' }, { key: 'C', name: 'Compound' }];
  activeType = [{ key: 'T', name: 'Yes' }, { key: 'F', name: 'No' }];
  constructor(tableId: string, tableName: string, customTableData: ICustomTableData) {
    this.tableName = tableName;
    this.window = { title: this.tableName, viewIndex: 'list' };
    if (customTableData.config) {
      this.columns = customTableData.config;
      this.columns[0].allowEditing = false;
      this.columns[0].formItem = { visible: false };
      this.setDataType();
      this.setOtherConfig(tableId);
      this.setValidationRule(tableId);
      this.editFormOptions.items = this.columns.map(c => {
        return {
          dataField: c.dataField,
          validationRules: c.validationRules
        };
      });
    }
  }

  protected setDataType() {
    this.columns.forEach(c => {
      c.dataType = c.dataType === 'Double' ? 'number' : 'string';
    });
  }

  protected validateStringLength(options): boolean {
    let length = options.value == null ? 0 : options.value.toString().length;
    if (length === 0) {
      return true;
    }
    let rule = options.rule;
    return (rule.min == null || length >= rule.min) && (rule.max == null || length <= rule.max);
  }

  protected setValidationRule(tableId: string) {
    this.columns.forEach(c => {
      if (c.validationRules) {
        let rulesFromServer = c.validationRules as IValidationRuleData[];
        let validationRules = CValidator.getValidationRules(rulesFromServer);
        c.validationRules = validationRules.map(r => {
          if (r.type === 'stringLength') {
            r.type = 'custom';
            r.validationCallback = this.validateStringLength;
          }
          return r;
        });
      }
    });
    this.columns.forEach(c => {
      if (c.lookup) {
        if (c.validationRules == null || c.validationRules.find(r => r.type === 'required') == null) {
          c.editorOptions = { showClearButton: true };
        }
      }
    });
  }

  setRequiredValidations(items: string[]) {
    items.forEach(col => {
      let column = this.columns.find(i => i.dataField === col);
      if (column != null) {
        column.validationRules = [{ type: 'required', message: `'${column.caption}' is a required field` }];
      }
    });
  }

  cancel(e) {
    this.window = { title: this.tableName, viewIndex: 'list' };
  }

  addEdit(e, type) {
    this.formColumns = this.columns;
    this.formColumns.find(i => i.dataField === 'STRUCTURE').template = 'fragmentStructureTemplate';
    this.formColumns.forEach(col => {
      if (col.caption) {
        col.label = { text: col.caption };
      }
      if (col.lookup) {
        col.editorType = 'dxSelectBox';
        col.editorOptions = col.lookup;
      }
    });
    if (type === 'add') {
      this.formData = {};
      this.window = { title: 'Add ' + this.tableName, viewIndex: type };
    } else {
      this.formData = e.data;
      this.window = { title: 'Edit ' + this.tableName, viewIndex: type };
    }
  }

  setOtherConfig(tableId: string) {
    switch (tableId) {
      case 'VW_PROJECT':
        this.setColumnConfig('ACTIVE', 'lookup', { dataSource: this.activeType, displayExpr: 'name', valueExpr: 'key' });
        this.setColumnConfig('TYPE', 'lookup', { dataSource: this.projectType, displayExpr: 'name', valueExpr: 'key' });
        break;
      case 'VW_PICKLIST':
        this.setColumnConfig('ACTIVE', 'lookup', { dataSource: this.activeType, displayExpr: 'name', valueExpr: 'key' });
        this.setColumnConfig('DESCRIPTION', 'cellTemplate', 'viewTemplate');
        break;
      case 'VW_PICKLISTDOMAIN':
        this.setColumnConfig('LOCKED', 'lookup', { dataSource: this.activeType, displayExpr: 'name', valueExpr: 'key' });
        this.setColumnConfig('LOCKED', 'allowEditing', false);
        this.setColumnConfig('EXT_TABLE', 'editCellTemplate', 'disabledTemplate');
        this.setColumnConfig('EXT_ID_COL', 'editCellTemplate', 'disabledTemplate');
        this.setColumnConfig('EXT_DISPLAY_COL', 'editCellTemplate', 'disabledTemplate');
        this.setColumnConfig('EXT_SQL_FILTER', 'editCellTemplate', 'disabledTemplate');
        break;
      case 'VW_NOTEBOOKS':
        this.setColumnConfig('ACTIVE', 'lookup', { dataSource: this.activeType, displayExpr: 'name', valueExpr: 'key' });
        this.setColumnConfig('USER_CODE', 'cellTemplate', 'viewTemplate');
        break;
      case 'VW_FRAGMENT':
        this.setColumnConfig('DESCRIPTION', 'cellTemplate', 'viewTemplate');
        this.setColumnConfig('FRAGMENTID', 'allowEditing', false);
        break;
      case 'VW_IDENTIFIERTYPE':
        this.setColumnConfig('ACTIVE', 'lookup', { dataSource: this.activeType, displayExpr: 'name', valueExpr: 'key' });
        this.setColumnConfig('TYPE', 'lookup', { dataSource: this.identifierType, displayExpr: 'name', valueExpr: 'key' });
        break;
      case 'VW_SEQUENCE':
        this.setColumnConfig('ACTIVE', 'lookup', { dataSource: this.activeType, displayExpr: 'name', valueExpr: 'key' });
        this.setColumnConfig('TYPE', 'lookup', { dataSource: this.sequenceType, displayExpr: 'name', valueExpr: 'key' });
        this.setColumnConfig('REGNUMBERLENGTH', 'lookup', this.getPaddedLength('R'));
        this.setColumnConfig('BATCHNUMLENGTH', 'lookup', this.getPaddedLength('B'));
        break;

    }
  }

  getPaddedLength(type: string) {
    let data = {
      dataSource: [],
      displayExpr: 'name', valueExpr: 'key'
    };
    if (type === 'R') {
      for (let i = 4; i <= 9; i++) {
        data.dataSource.push({ key: i, name: i + ' digits' });
      }
    }
    if (type === 'B') {
      data.dataSource.push({ key: -1, name: 'No padding' });
      for (let i = 2; i <= 6; i++) {
        data.dataSource.push({ key: i, name: i + ' digits' });
      }
    }
    return data;
  }

  setColumnConfig(field: string, property: string, value: any) {
    let column = this.columns.find(i => i.dataField === field);
    if (column != null) {
      column[property] = value;
    }
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
    dataField: 'label',
    allowEditing: true,
    validationRules:
    [
      {
        group: 'label', type: 'required',
        message: 'Invalid label text: Label can have a maximum of 30 characters and may not contain (~,@,#,$,%,^,&,*,\',\,<,>,=,+)'
      },
      {
        group: 'label', type: 'pattern',
        message: 'Invalid label text: Label can have a maximum of 30 characters and may not contain (~,@,#,$,%,^,&,*,\',\,<,>,=,+)',
        pattern: /^[a-zA-Z0-9.\-_,;:\?!\[\]\{\}\(\)][a-zA-Z0-9\s.\-_,;:\?!\[\]\{\}\(\)]{0,28}[a-zA-Z0-9.\-_,;:\?!\[\]\{\}\(\)]$/
      }
    ]
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
    template: 'validationTypeTemplate',
    dataType: 'string',
    isRequired: true
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

export class CPropertiesValidationFormData implements IValidationRuleData {
  name: string = '';
  min: number;
  max: number;
  maxLength: number;
  error: string = '';
  defaultValue: string;
  parameters: any[] = [];
  clientScript: string;
  validWord: string;
}

export class CPropertiesValidationFormDataModel implements IValidationRuleData {
  name: string = '';
  min: number;
  max: number;
  maxLength: number;
  error: string = '';
  defaultValue: string;
  parameters: any[] = [];
}

export class CConfigProperties {
  columns: any;
  formColumns: any;
  formData: CConfigPropertiesFormData;
  formDataValidation: CPropertiesValidationFormData;
  window: IWindow;
  addRuleVisible: boolean = false;
  formValidationColumns: any;
  validationGridColumns: any;
  constructor(lookups: ILookupData) {
    this.window = { title: 'Manage Data Properties', viewIndex: 'list' };
    this.columns = this.buildPropertiesColumnConfig(lookups);
    this.formValidationColumns = CONFIG_PROPERTIES_VALIDATION_FORM_COLUMNS;
    this.formColumns = CONFIG_PROPERTIES_FORMS;
    this.formData = new CConfigPropertiesFormData();
    this.formDataValidation = new CPropertiesValidationFormData();
    this.formColumns[0].editorOptions = { dataSource: [], valueExpr: 'groupName', displayExpr: 'groupLabel' };
    this.formColumns[0].editorOptions.dataSource = lookups.propertyGroups;
    this.formColumns[5].editorOptions = { dataSource: [], valueExpr: 'ID', displayExpr: 'DESCRIPTION' };
    this.formColumns[5].editorOptions.dataSource = lookups.pickListDomains;
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
      notify(`Type is required!`, 'warning', 5000);
      return false;
    }
    if (this.formDataValidation.name === 'requiredField'
      && (isBlank(this.formDataValidation.defaultValue))) {
      notify(`Default value is required!`, 'warning', 5000);
      return false;
    }
    if (this.formDataValidation.name === 'textLength' && this.formDataValidation.parameters.length !== 2) {
      notify(`Min, Max parameters are required!`, 'warning', 5000);
      return false;
    }
    if (this.formDataValidation.name === 'wordListEnumeration' && this.formDataValidation.parameters.length <= 0) {
      notify(`At least one parameter is required!`, 'warning', 5000);
      return false;
    }
    if (this.formDataValidation.name === 'custom' && isBlank(this.formDataValidation.clientScript)) {
      notify(`Client script is required!`, 'warning', 5000);
      return false;
    }
    if (this.formData.validationRules.filter(validationRule =>
      validationRule.name === this.formDataValidation.name).length > 0) {
      notify(`The Validation Rule that you are trying to add already exists!`, 'warning', 5000);
      return false;
    }
    return true;
  }

  addValidationRule(e) {
    this.formDataValidation = new CPropertiesValidationFormData();
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
    this.formDataValidation = new CPropertiesValidationFormData();
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
        case '':
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

  private buildPropertiesColumnConfig(lookups: ILookupData): any[] {
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
      lookup: { dataSource: lookups.pickListDomains, valueExpr: 'ID', displayExpr: 'DESCRIPTION' }
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
  window: IWindow;
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

export interface IWindow {
  title: string;
  viewIndex: string; // = 'list' || 'add' || 'edit' || 'validation';
}
