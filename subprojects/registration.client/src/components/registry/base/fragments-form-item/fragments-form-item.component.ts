import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IBatchComponentFragmentList } from '../registry-base.types';
import { RegDataGridFormItem } from '../data-grid-form-item';
import { NgRedux } from '@angular-redux/store';
import { apiUrlPrefix } from '../../../../configuration';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-fragments-form-item-template',
  template: require('../data-grid-form-item/data-grid-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFragmentsFormItem extends RegDataGridFormItem {
  private fragmentList: IBatchComponentFragmentList;

  constructor(private ngRedux: NgRedux<IAppState>) {
    super();
  }

  deserializeValue(value: IBatchComponentFragmentList): any {
    this.fragmentList = value;
    let fragments = value.BatchComponentFragment;
    return fragments.map(f => {
      let fragmentEntry: any = {
        id: f.ID,
        compoundFragmentID: f.ComponentFragmentID,
        equivalents: f.Equivalents
      };
      // ID, CompoundFragmentID, FragmentID, Equivalents
      let lookups = this.ngRedux.getState().session.lookups;
      if (lookups) {
        let filtered = lookups.fragments.filter(f2 => +f2.FRAGMENTID === +f.FragmentID);
        if (filtered.length > 0) {
          let fragment = filtered[0];
          fragmentEntry.fragmentTypeId = fragment.FRAGMENTTYPEID;
          fragmentEntry.code = fragment.CODE;
          fragmentEntry.description = fragment.DESCRIPTION;
          fragmentEntry.molWeight = fragment.MOLWEIGHT;
          fragmentEntry.formula = fragment.FORMULA;
          fragmentEntry.structure = fragment.STRUCTURE.replace('?', '/50/100?');
        }
      }
      return fragmentEntry;
    });
  }

  serializeValue(value: any): IBatchComponentFragmentList {
    return this.fragmentList;
  }

  protected update() {
    let lookups = this.ngRedux.getState().session.lookups;
    let options = this.viewModel.editorOptions;
    this.dataSource = options && options.value ? this.deserializeValue(options.value) : [];
    this.columns = lookups ? [{
      dataField: 'fragmentTypeId',
      caption: 'Type',
      lookup: {
        dataSource: lookups.fragmentTypes,
        displayExpr: 'DESCRIPTION',
        valueExpr: 'ID'
      },
      width: 60
    }, {
      dataField: 'equivalents',
      caption: 'Equivalent',
      width: 80
    }, {
      dataField: 'code',
      caption: 'Code',
      editCellTemplate: 'dropDownTemplate',
      editorOptions: {
        dataSource: lookups.fragments,
        displayExpr: 'CODE',
        valueExpr: 'CODE',
        dropDownWidth: 600
      },
      width: 50
    }, {
      dataField: 'structure',
      caption: 'Structure',
      allowEditing: false,
      allowFiltering: false,
      allowSorting: false,
      width: 150,
      cellTemplate: function (c, o) {
        jQuery(`<img src="${apiUrlPrefix}StructureImage/${o.data.structure}" class="fragment-image block mx-auto" />`).appendTo(c);
      }
    }, {
      dataField: 'description',
      caption: 'Description',
      allowEditing: false
    }, {
      dataField: 'molWeight',
      caption: 'MW',
      allowEditing: false,
      width: 80
    }, {
      dataField: 'formula',
      caption: 'MF',
      allowEditing: false,
      width: 120
    }] : [];
    this.checkCommandColumn();
    this.editingMode = 'row';
    this.allowUpdating = true;
    this.allowDeleting = true;
    this.allowAdding = true;
  }
};
