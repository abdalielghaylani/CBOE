import { Component, EventEmitter, Input, ElementRef, ChangeDetectionStrategy, ChangeDetectorRef, ViewChild, ViewEncapsulation } from '@angular/core';
import * as dxDialog from 'devextreme/ui/dialog';
import { IStructureData, RegStructureBaseFormItem, noStructureImage } from '../../../common';
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
  private structureData: IStructureData;
  private drawStructureImage = require('../assets/draw-structure.png');
  private unknownStructureImage = require('../assets/unknown-structure.png');
  private nonChemicalImage = require('../assets/non-chemical-content.png');
  private noStructureImage = noStructureImage;
  private titleCdxml = `<CDXML><fonttable><font id="3" charset="iso-8859-1" name="Arial"/>
</fonttable><page><t LineHeight="auto"><s font="3" size="18" color="0">{{title}}</s></t></page></CDXML>`;

  constructor(private changeDetector: ChangeDetectorRef, elementRef: ElementRef) {
    super(elementRef);
  }

  private extractMode(node: any): DrawingType {
    let modeStr = typeof node === 'object' ? node.__text : node;
    return modeStr ? +modeStr : DrawingType.Chemical;
  }

  deserializeValue(value: any): any {
    let mode = this.extractMode(value.DrawingType);
    this.structureData = undefined;
    if (value.Structure) {
      if ((mode in DrawingType) && (mode !== DrawingType.Chemical)
        && (typeof value.Structure.__text === 'undefined')) {
        let title = value.DrawingType.__text === DrawingType.NoStructure.toString() ? 'No Structure'
          : value.DrawingType.__text === DrawingType.Unknown.toString() ? 'Unknown Structure'
            : value.DrawingType.__text === DrawingType.NonChemicalContent.toString() ? 'Non-Chemical Content' : undefined;
        value.Structure.__text = this.titleCdxml.replace('{{title}}', title);
      }
      this.structureData = value;
      value = this.structureData.Structure.__text;
      if (typeof value === 'object' && value.viewModel) {
        value = value.toString();
      }
    }
    this.updateMode(this.extractMode(this.structureData.DrawingType), false);
    return value;
  }

  serializeValue(value: any): any {
    const data = this.structureData;
    if (data) {
      data.Structure.__text = value;
      data.Structure._update = 'yes';
      data.NormalizedStructure = undefined;
      let drawingTypeNodeType = typeof data.DrawingType;
      if (data.OrgDrawingType == null) {
        data.OrgDrawingType = JSON.parse(JSON.stringify(data.DrawingType));
      }
      if (drawingTypeNodeType === 'string') {
        data.DrawingType = (+this.mode).toString();
      } else if (drawingTypeNodeType === 'object') {
        data.DrawingType.__text = (+this.mode).toString();
      }
      value = data;
    }
    return value;
  }

  protected validate(options) {
    let vm = options.validator.peer.viewModel;
    options.validator.isValid = true;
    options.validator.validationRule.forEach(element => {
      if (element._validationRuleName === 'notEmptyStructure') {
        if (vm.editorOptions && vm.editorOptions.value && vm.editorOptions.value.Structure.__text) {
        } else {
          options.validator.isValid = false;
          options.validator.errorMessage = element._errorMessage;
        }
      }
    });
    return options.validator.isValid;
  }

  private updateMode(mode: DrawingType, forceRefresh: boolean = true) {
    this.mode = mode;
    this.editMode = this.mode === DrawingType.Chemical;
    if (this.cdd) {
      this.cdd.setViewOnly(!this.editMode);
      let title = this.mode === DrawingType.NoStructure ? 'No Structure'
        : this.mode === DrawingType.Unknown ? 'Unknown Structure'
          : this.mode === DrawingType.NonChemicalContent ? 'Non-Chemical Content'
            : undefined;
      this.cdd.clear();
      this.cdd.markAsSaved();
      if (title) {
        this.cdd.loadCDXML(this.titleCdxml.replace('{{title}}', title));
        // Force sending the update evant to the container
        this.viewModel.component.option('formData.' + this.viewModel.dataField, this.serializeValue(this));
        this.valueUpdated.emit(this);
      }
      if (this.mode === 0 && this.cdd.isBlankStructure() &&
        this.viewModel.editorOptions.value.Structure) {
        this.viewModel.editorOptions.value.Structure.__text = undefined;
        this.viewModel.component.option('formData.' + this.viewModel.dataField).Structure.__text = undefined;
        this.valueUpdated.emit(this);
      }
    }
    if (forceRefresh) {
      this.changeDetector.markForCheck();
    }
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
