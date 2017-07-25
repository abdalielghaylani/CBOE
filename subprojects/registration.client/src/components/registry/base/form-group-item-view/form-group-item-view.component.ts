import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl, CViewGroup, IRegistryRecord } from '../registry-base.types';

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
  @Input() viewModel: IRegistryRecord;
  @Input() viewConfig: CViewGroup;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();  
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
      this.readVM();
    }
  }

  private readVM() {
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    this.formData = this.viewConfig.getFormData(this.displayMode, validItems.map(i => i.dataField), this.viewModel);
  }

  private writeVM() {

  }

  protected onValueUpdated(e) {
    this.writeVM();
    this.valueUpdated.emit(this);
  }
};
