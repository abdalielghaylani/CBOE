import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
import { select } from '@angular-redux/store';
import { DxDataGridComponent } from 'devextreme-angular';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { ConfigurationActions } from '../../actions/configuration.actions';
import { notify, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { ICustomTableData, IConfiguration } from '../../store';

declare var jQuery: any;

@Component({
  selector: 'reg-configuration',
  template: require('./configuration.component.html'),
  styles: [require('./configuration.component.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },  
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegConfiguration implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @select(s => s.configuration.customTables) customTables$: Observable<any>;
  private tableId: string;
  private rows: any[] = [];
  private tableIdSubscription: Subscription;
  private dataSubscription: Subscription;
  private gridHeight: string;

  constructor(
    private route: ActivatedRoute,
    private http: Http,
    private changeDetector: ChangeDetectorRef,
    private configurationActions: ConfigurationActions,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {
    this.tableIdSubscription = this.route.params.subscribe(params => {
      let paramLabel = 'tableId';
      this.tableId = params[paramLabel];
      this.configurationActions.openTable(this.tableId);
    });
    this.dataSubscription = this.customTables$.subscribe((customTables: any) => this.loadData(customTables));
  }

  ngOnDestroy() {
    if (this.tableIdSubscription) {
      this.tableIdSubscription.unsubscribe();
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
    this.gridHeight = this.getGridHeight();
  }

  private getGridHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  private onResize(event: any) {
    this.gridHeight = this.getGridHeight();
    this.grid.height = this.getGridHeight();
    this.grid.instance.repaint();
  }

  private onDocumentClick(event: any) {
    if (event.srcElement.title === 'Full Screen') {
      let fullScreenMode = event.srcElement.className === 'fa fa-compress fa-stack-1x white';
      this.gridHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
      this.grid.height = this.gridHeight;
      this.grid.instance.repaint();
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
    // TODO: Should use redux
    let deferred = jQuery.Deferred();
    let id = e.data[Object.keys(e.data)[0]];
    let url = `${apiUrlPrefix}custom-tables/${this.tableId}/${id}`;
    this.http.delete(url)
      .toPromise()
      .then(result => {
        notifySuccess(`The record (Table: ${this.tableId}, ID: ${id}) was deleted successfully!`, 5000);
        deferred.resolve(false);
      })
      .catch(error => {
        let message = `The record (Table: ${this.tableId}, ID: ${id}) was not deleted due to a problem`;
        let reason;
        if (error._body) {
            let errorResult = JSON.parse(error._body);
            reason = errorResult.Message;
        }
        message += (reason) ? ': ' + reason : '!';
        notifyError(message, 5000);
        deferred.resolve(true);
      });
    e.cancel = deferred.promise();
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
