import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfigurationActions } from '../../actions/configuration.actions';
import { IConfiguration } from '../../store';
import * as _ from 'lodash';

@Component({
  selector: 'reg-configuration',
  template: `
    <div class="container-fluid border-light background-white pb2">
      <reg-page-header>{{ this.tableName() }}</reg-page-header>

      <dx-data-grid [dataSource]=this.configuration.rows [paging]='{pageSize: 10}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [5, 10, 20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }'
        rowAlternationEnabled=true,
        (onContentReady)='onContentReady($event)'
        (onCellPrepared)='onCellPrepared($event)'
        (onInitNewRow)='onInitNewRow($event)'
        (onEditingStart)='onEditingStart($event)'
        (onRowRemoving)='onRowRemoving($event)'>
        <dxo-editing mode="form" [allowUpdating]="true" [allowDeleting]="true" [allowAdding]="true">
        </dxo-editing>
        <div *dxTemplate="let data of 'cellTemplate'">
          <reg-structure-image [src]="data.value"></reg-structure-image>
        </div>
      </dx-data-grid>
    </div>
  `,
  styles: [require('./configuration.component.css')]
})
export class RegConfiguration implements OnInit, OnDestroy {
  @Input() configuration: IConfiguration;
  private sub: any;

  constructor(private route: ActivatedRoute, private configurationActions: ConfigurationActions) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      let paramLabel = 'tableId';
      this.configurationActions.openTable(params[paramLabel]);
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  onContentReady(e) {
    e.component.columnOption(0, 'visible', false);
    e.component.columnOption('STRUCTURE', {
      width: 150,
      cellTemplate: 'cellTemplate'
    });
    e.component.columnOption('command:edit', {
      visibleIndex: -1,
      width: 80
    });
  }

  onCellPrepared(e) {
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

  onInitNewRow(e) {
  }

  onEditingStart(e) {
  }

  onRowRemoving(e) {
  }

  tableName() {
    let tableName = this.configuration.tableId;
    tableName = tableName.toLowerCase()
      .replace('vw_', '').replace('domain', ' domain').replace('type', ' type');
    if (!tableName.endsWith('s')) {
      tableName += 's';
    }
    return tableName.split(' ').map(n => _.upperFirst(n)).join(' ');
  }
};
