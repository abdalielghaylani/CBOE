import { Component, Input, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { CSearchCriteria } from '../registry-base.types';
import { RegFormGroupItemBase } from '../form-group-item-base';

@Component({
  selector: 'reg-search-form-group-item-view',
  template: require('./search-form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchFormGroupItemView extends RegFormGroupItemBase {
  @Input() viewModel: CSearchCriteria;

  protected readVM() {
    let validItems = this.getValidItems().map(i => i.dataField);
    this.formData = (this.viewModel as CSearchCriteria).getQueryFormData(this.viewConfig, this.displayMode, validItems);
  }

  protected writeVM() {
    let validItems = this.getValidItems().map(i => i.dataField);
    (this.viewModel as CSearchCriteria).updateFromQueryFormData(this.formData, this.viewConfig, this.displayMode, validItems);
  }
}
