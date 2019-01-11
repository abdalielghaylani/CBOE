import CustomStore from 'devextreme/data/custom_store';
import { apiUrlPrefix } from '../../configuration';
import {
  IIdentifier, IIdentifierList, IProjectList, IFragment, IComponent,
  IBatchComponentFragment, IBatch, IProperty, IPropertyList
} from '../common';
import {
  IShareableObject, FormGroupType, SubFormType, IFormGroup, ICoeForm, IFormElement, IFormContainer
} from '../../common';
import { IAppState, IRecordsData, ILookupData } from '../../redux';

export enum RegistryStatus {
  NotSet,
  Submitted,
  Approved,
  Registered,
  Locked
}

export interface IResponseData {
  id?: number;
  regNumber?: string;
  message?: string;
  data?: any;
}

export interface ITemplateData extends IShareableObject {
  id?: number;
  dateCreated?: Date;
  username?: string;
  data?: string;
}

export class CTemplateData implements ITemplateData {
  constructor(
    public name: string = '',
    public description?: string,
    public isPublic?: boolean,
    public id?: number,
    dateCreated?:
      Date, username?: string,
    date?: string) { }
}

export interface ICopyActions {
  canContinueOptionVisibility: boolean;
  canCreateCopiesOptionVisibility: boolean;
  okOptionVisibility: boolean;
  caption: string;
  message: string;
  action: string;
}

export interface IRegMarkedPopupModel {
  description: string; 
  option: string; 
  isVisible: boolean;
}

