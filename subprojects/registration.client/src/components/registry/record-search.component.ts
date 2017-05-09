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
  private title: string;
  private tabSelected: string = 'search';
  private regSearch: regSearchTypes.CSearchFormVM;
  @ViewChild(ChemDrawingTool)
  private chemDrawWeb: ChemDrawingTool;

  constructor(
    private router: Router,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef,
    private ngRedux: NgRedux<IAppState>,
    private actions: RegistrySearchActions) {
  }

  ngOnInit() {
    this.title = this.temporary ? 'Search Temporary Records' : 'Search Permanent Registry';
    this.regSearch = new regSearchTypes.CSearchFormVM(this.ngRedux.getState());
  }

  ngOnDestroy() {
  }

  ngOnChanges() {
    if (this.activated && this.chemDrawWeb) {
      this.chemDrawWeb.activate();
    }
  }

  search() {
    this.regSearch.registrySearchVM.structureData = this.chemDrawWeb.getValue();
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

};
