import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegFormGroupItemView } from '../form-group-item-view';

@Component({
  selector: 'reg-search-form-group-item-view',
  template: require('./search-form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchFormGroupItemView extends RegFormGroupItemView {
  constructor() {
    super();
  }

  protected readVM() {
    // let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    // this.formData = this.getFormData(validItems.map(i => i.dataField));
  }

  protected writeVM() {
    // let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    // this.viewConfig.readFormData(this.displayMode, validItems.map(i => i.dataField), this.viewModel, this.formData);
  }
};
