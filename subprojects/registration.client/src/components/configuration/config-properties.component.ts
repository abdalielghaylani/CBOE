import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
import { select } from '@angular-redux/store';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { ConfigurationActions } from '../../actions/configuration.actions';
import { notify, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { ICustomTableData, IConfiguration } from '../../store';

declare var jQuery: any;

@Component({
  selector: 'reg-config-properties',
  template: require('./config-properties.component.html'),
  styles: [require('./config.component.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegConfigProperties implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @select(s => s.configuration.customTables) customTables$: Observable<any>;
  private rows: any[] = [];
  private dataSubscription: Subscription;
  private gridHeight: string;
  private dataSource: CustomStore;

  constructor(
    private http: Http,
    private changeDetector: ChangeDetectorRef,
    private configurationActions: ConfigurationActions,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {
    this.dataSubscription = this.customTables$.subscribe((customTables: any) => this.loadData(customTables));
  }

  ngOnDestroy() {
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
  }

  loadData(customTables: any) {
    if (customTables) {
      this.dataSource = this.createCustomStore(this);
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

  private createCustomStore(parent: RegConfigProperties): CustomStore {
    let tableName = 'properties';
    let apiUrlBase = `${apiUrlPrefix}${tableName}`;
    return new CustomStore({
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        parent.http.get(apiUrlBase)
          .toPromise()
          .then(result => {
            let rows = result.json().rows;
            deferred.resolve(rows, { totalCount: rows.length });
          })
          .catch(error => {
            let message = `The records of ${tableName} were not retrieved properly due to a problem`;
            let errorResult, reason;
            if (error._body) {
              errorResult = JSON.parse(error._body);
              reason = errorResult.Message;
            }
            message += (reason) ? ': ' + reason : '!';
            deferred.reject(message);
          });
        return deferred.promise();
      },

      update: function(key, values) {
        let deferred = jQuery.Deferred();
        let data = key;
        let newData = values;
        for (let k in newData) {
          if (newData.hasOwnProperty(k)) {
            data[k] = newData[k];
          }
        }
        let id = data[Object.getOwnPropertyNames(data)[0]];
        parent.http.put(`${apiUrlBase}/${id}`, data)
          .toPromise()
          .then(result => {
            notifySuccess(`The record ${id} of ${tableName} was updated successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = `The record ${id} of ${tableName} was not updated due to a problem`;
            let errorResult, reason;
            if (error._body) {
              errorResult = JSON.parse(error._body);
              reason = errorResult.Message;
            }
            message += (reason) ? ': ' + reason : '!';
            deferred.reject(message);
          });
        return deferred.promise();
      },

      insert: function (values) {
        let deferred = jQuery.Deferred();
        parent.http.post(`${apiUrlBase}`, values)
          .toPromise()
          .then(result => {
            let id = result.json().id;
            notifySuccess(`A new record ${id} of ${tableName} was created successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = `Creating A new record for ${tableName} was failed due to a problem`;
            let errorResult, reason;
            if (error._body) {
              errorResult = JSON.parse(error._body);
              reason = errorResult.Message;
            }
            message += (reason) ? ': ' + reason : '!';
            deferred.reject(message);
          });
        return deferred.promise();
      },

      remove: function (key) {
        let deferred = jQuery.Deferred();
        let id = key[Object.getOwnPropertyNames(key)[0]];
        parent.http.delete(`${apiUrlBase}/${id}`)
          .toPromise()
          .then(result => {
            notifySuccess(`The record ${id} of ${tableName} was deleted successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = `The record ${id} of ${tableName} was not deleted due to a problem`;
            let errorResult, reason;
            if (error._body) {
              errorResult = JSON.parse(error._body);
              reason = errorResult.Message;
            }
            message += (reason) ? ': ' + reason : '!';
            deferred.reject(message);
          });
        return deferred.promise();
      }
    });
  }
};
