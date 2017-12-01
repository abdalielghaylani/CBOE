import { HttpService } from './../../../services/http.service';
import { apiUrlPrefix } from './../../../configuration';
import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation, ChangeDetectorRef } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../redux';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-drop-down-form-item-template',
  template: require('./drop-down-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDropDownFormItem extends RegBaseFormItem {
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;
  protected useNumericValue: boolean = false;

  constructor(private ngRedux: NgRedux<IAppState>, private http: HttpService, private changeDetector: ChangeDetectorRef) {
    super();
  }

  deserializeValue(value: any): any {
    if (typeof value === 'string') {
      value = value.trim();
    }
    return this.useNumericValue ? +value : value;
  }

  serializeValue(value: any): any {
    return this.useNumericValue ? (value == null ? '' : value.toString()) : value;
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    if (options.pickListDomain) {
      let pickListDomainIndex = options.pickListDomain as number;
      let lookups = this.ngRedux.getState().session.lookups;
      if (lookups && lookups.pickListDomains) {
        let pickListDomain = lookups.pickListDomains.find(d => d.ID === pickListDomainIndex);
        if (pickListDomain && pickListDomain.data) {
          let valueCol = 'value';
          this.dataSource = pickListDomain.data.map(d => {
            if (d[valueCol] === '') {
              d[valueCol] = ' ';
            }
            return d;
          });
          this.valueExpr = 'key';
          this.displayExpr = 'value';
        }
      }
    } else if (options.dropDownItemsSelect) {
      let query: string;
      if (options.dropDownItemsSelect.__text) {
        query = options.dropDownItemsSelect.__text;
      } else {
        query = options.dropDownItemsSelect;
      }
      if (query && query.trim().toUpperCase().startsWith('SELECT')) {
        this.fillDropDown(this, query, options);
      }
    } else if (options.dataSource) {
      this.dataSource = options.dataSource;
      this.valueExpr = options.valueExpr;
      this.displayExpr = options.displayExpr;
      if (!this.value) {
        this.value = '';
      }
    }
    this.setValue(options);
  }

  private setValue(options) {
    // set default value, if default value is configured in the form group xml
    let isDefaultValueSet: boolean = false;

    if (this.editMode && options.defaultValue) {
      if (options.defaultValue === '&&useSortOrderTop') {
        if (this.dataSource.length > 0) {
          let topItem;
          if (options.value) {
            topItem = this.dataSource.find(item => item.key === +options.value);
          }
          if (!topItem) {
            topItem = this.dataSource[0];
          }
          options.value = topItem.key;
          isDefaultValueSet = true;
        }
      } else if (options.defaultValue === '&&loggedInUser') {
        if (!options.value) {
          let lookups = this.ngRedux.getState().session.lookups;
          if (lookups) {
            let loggedInUserName = this.ngRedux.getState().session.user.fullName.toUpperCase();
            let user = lookups.users.find(user => user.USERID.toUpperCase() === loggedInUserName);
            options.value = user.PERSONID;
            isDefaultValueSet = true;
          }
        }
      }
    }

    if (this.dataSource && this.dataSource.length > 0) {
      this.useNumericValue = typeof this.dataSource.slice(-1)[0][this.valueExpr] === 'number';
    }

    this.value = options && options.value ? this.deserializeValue(options.value) : undefined;
    // initialized default value, so update view model also
    if (isDefaultValueSet) {
      this.updateViewModel();
    }
  }

  fillDropDown(control: RegDropDownFormItem, query: string, options: any) {
    let deferred = jQuery.Deferred();
    let url = `${apiUrlPrefix}${'ViewConfig/query'}`;
    let body = { 'sql': query };

    this.http.post(url, body)
      .toPromise()
      .then(result => {
        let data = result.json().data.data;
        control.dataSource = data;
        control.valueExpr = 'KEY';
        control.displayExpr = 'VALUE';
        control.setValue(options);
        control.changeDetector.markForCheck();
        deferred.resolve(false);
      })
      .catch(error => deferred.resolve(true));
  }
};
