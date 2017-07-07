import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IFormItemTemplate } from '../item-templates.types';

@Component({
  selector: 'reg-data-grid-form-item-template',
  template: require('./data-grid-form-item.component.html'),
  styles: [require('../item-templates.css')],
})
export class RegDataGridFormItem implements IFormItemTemplate {
  @Input() editMode: boolean = false;
  @Input() data: any = {};

  private onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }

  private get dataSource(): any[] {
    return this.data.editorOptions && this.data.editorOptions.value ? this.data.editorOptions.value : [];
  }

  private get columns(): any[] {
    return this.data.editorOptions && this.data.editorOptions.columns ? this.data.editorOptions.columns : [];    
  }

  private get editingMode(): string {
    return this.data.editorOptions && this.data.editorOptions.editing && this.data.editorOptions.editing.mode
      ? this.data.editorOptions.editing.mode
      : 'row';
  }

  private get allowUpdating(): boolean {
    return this.data.editorOptions && this.data.editorOptions.editing && this.data.editorOptions.editing.allowUpdating
      ? this.data.editorOptions.editing.allowUpdating
      : false;
  }

  private get allowDeleting(): boolean {
    return this.data.editorOptions && this.data.editorOptions.editing && this.data.editorOptions.editing.allowDeleting
      ? this.data.editorOptions.editing.allowDeleting
      : false;
  }

  private get allowAdding(): boolean {
    return this.data.editorOptions && this.data.editorOptions.editing && this.data.editorOptions.editing.allowAdding
      ? this.data.editorOptions.editing.allowAdding
      : false;
  }

  private onCellPrepared(e) {
    if (e.rowType === 'data' && e.column.command === 'edit') {
      let isEditing = e.row.isEditing;
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      if (isEditing) {
        $links.filter('.dx-link-save').addClass('dx-icon-save');
        $links.filter('.dx-link-cancel').addClass('dx-icon-revert');
      } else {
        $links.filter('.dx-link-edit').addClass('dx-icon-edit');
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
      }
    }
  }

  private onContentReady(e) {
    let grid = e.component;
    if (grid.totalCount() === 0) {
      grid.option('height', 100);
    } else {
      grid.option('height', 'auto');                
    }
  }
};
