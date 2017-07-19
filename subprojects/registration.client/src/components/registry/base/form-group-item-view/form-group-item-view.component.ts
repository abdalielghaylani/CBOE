import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl, CViewGroup } from '../registry-base.types';

@Component({
  selector: 'reg-form-group-item-view',
  template: require('./form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupItemView implements IViewControl, OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() displayMode: string;
  @Input() viewModel: any;
  @Input() viewConfig: CViewGroup;
  private items: any[] = [];
  private formData: any = {};
  private colCount: number = 5;

  constructor() {
  }

  ngOnChanges() {
    this.update();
  }

  private update() {
    if (this.viewConfig) {
      this.items = this.viewConfig.getItems(this.displayMode);
    }
  }
};
