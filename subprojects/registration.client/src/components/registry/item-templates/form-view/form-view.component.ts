import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'reg-form-view',
  template: require('./form-view.component.html'),
  styles: [require('../item-templates.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormView implements OnChanges {
  @Input() id: string;
  @Input() editMode: boolean = false;
  @Input() formData: any;
  @Input() colCount: number;
  @Input() items: any[];

  ngOnChanges() {
  }
};
