import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { ChemDrawWeb } from '../common';

@Component({
  selector: 'reg-structure-fragment-template',
  template: `<div [id]="id" class="cd-container cdweb"></div>`,
  styles: [` .cd-container {border: 5px solid #f0f0f0; height: 300px; width: auto;}`],
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
        if (this.cdd.isBlankStructure()) {
          this.viewModel.STRUCTURE_XML = undefined;
          this.viewModel.FORMULA = undefined;
          this.viewModel.MOLWEIGHT = undefined;
          this.valueUpdated.emit(this);
        } else {
          this.viewModel.STRUCTURE_XML = this.getValue();
          this.getProperties(this);
        }
      }
    }
  }

  private getProperties(parent: RegStructureFragmentFormItem) {
    this.cdd.getProperties(function (props, error) {
      if (!error) {
        let properties = JSON.parse(props);

        let formula = properties.FORMULA.replace(new RegExp('<sub>', 'g'), '');
        formula = formula.replace(new RegExp('</sub>', 'g'), '');

        formula = formula.replace(new RegExp('<sup>', 'g'), '');
        formula = formula.replace(new RegExp('</sup>', 'g'), '');

        let mwWeight: string = properties.MW.substring(0, properties.MW.indexOf(' '));

        parent.viewModel.FORMULA = formula;
        parent.viewModel.MOLWEIGHT = (+mwWeight).toFixed(5).replace(/([0-9]+(\.[0-9]+[1-9])?)(\.?0+$)/, '$1');
        parent.valueUpdated.emit(null);
      } else {
        // deal with error
      }
    });
  }
}
