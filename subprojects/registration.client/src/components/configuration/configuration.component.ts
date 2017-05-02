import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select } from '@angular-redux/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { ConfigurationActions } from '../../actions/configuration.actions';
import { ICustomTableData, IConfiguration } from '../../store';

@Component({
  selector: 'reg-configuration',
  template: `
    <div class="viewcontainer">
      <reg-page-header>{{ this.tableName() }}</reg-page-header>

      <dx-data-grid [dataSource]=this.rows [paging]='{pageSize: 10}' 
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
  styles: [require('./configuration.component.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegConfiguration implements OnInit, OnDestroy {
  @select(s => s.configuration.customTables) customTables$: Observable<any>;
  private tableId: string;
  private rows: any[] = [];
  private sub: Subscription;
  private dataSubscription: Subscription;

  constructor(
    private route: ActivatedRoute,
    private changeDetector: ChangeDetectorRef,
    private configurationActions: ConfigurationActions
  ) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      let paramLabel = 'tableId';
      this.tableId = params[paramLabel];
      this.configurationActions.openTable(this.tableId);
    });
    this.dataSubscription = this.customTables$.subscribe((customTables: any) => this.loadData(customTables));
  }

  ngOnDestroy() {
    if (this.sub) {
      this.sub.unsubscribe();
    }
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }    
  }

  loadData(customTables: any) {
    if (customTables && customTables[this.tableId]) {
      let customTableData: ICustomTableData = customTables[this.tableId];
      this.rows = customTableData.rows;
      this.changeDetector.markForCheck();
    }
  }
  
  onContentReady(e) {
    e.component.columnOption(0, 'visible', false);
    e.component.columnOption('STRUCTURE', {
      width: 150,
      allowFiltering: false,
      allowSorting: false,
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
    let tableName = this.tableId;
    tableName = tableName.toLowerCase()
      .replace('vw_', '').replace('domain', ' domain').replace('type', ' type');
    if (!tableName.endsWith('s')) {
      tableName += 's';
    }
    return tableName.split(' ').map(n => n.charAt(0).toUpperCase() + n.slice(1)).join(' ');
  }
};
