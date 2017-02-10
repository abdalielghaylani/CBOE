import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  OnInit, OnDestroy, AfterViewInit,
  ElementRef, ChangeDetectorRef,
  ViewChildren, QueryList
} from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { select, NgRedux } from '@angular-redux/store';
import { DxFormComponent } from 'devextreme-angular';
import * as regSearchTypes from './registry-search.types';

declare var jQuery: any;

@Component({
  selector: 'reg-record-search',
  styles: [require('./records.css')],
  template: require('./record-search.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordSearch implements OnInit, OnDestroy {
  private title: string = 'Search Permanent Registry';
  private registrySearch: regSearchTypes.CRegSearchVM = new regSearchTypes.CRegSearchVM();
  private structureSearch: regSearchTypes.CStructureSearchVM = new regSearchTypes.CStructureSearchVM();
  private componentSearch: regSearchTypes.CCompoundSearchVM = new regSearchTypes.CCompoundSearchVM();
  private batchSearch: regSearchTypes.CBatchSearchVM = new regSearchTypes.CBatchSearchVM();
  private queryManagement: regSearchTypes.CQueryManagementVM = new regSearchTypes.CBatchSearchVM();


  constructor(
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.loadData();
  }

  ngOnDestroy() {
  }

  loadData() {
  // bind data for queryManagement here
  }

  search() {

  }
  clear() {

  }

};
