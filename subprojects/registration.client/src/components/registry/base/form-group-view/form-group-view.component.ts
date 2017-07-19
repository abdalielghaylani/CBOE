import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormGroup, IForm, ICoeForm } from '../../../../common';
import { CViewGroup } from '../registry-base.types';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-form-group-view',
  template: require('./form-group-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupView implements OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() displayMode: string = 'add';
  @Input() data: any;
  @Input() formGroupData: IFormGroup;
  private viewGroups: CViewGroup[] = [];

  constructor(private ngRedux: NgRedux<IAppState>) {
  }

  ngOnChanges() {
    let viewGroups: CViewGroup[] = [];
    let lookups = this.ngRedux.getState().session.lookups;
    if (lookups && lookups.disabledControls) {
      if (this.formGroupData && this.formGroupData.detailsForms && this.formGroupData.detailsForms.detailsForm.length > 0) {
        let pageId: string = this.displayMode === 'add' ? 'SUBMITMIXTURE' : this.displayMode === 'view' ? 'VIEWMIXTURE' : 'REVIEWREGISTERMIXTURE';
        let disabledControls = lookups.disabledControls.filter(dc => dc.pageId === pageId);
        let coeForms = this.formGroupData.detailsForms.detailsForm[0].coeForms.coeForm;
        coeForms.forEach(f => {
          if (f.formDisplay.visible === 'true') {
            if (viewGroups.length === 0) {
              viewGroups.push(new CViewGroup([], disabledControls));
            }
            let viewGroup = viewGroups[viewGroups.length - 1];
            if (!viewGroup.append(f)) {
              viewGroups.push(new CViewGroup([ f ], disabledControls));
            }
          }
          this.viewGroups = [];
          viewGroups.forEach(vg => {
            if (vg.getItems(this.displayMode).length > 0) {
              this.viewGroups.push(vg);
            }
          });
        });
      }
    }
  }

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }
};
