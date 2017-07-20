import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl } from '../registry-base.types';
import { RegDataGridFormItem } from '../data-grid-form-item';
import { NgRedux } from '@angular-redux/store';
import { apiUrlPrefix } from '../../../../configuration';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-fragments-form-item-template',
  template: require('../data-grid-form-item/data-grid-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFragmentsFormItem extends RegDataGridFormItem {
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;

  constructor(private ngRedux: NgRedux<IAppState>) {
    super();
  }

  protected update() {
    let lookups = this.ngRedux.getState().session.lookups;
    let options = this.viewModel.editorOptions;
    this.dataSource = options && options.value ? options.value : [];
    this.columns = lookups ? [{
        dataField: 'fragmentTypeId',
        caption: 'Type',
        editorType: 'dxSelectBox',
        lookup: {
          dataSource: lookups.fragmentTypes,
          displayExpr: 'DESCRIPTION',
          valueExpr: 'ID'
        },
        width: 60
      }, {
        dataField: 'equivalents',
        caption: 'Equivalent',
        width: 80
      }, {
        dataField: 'code',
        caption: 'Code',
        width: 50
      }, {
        dataField: 'structure',
        caption: 'Structure',
        allowEditing: false,
        allowFiltering: false,
        allowSorting: false,
        width: 150,
        cellTemplate: function (c, o) {
          jQuery(`<img src="${apiUrlPrefix}StructureImage/${o.data.structure}" />`).appendTo(c);
        }
      }, {
        dataField: 'description',
        caption: 'Description',
        allowEditing: false
      }, {
        dataField: 'molWeight',
        caption: 'MW',
        allowEditing: false
      }, {
        dataField: 'formula',
        caption: 'MF',
        allowEditing: false
      }] : [];
    if (this.columns.length > 0 && !this.columns[0].headerCellTemplate) {
      this.columns.unshift({
        cellTemplate: 'commandCellTemplate',
        headerCellTemplate: 'commandHeaderCellTemplate',
        width: 80
      });
    }
    this.editingMode = 'row';
    this.allowUpdating = true;
    this.allowDeleting = true;
    this.allowAdding = true;
  }
};
