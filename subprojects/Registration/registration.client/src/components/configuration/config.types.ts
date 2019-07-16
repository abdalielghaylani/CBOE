import { IAppState, ILookupData, CCustomTableData, ICustomTableData, IValidationRuleData } from '../../redux';
import { notifyWarning, notifyError, notifySuccess } from '../../common';
import { CValidator } from '../common';

export class CConfigTable {
  columns: any[];
  pickListDomains: any[];
  window: IWindow;
  editMode: string = 'form';
  tableName: string;
  editHeaderName: string;
  formData: any = {};
  formColumns: any[];
  editFormOptions = {
    showRequiredMark: true,
    requiredMark: '*',
    items: []
  };
  constructor(
    tableId: string = '',
    tableName: string = '',
    customTableData: ICustomTableData = new CCustomTableData()) {
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
        if (e.tableId === 'VW_FRAGMENT' && (col.dataField === 'MOLWEIGHT' || col.dataField === 'FORMULA')) {
          col.editorOptions = {};
          col.editorOptions.readOnly = true;
        }
      }
      if (col.lookup) {
        col.editorType = 'dxSelectBox';
        col.editorOptions = col.lookup;
      }
    });
    this.editHeaderName = this.tableName.replace('Fragments', 'Fragment');
    if (type === 'add') {
      this.formData = {};
      this.window = { title: 'Add ' + this.editHeaderName, viewIndex: type };
    } else {
      this.formData = e.data;
      this.window = { title: 'Edit ' + this.editHeaderName, viewIndex: type };
    }
  }

  setOtherConfig(tableId: string) {
    switch (tableId) {
      case 'VW_PICKLIST':
        this.setColumnConfig('DESCRIPTION', 'cellTemplate', 'viewTemplate');
        this.setColumnConfig('PICKLISTDOMAIN', 'caption', 'PICKLISTDOMAIN');
        break;
      case 'VW_PICKLISTDOMAIN':
        this.setColumnConfig('LOCKED', 'allowEditing', false);
        this.setColumnConfig('EXT_TABLE', 'editCellTemplate', 'disabledTemplate');
        this.setColumnConfig('EXT_ID_COL', 'editCellTemplate', 'disabledTemplate');
        this.setColumnConfig('EXT_DISPLAY_COL', 'editCellTemplate', 'disabledTemplate');
        this.setColumnConfig('EXT_SQL_FILTER', 'editCellTemplate', 'disabledTemplate');
        break;
      case 'VW_NOTEBOOKS':
        this.setColumnConfig('USER_CODE', 'cellTemplate', 'viewTemplate');
        break;
      case 'VW_FRAGMENT':
        this.setColumnConfig('STRUCTURE', 'width', 150);
        this.setColumnConfig('STRUCTURE', 'allowFiltering', false);
        this.setColumnConfig('STRUCTURE', 'allowSorting', false);
        this.setColumnConfig('STRUCTURE', 'cellTemplate', 'cellTemplate');
        this.setColumnConfig('DESCRIPTION', 'cellTemplate', 'viewTemplate');
        this.setColumnConfig('FRAGMENTID', 'allowEditing', false);
        this.setColumnConfig('FORMULA', 'allowEditing', false);
        this.setColumnConfig('MOLWEIGHT', 'allowEditing', false);
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
    groupIndex: 0,
    groupCellTemplate: 'groupCellTemplate',
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
  { dataField: 'min', width: 80, visible: false },
  { dataField: 'max', width: 80, visible: false },
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

export class CConfigPropertiesFormData {
  name: string;
  groupName: string;
  groupLabel: string;
  friendlyName: string;
  type: string;
  precision: string;
  pickListDomainId: string;
  validationRules: any[];
}

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
  pickListDomain: any;
  formData: CConfigPropertiesFormData;
  formDataValidation: CPropertiesValidationFormData;
  window: IWindow;
  addRuleVisible: boolean = false;
  formValidationColumns: any;
  validationGridColumns: any;
  precisionCache: string;
  constructor(lookups: ILookupData) {
    this.window = { title: 'Manage Data Properties', viewIndex: 'list' };
    this.columns = this.buildPropertiesColumnConfig(lookups);
    this.formValidationColumns = CONFIG_PROPERTIES_VALIDATION_FORM_COLUMNS;
    this.formColumns = this.getFormColumns();
    this.formData = new CConfigPropertiesFormData();
    this.formDataValidation = new CPropertiesValidationFormData();
    this.formColumns[0].editorOptions = { dataSource: [], valueExpr: 'groupName', displayExpr: 'groupLabel' };
    this.formColumns[0].editorOptions.dataSource = lookups.propertyGroups.filter(i => i.groupName.toLowerCase() !== 'batchcomponent');
    this.formColumns[5].editorOptions = { dataSource: [], valueExpr: 'ID', displayExpr: 'DESCRIPTION' };
    this.formColumns[5].editorOptions.dataSource = lookups.pickListDomains;
    this.pickListDomain = lookups.pickListDomains;
  }

  getFormColumns() {
    return [{
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
      validationRules: [{ group: 'always', type: 'required', message: 'Label required' },
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
      {
        group: 'precision', type: 'custom', parent: this, reevaluate: true,
        validationCallback: this.checkPrecision,
        message: 'You cannot reduce the precision or scale of a property. Please delete the property or create a new property.'
      },
      { group: 'precision', type: 'pattern', pattern: '^[0-9]+\.[0-9]+$', message: 'Precision required ( Eg:5.2 )' },
      ],
      validationGroup: 'number',
      dataType: 'Number',
      editorType: 'dxNumberBox'
    }];
  }

  checkPrecision() {
    const k: any = this;
    if (Number(k.parent.formData.precision) < Number(k.parent.precisionCache)) {
      return false;
    } else {
      return true;
    }
  }

  clearParams(p: string) {
    this.formDataValidation[p] = undefined;
  }

  addParams(n: string, v: string) {
    if (isBlank(v)) {
      notifyWarning(`Please enter the parameter value!`, 5000);
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
      this.clearParams(n);
    }
  }

  validate(broken: any) {
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

  getParamValue(params: any[], name: string) {
    const param = params.find(p => p.name === name);
    return param ? param.value : undefined;
  }

  isValidRule(): boolean {
    const dataValidation = this.formDataValidation;
    const params = dataValidation.parameters;
    const name = dataValidation.name;
    if (!name) {
      notifyWarning(`Type is required!`, 5000);
      return false;
    } else if (name === 'requiredField') {
      if (isBlank(dataValidation.defaultValue)) {
        notifyWarning(`Default value is required!`, 5000);
        return false;
      }
      if (this.formData.type === 'NUMBER') {
        try {
          this.formData.validationRules.forEach(vr => {
            let regexp;
            switch (vr.name) {
              case 'integer':
                regexp = new RegExp('^[0-9]+$');
                if (!regexp.test(dataValidation.defaultValue)) {
                  throw 'Default value must be an integer number!';
                }
                break;
              case 'textLength':
                regexp = new RegExp('^((?!(0))[0-9]{' + vr.parameters[0].value + ',' + vr.parameters[1].value + '})$');
                if (!regexp.test(dataValidation.defaultValue)) {
                  throw 'Default value can have between ' + vr.parameters[0].value + ' and ' + vr.parameters[1].value + ' characters!';
                }
                break;
              case 'float':
                regexp = new RegExp('^[0-9]+\.[0-9]+$');
                if (!regexp.test(dataValidation.defaultValue)) {
                  throw 'Default value must be a float number with at most ' + vr.parameters[0].value + ' integer and ' +
                  vr.parameters[1].value + ' decimal digits';
                }
                break;
            }
          });
        } catch (err) {
          notifyWarning(err, 5000);
          return false;
        }
      }
    } else if (name === 'textLength') {
      const minValue = dataValidation.min ? dataValidation.min : this.getParamValue(params, 'min');
      const maxValue = dataValidation.max ? dataValidation.max : this.getParamValue(params, 'max');
      if (!minValue || !maxValue) {
        notifyWarning(`Both Min and Max parameters are required!`, 5000);
        return false;
      }
      if (minValue > maxValue) {
        notifyWarning(`Max parameter must be greater than min parameter!`, 5000);
        return false;
      }
      if (dataValidation.min) {
        params.push({ name: 'min', value: dataValidation.min });
      }
      if (dataValidation.max) {
        params.push({ name: 'max', value: dataValidation.max });
      }
    } else if (name === 'wordListEnumeration') {
      if (params.length <= 0) {
        notifyWarning(`At least one parameter is required!`, 5000);
        return false;
      }
    } else if (name === 'custom') {
      if (isBlank(dataValidation.clientScript)) {
        notifyWarning(`Client script is required!`, 5000);
        return false;
      }
    }

    if (this.formData.validationRules.filter(validationRule => validationRule.name === name).length > 0) {
      notifyWarning(`The Validation Rule that you are trying to add already exists!`, 5000);
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
    this.clearFormDataValidations();
    this.showHideDataFields({ dataField: 'type', value: this.formData.type });
  }

  addEditProperty(w: string, d?: any) {
    this.formColumns[2].visible = w !== 'edit';
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
      this.precisionCache = this.formData.precision ? this.formData.precision : '';
    }
  }

  setValidationColumns(type: string, visible: boolean = false) {
    let c = this.formValidationColumns.editColumn.find(i => i.dataField === type);
    if (type === 'defaultValue') {
      c.editorType = 'dxTextBox';
      let t = this.formData.type.toLowerCase() === 'number' || this.formData.type.toLowerCase() === 'text';
      c.label.text = 'Default Value ' + (t ? this.formData.type : '');
      if (this.formData.type === 'PICKLISTDOMAIN') {
        c.editorType = 'dxSelectBox';
        c.editorOptions = {
          dataSource: this.pickListDomain.find(i => i.ID === Number(this.formData.pickListDomainId)).data,
          valueExpr: 'key',
          displayExpr: 'value'
        };
      } else if (this.formData.type === 'DATE') {
        c.editorType = 'dxDateBox';
      }
    }
    c.visible = visible;
  }

  onValidationTypeChanged(e) {
    if (e.dataField === 'name') {
      this.setValidationColumns('defaultValue', false);
      this.setValidationColumns('clientScript', false);
      this.formDataValidation.parameters = [];
      switch (e.value) {
        case 'requiredField':
          this.setValidationColumns('defaultValue', true);
          e.component._refresh();
          break;
        case 'custom':
          this.setValidationColumns('clientScript', true);
          e.component._refresh();
          break;
        default:
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
      groupCellTemplate: 'groupCellTemplate',
      allowEditing: false
    }, {
      dataField: 'name',
      allowSorting: false
    }, {
      caption: 'Type', dataField: 'type',
      allowSorting: false,
      lookup: { dataSource: ['BOOLEAN', 'DATE', 'NUMBER', 'PICKLISTDOMAIN', 'TEXT'] },
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
  }, {
    dataField: 'enable',
    caption: 'Enabled',
    dataType: 'boolean',
    width: '100px'
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
  constructor(lookups: ILookupData) {
    this.columns = CONFIG_ADDIN_COLUMNS;
    this.window = { title: 'Manage Addins', viewIndex: 'list' };
    this.addinAssemblies = lookups.addinAssemblies;
    this.columns.editColumn.addIn[1].editorOptions = { items: [this.addinAssemblies[0].name], value: this.addinAssemblies[0].name };
    this.columns.editColumn.addIn[2].editorOptions = { dataSource: this.addinAssemblies[0].classes, valueExpr: 'name', displayExpr: 'name' };
  }

  detectUnsavedChanges(e: boolean) {
    return e || ((this.editRow.Configuration !== this.configVal) && this.configView === 'edit');
  }

  addEditProperty(op, e) {
    if (op === 'edit') {
      this.window = { title: 'Edit Addins', viewIndex: 'edit' };
      this.editRow = e.data;
      this.configView = 'view';
      this.configVal = this.editRow.Configuration;
    }
    if (op === 'add') {
      this.editRow = new CAddInModel();
      this.window = { title: 'Add Addins', viewIndex: 'add' };
      this.configView = 'edit';
    }
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
