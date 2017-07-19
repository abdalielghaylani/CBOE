import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';

@Component({
  selector: 'reg-data-grid-form-item-template',
  template: require('./data-grid-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDataGridFormItem implements IFormItemTemplate, OnChanges {
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() data: any = {};
  protected dataSource: any[];
  protected columns: any[];
  protected editingMode: string;
  protected allowUpdating: boolean;
  protected allowDeleting: boolean;
  protected allowAdding: boolean;

  ngOnChanges() {
    this.update();
  }

  protected update() {
    this.dataSource = this.data.editorOptions && this.data.editorOptions.value ? this.data.editorOptions.value : [];
    this.columns = this.data.editorOptions && this.data.editorOptions.columns ? this.data.editorOptions.columns : [];
    if (this.columns.length > 0 && !this.columns[0].headerCellTemplate) {
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
  
  protected onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }

  protected onContentReady(e) {
    let grid = e.component;
    if (grid.totalCount() === 0) {
      grid.option('height', 60);
    } else {
      grid.option('height', 'auto');                
    }
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
};
