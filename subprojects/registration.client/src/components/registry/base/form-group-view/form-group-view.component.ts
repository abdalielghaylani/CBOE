import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy } from '@angular/core';
import { CFormGroup, CForm, CCoeForm } from '../../../../common';

@Component({
  selector: 'reg-form-group-view',
  template: require('./form-group-view.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupView implements OnChanges {
  @Input() id: string;
  @Input() editMode: boolean = false;
  @Input() formData: any;
  @Input() formGroup: CFormGroup;
  private groups: CCoeForm[][] = [];

  constructor() {
    if (this.formGroup && this.formGroup.detailsForms && this.formGroup.detailsForms.detailsForm.length > 0) {
      let coeForms = this.formGroup.detailsForms.detailsForm[0].coeForms.coeForm;
      coeForms.forEach(f => {
        if (f.title) {
          this.groups.push([ f ]);
        } else if (this.groups.length > 0) {
          this.groups[this.groups.length - 1].push(f);
        }
      });
    }
  }

  ngOnChanges() {
  }

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }
};
