import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl } from '../registry-base.types';
import { RegStructureQueryFormItem } from '../structure-query-form-item';
import { copyObject } from '../../../../common';

@Component({
  selector: 'reg-search-form-view',
  template: require('./search-form-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchFormView implements IViewControl, OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() viewModel: any;
  @Input() viewConfig: any[];
  @Input() colCount: number;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();  
  private viewModelCopy: any;
  private structureFormItem: RegStructureQueryFormItem;

  ngOnChanges() {
    this.viewModelCopy = copyObject(this.viewModel);
  }

  protected onFieldDataChanged(e) {
    if (this.viewModelCopy[e.dataField] !== e.value) {
      this.viewModelCopy[e.dataField] = e.value;
      this.onValueUpdated(this);
    }
  }

  protected onStructureValueUpdated(structureFormItem: RegStructureQueryFormItem) {
    let dataField = structureFormItem.viewModel.dataField;
    this.viewModel[dataField] = structureFormItem;
    this.onValueUpdated(this);
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }
};
