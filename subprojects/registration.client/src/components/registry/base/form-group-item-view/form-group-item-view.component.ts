import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { CViewGroup } from '../registry-base.types';

@Component({
  selector: 'reg-form-group-item-view',
  template: require('./form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupItemView implements OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() displayMode: string;
  @Input() data: any;
  @Input() viewGroup: CViewGroup;
  private items: any[] = [];
  private formData: any = {};
  private colCount: number = 5;

  constructor() {
  }

  ngOnChanges() {
    this.buildItems();
  }

  private buildItems() {
    if (this.viewGroup) {
      this.items = this.viewGroup.getItems(this.displayMode);
    }
  }
};
