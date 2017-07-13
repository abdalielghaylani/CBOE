import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../store';

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

export class CDisplayInfo {
  cssClass?: string;
  type?: string;
  visible?: string;
}

export class CFormElement {
  _name?: string;
  label?: string;
  showHelp?: string;
  isFileUpload?: string;
  pageCommunicationProvider?: any;
  fileUploadBindingExpression?: any;
  helpText?: string;
  defaultValue?: string;
  Id?: string;
  displayInfo?: CDisplayInfo;
  // TODO: ...
}

export class CLayoutInfo {
  formElement?: CFormElement[] = [];
}

export class CCoeFormMode {
  formElement: CFormElement[] = [];
}

export class CCoeForm {
  _id?: string;
  _dataSourceId?: string;
  title?: string;
  titleCssClass?: string;
  validationRuleList?: any;
  layoutInfo?: CLayoutInfo;
  formDisplay: any;
  addMode?: CCoeFormMode;
  editMode?: CCoeFormMode;
  viewMode?: CCoeFormMode;
  clientScript?: any;
}

export class CCoeForms {
  _id?: Number;
  _defaultDisplayMode?: string;
  coeForm: CCoeForm[] = [];
}

export class CForm {
  _id?: Number;
  _dataSourceId?: string;
  coeForms?: CCoeForms;
}

export class CQueryForms {
  _defaultForm?: Number;
  queryForm: CForm[] = [];
}

export class CDetailsForms {
  _defaultForm?: Number;
  detailsForm: CForm[] = [];
}

export class CListForms {
  _defaultForm?: Number;
  listForm: CForm[] = [];
}

export class CFormGroup {
  constructor(
    public queryForms?: CQueryForms,
    public detailsForms?: CDetailsForms,
    public listForms?: CListForms
  ) { }
}

export interface IFormContainer {
  ngRedux: NgRedux<IAppState>;
  formGroup: CFormGroup;
  editMode: boolean;
}
