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
  protected dataSource: any[];
  protected value: Number;
  protected valueExpr: string;
  protected displayExpr: string;

  constructor(private ngRedux: NgRedux<IAppState>) {
  }

  ngOnChanges() {
    this.update();
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    this.value = options && options.value ? +options.value : undefined;
    let pickListDomain = options.pickListDomain as Number;
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
  }

  protected onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }
};
