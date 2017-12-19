import { Component, ViewChild, ElementRef, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DxFormComponent } from 'devextreme-angular';
import DevExtreme from 'devextreme/bundles/dx.all.d';
import CustomStore from 'devextreme/data/custom_store';
import { CConfigTable } from './config.types';
import { getExceptionMessage, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { ILookupData } from '../../redux';
import { HttpService } from '../../services';
import { PrivilegeUtils } from '../../common';
import { RegConfigBaseComponent } from './config-base';

@Component({
  selector: 'reg-config-tables',
  template: require('./config-tables.component.html'),
  styles: [require('./config.component.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegConfigTables extends RegConfigBaseComponent {
  @ViewChild(DxFormComponent) form: DxFormComponent;
  private tableId: string;
  private dataSource: CustomStore;
  private configTable: CConfigTable = new CConfigTable();

  constructor(
    private route: ActivatedRoute,
    private changeDetector: ChangeDetectorRef,
    elementRef: ElementRef,
    http: HttpService
  ) {
    super(elementRef, http);
  }

  refreshDataGrid() {
    if (this.grid.instance) {
      this.grid.instance.refresh();
      this.grid.instance.clearFilter();
    }
  }

  loadData(lookups: ILookupData) {
    this.route.params.subscribe((params => {
      const paramLabel = 'tableId';
      const tableId = params[paramLabel];
      if (this.tableId !== tableId) {
        this.http.get(`${apiUrlPrefix}custom-tables/${tableId}?configOnly=true`)
          .subscribe((r => {
            this.tableId = tableId;
            this.configTable = new CConfigTable(this.tableId, this.tableName(), r.json());
            this.dataSource = this.createCustomStore();
            this.gridHeight = this.getGridHeight();
            this.changeDetector.markForCheck();
          }).bind(this), err => {
            if (err.status == null || err.status !== 404) {
              notifyError(err, 5000);
            }
          });
      }
    }).bind(this));
  }

  onCellPrepared(e) {
    if (e.rowType === 'data' && e.column.command === 'edit') {
      let isEditing = e.row.isEditing;
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      if (isEditing) {
        $links.filter('.dx-link-save').addClass('dx-icon-save');
        $links.filter('.dx-link-save').attr({ 'data-toggle': 'tooltip', 'title': 'Save' });
        $links.filter('.dx-link-cancel').addClass('dx-icon-revert');
        $links.filter('.dx-link-cancel').attr({ 'data-toggle': 'tooltip', 'title': 'Cancel' });
      } else {
        if (this.hasPrivilege('EDIT')) {
          $links.filter('.dx-link-edit').addClass('dx-icon-edit');
          $links.filter('.dx-link-edit').attr({ 'data-toggle': 'tooltip', 'title': 'Edit' });
        }
        if (this.hasPrivilege('DELETE')) {
          $links.filter('.dx-link-delete').addClass('dx-icon-trash');
          $links.filter('.dx-link-delete').attr({ 'data-toggle': 'tooltip', 'title': 'Delete' });
        }
      }
    }
  }

  onInitNewRow(e) {
    if (this.tableId === 'VW_FRAGMENT') {
      this.configTable.addEdit(e, 'add');
    }
  }

  onEditingStart(e) {
    if (this.tableId === 'VW_FRAGMENT') {
      this.configTable.addEdit(e, 'edit');
      e.cancel = true;
    }
  }

  cancel(e) {
    this.configTable.cancel(e);
    this.grid.instance.cancelEditData();
  }

  addConfigData(e) {
    let res: any = this.form.instance.validate();

    if (this.tableId === 'VW_FRAGMENT' &&
      this.configTable.formColumns.find(i => i.dataField === 'STRUCTURE').validationRules.find(r => r.type === 'required') !== null
      && this.configTable.formData.STRUCTURE_XML === undefined) {
      notifyError('There must be a valid chemical structure.');
      return;
    }

    if (res.isValid) {
      if (this.tableId === 'VW_FRAGMENT') {
        this.configTable.formData.STRUCTURE = this.configTable.formData.STRUCTURE_XML;
      }
      this.dataSource.insert(this.configTable.formData).done(result => {
        if (this.tableId === 'VW_FRAGMENT') {
          this.configTable.cancel(e);
        }
        this.grid.instance.refresh();
      }).fail(err => {
        notifyError(err, 5000);
      });
    }
  }

  saveConfigData(e) {
    let res: any = this.form.instance.validate();
    if (res.isValid) {
      if (this.tableId === 'VW_FRAGMENT') {
        this.configTable.formData.STRUCTURE = this.configTable.formData.STRUCTURE_XML;
      }
      this.dataSource.update(this.configTable.formData, []).done(result => {
        if (this.tableId === 'VW_FRAGMENT') {
          this.configTable.cancel(e);
        }
        this.grid.instance.refresh();
      }).fail(err => {
        notifyError(err, 5000);
      });
    }
  }

  private hasPrivilege(action: string): boolean {
    if (this.lookups == null) {
      return false;
    }
    let retValue: boolean = false;
    switch (this.tableId) {
      case 'VW_PROJECT':
        retValue = PrivilegeUtils.hasProjectsTablePrivilege(action, this.lookups.userPrivileges);
        break;
      case 'VW_NOTEBOOKS':
        // deletion is not allowed for Notebooks as it will corrupt existing data
        if (action === 'DELETE') {
          return false;
        }
        retValue = PrivilegeUtils.hasNotebookTablePrivilege(action, this.lookups.userPrivileges);
        break;
      case 'VW_FRAGMENT':
      case 'VW_FRAGMENTTYPE':
        retValue = PrivilegeUtils.hasSaltTablePrivilege(action, this.lookups.userPrivileges);
        break;
      case 'VW_SEQUENCE':
        retValue = PrivilegeUtils.hasSequenceTablePrivilege(action, this.lookups.userPrivileges);
        break;
      case 'VW_PICKLIST':
      case 'VW_PICKLISTDOMAIN':
        // deletion is not allowed for Picklist Values, Picklist Domains as it will corrupt existing data
        if (action === 'DELETE') {
          return false;
        }
        retValue = PrivilegeUtils.hasPicklistTablePrivilege(action, this.lookups.userPrivileges);
        break;
      case 'VW_IDENTIFIERTYPE':
        // deletion is not allowed for Identifier Types as it will corrupt existing data
        if (action === 'DELETE') {
          return false;
        }
        retValue = PrivilegeUtils.hasIdentifierTablePrivilege(action, this.lookups.userPrivileges);
        break;
      case 'VW_SITES':
        retValue = PrivilegeUtils.hasSitesTablePrivilege(action, this.lookups.userPrivileges);
        break;
    }
    return retValue;
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

  private createCustomStore(): CustomStore {
    let tableName = this.tableId;
    let apiUrlBase = `${apiUrlPrefix}custom-tables/${tableName}`;
    return new CustomStore({
      load: ((loadOptions: DevExtreme.data.LoadOptions): Promise<any> => {
        return new Promise<void>((resolve, reject) => {
          this.http.get(apiUrlBase)
            .toPromise()
            .then(result => {
              let rows = result.json().rows;
              resolve(rows);
            })
            .catch(error => {
              let message = getExceptionMessage(`The records of ${tableName} were not retrieved properly due to a problem`, error);
              reject(message);
            });
        });
      }).bind(this),

      update: ((key, values): Promise<any> => {
        let data = key;
        let newData = values;
        for (let k in newData) {
          if (newData.hasOwnProperty(k)) {
            data[k] = newData[k];
          }
        }
        let id = data[Object.getOwnPropertyNames(data)[0]];
        return new Promise<any>((resolve, reject) => {
          this.http.put(`${apiUrlBase}/${id}`, data)
            .toPromise()
            .then(result => {
              notifySuccess(`The record ${id} of ${tableName} was updated successfully!`, 5000);
              resolve(result.json());
            })
            .catch(error => {
              let message = getExceptionMessage(`The record ${id} of ${tableName} was not updated due to a problem`, error);
              reject(message);
            });
        });
      }).bind(this),

      insert: ((values): Promise<any> => {
        return new Promise<any>((resolve, reject) => {
          this.http.post(`${apiUrlBase}`, values)
            .toPromise()
            .then(result => {
              let id = result.json().id;
              notifySuccess(`A new record ${id} of ${tableName} was created successfully!`, 5000);
              resolve(result.json());
            })
            .catch(error => {
              let message = getExceptionMessage(`Creating a new record for ${tableName} failed due to a problem`, error);
              reject(message);
            });
        });
      }).bind(this),

      remove: ((key): Promise<any> => {
        let id = key[Object.getOwnPropertyNames(key)[0]];
        return new Promise<any>((resolve, reject) => {
          this.http.delete(`${apiUrlBase}/${id}`)
            .toPromise()
            .then(result => {
              notifySuccess(`The record ${id} of ${tableName} was deleted successfully!`, 5000);
              resolve(result.json());
            })
            .catch(error => {
              let message = getExceptionMessage(`The record ${id} of ${tableName} was not deleted due to a problem`, error);
              reject(message);
            });
        });
      }).bind(this)
    });
  }

  onValueChanged(e, d) {
    d.setValue(e.value, d.column.dataField);
  }
};
