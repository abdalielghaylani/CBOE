import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';
import { RegTagBoxFormItem } from '../tag-box-form-item';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { ChemDrawWeb } from '../../../common';

@Component({
  selector: 'reg-structure-form-query-item-template',
  template: require('./structure-form-query-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureQueryFormItem extends ChemDrawWeb implements IFormItemTemplate {
  @Input() viewModel: any = {};
  @Input() editMode: boolean;
  @Input() viewConfig;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();

  constructor(private ngRedux: NgRedux<IAppState>, elementRef: ElementRef) {
    super(elementRef);
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    this.setValue(options && options.value ? options.value : null);
    super.update();
  }

  protected onContentChanged(e) {
    if (this.cdd && !this.cdd.isSaved()) {
      this.valueUpdated.emit(this);
    }
  }
};
