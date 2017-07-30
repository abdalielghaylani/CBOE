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

  protected onFieldDataChanged(e) {
    if (this.viewModelCopy[e.dataField] !== e.value) {
      this.viewModelCopy[e.dataField] = e.value;
      this.onValueUpdated(this);
    }
  }

  protected onStructureValueUpdated(structureFormItem: RegStructureFormItem) {
    let dataField = structureFormItem.viewModel.dataField;
    this.viewModel[dataField] = structureFormItem;
    this.onValueUpdated(this);
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }
};
