import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate, RegStructureBaseFormItem } from '../../../common';
import { StructureQueryOptionsModel } from '../registry-base.types';

@Component({
  selector: 'reg-structure-query-form-item-template',
  template: require('./structure-query-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureQueryFormItem extends RegStructureBaseFormItem {
  private queryModel = new StructureQueryOptionsModel();
  private index: number = 0;
  private searchTypeOptions: string[] = ['Substructure', 'Full Structure', 'Exact', 'Similarity'];
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
  private viewConfigSterioChem = [{
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

  viewOptions(e) {
    this.index = e;
  }

  getSelectedValue(e) {
    this.queryModel.searchTypeValue = e;
  }

};
