import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChildren,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, NgRedux } from '@angular-redux/store';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { CConfigProperties, CPropertiesValidationFormDataModel } from './config.types';
import { getExceptionMessage, notify, notifyError, notifyException, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { ConfigurationActions, IAppState, ICustomTableData, IConfiguration } from '../../redux';
import { HttpService } from '../../services';

declare var jQuery: any;

@Component({
  selector: 'reg-config-properties',
  template: require('./config-properties.component.html'),
  styles: [require('./config.component.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegConfigProperties implements OnInit, OnDestroy {
  @ViewChildren(DxDataGridComponent) grid;
  @ViewChildren(DxFormComponent) forms;
  @select(s => s.configuration.customTables) customTables$: Observable<any>;
  private rows: any[] = [];
  private dataSubscription: Subscription;
  private gridHeight: string;
  private dataSource: CustomStore;
  private configProperties: CConfigProperties;

  constructor(
    private http: HttpService,
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
    this.grid._results[0].instance.repaint();
  }

  private onDocumentClick(event: any) {
    if (event.srcElement.title === 'Full Screen') {
      let fullScreenMode = event.srcElement.className === 'fa fa-compress fa-stack-1x white';
      this.gridHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
      this.grid.height = this.gridHeight;
      this.grid._results[0].instance.repaint();
    }
  }

  onContentReady(e) {
    e.component.columnOption('command:edit', {
      visibleIndex: -1,
      width: 80
    });
  }

  onInitNewRow(e, parent?: boolean) {
    if (parent) {
      this.configProperties.addEditProperty('add');
    } else {
      e.component.cancelEditData();
      e.component.refresh();
    }
  }

  onEditingStart(e) {
    e.cancel = true;
    if (e.data.editable) {
      this.configProperties.addEditProperty('edit', e.data);
    }
  }

  cancel() {
    this.configProperties.window = { title: 'Manage Data Properties', viewIndex: 'list' };
    this.configProperties.clearFormData();
    this.grid._results[0].instance.cancelEditData();
  }

  showValidationRule(d: any) {
    this.configProperties.window = { title: 'Validation Rule', viewIndex: 'validation' };
    this.configProperties.formData = d.data;
  }
  moveupordown(data, move) {
    if ((move === 'up' && data.value > 0) || (move === 'down' && data.data.sortOrderMax)) {
      let val = data.data;
      if (move === 'up') {
        val.sortOrder = val.sortOrder - 1;
      } else {
        val.sortOrder = val.sortOrder + 1;
      }
      this.dataSource.update(val, []).done(result => {
        this.grid._results[0].instance.refresh();
      }).fail(err => {
        notifyError(err, 5000);
      });
    }
  }

  addProperty(e) {
    let valid = this.forms._results[0].instance.validate();
    if (this.configProperties.combuteValidation(valid.brokenRules)) {
      this.dataSource.insert(this.configProperties.formData).done(result => {
        this.grid._results[0].instance.refresh();
      }).fail(err => {
        notifyError(err, 5000);
      });
      this.cancel();
    }
  }

  saveProperty(e) {
    let valid = this.forms._results[0].instance.validate();
    if (this.configProperties.combuteValidation(valid.brokenRules)) {
      this.dataSource.update(this.configProperties.formData, []).done(result => {
        this.grid._results[0].instance.refresh();
      }).fail(err => {
        notifyError(err, 5000);
      });
      this.cancel();
    }
  }
  onValidationRowRemoved(e) {
    this.dataSource.update(this.configProperties.formData, []);
  }

  saveValidationRule(e) {
    if (this.configProperties.isValidRule()) {
      if (this.configProperties.formData.validationRules.filter(validationRule =>
            validationRule.name === this.configProperties.formDataValidation.name).length > 0) {
            notifyError('The Validation Rule that you are trying to add already exists', 5000);
          } else {
            let validationModel: CPropertiesValidationFormDataModel;
            switch (this.configProperties.formDataValidation.name) {
              case 'custom':
                validationModel = this.configProperties.formDataValidation;
                validationModel.parameters.push({ name: 'clientscript', value: this.configProperties.formDataValidation.clientScript });
                this.configProperties.formData.validationRules.push(validationModel);
                break;
              default:
                validationModel = this.configProperties.formDataValidation;
                this.configProperties.formData.validationRules.push(validationModel);
                break;

            }
            this.dataSource.update(this.configProperties.formData, []).then((result) => {
              this.configProperties.clearFormDataValidations();
              this.grid._results[1].instance.refresh();
            });
          }
    }
  }

  onCellPrepared(e, t?: string) {

    let disableEditCause = 'This type of property is not editable because it would '
      + 'affect existing data. You can either delete or hide this property and '
      + 'create a new property with the desired changes. In order to delete the '
      + 'property, you must first delete any data values previously stored in it.';

    let disableEditIcon = '<i class="dx-icon-edit" rel="tooltip" title="'
      + disableEditCause + '" style="font-size:18px;color:silver;cursor:pointer"></i>';

    if (e.rowType === 'data' && e.column.command === 'edit') {
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      if (e.data.editable || t === 'validation') {
        $links.filter('.dx-link-edit').addClass('dx-icon-edit');
        $links.filter('.dx-link-edit').attr({ 'data-toggle': 'tooltip', 'title': 'Edit' });
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
        $links.filter('.dx-link-delete').attr({ 'data-toggle': 'tooltip', 'title': 'Delete' });
      } else {
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
        $links.filter('.dx-link-delete').attr({ 'data-toggle': 'tooltip', 'title': 'Delete' });
        $links.filter('.dx-link-edit').append(disableEditIcon);
      }
    }
  }

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
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
            let rows = result.json();
            let groupBy = function (xs, key) {
              return xs.reduce(function (rv, x) {
                (rv[x[key]] = rv[x[key]] || []).push(x);
                return rv;
              }, {});
            };
            let groubedByTeam = groupBy(rows, 'groupLabel');
            rows.forEach(element => {
              element.sortOrderMax = true;
              if (groubedByTeam[element.groupLabel].length === (element.sortOrder + 1)) {
                element.sortOrderMax = false;
              }
            });
            deferred.resolve(rows, { totalCount: rows.length });
          })
          .catch(error => {
            let message = getExceptionMessage(`The records of ${tableName} were not retrieved properly due to a problem`, error);
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
            let message = getExceptionMessage(`The Property ${data.name} was not updated due to a problem`, error);
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
            notifySuccess(`The Property was created successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = getExceptionMessage(`Creating a new property failed due to a problem`, error);
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
            let message = getExceptionMessage(`The record ${id} of ${tableName} was not deleted due to a problem`, error);
            deferred.reject(message);
          });
        return deferred.promise();
      }
    });
  }
};
