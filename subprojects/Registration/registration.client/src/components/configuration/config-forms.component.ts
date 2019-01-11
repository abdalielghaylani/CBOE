import { Component, ElementRef } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import { CConfigForms } from './config.types';
import { getExceptionMessage, notifyError, notifySuccess } from '../../common';
import { ILookupData } from '../../redux';
import { apiUrlPrefix } from '../../configuration';
import { HttpService } from '../../services';
import { RegConfigBaseComponent } from './config-base';

declare var jQuery: any;

@Component({
  selector: 'reg-config-forms',
  template: require('./config-forms.component.html'),
  styles: [require('./config.component.css')],
  host: { '(document:click)': 'onDocumentClick($event)' }
})
export class RegConfigForms extends RegConfigBaseComponent {
  private rows: any[] = [];
  private dataSource: CustomStore;
  private configForms: CConfigForms;

  constructor(elementRef: ElementRef, http: HttpService) {
    super(elementRef, http);
  }

  loadData(lookups: ILookupData) {
    this.dataSource = this.createCustomStore(this);
    this.gridHeight = this.getGridHeight();
    this.configForms = new CConfigForms();
  }

  editLookupValueChanged(e, d) {
    d.setValue(e.value, d.column.dataField);
  }

  private createCustomStore(parent: RegConfigForms): CustomStore {
    let tableName = 'forms';
    let apiUrlBase = `${apiUrlPrefix}${tableName}`;
    return new CustomStore({
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        parent.http.get(apiUrlBase)
          .toPromise()
          .then(result => {
            let rows = result.json().filter(i => i.group.toLowerCase() !== 'batch component');
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
            notifySuccess(`The property ' ${data.label} ' of ${data.group} was updated successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = getExceptionMessage(`The property ' ${data.label} ' of ${data.group} was not updated due to a problem`, error);
            deferred.reject(message);
          });
        return deferred.promise();
      }
    });
  }
}
