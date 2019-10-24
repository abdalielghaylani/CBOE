import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation, ViewChild } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import { RegBaseFormItem } from '../base-form-item';
import * as dxDialog from 'devextreme/ui/dialog';

export const dataGridFormItemTemplate = require('./data-grid-form-item.component.html');

@Component({
  selector: 'reg-data-grid-form-item-template',
  template: dataGridFormItemTemplate,
  styles: [require('../common.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDataGridFormItem extends RegBaseFormItem {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  protected dataSource: any[];
  protected columns: any[];
  protected editingMode: string;
  protected allowUpdating: boolean;
  protected allowDeleting: boolean;
  protected allowAdding: boolean;

  protected checkCommandColumn() {
    if (this.editMode && this.columns.length > 0 && !this.columns[0].headerCellTemplate) {
      this.columns.unshift({
        cellTemplate: 'commandCellTemplate',
        headerCellTemplate: 'commandHeaderCellTemplate',
        width: 80
      });
    } else if (!this.editMode && this.columns.length > 0 && this.columns[0].headerCellTemplate) {
      this.columns.splice(0, 1);
    }
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    this.dataSource = options && options.value ? options.value : [];
    this.columns = options && options.columns ? options.columns : [];
    this.checkCommandColumn();
    this.editingMode = options && options.editing && options.editing.mode
      ? options.editing.mode
      : 'row';
    this.allowUpdating = options && options.editing && options.editing.allowUpdating
      ? options.editing.allowUpdating
      : false;
    this.allowDeleting = options && options.editing && options.editing.allowDeleting
      ? options.editing.allowDeleting
      : false;
    this.allowAdding = options && options.editing && options.editing.allowAdding
      ? options.editing.allowAdding
      : false;
  }

  protected onContentReady(e) {
    let grid = e.component;
    if (grid.getRowElement(0) == null) {
      grid.option('height', 60);
    } else {
      grid.option('height', 'auto');
    }
  }

  protected onRowInserting(e, d) {
  }

  protected onRowUpdating(e, d) {
  }

  protected onRowRemoving(e, d) {
  }

  protected onRowInserted(e, d) {
    this.onGridChanged(d.component);
  }

  protected onRowUpdated(e, d) {
    this.onGridChanged(d.component);
  }

  protected onRowRemoved(e, d) {
    this.onGridChanged(d.component);
  }

  protected addRow(e) {
    let isEditing = e.component.hasEditData();
    if (!isEditing) {
      e.component.addRow();
    } else {
      let dialogResult = dxDialog.confirm(
        `Are you sure you want to continue? Unsaved changes will be lost while adding new row.`,
        'Confirm Add Row');
      dialogResult.done(r => {
        if (r) {
          e.component.addRow();
        }
      });
    }
  }

  protected edit(e) {
    if (this.allowUpdating) {
      e.component.editRow(e.row.rowIndex);
    }
  }

  getName() {
    let validItems = [];
    this.viewConfig.forEach(i => {
      if (i.itemType === 'group') {
        validItems = validItems.concat(i.items.filter(ix => !ix.itemType || ix.itemType !== 'empty'));
      } else if (i.itemType !== 'empty') {
        validItems.push(i);
      }
    });
    let val = validItems.find(r => r.dataField === this.viewModel.dataField);
    if (val && val.label && val.label.text) {
      return val.label.text;
    }
    return 'list item';
  }

  protected delete(e) {
    if (this.allowDeleting) {
      let name = this.getName();
      let dialogResult = dxDialog.confirm(
        `Alert: Are you sure you want to delete this ` + name + ` ?`,
        `Remove ` + name);
      dialogResult.done(r => {
        if (r) {
          e.component.deleteRow(e.row.rowIndex);
        }
      });
    }
  }

  protected save(e) {
    e.component.saveEditData();
  }

  protected cancel(e) {
    e.component.cancelEditData();
  }

  protected onGridChanged(component) {
    let value = this.serializeValue(this.dataSource);
    component.option('formData.' + this.viewModel.dataField, value);
    this.valueUpdated.emit(this);
  }

  protected onDropDownValueUpdated(e, d) {
    this.grid.instance.cellValue(d.rowIndex, d.column.dataField, e);
  }

  protected validate(options) {
    let component = options.validator.peer;
    let vm = component.viewModel;
    options.validator.isValid = true;
    if (component.grid.instance.hasEditData()) {
      let vc = component.viewConfig;
      let items = vc.length > 0 && vc[0].itemType === 'group' ? vc.map(g => g.items).reduce((v, a) => a.concat(v), []) : vc;
      let item = items.find(i => i.dataField === vm.dataField);
      let label = item && item.label && item.label.text ? item.label.text : vm.dataField;
      options.validator.errorMessage = `You must complete editing the '${label}' field`;
      options.validator.isValid = false;
    } else {
      super.validate(options);
    }
    return options.validator.isValid;
  }

  getIdentifierName(e) {
    try {
      return e.column.items ?
        e.column.items.find(i => i[e.column.lookup.valueExpr] === e.value)[e.column.lookup.displayExpr] : '';
    } catch (e) {
      console.trace(e);
      return '';
    }
  }

}
