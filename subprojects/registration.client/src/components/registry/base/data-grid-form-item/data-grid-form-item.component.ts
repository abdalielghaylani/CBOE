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
