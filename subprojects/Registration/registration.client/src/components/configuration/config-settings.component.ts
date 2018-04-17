import { Component, ElementRef, ChangeDetectionStrategy } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import CustomStore from 'devextreme/data/custom_store';
import { getExceptionMessage, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { ISettingData, IAppState, ILookupData } from '../../redux';
import { HttpService } from '../../services';
import { RegConfigBaseComponent } from './config-base';

declare var jQuery: any;

@Component({
  selector: 'reg-config-settings',
  template: require('./config-settings.component.html'),
  styles: [require('./config.component.css')],
  host: { '(document:click)': 'onDocumentClick($event)' }
})
export class RegConfigSettings extends RegConfigBaseComponent {
  private dataSource: CustomStore;
  private columns = [{
    dataField: 'groupLabel',
    dataType: 'string',
    caption: 'Group',
    groupIndex: 0,
    groupCellTemplate: 'groupCellTemplate',
    allowEditing: false
  }, {
    dataField: 'name',
    dataType: 'string',
    allowEditing: false
  }, {
    dataField: 'controlType',
    dataType: 'string',
    caption: 'Type',
    width: '100px',
    allowEditing: false
  }, {
    dataField: 'value',
    dataType: 'string',
    editCellTemplate: 'valueEditTemplate'
  }, {
    dataField: 'description',
    dataType: 'string',
    allowEditing: false
  }, {
    dataField: 'processorClass',
    dataType: 'string',
    caption: 'Processor',
    width: '100px',
    allowEditing: false
  }];

  constructor(private ngRedux: NgRedux<IAppState>, elementRef: ElementRef, http: HttpService) {
    super(elementRef, http);
  }

  loadData(lookups: ILookupData) {
    this.dataSource = this.createCustomStore(this);
    this.gridHeight = this.getGridHeight();
  }

  onRowCollapsing(e) {
    e.component.cancelEditData();
  }

  private createCustomStore(parent: RegConfigSettings): CustomStore {
    let tableName = 'settings';
    let apiUrlBase = `${apiUrlPrefix}${tableName}`;
    return new CustomStore({
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        parent.http.get(apiUrlBase)
          .toPromise()
          .then(result => {
            let rows: ISettingData[] = result.json().filter(function (i) {
              if (i.name === 'EnableMixtures' || i.name === 'AllowUnregisteredComponentsInMixtures') {
                return false;
              } else { return true; }
            });
            parent.ngRedux.getState().session.lookups.systemSettings = rows;
            deferred.resolve(rows, { totalCount: rows.length });
          })
          .catch(error => {
            let message = getExceptionMessage(`The records of ${tableName} were not retrieved properly due to a problem`, error);
            deferred.reject(message);
          });
        return deferred.promise();
      },

      update: function (key, values) {
        let deferred = jQuery.Deferred();
        let data = key;
        let newData = values;
        for (let k in newData) {
          if (newData.hasOwnProperty(k)) {
            data[k] = newData[k];
          }
        }
        parent.http.put(`${apiUrlBase}`, data)
          .toPromise()
          .then(result => {
            notifySuccess(`The setting ${data.name} in ${data.groupLabel} was updated successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = getExceptionMessage(`The setting ${data.name} in ${data.groupLabel} was not updated due to a problem`, error);
            deferred.reject(message);
          });
        return deferred.promise();
      }
    });
  }
};
