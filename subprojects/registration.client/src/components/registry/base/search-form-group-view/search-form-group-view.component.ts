import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IViewControl, CViewGroup, ITabularData } from '../registry-search-base.types';
import { IFormGroup, IForm, ICoeForm } from '../../../../common';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-search-form-group-view',
  template: require('./search-form-group-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchFormGroupView implements IViewControl, OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() displayMode: string = 'add';
  @Input() viewModel: any; // IRegistryRecord;
  @Input() viewConfig: IFormGroup;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  private viewGroups: CViewGroup[] = [];

  constructor(private ngRedux: NgRedux<IAppState>) {
  }

  ngOnChanges() {
    this.update();
  }

  protected update() {
    let viewGroups: CViewGroup[] = [];
    let lookups = this.ngRedux.getState().session.lookups;
    let config = this.viewConfig;
    if (lookups && lookups.disabledControls) {
      if (config && config.queryForms && config.queryForms.queryForm.length > 0) {
        let disabledControls = []; 
        let coeForms = this.sortForms(config.queryForms.queryForm[0].coeForms.coeForm);
        coeForms.forEach(f => {
          if (f.formDisplay.visible === 'true') {
            if (viewGroups.length === 0) {
              viewGroups.push(new CViewGroup([], disabledControls));
            }
            let viewGroup = viewGroups[viewGroups.length - 1];
            if (!viewGroup.append(f)) {
              viewGroups.push(new CViewGroup([f], disabledControls));
            }
          }
          this.viewGroups = [];
          viewGroups.forEach(vg => {
            this.viewGroups.push(vg);
          });
        });
      }
    }
  }

  private sortForms(forms: ICoeForm[]): ICoeForm[] {
    // The form aray sometimes is not sorted property.
    // Registry should go first.
    // For now, doc-manager and inventory integration forms are removed.
    let sorted: ICoeForm[] = [];
    forms.forEach(f => {
      let dataSource = f._dataSourceId ? f._dataSourceId.toLowerCase() : '';
      if (dataSource.startsWith('mixture')) {
        sorted.push(f);
      }
    });
    forms.forEach(f => {
      let dataSource = f._dataSourceId ? f._dataSourceId.toLowerCase() : '';
      if (!dataSource.startsWith('mixture') && !dataSource.startsWith('docmgr') && !dataSource.startsWith('inv')) {
        sorted.push(f);
      }
    });
    return sorted;
  }

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }
};
