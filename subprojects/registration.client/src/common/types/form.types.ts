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

export class CDisplayInfo {
  cssClass?: String;
  type?: String;
  visible?: String;
}

export class CFormElement {
  _name?: String;
  label?: String;
  showHelp?: String;
  isFileUpload?: String;
  pageCommunicationProvider?: any;
  fileUploadBindingExpression?: any;
  helpText?: String;
  defaultValue?: String;
  Id?: String;
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
  _id?: Number;
  _dataSourceId?: String;
  title?: String;
  titleCssClass?: String;
  validationRuleList?: any;
  layoutInfo?: CLayoutInfo;
  formDisplay: any;
  addMode?: CCoeFormMode;
  editMode?: CCoeFormMode;
  viewMode?: CCoeFormMode;
  clientScript?: any;
}

export class CCoeForms {
  coeForm: CCoeForm[] = [];
}

export class CForm {
  _id?: Number;
  _dataSourceId?: String;
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
