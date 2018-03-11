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
import { CViewGroup, CSearchCriteria, ISearchCriteriaItem } from './base';
import { RegistrySearchActions, IAppState, ISearchRecords, IQueryData, ILookupData } from '../../redux';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroupType, prepareFormGroupData, IFormGroup, notify, notifyException } from '../../common';
import { HttpService } from '../../services';
import { apiUrlPrefix } from '../../configuration';
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
  @Input() hitListId: number;
  @Input() parentHeight: string;
  @Input() activated: boolean;
  @Input() totalSearchableCount: number;
  @Input() isRefine: boolean;
  @Output() onClose = new EventEmitter<any>();
  @Output() onSearch = new EventEmitter<any>();
  @Output() onRefine = new EventEmitter<any>();
  @select(s => s.session.lookups) lookups$: Observable<any>;
  public formGroup: IFormGroup;
  private lookups: ILookupData;
  private lookupsSubscription: Subscription;
  private searchCriteria: CSearchCriteria = new CSearchCriteria();
  private viewGroups: CViewGroup[] = [];
  private title: string;
  private displayMode: string = 'query';
  private cddActivated: boolean = false;
  private optionData = { isHighLightSubstructure: undefined };

  constructor(
    private router: Router,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef,
    private ngRedux: NgRedux<IAppState>,
    private actions: RegistrySearchActions,
    private http: HttpService) {
  }

  ngOnInit() {
    this.title = this.temporary ? 'Search Temporary Records' : 'Search Permanent Registry';
    // this.regSearch = new searchTypes.CSearchFormVM(this.temporary, this.ngRedux.getState());
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.loadData(d); } });
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
  }

  loadData(lookups: ILookupData) {
    this.lookups = lookups;
    let formGroupType = this.temporary ? FormGroupType.SearchTemporary : FormGroupType.SearchPermanent;
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
    this.viewGroups = this.lookups ? CViewGroup.getViewGroups(true, this.formGroup, this.displayMode, this.lookups.disabledControls) : [];
    this.searchCriteria = new CSearchCriteria(CSearchCriteria.getConfiguredItems(this.viewGroups));
    this.optionData.isHighLightSubstructure = this.ngRedux.getState().registrysearch.highLightSubstructure;
    this.update();
  }

  ngOnChanges() {
    this.update(false);
  }

  private update(forceUpdate: boolean = true) {
    // Don't keep changing cdd configuration
    if (!this.cddActivated && this.activated) {
      this.cddActivated = true;
    }
    if (forceUpdate) {
      this.changeDetector.markForCheck();
    }
  }

  private search() {
    let criteria = this.searchCriteria.serialize();
    if (criteria.match(/searchcriteriaitem/gi).length > 1) {
      let highLightSubstructure = this.ngRedux.getState().registrysearch.highLightSubstructure;
      let queryData: IQueryData = {
        temporary: this.temporary,
        searchCriteria: criteria,
        highlightSubStructures: highLightSubstructure
      };
      let url = `${apiUrlPrefix}search/`;
      if (this.isRefine) {
        url += `refineHitlist?hitlistId=${this.hitListId}`;
      } else {
        url += `${this.temporary ? 'temp-' : ''}records`;
      }
      this.http.post(url, queryData).toPromise()
        .then(res => {
          let response = res.json();
          if (this.isRefine) { 
            this.onRefine.emit(response);
          } else { 
            this.onSearch.emit(response);
          }
        })
        .catch(error => {
          notifyException(`The search failed due to a problem`, error, 5000);
        });
    } else {
      notify(`You must specify at least one field.`, `warning`, 5000);
    }
  }

  private get x2jsTool() {
    return new X2JS.default({
      arrayAccessFormPaths: [
        'searchCriteria.searchCriteriaItem',
      ]
    });
  }

  public restore(queryData: IQueryData) {
    this.searchCriteria = CSearchCriteria.deserialize(this.viewGroups, queryData.searchCriteria);
    this.changeDetector.markForCheck();
  }

  public clear() {
    this.searchCriteria = new CSearchCriteria(CSearchCriteria.getConfiguredItems(this.viewGroups));
    this.changeDetector.markForCheck();
  }

  private retrieveAll() {
    this.router.navigate([`records/${this.temporary ? 'temp' : ''}`]);
  }

  private savePreference(e) {
  }

  private restorePreference(e) {
  }

  private restoreCurrentQueryToForm() {
    let url = `${apiUrlPrefix}hitlists/${this.hitListId}/query${this.temporary ? '?temp=true' : ''}`;
    this.http.get(url).toPromise()
      .then(res => {
        let queryData = res.json() as IQueryData;
        this.restore(queryData);
      })
      .catch(error => {
        notifyException(`Restoring the current query failed due to a problem`, error, 5000);
      });
  }

  private restoreLastQueryToForm() {
    let url = `${apiUrlPrefix}hitlists/${this.temporary}/restoreLastQuery`;
    this.http.get(url).toPromise()
      .then(res => {
        let queryData = res.json() as IQueryData;
        this.restore(queryData);
      })
      .catch(error => {
        notifyException(`Restoring the last query failed due to a problem`, error, 5000);
      });
  }

  private cancel(e) {
    this.onClose.emit(e);
  }

  private onValueUpdated(e) {
    // console.log(this.searchItems);
  }

  private onOptionChanged(e) {
    this.actions.seachOptionChanged(this.optionData.isHighLightSubstructure);
  }
};
