import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewChild, ViewEncapsulation } from '@angular/core';
import { RegStructureBaseFormItem } from '../structure-base-form-item';
import { ChemDrawWeb } from '../../../common';

@Component({
  selector: 'reg-structure-form-item-template',
  template: require('./structure-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureFormItem extends RegStructureBaseFormItem {
  protected mode: number = 0;
  private drawStructureImage = require('../assets/draw-structure.png');
  private noStructureImage = require('../assets/no-structure.png');
  private unknownStructureImage = require('../assets/unknown-structure.png');
  private nonChemicalImage = require('../assets/non-chemical-content.png');
  private titleCdxml = `<CDXML><fonttable><font id="3" charset="iso-8859-1" name="Arial"/>
</fonttable><page><t LineHeight="auto"><s font="3" size="18" color="0">{{title}}</s></t></page></CDXML>`;

  constructor(elementRef: ElementRef) {
    super(elementRef);
  }

  protected validate(options) {
    return true;
  }

  private changeMode(e) {
    let mode = +e.element[0].id.slice(-1);
    if (mode !== this.mode) {
      this.mode = mode;
      this.cdd.setViewOnly(this.mode !== 0);
      let title = this.mode === 1 ? 'No Structure'
        : this.mode === 2 ? 'Unknown Structure'
        : this.mode === 3 ? 'Non-Chemical Content'
        : undefined;
      if (title) {
        this.cdd.loadCDXML(this.titleCdxml.replace('{{title}}', title));
      } else {
        this.cdd.clear();
      }
    }
  }
};
