import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { CViewGroup, IViewControl } from '../registry-base.types';

@Component({
  selector: 'reg-search-form-group-item-view',
  template: require('./search-form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchFormGroupItemView implements IViewControl, OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() displayMode: string;
  @Input() viewModel: any; // IRegistryRecord;
  @Input() viewConfig: CViewGroup;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  private items: any[] = [];
  private formData: any = {};
  private colCount: number = 5;

  constructor(private ngRedux: NgRedux<IAppState>) {
  }

  ngOnChanges() {
    this.update();
  }

  private update() {
    if (this.viewConfig) {
      this.items = this.viewConfig.getItems('query');
      this.readVM();
    }
  }

  private readVM() {
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    this.formData = this.viewConfig.getFormData(this.displayMode, validItems.map(i => i.dataField), this.viewModel);
  }

  private writeVM() {
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    this.viewConfig.readFormData(this.displayMode, validItems.map(i => i.dataField), this.viewModel, this.formData);
  }

  protected onValueUpdated(e) {
    this.writeVM();
    this.valueUpdated.emit(this);
  }
};
