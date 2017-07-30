import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { IFormItemTemplate } from '../registry-base.types';

@Component({
  selector: 'reg-drop-down-form-item-template',
  template: require('./drop-down-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDropDownFormItem implements IFormItemTemplate, OnChanges {
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() viewModel: any = {};
  @Input() viewConfig: any;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();  
  protected dataSource: any[];
  protected value: number;
  protected valueExpr: string;
  protected displayExpr: string;

  constructor(private ngRedux: NgRedux<IAppState>) {
  }

  ngOnChanges() {
    this.update();
  }

  deserializeValue(value: string): number {
    return +value;
  }

  serializeValue(value: number): string  {
    return value.toString();
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    this.value = options && options.value ? this.deserializeValue(options.value) : undefined;
    if (options.pickListDomain) {
      let pickListDomain = options.pickListDomain as number;
      let lookups = this.ngRedux.getState().session.lookups;
      if (lookups) {
        let filtered = lookups.pickListDomains.filter(d => d.ID === pickListDomain);
        if (filtered) {
          let pickListDomainInfo = filtered[0];
          this.dataSource = pickListDomainInfo.data;
          this.valueExpr = pickListDomainInfo.EXT_ID_COL;
          this.displayExpr = pickListDomainInfo.EXT_DISPLAY_COL;
        }
      }
    } else if (options.dropDownItemsSelect) {
      // TODO: Parse the select statement and use proper data for drop-down
    }
  }

  protected onValueChanged(e, d) {
    if (e.previousValue !== e.value) {
      let value = e.value;
      d.component.option('formData.' + d.dataField, this.serializeValue(e.value));
      this.onValueUpdated(this);
    }
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }
};
