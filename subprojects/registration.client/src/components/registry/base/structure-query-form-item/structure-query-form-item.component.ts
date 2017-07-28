import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';
import { RegTagBoxFormItem } from '../tag-box-form-item';
import { NgRedux } from '@angular-redux/store';
import { RegStructureFormItem } from '../structure-form-item';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-structure-query-form-item-template',
  template: require('./structure-query-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureQueryFormItem extends RegStructureFormItem {
  constructor(ngRedux: NgRedux<IAppState>, elementRef: ElementRef) {
    super(ngRedux, elementRef);
  }
};
