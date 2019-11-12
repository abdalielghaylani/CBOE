import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFieldConfig } from '../../../../common';
import { IViewControl, IIdentifierList, CIdentifierList, RegDataGridFormItem, dataGridFormItemTemplate } from '../../../common';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-id-list-form-item-template',
  template: dataGridFormItemTemplate,
  styles: [require('../registry-base.css')],
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
    try {
      let lookups = this.ngRedux.getState().session.lookups;
      let options = this.viewModel.editorOptions;
      let value = options && options.value ? options.value : [];
      this.dataSource = this.deserializeValue(value);
      let identifierList = lookups.identifierTypes.filter(i => (i.TYPE === options.idListType || i.TYPE === 'A'));
      let activeItems = identifierList.filter(i => i.ACTIVE === 'T');
      let items = identifierList.filter(i => i.ACTIVE === 'T');
      this.dataSource.forEach(i => {
        if (i.id && activeItems.find(j => j.ID === i.id) === undefined) {
          items.push(identifierList.find(k => k.ID === i.id));
        }
      });
      // TODO: Now, the field configuration object includes the full details of how the grid should behave.
      // All fields as well as their validation rules must come from the configuration.
      const fieldConfig: IFieldConfig = options.fieldConfig;
      let maxLength = fieldConfig.tables.table[0].Columns.Column.find(i => i._name === 'Value').formElement.configInfo.MaxLength;
      this.columns = lookups ? [{
        dataField: 'id',
        caption: 'Identifier',
        allowSorting: false,
        cellTemplate: 'identifierCellTemplate',
        editorType: 'dxSelectBox',
        items: items,
        lookup: {
          dataSource: items,
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
        allowSorting: false,
        editorOptions: { maxLength: maxLength },
        validationRules: [
          { type: 'required', message: 'A valid identifier value is required.' },
          {
            type: 'custom',
            validationCallback: (o): boolean => {
              o.rule.isValue = true;
              if (o.value && fieldConfig.ClientSideEvents && fieldConfig.ClientSideEvents.Event) {
                const v = fieldConfig.ClientSideEvents.Event.find(e => e._name === 'CustomValidation');
                if (v && v.Params && v.Params.param) {
                  const v2 = v.Params.param.find(p => p._validationMethod === 'IsAValidCas');
                  if (v2 && v2._parentColValue === o.data.id.toString()) {
                    let isValid = /^[0-9]{1,7}-[0-9][0-9]-[0-9]$/.test(o.value);
                    if (isValid) {
                      const freeCas = o.value.replace(/-/g, '');
                      let casSum = 0;
                      for (let casIndex = freeCas.length - 1; casIndex > 0; --casIndex) {
                        casSum += casIndex * (freeCas.substring(freeCas.length - casIndex - 1, freeCas.length - casIndex));
                      }
                      isValid = (casSum % 10) === ((freeCas.substring(freeCas.length - 1, freeCas.length)) % 10);
                    }
                    if (!isValid) {
                      o.rule.isValue = false;
                      if (v2._errorMessage) {
                        o.rule.message = v2._errorMessage;
                      }
                    }
                  }
                }
              }
              return o.rule.isValue;
            }
          }]
      }] : [];
      this.checkCommandColumn();
      this.editingMode = 'row';
      this.allowUpdating = true;
      this.allowDeleting = true;
      this.allowAdding = true;
    } catch (e) {
      console.trace(e);
    }
  }
}
