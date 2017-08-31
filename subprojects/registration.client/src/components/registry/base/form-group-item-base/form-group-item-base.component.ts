import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl } from '../../../common';
import { CViewGroupContainer } from '../registry-base.types';

@Component({
  selector: 'reg-search-form-group-item-base',
  template: ``,
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupItemBase implements IViewControl, OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() displayMode: string;
  @Input() viewModel: any;
  @Input() viewConfig: CViewGroupContainer;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  protected items: any[] = [];
  protected formData: any = {};
  protected colCount: number = 5;

  ngOnChanges() {
    this.update();
  }

  protected update() {
    if (this.viewConfig) {
      this.items = this.viewConfig.getItems(this.displayMode);
      if (this.items.find(i => i.itemType === 'group') != null) {
        this.colCount = 1;
      }
      this.readVM();
    }
  }

  protected readVM() {
    // Should update this.formData here
  }

  protected writeVM() {
    // Should serialize this.formData properly here
  }

  protected onValueUpdated(e) {
    this.writeVM();
    this.valueUpdated.emit(this);
  }

  protected getValidItems(): any[] {
    return RegFormGroupItemBase.getValidItems(this.items);
  }

  public static getValidItems(items: any[]): any[] {
    let validItems = [];
    items.forEach(i => {
      if (i.itemType === 'group') {
        validItems = validItems.concat(i.items.filter(ix => !ix.itemType || ix.itemType !== 'empty'));
      } else if (i.itemType !== 'empty') {
        validItems.push(i);
      }
    });
    return validItems;
  }
};
