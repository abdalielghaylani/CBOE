import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl, IIdentifierList, CIdentifierList, RegDataGridFormItem, dataGridFormItemTemplate } from '../../../common';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-id-list-form-item-template',
  template: dataGridFormItemTemplate,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegIdListFormItem extends RegDataGridFormItem {
  constructor(private ngRedux: NgRedux<IAppState>) {
    super();
  }

  deserializeValue(value: IIdentifierList): any {
    return value.Identifier ? value.Identifier.map(i => {
      if (!i.IdentifierID.__text) {
        // for data loading from template
        i.IdentifierID = { __text: i.IdentifierID.toString() };
      }
      let converted: any = { id: +i.IdentifierID.__text, inputText: i.InputText };
      if (i.ID != null) {
        converted.persistId = i.ID;
      }
      return converted;
    }) : [];
  }

  serializeValue(value: any): IIdentifierList {
    let serialized = value && value.length > 0 ? {
      Identifier: value.map(v => {
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
      })
    } : undefined;
    return serialized;
  }

  protected update() {
    let lookups = this.ngRedux.getState().session.lookups;
    let options = this.viewModel.editorOptions;
    let value = options && options.value ? options.value : [];
    this.dataSource = this.deserializeValue(value);
    let identifierList = lookups.identifierTypes.filter(i => i.TYPE === options.idListType);
    let activeItems = identifierList.filter(i => i.ACTIVE === 'T');
    this.dataSource.forEach(i => {
      if (i.id && activeItems.find(j => j.ID === i.id) === undefined) {
        activeItems.push(identifierList.find(k => k.ID === i.id));
      }
    });
    let maxLength = options.fieldConfig.Columns.Column.find(i => i._name === 'Value').formElement.configInfo.MaxLength;
    this.columns = lookups ? [{
      dataField: 'id',
      caption: 'Identifier',
      editorType: 'dxSelectBox',
      lookup: {
        dataSource: activeItems,
        displayExpr: 'NAME',
        valueExpr: 'ID',
        placeholder: 'Select Identifier'
      },
      validationRules: [
        { type: 'required', message: 'A valid identifier type is required.' }
      ]
    }, {
      dataField: 'inputText',
      caption: 'Value',
      editorOptions: { maxLength: maxLength }
    }] : [];
    this.checkCommandColumn();
    this.editingMode = 'row';
    this.allowUpdating = true;
    this.allowDeleting = true;
    this.allowAdding = true;
  }
};
