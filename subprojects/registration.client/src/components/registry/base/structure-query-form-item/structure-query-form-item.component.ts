import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';
import { RegStructureBaseFormItem } from '../structure-base-form-item';

@Component({
  selector: 'reg-structure-query-form-item-template',
  template: require('./structure-query-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureQueryFormItem extends RegStructureBaseFormItem {
  constructor(elementRef: ElementRef) {
    super(elementRef);
  }
};
