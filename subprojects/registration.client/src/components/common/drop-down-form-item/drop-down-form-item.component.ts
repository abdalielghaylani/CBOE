import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
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

  constructor(private ngRedux: NgRedux<IAppState>) {
    super();
  }

  deserializeValue(value: any): any {
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
      if (lookups) {
        let pickListDomain = lookups.pickListDomains.find(d => d.ID === pickListDomainIndex);
        if (pickListDomain != null) {
          this.dataSource = pickListDomain.data;
          this.valueExpr = pickListDomain.EXT_ID_COL;
          this.displayExpr = pickListDomain.EXT_DISPLAY_COL;
        }
      }
    } else if (options.dropDownItemsSelect) {
      // TODO: Parse the select statement and use proper data for drop-down
    } else if (options.dataSource) {
      this.dataSource = options.dataSource;
      this.valueExpr = options.valueExpr;
      this.displayExpr = options.displayExpr;
      if (!this.value) {
        this.value = '';
      }
    }
    if (this.dataSource && this.dataSource.length > 0) {
      this.useNumericValue = typeof this.dataSource.slice(-1)[0][this.valueExpr] === 'number';
    }
    this.value = options && options.value ? this.deserializeValue(options.value) : undefined;
  }
};
