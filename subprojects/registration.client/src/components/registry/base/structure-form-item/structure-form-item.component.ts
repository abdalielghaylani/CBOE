import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';
import { RegTagBoxFormItem } from '../tag-box-form-item';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { ChemDrawWeb } from '../../../common';

@Component({
  selector: 'reg-structure-form-item-template',
  template: require('./structure-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureFormItem extends ChemDrawWeb implements IFormItemTemplate {
  @Input() viewModel: any = {};
  @Input() viewConfig;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();

  constructor(private ngRedux: NgRedux<IAppState>, elementRef: ElementRef) {
    super(elementRef);
  }

  deserializeValue(value: any): any {
    if (typeof value === 'object' && value.viewModel) {
      value = value.toString();
    }
    return value;
  }

  serializeValue(value: any): any  {
    return value;
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    this.setValue(options && options.value ? this.deserializeValue(options.value) : null);
    super.update();
  }

  protected onContentChanged(e) {
    if (this.cdd && !this.cdd.isSaved()) {
      this.valueUpdated.emit(this);
    }
  }

  protected onValidatorInitialized(e, d) {
    e.component.customRules = d.editorOptions.customRules;
  }

  protected validate(options) {
    return true;
  }
};
