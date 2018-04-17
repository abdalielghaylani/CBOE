import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegFormView } from '../form-view';
import { RegStructureFormItem } from '../structure-form-item';

@Component({
  selector: 'reg-search-form-view',
  template: require('./search-form-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchFormView extends RegFormView {
  constructor() {
    super();
  }
};
