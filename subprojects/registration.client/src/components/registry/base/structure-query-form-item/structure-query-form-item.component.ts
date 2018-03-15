import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate, RegStructureBaseFormItem } from '../../../common';
import { IStructureQueryOptions, CStructureQueryOptions } from '../registry-base.types';

@Component({
  selector: 'reg-structure-query-form-item-template',
  template: require('./structure-query-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureQueryFormItem extends RegStructureBaseFormItem {
  private searchTypeOptions: string[] = ['Substructure', 'Full Structure', 'Exact', 'Similarity'];
  private searchTypeValue: string = this.searchTypeOptions[0];
  private hitAnyChargeHetero: boolean = true;
  private reactionCenter: boolean = true;
  private hitAnyChargeCarbon: boolean = true;
  private permitExtraneousFragments: boolean = false;
  private permitExtraneousFragmentsIfRXN: boolean = false;
  private fragmentsOverlap: boolean = false;
  private tautometer: boolean = false;
  private fullSearch: boolean = true;
  private simThreshold: number = 100;
  private matchStereochemistry: boolean = true;
  private tetrahedralStereo: string = 'Same';
  private doubleBondStereo: string = 'Same';
  private relativeTetStereo: boolean = false;
  private index: number = 0;
  private viewConfigGeneral = [{
    dataField: 'hitAnyChargeHetero',
    label: { text: 'Hit any charge on hetero-atom', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'reactionCenter',
    label: { text: 'Reaction query must hit reaction center', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'hitAnyChargeCarbon',
    label: { text: 'Hit any charge on carbon', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'permitExtraneousFragments',
    label: { text: 'Permit extraneous fragments in full-structure searches', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'permitExtraneousFragmentsIfRXN',
    label: { text: 'Permit extraneous fragments in reaction full-structure searches', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'fragmentsOverlap',
    label: { text: 'Query fragments can overlap in target', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'tautometer',
    label: { text: 'Tautomeric', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'fullSearch',
    label: { text: 'Full-structure similarity', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'simThreshold',
    label: { text: 'Similarity search (20-100%)', alignment: 'right' },
    dataType: 'number',
    editorType: 'dxNumberBox',
    editorOptions: { width: '100px' },
    validationRules: [{ type: 'required', message: 'Value between 20 and 100 is required' },
    { type: 'range', min: 20, max: 100, message: 'Value between 20 and 100 is required' }]
  }];
  private viewConfigStereoChem = [{
    dataField: 'matchStereochemistry',
    label: { text: 'Match stereochemistry', alignment: 'right' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'tetrahedralStereo',
    label: { text: 'Tetrahedral stereo center hits', alignment: 'right' },
    template: 'tetrahedralTemplate',
  }, {
    dataField: 'relativeTetStereo',
    label: { text: 'Thick bonds represent relative stereochemistry', alignment: 'right' },
    template: 'relativeTetStereoTemplate'
  }, {
    dataField: 'doubleBondStereo',
    label: { text: 'Double bonds hits', alignment: 'right' },
    template: 'doubleBondTemplate'
  }];

  constructor(elementRef: ElementRef) {
    super(elementRef);
  }

  deserializeValue(value: any): any {
    this.searchTypeValue =
      value._fullSearch === 'YES' ? this.searchTypeOptions[1] :
        value._identity === 'YES' ? this.searchTypeOptions[2] :
          value._similar === 'YES' ? this.searchTypeOptions[3] :
            this.searchTypeOptions[0];
    this.hitAnyChargeHetero = value._hitAnyChargeHetero === 'YES';
    this.reactionCenter = value._reactionCenter === 'YES';
    this.hitAnyChargeCarbon = value._hitAnyChargeCarbon === 'YES';
    this.permitExtraneousFragments = value._permitExtraneousFragments === 'YES';
    this.permitExtraneousFragmentsIfRXN = value._permitExtraneousFragmentsIfRXN === 'YES';
    this.fragmentsOverlap = value._fragmentsOverlap === 'YES';
    this.tautometer = value._tautometer === 'YES';
    this.simThreshold = +value._simThreshold;
    this.matchStereochemistry = (value._tetrahedralStereo !== 'NO' && value._tetrahedralStereo !== 'ANY')
      || value._relativeTetStereo === 'YES' || value._doubleBondStereo === 'YES';
    this.tetrahedralStereo = !this.matchStereochemistry ? 'SAME' :
      value._tetrahedralStereo === 'NO' ? 'ANY' : value._tetrahedralStereo === 'YES' ? 'SAME' : value._tetrahedralStereo;
    this.relativeTetStereo = value._relativeTetStereo === 'YES';
    this.doubleBondStereo = this.matchStereochemistry && value._doubleBondStereo === 'NO' ? 'Any' : 'Same';
    const structureValue = value.__text;
    if (typeof structureValue === 'object' && structureValue.viewModel) {
      value.__text = structureValue.toString();
    }
    return value.__text;
  }

  serializeValue(value: any): any {
    let serialized = this.viewModel.component.option('formData.' + this.viewModel.dataField);
    if (!serialized) {
      serialized = new CStructureQueryOptions();
    }
    let criteria = serialized.CSCartridgeStructureCriteria ? serialized.CSCartridgeStructureCriteria : serialized;
    criteria._hitAnyChargeHetero = this.hitAnyChargeHetero ? 'YES' : 'NO';
    criteria._reactionCenter = this.reactionCenter ? 'YES' : 'NO';
    criteria._hitAnyChargeCarbon = this.hitAnyChargeCarbon ? 'YES' : 'NO';
    criteria._permitExtraneousFragments = this.permitExtraneousFragments ? 'YES' : 'NO';
    criteria._permitExtraneousFragmentsIfRXN = this.permitExtraneousFragmentsIfRXN ? 'YES' : 'NO';
    criteria._fragmentsOverlap = this.fragmentsOverlap ? 'YES' : 'NO';
    criteria._tautometer = this.tautometer ? 'YES' : 'NO';
    criteria._simThreshold = this.simThreshold.toString();
    criteria._tetrahedralStereo = this.matchStereochemistry ? this.tetrahedralStereo : 'NO';
    criteria._relativeTetStereo = this.matchStereochemistry && this.relativeTetStereo ? 'YES' : 'NO';
    criteria._doubleBondStereo = this.matchStereochemistry && this.doubleBondStereo !== 'Any' ? 'YES' : 'NO';
    criteria._fullSearch = this.searchTypeValue === this.searchTypeOptions[1] ? 'YES' : 'NO';
    criteria._identity = this.searchTypeValue === this.searchTypeOptions[2] ? 'YES' : 'NO';
    criteria._similar = this.searchTypeValue === this.searchTypeOptions[3] ? 'YES' : 'NO';
    criteria.__text = this.cdd == null || this.cdd.isBlankStructure() ? undefined : this;
    return serialized;
  }

  protected onContentChanged(e) {
    this.viewModel.component.option('formData.' + this.viewModel.dataField, this.serializeValue(this));
    this.valueUpdated.emit(this);
  }

  viewOptions(e) {
    this.index = e;
  }

  updateSearchType(e) {
    this.searchTypeValue = e;
    this.viewModel.component.option('formData.' + this.viewModel.dataField, this.serializeValue(this));
    this.valueUpdated.emit(this);
  }

  onOptionUpdated(e) {
    this.viewModel.component.option('formData.' + this.viewModel.dataField, this.serializeValue(this));
    this.valueUpdated.emit(this);
  }
}
