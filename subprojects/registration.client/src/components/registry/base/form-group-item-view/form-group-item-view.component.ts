import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { CFormGroup, CForm, CCoeForm } from '../../../../common';
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
  @Input() editMode: boolean = false;
  @Input() data: any;
  @Input() viewGroup: CViewGroup;

  constructor() {
  }

  ngOnChanges() {
  }
};
