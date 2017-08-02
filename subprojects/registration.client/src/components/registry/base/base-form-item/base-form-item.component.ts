import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate, IValidationRuleList, IParam } from '../registry-base.types';

@Component({
  selector: 'reg-text-form-item-template',
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

  constructor() {
  }

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

  private getParamNumber(params: IParam[], name: string, defaultValue: number): number {
    let filtered = params.filter(p => p._name === name);
    return filtered.length > 0 ? +filtered[0]._value : defaultValue;
  }

  protected validate(e) {
    e.rule.isValid = true;
    let peer = e.validator.peer;
    let ruleList: IValidationRuleList = peer.viewModel.editorOptions.customRules;
    if (ruleList && ruleList.validationRule) {
      ruleList.validationRule.forEach(r => {
        if (r._validationRuleName === 'requiredField' && !e.value) {
          e.rule.isValid = false;
        } else if (r._validationRuleName === 'textLength' && e.value) {
          let min = peer.getParamNumber(r.params.param, 'min', 0);
          let max = peer.getParamNumber(r.params.param, 'max', -1);
          let length = e.value.length;
          if (length < min || (max > 0 && length > max)) {
            e.rule.isValid = false;
          }
        }
        if (!e.rule.isValid) {
          e.rule.message = r._errorMessage;
        }
      });
    }
    return e.rule.isValid;
  }
};
