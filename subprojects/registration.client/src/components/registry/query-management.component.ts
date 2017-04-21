import {
  Component,
  Input, ViewChild,
  Output,
  OnInit,
  OnDestroy,
  EventEmitter,
  ChangeDetectorRef, ChangeDetectionStrategy,
} from '@angular/core';
import { select, NgRedux } from '@angular-redux/store';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { RegistrySearchActions } from '../../actions';
import { IAppState, ISearchRecords } from '../../store';
import { DxDataGridComponent } from 'devextreme-angular';
import * as regSearchTypes from './registry-search.types';

@Component({
  selector: 'reg-search-query',
  template: require('./query-management.component.html'),
  styles: [require('./records.css')],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegQueryManagemnt implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @Input() hitlistVM: any;
  private hitlistData$: Observable<ISearchRecords>;
  private records: any[];
  private currentHitlist: any[any];
  private selectedHitlist: any[any];
  private recordsSubscription: Subscription;

  constructor(
    private router: Router,
    private ngRedux: NgRedux<IAppState>,
    private actions: RegistrySearchActions,
    private changeDetector: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.hitlistData$ = this.ngRedux.select(['registrysearch', 'hitlist']);
    this.recordsSubscription = this.hitlistData$.subscribe(() => { this.loadData(); });
  }

  ngOnDestroy() {
    if (this.recordsSubscription) {
      this.recordsSubscription.unsubscribe();
    }
  }

  loadData() {
    this.hitlistVM.advancedRestoreType = 0;
    this.records = this.ngRedux.getState().registrysearch.hitlist.rows;
    this.currentHitlist = this.ngRedux.getState().registrysearch.hitlist.currentHitlistInfo;
    this.changeDetector.markForCheck();
  }

  onRowRemoving(e) {
    this.actions.deleteHitlist(e.data.ID);
    this.loadData();
  }

  onContentReady(e) {
    e.component.columnOption('command:edit', {
      visibleIndex: -1,
      width: 80
    });
  }

  onCellPrepared(e) {
    if (e.rowType === 'data' && e.column.command === 'edit') {
      let isEditing = e.row.isEditing;
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      if (isEditing) {
        $links.filter('.dx-link-save').addClass('dx-icon-todo');
        $links.filter('.dx-link-cancel').addClass('dx-icon-revert');
      } else {
        $links.filter('.dx-link-edit').addClass('dx-icon-edit');
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
      }
    }
  }

  onRowUpdating(e) {
    this.actions.updateHitlist({
      Name: e.newData.Name ? e.newData.Name : e.oldData.Name,
      Description: e.newData.Description ? e.newData.Description : e.oldData.Description,
      IsPublic: (e.newData.IsPublic === undefined ? e.oldData.IsPublic : e.newData.IsPublic) === true ? 1 : 0,
      HitlistType: e.oldData.HistlistType,
      hitlistID: e.oldData.ID
    });
  }

  moveToSaveHitlist(e) {
    this.actions.updateHitlist({
      Name: e.Name,
      Description: e.Description,
      IsPublic: e.IsPublic,
      HitlistType: 1,
      hitlistID: e.ID
    });
  }

  onEditorPreparing(e) {
    if (e.rowType === 'data') {
      e.editRow(e.rowIndex);
    }
  }

  showRestore(e) {
    e.component.collapseAll(-1);
    e.component.expandRow(e.key);
    this.selectedHitlist = { 'HitlistID': e.data.ID, 'HitlistType': e.data.HistlistType };
    if (this.currentHitlist) {
      this.hitlistVM.isCurrentHitlist = true;
    }
  }
  hideRestore(e) {
    e.component.collapseAll(-1);
  }

  advancedRestorePopup(e) {
    this.actions.retrieveHitlist(
      {
        type: 1,
        data: {
          HitlistID1: this.selectedHitlist.HitlistID,
          HitlistID2: !this.currentHitlist.HitlistID ? 0 : this.currentHitlist.HitlistID,
          RestoreType: this.hitlistVM.advancedRestoreType,
          HitlistType1: this.selectedHitlist.HitlistType,
          HitlistType2: !this.currentHitlist.HitlistType ? 0 : this.currentHitlist.HitlistType,
        }
      });
    this.router.navigate([`records/restore`]);
  }

  restoreSelectedHitlist(e) {
    this.actions.retrieveHitlist({ id: e.ID, type: 0 });
  }

};
