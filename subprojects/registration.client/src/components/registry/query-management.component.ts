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
import { IAppState, HitlistType, IHitlistData, IHitlistInfo, ISearchRecords } from '../../store';
import { DxDataGridComponent } from 'devextreme-angular';
import * as regSearchTypes from './registry-search.types';

@Component({
  selector: 'reg-search-query',
  template: require('./query-management.component.html'),
  styles: [require('./records.css')],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegQueryManagement implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @Input() temporary: boolean;
  @Input() hitlistVM: regSearchTypes.CQueryManagementVM;
  @Input() parentHeight: string;
  @Input() hitlistId: number;
  @Output() onClose = new EventEmitter<any>();
  private hitlistData$: Observable<ISearchRecords>;
  private records: any[];
  private selectedHitlist: { id: number, type: number };
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
    this.records = this.ngRedux.getState().registrysearch.hitlist.rows;
    this.changeDetector.markForCheck();
  }

  onRowRemoving(e) {
    this.actions.deleteHitlist(e.data.id);
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
    let oldData = <IHitlistData>e.oldData;
    let newData = <IHitlistData>e.newData;
    this.actions.updateHitlist({
      name: newData.name ? newData.name : oldData.name,
      description: newData.description ? newData.description : oldData.description,
      isPublic: newData.isPublic ? newData.isPublic : oldData.isPublic,
      hitlistType: oldData.hitlistType,
      hitlistId: oldData.hitlistId
    });
  }

  moveToSaveHitlist(e: IHitlistData) {
    this.actions.updateHitlist({
      name: e.name,
      description: e.description,
      isPublic: e.isPublic,
      hitlistType: HitlistType.SAVED,
      hitlistId: e.hitlistId
    });
  }

  onEditorPreparing(e) {
    if (e.rowType === 'data') {
      e.editRow(e.rowIndex);
    }
  }

  showAdvRestorePopup(e) {
    let data = <IHitlistData>e.data;
    if (this.hitlistId && this.hitlistId > 0 && data.hitlistId !== this.hitlistId) {
      e.component.collapseAll(-1);
      e.component.expandRow(e.key);
      this.selectedHitlist = { id: data.id, type: data.hitlistType };
    }
  }

  hideRestore(e) {
    e.component.collapseAll(-1);
  }

  advancedRestorePopup(e: IHitlistData) {
    this.actions.retrieveHitlist({
      type: 'Advanced',
      id: e.id,
      temporary: this.temporary,
      data: {
        id1: this.hitlistId,
        id2: this.selectedHitlist.id,
        op: this.hitlistVM.advancedRestoreType
      }
    });
    this.onClose.emit(e);
  }

  restoreSelectedHitlist(e: IHitlistData) {
    this.actions.retrieveHitlist({ type: 'Retrieve', temporary: this.temporary, id: e.id });
    this.onClose.emit(e);
  }

  refreshSelectedHitlist(e: IHitlistData) {
    this.actions.retrieveHitlist({ type: 'Refresh', temporary: this.temporary, id: e.id });
    this.onClose.emit(e);
  }

  cancel(e) {
    this.onClose.emit(e);
  }

};
