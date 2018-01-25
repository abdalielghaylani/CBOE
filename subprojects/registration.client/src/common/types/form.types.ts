import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../redux';

export enum FormGroupType {
  SubmitMixture,
  ReviewRegisterMixture,
  ViewMixture,
  SearchTemporary,
  SearchPermanent,
  ELNSearchTempForm,
  ELNSearchPermForm,
  DataLoaderForm,
  RegistryDuplicatesForm,
  ComponentDuplicatesForm,
  SendToRegistrationForm,
  DeleteLogFrom,
  SearchComponentToAddForm,
  SearchComponentToAddFormRR
}

export enum SubFormType {
  MixtureDrawingForm = 0,
  ComponentForm = 1,
  BatchComponentForm = 3,
  BatchForm = 4,
  BatchComponentFragmentsForm = 5,
  BatchComponentFragmentsFormEdit = 6,
  BatchFragmentList = 7,
  DocManagerForm = 8,
  InvContainerForm = 9,
  RegistryCustomProperties = 1000,
  CompoundCustomProperties = 1001,
  BatchCustomProperties = 1002,
  BatchComponentCustomProperties = 1003
}

export interface IDisplayInfo {
  cssClass?: string;
  type?: string;
  visible?: string;
}

export interface IColumn {
  _name?: string;
  headerText?: string;
  _hidden?: string;
  width: string;
  formElement?: IFormElement;
  _childTableName?: string;
}

export interface IColumns {
  Column?: IColumn[];
}

export interface ITable {
  _name?: string;
  Columns?: IColumns;
}

export interface ITables {
  table?: ITable[];
}

export interface IFieldConfig {
  CSSLabelClass?: string;
  CSSClass?: string;
  dropDownItemsSelect?: string;
  PickListDomain?: any;
  DefaultDate?: string;
  tables?: ITables;
  ClientSideEvents?: any;
  DefaultRows?: any;
  Mask?: string;
}

export interface IConfigInfo {
  fieldConfig: IFieldConfig;
  MaxLength?: any;
}

export interface IFormElement {
  _name?: string;
  label?: string;
  showHelp?: string;
  isFileUpload?: string;
  pageCommunicationProvider?: any;
  fileUploadBindingExpression?: any;
  helpText?: string;
  defaultValue?: string;
  Id?: string;
  displayInfo?: IDisplayInfo;
  bindingExpression?: string;
  configInfo?: IConfigInfo;
  dataSource?: string;
  searchCriteriaItem?: any;
  validationRuleList?: any;
  clientEvents?: any;
  // TODO: ...
}

export interface ICoeFormMode {
  formElement?: IFormElement[];
}

export interface ICoeForm {
  _id?: string;
  _dataSourceId?: string;
  title?: string;
  titleCssClass?: string;
  validationRuleList?: any;
  layoutInfo?: ICoeFormMode;
  formDisplay: IDisplayInfo;
  addMode?: ICoeFormMode;
  editMode?: ICoeFormMode;
  viewMode?: ICoeFormMode;
  clientScript?: any;
}

export interface ICoeForms {
  _id?: Number;
  _defaultDisplayMode?: string;
  coeForm: ICoeForm[];
}

export interface IForm {
  _id?: Number;
  _dataSourceId?: string;
  coeForms?: ICoeForms;
}

export interface IQueryForms {
  _defaultForm?: Number;
  queryForm: IForm[];
}

export interface IDetailsForms {
  _defaultForm?: Number;
  detailsForm: IForm[];
}

export interface IListForms {
  _defaultForm?: Number;
  listForm: IForm[];
}

export interface IFormGroup {
  queryForms?: IQueryForms;
  detailsForms?: IDetailsForms;
  listForms?: IListForms;
}

export interface IFormContainer {
  ngRedux: NgRedux<IAppState>;
  formGroup: IFormGroup;
  editMode: boolean;
}
