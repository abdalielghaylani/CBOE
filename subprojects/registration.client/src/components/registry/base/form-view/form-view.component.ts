import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl } from '../registry-base.types';
import { RegStructureFormItem } from '../structure-form-item';
import { copyObject } from '../../../../common';

@Component({
  selector: 'reg-form-view',
  template: require('./form-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormView implements IViewControl, OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() viewModel: any;
  @Input() viewConfig: any[];
  @Input() colCount: number;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  protected viewModelCopy: any;

  ngOnChanges() {
    // Keep a copy of view-model to check there is any change
    this.viewModelCopy = copyObject(this.viewModel);
  }

  protected onFieldDataChanged(e) {
    if (this.viewModelCopy[e.dataField] !== e.value) {
      this.viewModelCopy[e.dataField] = e.value;
      this.onValueUpdated(this);
    }
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }
};
