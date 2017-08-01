import { NgRedux } from '@angular-redux/store';
import { DOMParser, DOMParserStatic, XMLSerializer } from 'xmldom';
import * as X2JS from 'x2js';
import { ConfigurationActions, IAppState } from '../../redux';
import { FormGroupType, IFormGroup } from '../types/form.types';

export function getFormGroupData(state: IAppState, type: FormGroupType): string {
  let groups = (state.session.lookups.formGroups as Array<{ name, data }>).filter(fg => fg.name === FormGroupType[type]);
  return groups && groups.length > 0 ? groups[0].data : null;
}

export function getFormGroup(state: IAppState, type: FormGroupType): IFormGroup {
  let data = getFormGroupData(state, type);
  return data ? convertToFormGroup(data) : null;
}

function getValidationArrayPaths(basePath: string): string[] {
  return [
    `${basePath}`,
    `${basePath}.validationRuleList.validationRule`,
    `${basePath}.validationRuleList.validationRule.params.param`
  ];
}

function getCoeFormArrayPaths(basePath: string): string[] {
  return getValidationArrayPaths(`${basePath}`)
    .concat(getValidationArrayPaths(`${basePath}.layoutInfor.formElement`))
    .concat(getValidationArrayPaths(`${basePath}.addMode.formElement`))
    .concat(getValidationArrayPaths(`${basePath}.editMode.formElement`))
    .concat(getValidationArrayPaths(`${basePath}.viewMode.formElement`));
}

function getFormArrayPaths(basePath: string): string[] {
  return [ `${basePath}` ].concat(getCoeFormArrayPaths(`${basePath}.coeForms.coeForm`));
}

export function convertToFormGroup(data: string): IFormGroup {
  let doc = new DOMParser().parseFromString(data);
  let arrayPaths = getFormArrayPaths('formGroup.queryForms.queryForm')
    .concat(getFormArrayPaths('formGroup.detailsForms.detailsForm'))
    .concat(getFormArrayPaths('formGroup.listForms.listForm'));
  let x2jsTool = new X2JS.default({
    arrayAccessFormPaths: arrayPaths
  });
  return (x2jsTool.dom2js(doc) as any).formGroup as IFormGroup;
}

export function prepareFormGroupData(formGroupType: FormGroupType, ngRedux: NgRedux<IAppState>) {
  let state = ngRedux.getState();
  if (!state.configuration.formGroups[FormGroupType[formGroupType]]) {
    ngRedux.dispatch(ConfigurationActions.loadFormGroupAction({
      type: formGroupType,
      data: getFormGroupData(state, formGroupType)
    }));
  }
}
