import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  OnInit, OnDestroy, OnChanges,
  ElementRef, ChangeDetectorRef,
  ViewChildren, QueryList, ViewChild
} from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { select, NgRedux } from '@angular-redux/store';
import { DxFormComponent } from 'devextreme-angular';
import * as regSearchTypes from './registry-search.types';
import { RegistrySearchActions, ConfigurationActions } from '../../actions';
import { IAppState, ISearchRecords, INITIAL_STATE } from '../../store';
import { Router, ActivatedRoute } from '@angular/router';
import { ChemDrawingTool } from '../../common/tool';
import { FormGroupType, CFormGroup, prepareFormGroupData, notify } from '../../common';
import * as X2JS from 'x2js';

declare var jQuery: any;

@Component({
  selector: 'reg-record-search',
  styles: [require('./records.css')],
  template: require('./record-search.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordSearch implements OnInit, OnDestroy, OnChanges {
  @Input() temporary: boolean;
  @Input() parentHeight: string;
  @Input() activated: boolean;
  @Output() onClose = new EventEmitter<any>();
  @select(s => s.session.lookups) lookups$: Observable<any>;
  @ViewChild(ChemDrawingTool)
  private chemDrawWeb: ChemDrawingTool;
  private lookupsSubscription: Subscription;
  private title: string;
  private regSearch: regSearchTypes.CSearchFormVM;
  private formGroup: CFormGroup;

  constructor(
    private router: Router,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef,
    private ngRedux: NgRedux<IAppState>,
    private actions: RegistrySearchActions) {
  }

  ngOnInit() {
    this.title = this.temporary ? 'Search Temporary Records' : 'Search Permanent Registry';
    let lookups = this.ngRedux.getState().session.lookups;
    if (lookups) {
      this.loadData(lookups);
    } else {
      this.regSearch = new regSearchTypes.CSearchFormVM(INITIAL_STATE);
      this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.loadData(d); } });
    }
  }

  ngOnDestroy() {
  }

  loadData(lookups: any) {
    let formGroupType = FormGroupType.SearchPermanent;
    prepareFormGroupData(formGroupType, this.ngRedux);
    this.regSearch = new regSearchTypes.CSearchFormVM(this.ngRedux.getState());
  }

  ngOnChanges() {
    if (this.activated && this.chemDrawWeb) {
      this.chemDrawWeb.activate();
    }
  }

  search() {
    this.actions.searchRecords({ temporary: this.temporary, searchCriteria: this.generateSearchCriteriaXML() });
  }

  private generateSearchCriteriaXML(): string {
    let searchCriteria = `<?xml version="1.0" encoding="UTF-8"?><searchCriteria xmlns="COE.SearchCriteria">`;
    searchCriteria += this.getSearchCriteria(this.regSearch.registrySearchVM);
    searchCriteria += this.getSearchCriteria(this.regSearch.structureSearchVM);
    searchCriteria += this.getSearchCriteria(this.regSearch.componentSearchVM);
    searchCriteria += this.getSearchCriteria(this.regSearch.batchSearchVM);
    return searchCriteria + '</searchCriteria>';
  }

  private getSearchCriteria(tableData: { columns: any[], data: any }): string {
    let searchCriteria = '';
    tableData.columns.forEach(column => {
      let value = column.coeType === 'COEStructureQuery' ? this.chemDrawWeb.getValue() : tableData.data[column.dataField];
      if (value) {
        for (const prop in column.searchCriteriaItem) {
          if (typeof column.searchCriteriaItem[prop] === 'object') {
            column.searchCriteriaItem[prop].__text = value;
          }
        }
        searchCriteria += new X2JS.default().js2xml({ searchCriteriaItem: column.searchCriteriaItem });
      }
    });
    return searchCriteria;
  }

  clear() {
    this.chemDrawWeb.loadCdxml(null);
  }

  retrieveAll() {
    this.router.navigate([`records/${this.temporary ? 'temp' : ''}`]);
  }

  savePreference(e) {
  }

  restorePreference(e) {
  }

  cancel(e) {
    this.onClose.emit(e);
  }

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }
};
