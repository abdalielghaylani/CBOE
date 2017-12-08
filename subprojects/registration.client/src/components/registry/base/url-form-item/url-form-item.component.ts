import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import * as dxDialog from 'devextreme/ui/dialog';
import { RegBaseFormItem } from '../../../common';

@Component({
  selector: 'reg-url-form-item-template',
  template: require('./url-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegUrlFormItem extends RegBaseFormItem {
  protected label: string;

  protected update() {
    super.update();
    let options = this.viewModel.editorOptions;
    this.label = options.label;
    if (!options.value) {
      if (this.editMode && options.defaultValue) {
        options.value = options.defaultValue;
      }
    }
    this.value = options && options.value ? this.deserializeValue(options.value) : undefined;
  }

  protected onClick(e) {
    console.log(this);
    let clientEvents = this.viewModel.editorOptions.config.clientEvents;
    if (clientEvents != null && clientEvents.event != null) {
      let script = clientEvents.event.__text;
      if (script.indexOf('RestoreOriginalStructure') > 0) {
        // `return RestoreOriginalStructure('@BaseFragmentStructure', '@BaseFragmentNormalizedStructure')`
        let dialogResult = dxDialog.confirm(
          `Alert: Are you sure you want to revert to the original structure?`,
          this.label);
        dialogResult.done(r => {
          if (r) {
            let baseStructure = this.viewModel.component.option(`formData.BaseFragmentStructure`);
            baseStructure.Structure.__text = this.viewModel.component.option(`formData.BaseFragmentNormalizedStructure`);
            this.viewModel.component.option(`formData.BaseFragmentStructure`, baseStructure);
            this.viewModel.component.repaint();
          }
        });
      }
    }
    return false;
  }
};
