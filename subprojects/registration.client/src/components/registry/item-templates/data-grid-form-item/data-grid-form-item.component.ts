import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'reg-data-grid-form-item-template',
  template: require('./data-grid-form-item.component.html'),
  styles: [require('../item-templates.css')],
})
export class RegDataGridFormItem {
  @Input() private disabled: boolean = false;
  @Input() private name: string;
  @Input() private data: any;

  private get value(): any[] {
    return this.data.editorOptions.value ? this.data.editorOptions.value : [];
  }

  private onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }

  /*
.dxDataGrid({
            disabled: !container.editMode,
            dataSource: d.editorOptions.value ? d.editorOptions.value : [],
            columns: [{
              dataField: 'fragmentTypeId',
              caption: 'Type',
              editorType: 'dxSelectBox',
              lookup: {
                dataSource: lookups ? lookups.fragmentTypes : [],
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
            }],
            editing: {
              mode: 'row',
              allowUpdating: true,
              allowDeleting: true,
              allowAdding: true
            }
          });  
   */
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
