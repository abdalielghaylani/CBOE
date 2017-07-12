import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';

@Component({
  selector: 'reg-data-grid-form-item-template',
  template: require('./data-grid-form-item.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDataGridFormItem implements IFormItemTemplate, OnChanges {
  @Input() editMode: boolean = false;
  @Input() data: any = {};
  private dataSource: any[];
  private columns: any[];
  private editingMode: string;
  private allowUpdating: boolean;
  private allowDeleting: boolean;
  private allowAdding: boolean;

  ngOnChanges() {
    this.dataSource = this.data.editorOptions && this.data.editorOptions.value ? this.data.editorOptions.value : [];
    this.columns = this.data.editorOptions && this.data.editorOptions.columns ? this.data.editorOptions.columns : [];
    if (this.columns.length > 0) {
      this.columns.unshift({
        cellTemplate: 'commandCellTemplate',
        headerCellTemplate: 'commandHeaderCellTemplate',
        width: 80
      });
    }
    this.editingMode = this.data.editorOptions && this.data.editorOptions.editing && this.data.editorOptions.editing.mode
      ? this.data.editorOptions.editing.mode
      : 'row';
    this.allowUpdating = this.data.editorOptions && this.data.editorOptions.editing && this.data.editorOptions.editing.allowUpdating
      ? this.data.editorOptions.editing.allowUpdating
      : false;
    this.allowDeleting = this.data.editorOptions && this.data.editorOptions.editing && this.data.editorOptions.editing.allowDeleting
      ? this.data.editorOptions.editing.allowDeleting
      : false;
    this.allowAdding = this.data.editorOptions && this.data.editorOptions.editing && this.data.editorOptions.editing.allowAdding
      ? this.data.editorOptions.editing.allowAdding
      : false;
  }
  
  private onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }

  private onContentReady(e) {
    let grid = e.component;
    if (grid.totalCount() === 0) {
      grid.option('height', 60);
    } else {
      grid.option('height', 'auto');                
    }
  }

  private addRow(e) {
    e.component.addRow();
  }

  private edit(e) {
    if (this.allowUpdating) {
      e.component.editRow(e.row.rowIndex);
    }
  }

  private delete(e) {
    if (this.allowDeleting) {
      e.component.deleteRow(e.row.rowIndex);
    }
  }

  private save(e) {
    e.component.saveEditData();
  }

  private cancel(e) {
    e.component.cancelEditData();
  }
};
