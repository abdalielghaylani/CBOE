import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegStructureFormItem } from '../structure-form-item';
import { IViewControl } from '../../../common';
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
  @Input() template: boolean;
  @Input() viewModel: any;
  @Input() viewConfig: any[];
  @Input() colCount: number;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  protected viewModelCopy: any;
  private structureDataVisible: boolean = false;
  private structureData: string;

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

  private structureImageClicked(e: string) {
    if (e) {
      this.structureData = e;
      this.structureDataVisible = true;
    }
  }
}
