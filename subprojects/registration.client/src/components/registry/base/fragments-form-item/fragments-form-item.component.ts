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
          fragmentEntry.fragmentId = fragment.FRAGMENTID.toString();
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
    let orderIndex = 0;
    this.fragmentList = { 
      BatchComponentFragment: this.grid.dataSource.map(r => {
        ++orderIndex;
        let fragment: any = {
          Equivalents: r.equivalents,
          FragmentID: r.fragmentId,
          ID: r.id,
          ComponentFragmentID: r.compoundFragmentID,
          OrderIndex: orderIndex.toString()
        };
        if (r.id !== undefined) {
          fragment._insert = 'yes';
        }
        return fragment;
      })
    };
    return this.fragmentList;
  }

  protected update() {
    let lookups = this.ngRedux.getState().session.lookups;
    let options = this.viewModel.editorOptions;
    this.dataSource = options && options.value ? this.deserializeValue(options.value) : [];
    this.columns = lookups ? [{
      dataField: 'code',
      caption: 'Code',
      editCellTemplate: 'dropDownTemplate',
      editorOptions: {
        dataSource: lookups.fragments,
        displayExpr: 'CODE',
        valueExpr: 'CODE',
        dropDownWidth: 600,
        columns: [{
          dataField: 'FRAGMENTID',
          caption: 'ID',
          width: 60,
          visible: false
        }, {
          dataField: 'CODE',
          caption: 'Code'
        }, {
          dataField: 'FRAGMENTTYPEID',
          caption: 'Type',
          width: 60,
          lookup: {
            dataSource: lookups.fragmentTypes,
            displayExpr: 'DESCRIPTION',
            valueExpr: 'ID'
          }
        }, {
          dataField: 'STRUCTURE',
          caption: 'Structure',
          allowFiltering: false,
          allowSorting: false,
          width: 100,
          cellTemplate: 'structureTemplate',
          editorOptions: {
            smallImage: true
          }
        }, {
          dataField: 'DESCRIPTION',
          caption: 'Description'
        }, {
          dataField: 'MOLWEIGHT',
          caption: 'MW'
        }, {
          dataField: 'FORMULA',
          caption: 'MF'
        }]
      },
      width: 60,
      validationRules: [
        { type: 'required', message: 'A valid code is required.' }
      ]
    }, {
      dataField: 'equivalents',
      caption: 'Equivalent',
      width: 80,
      validationRules: [
        { type: 'required', message: 'A valid equivalent value is required.' },
        { type: 'numeric', message: 'The equivalent value must be numeric.'},
        { type: 'range', min: 0 + Number.EPSILON, message: 'The equivalent value must be greater than 0.' }
      ]
    }, {
      dataField: 'fragmentTypeId',
      caption: 'Type',
      allowEditing: false,
      lookup: {
        dataSource: lookups.fragmentTypes,
        displayExpr: 'DESCRIPTION',
        valueExpr: 'ID'
      },
      width: 60
    }, {
      dataField: 'structure',
      caption: 'Structure',
      allowEditing: false,
      allowFiltering: false,
      allowSorting: false,
      width: 150,
      cellTemplate: function (c, o) {
        jQuery(`<img src="${apiUrlPrefix}StructureImage/${o.data.structure}" class="structure-column block mx-auto" />`).appendTo(c);
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
    }, {
      dataField: 'id',
      dataType: 'string',
      visible: false
    }, {
      dataField: 'fragmentId',
      dataType: 'string',
      visible: false
    }] : [];
    this.checkCommandColumn();
    this.editingMode = 'row';
    this.allowUpdating = true;
    this.allowDeleting = true;
    this.allowAdding = true;
  }

  protected onDropDownValueUpdated(e, d) {
    this.grid.instance.cellValue(d.rowIndex, d.column.dataField, e);
    let lookups = this.ngRedux.getState().session.lookups;
    if (lookups) {
      let filtered = lookups.fragments.filter(f => f.CODE === e);
      if (filtered.length > 0) {
        let fragment = filtered[0];
        this.grid.instance.cellValue(d.rowIndex, 'fragmentTypeId', fragment.FRAGMENTTYPEID);
        this.grid.instance.cellValue(d.rowIndex, 'fragmentId', fragment.FRAGMENTID);
        this.grid.instance.cellValue(d.rowIndex, 'description', fragment.DESCRIPTION);
        this.grid.instance.cellValue(d.rowIndex, 'molWeight', fragment.MOLWEIGHT);
        this.grid.instance.cellValue(d.rowIndex, 'formula', fragment.FORMULA);
        this.grid.instance.cellValue(d.rowIndex, 'structure', fragment.STRUCTURE.replace('?', '/50/100?'));
      }
    }
  }
};
