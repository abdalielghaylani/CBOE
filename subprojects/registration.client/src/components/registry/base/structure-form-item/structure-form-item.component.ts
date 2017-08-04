import { Component, EventEmitter, Input, ElementRef, ChangeDetectionStrategy, ChangeDetectorRef, ViewChild, ViewEncapsulation } from '@angular/core';
import * as dxDialog from 'devextreme/ui/dialog';
import { RegStructureBaseFormItem } from '../structure-base-form-item';
import { DrawingType } from '../registry-base.types';
import { ChemDrawWeb } from '../../../common';

@Component({
  selector: 'reg-structure-form-item-template',
  template: require('./structure-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureFormItem extends RegStructureBaseFormItem {
  protected mode: DrawingType = DrawingType.Chemical;
  private drawStructureImage = require('../assets/draw-structure.png');
  private noStructureImage = require('../assets/no-structure.png');
  private unknownStructureImage = require('../assets/unknown-structure.png');
  private nonChemicalImage = require('../assets/non-chemical-content.png');
  private titleCdxml = `<CDXML><fonttable><font id="3" charset="iso-8859-1" name="Arial"/>
</fonttable><page><t LineHeight="auto"><s font="3" size="18" color="0">{{title}}</s></t></page></CDXML>`;

  constructor(private changeDetector: ChangeDetectorRef, elementRef: ElementRef) {
    super(elementRef);
  }

  protected validate(options) {
    return true;
  }

  private updateMode(mode: DrawingType) {
    this.mode = mode;
    this.cdd.setViewOnly(this.mode !== DrawingType.Chemical);
    let title = this.mode === DrawingType.NoStructure ? 'No Structure'
      : this.mode === DrawingType.Unknown ? 'Unknown Structure'
      : this.mode === DrawingType.NonChemicalContent ? 'Non-Chemical Content'
      : undefined;
    if (title) {
      this.cdd.loadCDXML(this.titleCdxml.replace('{{title}}', title));
    } else {
      this.cdd.clear();
    }
    this.changeDetector.markForCheck();
  }

  private onClick(e) {
    let mode = +e.element[0].id.slice(-1);
    if (mode !== this.mode) {
      if (mode !== DrawingType.Chemical && this.mode === DrawingType.Chemical && !this.cdd.isBlankStructure()) {
        let dialogResult = dxDialog.confirm(
          `Alert: Are you sure you want to continue?  You will lose the currently drawn structure.`,
          'Confirm Clearing Structure');
        dialogResult.done(r => {
          if (r) {
            this.updateMode(mode);
          }
        });
      } else {
        this.updateMode(mode);
      }
    }
  }
};
