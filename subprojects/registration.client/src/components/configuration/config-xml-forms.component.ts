import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
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
import { HttpService } from '../../services';

declare var jQuery: any;

@Component({
  selector: 'reg-config-xml-forms',
  template: require('./config-xml-forms.component.html'),
  styles: [require('./config.component.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegConfigXmlForms implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @select(s => s.configuration.customTables) customTables$: Observable<any>;
  private rows: any[] = [];
  private dataSubscription: Subscription;
  private gridHeight: string;
  private dataSource: CustomStore;
  private popup = { visible: false, title: '', data: '', key: {} };

  constructor(
    private http: HttpService,
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
  onEditingStart(e) {
    this.popup = { visible: true, data: e.data.data, title: e.data.name, key: e.key };
    e.cancel = true;
  }
  onContentReady(e) {
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
      }
    }
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
            let message = `The XML- ${key.name} was not updated due to a problem`;
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

  cancel(e) {
    this.popup.visible = false;
  }

};
