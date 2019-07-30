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
  private loadingVisible: boolean = false;
  public isEditable: boolean = false;

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

  pasteFromClipboard(event) {
    let key = 'clipboard';
    navigator[key].readText().then(clipText => {
      this.popup.data = clipText;
    });
  }

  onEditingStart(e) {
    this.isEditable = false;
    this.popup = { visible: true, data: e.data.data, title: e.data.name, key: e.key };
    e.cancel = true;
  }

  onCellPrepared(e) {
    if (e.rowType === 'data' && e.column.command === 'edit') {
      let isEditing = e.row.isEditing;
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      if (!isEditing) {
        // For Edit
        let $editIcon = $links.filter('.dx-link-edit');
        $editIcon.addClass('fa fa-info-circle');
        $editIcon.attr({ 'data-toggle': 'tootip', 'title': 'XML Detail view' });
      }
    }
  }

  editXML(e) {
    this.isEditable = e;
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
        parent.loadingVisible = true;
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
            parent.cancel();
            parent.loadingVisible = false;
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = getExceptionMessage(`The XML- ${key.name} was not updated due to a problem`, error);
            parent.loadingVisible = false;
            deferred.reject(notifyError(message, 5000));
          });
        return deferred.promise();
      }
    });
  }

  cancel() {
    this.popup.visible = false;
  }

}
