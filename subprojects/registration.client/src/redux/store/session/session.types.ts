import { TypedRecord } from 'typed-immutable-record';

export interface IUser {
  fullName: string;
};

export interface IUserRecord extends TypedRecord<IUserRecord>, IUser {
};

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
  isAdmin?: boolean;
  isHidden?: boolean;
}

export interface IAppPrivilege {
  appName: string;
  privileges?: any[];
}

export interface ILookupData {
  users?: any[];
  fragments?: any[];
  fragmentTypes?: any[];
  identifierTypes?: any[];
  pickList?: any[];
  pickListDomains?: any[];
  projects?: any[];
  sites?: any[];
  formGroups?: any[];
  customTables?: any[];
  systemSettings?: ISettingData[];
  addinAssemblies?: any[];
  propertyGroups?: any[];
  homeMenuPrivileges?: any[];
  userPrivileges?: IAppPrivilege[];
  disabledControls?: any[];
}

export interface ISession {
  token: string;
  user: IUser;
  hasError: boolean;
  isLoading: boolean;
  lookups: ILookupData;
};

export interface ISessionRecord extends TypedRecord<ISessionRecord>, ISession {
};

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

  public get isSameBatchesIdentity(): boolean {
    return this.isSettingTrue('SameBatchesIdentity');
  }

  public get isMoveBatchEnabled(): boolean {
    return this.isSettingTrue('EnableMoveBatch');
  }

  public get isInventoryIntegrationEnabled(): boolean {
    let setting: ISettingData = this.getSetting('Inventory', 'InventoryIntegration');
    return setting.value === 'Enabled' ? true : false;
  }

  public get reviewRegisterSearchFormGroupId(): number {
    return +this.getSetting('Search', 'ReviewRegisterSearchFormGroupId');
  }

  public get viewRegistrySearchFormGroupId(): number {
    return +this.getSetting('Search', 'ViewRegistrySearchFormGroupId');
  }

  public get sendToRegistrationFormGroupId(): number {
    return +this.getSetting('Search', 'SendToRegistrationFormGroupId');
  }

  public get elnReviewRegisterSearchFormGroupId(): number {
    return +this.getSetting('Search', 'ELNReviewRegisterSearchFormGroupId');
  }

  public get elnViewRegistrySearchFormGroupId(): number {
    return +this.getSetting('Search', 'ELNViewRegistrySearchFormGroupId');
  }

  public get deleteLogFormGroupId(): number {
    return +this.getSetting('Search', 'DeleteLogFormGroupId');
  }

  public get submitRegistryFormGroupId(): number {
    return +this.getSetting('Search', 'SubmitRegistryFormGroupId');
  }

  public get reviewRegisterRegistryFormGroupId(): number {
    return +this.getSetting('Search', 'ReviewRegisterRegistryFormGroupId');
  }

  public get viewRegistryFormGroupId(): number {
    return +this.getSetting('Search', 'ViewRegistryFormGroupId');
  }

  public get componentDuplicatesFormGroupId(): number {
    return +this.getSetting('Search', 'ComponentDuplicatesFormGroupId');
  }

  public get registryDuplicatesFormGroupId(): number {
    return +this.getSetting('Search', 'RegistryDuplicatesFormGroupId');
  }

  public get dataLoaderFormGroupId(): number {
    return +this.getSetting('Search', 'DataLoaderFormGroupId');
  }

  public get searchComponentsToAddFormGroupId(): number {
    return +this.getSetting('Search', 'SearchComponentsToAddFormGroupId');
  }

  public get searchComponentsToAddRRFormGroupId(): number {
    return +this.getSetting('Search', 'SearchComponentsToAddRRFormGroupId');
  }

  public get searchComponentsToAddVMFormGroupId(): number {
    return +this.getSetting('Search', 'SearchComponentsToAddVMFormGroupId');
  }

  public get searchDuplicatesComponentsFormGroupId(): number {
    return +this.getSetting('Search', 'SearchDuplicatesComponentsFormGroupId');
  }

  public get markedHitsMax(): number {
    return +this.getSetting('Search', 'MarkedHitsMax');
  }
}
