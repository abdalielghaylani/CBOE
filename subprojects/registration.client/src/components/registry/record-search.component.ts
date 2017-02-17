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
import { RegistrySearchActions, ConfigurationActions } from '../../actions';
import { IAppState, IRecordDetail } from '../../store';
import { Router, ActivatedRoute } from '@angular/router';

declare var jQuery: any;

@Component({
  selector: 'reg-record-search',
  styles: [require('./records.css')],
  template: require('./record-search.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordSearch implements OnInit, OnDestroy {
  private title: string = 'Search Permanent Registry';
  private regsearch: regSearchTypes.CSearchFormVM;

  constructor(
    private router: Router,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef,
    private ngRedux: NgRedux<IAppState>,
    private actions: RegistrySearchActions) {
  }

  ngOnInit() {
    this.regsearch = new regSearchTypes.CSearchFormVM(this.ngRedux.getState());
    this.loadData();
  }

  ngOnDestroy() {
  }

  loadData() {

  }

  search() {

  }
  clear() {

  }
  back() {
    this.router.navigate(['records']);
  }
  editHitlist() {
    this.regsearch.hitlistVM.hitlistEdit = true;
    this.regsearch.hitlistVM.hitlistRestore = false;
  }
  restoreHitlist() {
    this.regsearch.hitlistVM.hitlistEdit = false;
    this.regsearch.hitlistVM.hitlistRestore = true;
  }
};
