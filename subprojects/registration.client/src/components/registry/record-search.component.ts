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
import { IAppState, ISearchRecords } from '../../store';
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
  private lookupsSubscription: Subscription;
  @Input() temporary: boolean;
  @Input() parentHeight: string;
  @Input() activated: boolean;
  @Output() onClose = new EventEmitter<any>();
  private title: string;
  private tabSelected: string = 'search';
  private regSearch: regSearchTypes.CSearchFormVM;
  @ViewChild(ChemDrawingTool)
  private chemDrawWeb: ChemDrawingTool;
  public formGroup: CFormGroup;
  @select(s => s.session.lookups) lookups$: Observable<any>;

  constructor(
    private router: Router,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef,
    public ngRedux: NgRedux<IAppState>,
    private actions: RegistrySearchActions) {
  }

  ngOnInit() {
    this.title = this.temporary ? 'Search Temporary Records' : 'Search Permanent Registry';
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.loadData(d); } });
    this.regSearch = new regSearchTypes.CSearchFormVM(this.ngRedux.getState());
  }

  ngOnDestroy() {
  }

  loadData(lookups: any) {
    let formGroupType = FormGroupType.SearchPermanent;
    prepareFormGroupData(formGroupType, this.ngRedux);
  }

  ngOnChanges() {
    if (this.activated && this.chemDrawWeb) {
      this.chemDrawWeb.activate();
    }
  }

  search() {
    this.actions.searchRecords(this.temporary, this.generateSearchCriteriaXML());
  }

  private generateSearchCriteriaXML(): string {
    this.regSearch.registrySearchVM.structureData = this.chemDrawWeb.getValue();
    let searchCriteria = `<?xml version="1.0" encoding="UTF-8"?><searchCriteria xmlns="COE.SearchCriteria">`;
    searchCriteria += this.getSearchCriteria(this.regSearch.registrySearchVM);
    searchCriteria += this.getSearchCriteria(this.regSearch.structureSearchVM);
    searchCriteria += this.getSearchCriteria(this.regSearch.componentSearchVM);
    searchCriteria += this.getSearchCriteria(this.regSearch.batchSearchVM);
    return searchCriteria + '</searchCriteria>';
  }

  private getSearchCriteria(c: { columns: any[] }): string {
    let searchCriteria = '';
    c.columns.forEach(e => {
      let m: { data } = this.regSearch.registrySearchVM;
      if (m.data[e.dataField]) {
        for (const prop in e.searchCriteriaItem) {
          if (typeof e.searchCriteriaItem[prop] === 'object') {
            e.searchCriteriaItem[prop].__text = m.data[e.dataField];
          }
        }
      }
      searchCriteria += new X2JS.default().js2xml({ searchCriteriaItem: e.searchCriteriaItem });
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
