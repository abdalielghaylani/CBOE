import { EventEmitter } from '@angular/core';
import { IViewGroup } from './registry-base.types';
import { IFormGroup, IForm, ICoeForm, ICoeFormMode, IFormElement } from '../../../common';
import { IPropertyList } from './registry-base.types';

export interface ISearchCriteriaValue {
  _negate: string;
  __text?: string;
}

export interface ISearchCriteriaItem {
  _id: string;
  _tableid: string;
  _fieldid: string;
  _modifier?: string;
  _aggregateFunctionName?: string;
  _searchLookupByID?: string;
}

export function getSearchCriteriaItemObj(item: ISearchCriteriaItem): ISearchCriteriaValue {
  let objectProp = Object.getOwnPropertyNames(item).find(n => typeof item[n] === 'object');
  return item[objectProp] as ISearchCriteriaValue;
}
