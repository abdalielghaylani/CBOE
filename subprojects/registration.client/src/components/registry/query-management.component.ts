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
import { DxDataGridComponent } from 'devextreme-angular';
import { RegistrySearchActions } from '../../actions';
import { IAppState, HitlistType, IHitlistData, IHitlistInfo, ISearchRecords, IQueryData } from '../../store';
import * as regSearchTypes from './registry-search.types';
import { apiUrlPrefix } from '../../configuration';
import { HttpService } from '../../services';
import { notifyError, notifyException, notifySuccess } from '../../common';

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
    private http: HttpService,
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
    this.actions.deleteHitlist(this.temporary, e.data.id);
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
    this.actions.updateHitlist(this.temporary, {
      name: newData.name ? newData.name : oldData.name,
      description: newData.description ? newData.description : oldData.description,
      isPublic: newData.isPublic ? newData.isPublic : oldData.isPublic,
      hitlistType: oldData.hitlistType,
      hitlistId: oldData.hitlistId
    });
  }

  moveToSaveHitlist(e: IHitlistData) {
    this.actions.updateHitlist(this.temporary, {
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

  private showAdvRestorePopup(e) {
    let data = <IHitlistData>e.data;
    if (this.hitlistId && this.hitlistId > 0 && data.hitlistId !== this.hitlistId) {
      e.component.collapseAll(-1);
      e.component.expandRow(e.key);
      this.selectedHitlist = { id: data.id, type: data.hitlistType };
    }
  }

  private hideRestore(e) {
    e.component.collapseAll(-1);
  }

  private advancedRestorePopup(e: IHitlistData) {
    this.actions.retrieveHitlist(this.temporary, {
      type: 'Advanced',
      id: e.id,
      data: {
        id1: this.hitlistId,
        id2: this.selectedHitlist.id,
        op: this.hitlistVM.advancedRestoreType
      }
    });
    this.grid.instance.collapseAll(-1);
    this.onClose.emit(e);
  }

  private restoreSelectedHitlist(e: IHitlistData) {
    if (this.hitlistId !== e.id) {
      this.actions.retrieveHitlist(this.temporary, { type: 'Retrieve', id: e.id });
      this.onClose.emit(e);
    }
  }

  private refreshSelectedHitlist(e: IHitlistData) {
    this.actions.retrieveHitlist(this.temporary, { type: 'Refresh', id: e.id });
    this.onClose.emit(e);
  }

  private restoreQueryToForm(e: IHitlistData) {
    let url = `${apiUrlPrefix}${this.temporary ? 'temp-' : ''}records/${e.hitlistId}/query`;
    this.http.get(url).toPromise()
      .then(res => {
        let queryData = res.json() as IQueryData;
      })
      .catch(error => {
        notifyException(`Restoring the selected query failed due to a problem`, error, 5000);
      });
  }

  private cancel(e) {
    this.onClose.emit(e);
  }
};
