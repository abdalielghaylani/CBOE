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
  systemInformation?: any;
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
  static REG_GROUP = 'Registration';
  constructor(private systemSettings: ISettingData[]) {
  }

  private getSetting(groupLabel: string, settingName: string): ISettingData {
    return this.systemSettings.find(s => s.groupLabel === groupLabel && s.name === settingName);
  }

  private getRegSetting(settingName: string): ISettingData {
    return this.getSetting(CSystemSettings.REG_GROUP, settingName);
  }

  private isSettingTrue(groupLabel: string, settingName: string): boolean {
    const setting = this.getSetting(groupLabel, settingName);
    return setting && setting.value && setting.value.toLowerCase() === 'true';
  }

  private isRegSettingTrue(settingName: string): boolean {
    return this.isSettingTrue(CSystemSettings.REG_GROUP, settingName);
  }

  private getNumberSetting(groupLabel: string, settingName: string, defaultValue: number = 0): number {
    const setting = this.getSetting(groupLabel, settingName);
    return setting == null || setting.value == null || isNaN(Number(setting.value)) ? defaultValue : +setting.value;
  }

  public get isApprovalsEnabled(): boolean {
    return this.isRegSettingTrue('ApprovalsEnabled');
  }

  public get isSubmissionTemplateEnabled(): boolean {
    return this.isRegSettingTrue('EnableSubmissionTemplates');
  }

  public get isSameBatchesIdentity(): boolean {
    return this.isRegSettingTrue('SameBatchesIdentity');
  }

  public get isMoveBatchEnabled(): boolean {
    return this.isRegSettingTrue('EnableMoveBatch');
  }

  public get isLockingEnabled(): boolean {
    return this.isRegSettingTrue('LockingEnabled');
  }

  public get isInventoryIntegrationEnabled(): boolean {
    let setting: ISettingData = this.getSetting('Inventory', 'InventoryIntegration');
    return setting && setting.value === 'Enabled' ? true : false;
  }

  public get isInventoryUseFullContainerForm(): boolean {
    return this.isSettingTrue('Inventory', 'UseFullContainerForm');
  }

  public get isSendToInventoryEnabled(): boolean {
    return this.isSettingTrue('Inventory', 'SendtoInventory');
  }

  public get invSendToInventoryURL(): string {
    let setting: ISettingData = this.getSetting('Inventory', 'SendToInventoryURL');
    return setting && setting.value ? setting.value : '';
  }

  public get showRequestFromContainer(): boolean {
    return this.isSettingTrue('Inventory', 'ShowRequestFromContainer');
  }

  public get showRequestMaterial(): boolean {
    return this.isSettingTrue('Inventory', 'ShowRequestMaterial');
  }

  public get showRequestFromBatch(): boolean {
    return this.isSettingTrue('Inventory', 'ShowRequestFromBatch');
  }

  public get invNewContainerURL(): string {
    let setting: ISettingData = this.getSetting('Inventory', 'NewContainerURL');
    return setting && setting.value ? setting.value : '';
  }

  public get reviewRegisterSearchFormGroupId(): number {
    return this.getNumberSetting('Search', 'ReviewRegisterSearchFormGroupId');
  }

  public get viewRegistrySearchFormGroupId(): number {
    return this.getNumberSetting('Search', 'ViewRegistrySearchFormGroupId');
  }

  public get sendToRegistrationFormGroupId(): number {
    return this.getNumberSetting('Search', 'SendToRegistrationFormGroupId');
  }

  public get elnReviewRegisterSearchFormGroupId(): number {
    return this.getNumberSetting('Search', 'ELNReviewRegisterSearchFormGroupId');
  }

  public get elnViewRegistrySearchFormGroupId(): number {
    return this.getNumberSetting('Search', 'ELNViewRegistrySearchFormGroupId');
  }

  public get deleteLogFormGroupId(): number {
    return this.getNumberSetting('Search', 'DeleteLogFormGroupId');
  }

  public get submitRegistryFormGroupId(): number {
    return this.getNumberSetting('Search', 'SubmitRegistryFormGroupId');
  }

  public get reviewRegisterRegistryFormGroupId(): number {
    return this.getNumberSetting('Search', 'ReviewRegisterRegistryFormGroupId');
  }

  public get viewRegistryFormGroupId(): number {
    return this.getNumberSetting('Search', 'ViewRegistryFormGroupId');
  }

  public get componentDuplicatesFormGroupId(): number {
    return this.getNumberSetting('Search', 'ComponentDuplicatesFormGroupId');
  }

  public get registryDuplicatesFormGroupId(): number {
    return this.getNumberSetting('Search', 'RegistryDuplicatesFormGroupId');
  }

  public get dataLoaderFormGroupId(): number {
    return this.getNumberSetting('Search', 'DataLoaderFormGroupId');
  }

  public get searchComponentsToAddFormGroupId(): number {
    return this.getNumberSetting('Search', 'SearchComponentsToAddFormGroupId');
  }

  public get searchComponentsToAddRRFormGroupId(): number {
    return this.getNumberSetting('Search', 'SearchComponentsToAddRRFormGroupId');
  }

  public get searchComponentsToAddVMFormGroupId(): number {
    return this.getNumberSetting('Search', 'SearchComponentsToAddVMFormGroupId');
  }

  public get searchDuplicatesComponentsFormGroupId(): number {
    return this.getNumberSetting('Search', 'SearchDuplicatesComponentsFormGroupId');
  }

  public get markedHitsMax(): number {
    return this.getNumberSetting('Search', 'MarkedHitsMax');
  }
}
