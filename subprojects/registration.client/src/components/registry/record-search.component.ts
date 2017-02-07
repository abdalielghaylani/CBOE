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
import { select, NgRedux } from 'ng2-redux';
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
  private registrySearchItems: any;
  private structureSearchItems: any;
  private componentSearchItems: any;
  private batchSearchItems: any;


  constructor(
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.registrySearchItems = regSearchTypes.REGISTRY_SEARCH_DESC_LIST;
    this.structureSearchItems = regSearchTypes.STRUCTURE_SEARCH_DESC_LIST;
    this.componentSearchItems = regSearchTypes.COMPONENT_SEARCH_DESC_LIST;
    this.batchSearchItems = regSearchTypes.BATCH_SEARCH_DESC_LIST;
  }

  ngOnDestroy() {
  }

  search() {

  }
  clear() {

  }

};
