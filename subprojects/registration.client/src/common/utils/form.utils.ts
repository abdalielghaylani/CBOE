import { NgRedux } from '@angular-redux/store';
import { DOMParser, DOMParserStatic, XMLSerializer } from 'xmldom';
import * as X2JS from 'x2js';
import { ConfigurationActions } from '../../actions';
import { IAppState } from '../../store';
import { FormGroupType, IFormGroup } from '../types/form.types';

export function getFormGroupData(state: IAppState, type: FormGroupType): string {
  let groups = (state.session.lookups.formGroups as Array<{ name, data }>).filter(fg => fg.name === FormGroupType[type]);
  return groups && groups.length > 0 ? groups[0].data : null;
}

export function getFormGroup(state: IAppState, type: FormGroupType): IFormGroup {
  let data = getFormGroupData(state, type);
  return data ? convertToFormGroup(data) : null;
}

export function convertToFormGroup(data: string): IFormGroup {
  let doc = new DOMParser().parseFromString(data);
  let x2jsTool = new X2JS.default({
    arrayAccessFormPaths: [
      'formGroup.queryForms.queryForm',
      'formGroup.queryForms.queryForm.coeForms.coeForm',
      'formGroup.queryForms.queryForm.coeForms.coeForm.layoutInfor.formElement',
      'formGroup.queryForms.queryForm.coeForms.coeForm.addMode.formElement',
      'formGroup.queryForms.queryForm.coeForms.coeForm.editMode.formElement',
      'formGroup.queryForms.queryForm.coeForms.coeForm.viewMode.formElement',
      'formGroup.detailsForms.detailsForm',
      'formGroup.detailsForms.detailsForm.coeForms.coeForm',
      'formGroup.detailsForms.detailsForm.coeForms.coeForm.layoutInfor.formElement',
      'formGroup.detailsForms.detailsForm.coeForms.coeForm.addMode.formElement',
      'formGroup.detailsForms.detailsForm.coeForms.coeForm.editMode.formElement',
      'formGroup.detailsForms.detailsForm.coeForms.coeForm.viewMode.formElement',
      'formGroup.listForms.listForm',
      'formGroup.listForms.listForm.coeForms.coeForm',
      'formGroup.listForms.listForm.coeForms.coeForm.layoutInfor.formElement',
      'formGroup.listForms.listForm.coeForms.coeForm.addMode.formElement',
      'formGroup.listForms.listForm.coeForms.coeForm.editMode.formElement',
      'formGroup.listForms.listForm.coeForms.coeForm.viewMode.formElement'
    ]
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
