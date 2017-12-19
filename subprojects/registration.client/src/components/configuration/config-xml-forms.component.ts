import { Component, ElementRef } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import { getExceptionMessage, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { ILookupData } from '../../redux';
import { HttpService } from '../../services';
import { RegConfigBaseComponent } from './config-base';

declare var jQuery: any;

@Component({
  selector: 'reg-config-xml-forms',
  template: require('./config-xml-forms.component.html'),
  styles: [require('./config.component.css')],
  host: { '(document:click)': 'onDocumentClick($event)' }
})
export class RegConfigXmlForms extends RegConfigBaseComponent {
  private rows: any[] = [];
  private dataSource: CustomStore;
  private popup = { visible: false, title: '', data: '', key: {} };

  constructor(elementRef: ElementRef, http: HttpService) {
    super(elementRef, http);
  }

  loadData(lookups: ILookupData) {
    this.dataSource = this.createCustomStore(this);
    this.gridHeight = this.getGridHeight();
  }

  saveXml() {
    this.dataSource.update(this.popup.key, { name: this.popup.title, data: this.popup.data });
  }

  copyToClipboard(event) {
    let $temp = $('<input>');
    $('body').append($temp);
    $temp.val(this.popup.data).select();
    document.execCommand('copy');
    $temp.remove();
  }

  onEditingStart(e) {
    this.popup = { visible: true, data: e.data.data, title: e.data.name, key: e.key };
    e.cancel = true;
  }

  private createCustomStore(parent: RegConfigXmlForms): CustomStore {
    let tableName = 'xml-forms';
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
        parent.http.put(`${apiUrlBase}`, values)
          .toPromise()
          .then(result => {
            notifySuccess(`The XML- ${key.name} was updated successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = getExceptionMessage(`The XML- ${key.name} was not updated due to a problem`, error);
            deferred.reject(message);
          });
        return deferred.promise();
      }
    });
  }

  cancel(e) {
    this.popup.visible = false;
  }

};
