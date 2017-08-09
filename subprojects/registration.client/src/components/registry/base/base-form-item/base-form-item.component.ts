import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate, CValidator } from '../registry-base.types';

@Component({
  selector: 'reg-base-form-item-template',
  template: ``,
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBaseFormItem implements IFormItemTemplate, OnChanges {
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() viewModel: any = {};
  @Input() viewConfig: any;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  protected readOnly: boolean;
  protected value: any;

  ngOnChanges() {
    this.update();
  }

  deserializeValue(value: any): any {
    return value;
  }

  serializeValue(value: any): any {
    return value;
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    this.value = options && options.value ? this.deserializeValue(options.value) : undefined;
    this.readOnly = !this.editMode;
    if (options.readOnly) {
      this.readOnly = this.readOnly || options.readOnly;
    }
  }

  protected onValueChanged(e, d) {
    if (e.previousValue !== e.value) {
      let value = this.serializeValue(e.value);
      d.component.option('formData.' + d.dataField, value);
      this.onValueUpdated(this);
    }
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }

  protected onValidatorInitialized(e, d) {
    e.component.peer = d;
  }

  protected validate(e): boolean {
    return CValidator.validate(e);
  }
};
