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
    label: { text: 'Hit any charge on heteroatom' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'reactionCenter',
    label: { text: 'Reaction query must hit reaction center' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'hitAnyChargeCarbon',
    label: { text: 'Hit any charge on carbon' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'permitExtraneousFragments',
    label: { text: 'Permit extraneous fragments in Full Structure Searches' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'permitExtraneousFragmentsIfRXN',
    label: { text: 'Permit extraneous fragments in Reaction Full Structure Searches' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'fragmentsOverlap',
    label: { text: 'Query fragments can overlap in target' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'tautometer',
    label: { text: 'Tautomeric' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'fullSearch',
    label: { text: 'Full structure similarity' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'simThreshold',
    label: { text: 'Similarity search (20-100%)' },
    dataType: 'number',
    editorType: 'dxNumberBox',
    editorOptions: { width: '100px' },
    validationRules: [{ type: 'required', message: 'Value between 20 and 100 is required' },
    { type: 'range', min: 20, max: 100, message: 'Value between 20 and 100 is required' }]
  }];
  private viewConfigSterioChem = [{
    dataField: 'matchStereochemistry',
    label: { text: 'Match Stereochemistry' },
    editorType: 'dxCheckBox'
  }, {
    dataField: 'tetrahedralStereo',
    label: { text: 'Tetrahedral stereo center hits' },
    template: 'tetrahedralTemplate',
  }, {
    dataField: 'relativeTetStereo',
    label: { text: 'Thick bonds represent relative stereochemistry' },
    template: 'relativeTetStereoTemplate'
  }, {
    dataField: 'doubleBondStereo',
    label: { text: 'Double bonds hits' },
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
