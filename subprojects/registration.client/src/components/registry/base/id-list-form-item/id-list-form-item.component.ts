import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl, IIdentifierList, CIdentifierList } from '../registry-base.types';
import { RegDataGridFormItem } from '../data-grid-form-item';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-id-list-form-item-template',
  template: require('../data-grid-form-item/data-grid-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegIdListFormItem extends RegDataGridFormItem {
  constructor(private ngRedux: NgRedux<IAppState>) {
    super();
  }

  deserializeValue(value: IIdentifierList): any {
    return value.Identifier ? value.Identifier.map(i => {
      let converted: any = { id: +i.IdentifierID.__text, inputText: i.InputText };
      if (i.ID != null) {
        converted.persistId = i.ID;
      }
      return converted;
    }) : [];
  }

  serializeValue(value: any): IIdentifierList {
    let serialized = value && value.length > 0 ? { Identifier: value.map(v => {
      if (!v.id) {
        return { IdentifierID: {}, InputText: undefined };
      }
      let idColumn = this.columns.find(c => c.dataField === 'id');
      let name = idColumn.lookup.dataSource.find(i => i.ID === +v.id).NAME;
      let converted: any = {
        IdentifierID: { __text: v.id, _Active: 'T', _Name: name, _Description: name },
        InputText: v.inputText
      };
      if (v.persistId != null) {
        converted.ID = v.persistId;
      }
      return converted;
    })} : undefined;
    return serialized;
  }

  protected update() {
    let lookups = this.ngRedux.getState().session.lookups;
    let options = this.viewModel.editorOptions;
    let value = options && options.value ? options.value : [];
    this.dataSource = this.deserializeValue(value);
    this.columns = lookups ? [{
      dataField: 'id',
      caption: 'Identifier',
      editorType: 'dxSelectBox',
      lookup: {
        dataSource: lookups.identifierTypes.filter(i => i.TYPE === options.idListType && i.ACTIVE === 'T'),
        displayExpr: 'NAME',
        valueExpr: 'ID',
        placeholder: 'Select Identifier'
      },
      validationRules: [
        { type: 'required', message: 'A valid identifier type is required.' }
      ]
    }, {
      dataField: 'inputText',
      caption: 'Value'
    }] : [];
    this.checkCommandColumn();
    this.editingMode = 'row';
    this.allowUpdating = true;
    this.allowDeleting = true;
    this.allowAdding = true;
  }
};
