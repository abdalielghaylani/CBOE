import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
import { select, NgRedux } from '@angular-redux/store';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { ConfigurationActions } from '../../actions/configuration.actions';
import { notify, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { IAppState, ICustomTableData, IConfiguration } from '../../store';
import { CConfigProperties } from './config.types';

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
  private configProperties: CConfigProperties;

  constructor(
    private http: Http,
    private changeDetector: ChangeDetectorRef,
    private ngRedux: NgRedux<IAppState>,
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
      this.configProperties = new CConfigProperties(this.ngRedux.getState());
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
  onInitNewRow(e) {
    e.cancel = true;
    this.configProperties.addEditProperty('add');
  }

  onEditingStart(e) {
    e.cancel = true;
    this.configProperties.addEditProperty('edit', e.data);
  }

  cancel() {
    this.configProperties.window = { title: 'Manage Data Properties', viewIndex: 'list' };
    this.configProperties.clearFormData();
  }

  showValidationRule() {
    this.configProperties.window = { title: 'Validation Rule', viewIndex: 'validation' };
  }

  addProperty(e) {
    this.dataSource.insert(this.configProperties.formData);
    this.cancel();
  }
  saveProperty(e) {
    this.dataSource.update(this.configProperties.formData, []);
    this.cancel();
  }
  onCellPrepared(e) {
    if (e.rowType === 'data' && e.column.command === 'edit') {
      let isEditing = e.row.isEditing;
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      if (e.data.editable) {
        $links.filter('.dx-link-edit').addClass('dx-icon-edit');
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
      } else {
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
      }
    }
  }

  onFieldDataChanged(e) {
    this.configProperties.showHideDataFields(e.value, e.component._options.items, this.configProperties.formData, e.component, true);
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
            let rows = result.json();
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

      update: function (data) {
        let deferred = jQuery.Deferred();
        parent.http.put(`${apiUrlBase}`, data)
          .toPromise()
          .then(result => {
            notifySuccess(`The Property ${data.name} was updated successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = `The Property ${data.name} was not updated due to a problem`;
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

      insert: function (data) {
        let deferred = jQuery.Deferred();
        parent.http.post(`${apiUrlBase}`, data)
          .toPromise()
          .then(result => {
            let id = result.json().id;
            notifySuccess(`The Property was created successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = `Creating A new Property was failed due to a problem`;
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
