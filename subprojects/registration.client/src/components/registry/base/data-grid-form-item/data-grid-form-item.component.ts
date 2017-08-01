import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-data-grid-form-item-template',
  template: require('./data-grid-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDataGridFormItem extends RegBaseFormItem {
  protected dataSource: any[];
  protected columns: any[];
  protected editingMode: string;
  protected allowUpdating: boolean;
  protected allowDeleting: boolean;
  protected allowAdding: boolean;

  protected update() {
    let options = this.viewModel.editorOptions;
    this.dataSource = options && options.value ? options.value : [];
    this.columns = options && options.columns ? options.columns : [];
    if (this.columns.length > 0 && !this.columns[0].headerCellTemplate) {
      this.columns.unshift({
        cellTemplate: 'commandCellTemplate',
        headerCellTemplate: 'commandHeaderCellTemplate',
        width: 80
      });
    }
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
    if (grid.totalCount() === 0) {
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
  }

  protected onRowUpdated(e, d) {
  }

  protected onRowRemoved(e, d) {
  }

  protected addRow(e) {
    e.component.addRow();
  }

  protected edit(e) {
    if (this.allowUpdating) {
      e.component.editRow(e.row.rowIndex);
    }
  }

  protected delete(e) {
    if (this.allowDeleting) {
      e.component.deleteRow(e.row.rowIndex);
    }
  }

  protected save(e) {
    e.component.saveEditData();
  }

  protected cancel(e) {
    e.component.cancelEditData();
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }
};
