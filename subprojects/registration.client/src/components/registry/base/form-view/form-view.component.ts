import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl } from '../registry-base.types';

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

  ngOnChanges() {
  }
};
