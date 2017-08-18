import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { ChemDrawWeb } from '../common';

@Component({
  selector: 'reg-structure-fragment-template',
  template: `<div [id]="id" class="cd-container cdweb"></div>`,
  styles: [` .cd-container {
          border: 5px solid #f0f0f0;
          height: 300px;
          width: auto;
        }`],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureFragmentFormItem extends ChemDrawWeb {
  @Input() viewModel: any = {};
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();

  constructor(elementRef: ElementRef) {
    super(elementRef);
  }

  protected update() {
    this.setValue(this.viewModel.STRUCTURE_XML);
    super.update();
  }

  protected onContentChanged(e) {
    if (this.cdd && !this.cdd.isSaved()) {
      if (this.viewModel) {
        this.viewModel.STRUCTURE_XML = this.getValue();
      }
      this.valueUpdated.emit(this);
    }
  }

};
