import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';
import { RegTagBoxFormItem } from '../tag-box-form-item';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { ChemDrawWeb } from '../../../common';

@Component({
  selector: 'reg-structure-form-item-template',
  template: require('./structure-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureFormItem extends ChemDrawWeb implements IFormItemTemplate {
  @Input() viewModel: any = {};
  @Input() viewConfig;

  constructor(private ngRedux: NgRedux<IAppState>, elementRef: ElementRef) {
    super(elementRef);
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    this.setValue(options && options.value ? options.value : null);
    super.update();
  }
};
