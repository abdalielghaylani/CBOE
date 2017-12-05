import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IBatchComponentFragmentList, RegDataGridFormItem, dataGridFormItemTemplate } from '../../../common';
import { apiUrlPrefix } from '../../../../configuration';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-fragments-form-item-template',
  template: dataGridFormItemTemplate,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFragmentsFormItem extends RegDataGridFormItem {
  private fragmentList: IBatchComponentFragmentList;
  private removedItem: any = [];

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
        let fragment = lookups.fragments.find(f2 => +f2.FRAGMENTID === +f.FragmentID);
        if (fragment != null) {
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

  // Fragment item should be removed from lookup once it is added in list
  modifyLookups(value: any) {
    let component = this.grid.instance.columnOption(1, 'editorOptions');
    this.grid.instance.columnOption(1, 'editorOptions', []);
    value.forEach(element => {
      let index = component.dataSource.findIndex(i => i.CODE === element.code);
      if (index >= 0) {
        this.removedItem.push(component.dataSource.find(i => i.CODE === element.code));
        component.dataSource.splice(index, 1);
      }
    });
    let notdeleted = [];
    this.removedItem.forEach(item => {
      let isDeleted = value.findIndex(m => m.code === item.CODE);
      if (isDeleted < 0) {
        component.dataSource.push(item);
      } else {
        notdeleted.push(item);
      }
    });
    this.removedItem = notdeleted;
    component.dataSource = this.sortList(component.dataSource);
    this.grid.instance.columnOption(1, 'editorOptions', component);

  }

  sortList(c) {
    return c.sort(function (a, b) { return (a.FRAGMENTID > b.FRAGMENTID) ? 1 : ((b.FRAGMENTID > a.FRAGMENTID) ? -1 : 0); });
  }

  serializeValue(value: any): IBatchComponentFragmentList {
    this.modifyLookups(value);
    let orderIndex = 0;
    this.fragmentList = {
      BatchComponentFragment: (this.grid.dataSource as any[]).map(r => {
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
        dataSource: this.sortList(lookups.fragments),
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
        { type: 'numeric', message: 'The equivalent value must be numeric.' },
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
      let fragment = lookups.fragments.find(f => f.CODE === e);
      if (fragment != null) {
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
