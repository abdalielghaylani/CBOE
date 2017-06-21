import { TypedRecord } from 'typed-immutable-record';
import { ISettingData } from '../../components';

export interface IUser {
  fullName: string;
};

export interface IUserRecord extends TypedRecord<IUserRecord>, IUser {
};

export interface ILookupData {
  users?: any[];
  fragments?: any[];
  fragmentTypes?: any[];
  identifierTypes?: any[];
  notebooks?: any[];
  pickList?: any[];
  pickListDomains?: any[];
  projects?: any[];
  sequences?: any[];
  sites?: any[];
  units?: any[];
  formGroups?: any[];
  customTables?: any[];
  systemSettings?: ISettingData[];
  addinAssemblies?: any[];
  propertyGroups?: any[];
  homeMenuPrivileges?: any[];
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
