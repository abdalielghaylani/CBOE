import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { CSearchCriteria, ISearchCriteriaItem, getSearchCriteriaItemObj } from '../registry-base.types';
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
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    this.formData = (this.viewModel as CSearchCriteria).getQueryFormData(this.viewConfig, this.displayMode, validItems.map(i => i.dataField));
  }

  protected writeVM() {
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    (this.viewModel as CSearchCriteria).updateFromQueryFormData(this.formData, this.viewConfig, this.displayMode, validItems);
  }
};
