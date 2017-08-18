import { Component, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'reg-base-column-item-template',
  template: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBaseColumnItem implements OnChanges {
  @Input() viewModel: any;
  viewConfig: any;
  value: any;

  deserializeValue(value: any): any {
    return value;
  }

  ngOnChanges() {
    this.viewConfig = this.viewModel.column.editorOptions;
    this.update();
  }

  protected update() {
    if (this.viewConfig && this.viewModel) {
      this.value = this.deserializeValue(this.viewModel.value);
    }
  }
};
