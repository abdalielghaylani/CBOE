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
import * as searchTypes from './registry-search.types';
import { RegistrySearchActions, IAppState, ISearchRecords, IQueryData } from '../../redux';
import { Router, ActivatedRoute } from '@angular/router';
import { ChemDrawWeb } from '../common';
import { FormGroupType, prepareFormGroupData, IFormGroup, notify } from '../../common';
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
  @ViewChild(ChemDrawWeb) private chemDrawWeb: ChemDrawWeb;
  private lookupsSubscription: Subscription;
  public formGroup: IFormGroup;
  private title: string;
  private regSearch: searchTypes.CSearchFormVM;
  private regSearchViewModel: any = {};
  private cddActivated: boolean;

  constructor(
    private router: Router,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef,
    private ngRedux: NgRedux<IAppState>,
    private actions: RegistrySearchActions) {
  }

  ngOnInit() {
    this.title = this.temporary ? 'Search Temporary Records' : 'Search Permanent Registry';
    this.regSearch = new searchTypes.CSearchFormVM(this.temporary, this.ngRedux.getState());
    let lookups = this.ngRedux.getState().session.lookups;
    if (lookups) {
      this.loadData(lookups);
    } else {
      this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.loadData(d); } });
    }
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
  }

  loadData(lookups: any) {
    let formGroupType = this.temporary ? FormGroupType.SearchTemporary : FormGroupType.SearchPermanent;
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
  }

  ngOnChanges() {
    this.update();
  }

  protected update() {
    // Don't keep changing cdd configuration
    this.cddActivated = this.cddActivated || this.activated;
  }

  private getSearchCriteria(tabularData: searchTypes.ITabularData): string {
    let searchCriteria = '';
    tabularData.columns.forEach(column => {
      let value = column.coeType === 'COEStructureQuery' ? this.chemDrawWeb.getValue() : tabularData.data[column.dataField];
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

  private generateSearchCriteriaXML(): string {
    let searchCriteria = `<?xml version="1.0" encoding="UTF-8"?><searchCriteria xmlns="COE.SearchCriteria">`;
    for (let key in this.regSearchViewModel) {
      if (this.regSearchViewModel[key]) {
        searchCriteria += new X2JS.default().js2xml({ searchCriteriaItem: this.regSearchViewModel[key] });
      }
    }
    return searchCriteria + '</searchCriteria>';
  }

  private search() {
    let queryData: IQueryData = {
      temporary: this.temporary,
      searchCriteria: this.generateSearchCriteriaXML()
    };
    this.actions.searchRecords(queryData);
  }

  private setSearchCriteriaFor(tabularData: searchTypes.ITabularData, item: searchTypes.ISearchCriteriaItem) {
    let self = this;
    tabularData.columns.forEach(column => {
      let columnSearchCriteriaItem = column.searchCriteriaItem as searchTypes.ISearchCriteriaItem;
      if (columnSearchCriteriaItem._fieldid === item._fieldid && columnSearchCriteriaItem._tableid === item._tableid) {
        let searchCriteria = searchTypes.getSearchCriteria(item);
        if (searchCriteria) {
          if (column.coeType === 'COEStructureQuery') {
            let structureCriteria: any = searchCriteria;
            if (structureCriteria.CSCartridgeStructureCriteria && structureCriteria.CSCartridgeStructureCriteria.__text) {
              setTimeout(() => {
                self.chemDrawWeb.setValue(structureCriteria.CSCartridgeStructureCriteria.__text);
              }, 100);
            }
          } else if (searchCriteria.__text) {
            tabularData.data[column.dataField] = column.dataType === 'number' ? +searchCriteria.__text : searchCriteria.__text;
          }
        }
      }
    });
  }

  private setSearchCriteria(item: searchTypes.ISearchCriteriaItem) {
    this.setSearchCriteriaFor(this.regSearch.registrySearchVM, item);
    this.setSearchCriteriaFor(this.regSearch.structureSearchVM, item);
    this.setSearchCriteriaFor(this.regSearch.componentSearchVM, item);
    this.setSearchCriteriaFor(this.regSearch.batchSearchVM, item);
  }

  private fillSearchCriteriaFromXML(queryXml: string) {
    let x2jsTool = new X2JS.default({
      arrayAccessFormPaths: [
        'searchCriteria.searchCriteriaItem',
      ]
    });
    let query: any = x2jsTool.xml2js(queryXml);
    query.searchCriteria.searchCriteriaItem.forEach(i => {
      this.setSearchCriteria(i);
    });
  }

  public restore(queryData: IQueryData) {
    this.fillSearchCriteriaFromXML(queryData.searchCriteria);
    this.changeDetector.markForCheck();
  }

  public clear() {
    this.chemDrawWeb.setValue(null);
  }

  private retrieveAll() {
    this.router.navigate([`records/${this.temporary ? 'temp' : ''}`]);
  }

  private savePreference(e) {
  }

  private restorePreference(e) {
  }

  private cancel(e) {
    this.onClose.emit(e);
  }

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }

  private onValueUpdated(e) {
    // console.log(`form value changed`);
  }

};
