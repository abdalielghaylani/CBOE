import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate, RegStructureBaseFormItem } from '../../../common';
import { StructureQueryOptionsModel, IStructureQueryOptions } from '../registry-base.types';

@Component({
  selector: 'reg-structure-query-form-item-template',
  template: require('./structure-query-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureQueryFormItem extends RegStructureBaseFormItem {
  private queryModel = new StructureQueryOptionsModel();
  private structureCriteriaOptions: IStructureQueryOptions;
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
    this.onValueUpdated(this);
  }

  onValueUpdated(e) {
    this.setStructureSearchOptions();
    if (this.viewModel) {
      this.viewModel.component.option('formData.' + this.viewModel.dataField, this.serializeValue(this));
    }
    this.valueUpdated.emit(this);
  }

  public setStructureSearchOptions() {
    // Set structure search Attributes
    if (this.queryModel) {
      this.structureCriteriaOptions = {
        _hitAnyChargeHetero: this.queryModel.hitAnyChargeCarbon ? 'YES' : 'NO',
        _reactionCenter: this.queryModel.reactionCenter ? 'YES' : 'NO',
        _hitAnyChargeCarbon: this.queryModel.hitAnyChargeCarbon ? 'YES' : 'NO',
        _permitExtraneousFragments: this.queryModel.permitExtraneousFragments ? 'YES' : 'NO',
        _permitExtraneousFragmentsIfRXN: this.queryModel.permitExtraneousFragmentsIfRXN ? 'YES' : 'NO',
        _fragmentsOverlap: this.queryModel.fragmentsOverlap ? 'YES' : 'NO',
        _tautometer: this.queryModel.tautometer ? 'YES' : 'NO',
        _simThreshold: this.queryModel.simThreshold.toString(),
        _fullSearch: this.queryModel.searchTypeValue === 'Full Structure' ? 'YES' : 'NO',
        _identity: this.queryModel.searchTypeValue === 'Exact' ? 'YES' : 'NO',
        _similar: this.queryModel.searchTypeValue === 'Similarity' ? 'YES' : 'NO',
        _tetrahedralStereo: this.queryModel.matchStereochemistry ? this.queryModel.tetrahedralStereo : '',
        _relativeTetStereo: this.queryModel.matchStereochemistry ? (this.queryModel.relativeTetStereo ? 'YES' : 'NO') : 'NO',
        _doubleBondStereo: this.queryModel.matchStereochemistry ? (this.queryModel.doubleBondStereo === 'Any' ? 'NO' : 'YES') : 'NO'
      };
    }
  }

}
