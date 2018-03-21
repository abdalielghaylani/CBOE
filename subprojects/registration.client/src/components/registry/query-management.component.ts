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
import * as regSearchTypes from './registry-search.types';
import { notifyError, notifyException, notifySuccess } from '../../common';
import { RegistrySearchActions, IAppState, HitlistType, IHitlistData, IHitlistInfo, ISearchRecords, IQueryData } from '../../redux';
import { apiUrlPrefix } from '../../configuration';
import { HttpService } from '../../services';

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
  @Output() onClose = new EventEmitter<IHitlistData>();
  @Output() onRestore = new EventEmitter<IQueryData>();
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

  onInitialized(e) {
    if (!e.component.columnOption('command:edit', 'visibleIndex')) {
      e.component.columnOption('command:edit', {
        visibleIndex: -1,
        width: 80
      });
    }
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
      isPublic: newData.isPublic !== undefined ? newData.isPublic : oldData.isPublic,
      hitlistType: oldData.hitlistType,
      hitlistId: oldData.hitlistId
    });
  }

  moveToSaveHitlist(e: IHitlistData) {
    this.http.put(`${apiUrlPrefix}hitlists/${e.hitlistId}${this.temporary ? '?temp=true' : ''}`, {
      name: e.name,
      description: e.description,
      isPublic: e.isPublic,
      hitlistType: HitlistType.SAVED,
      hitlistId: e.hitlistId
    }).toPromise()
      .then(res => {
        notifySuccess(`The selected hitlist was saved successfully!`, 5000);
        this.hitlistId = res.json().id;
        this.selectedHitlist = { id: res.json().id, type: HitlistType.SAVED };
        this.actions.openHitlists(this.temporary);
      })
      .catch(error => Observable.of(RegistrySearchActions.updateHitlistErrorAction()));
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
    let params = this.temporary ? '?temp=true' : '';
    let url = `${apiUrlPrefix}hitlists/${this.hitlistId}/${this.hitlistVM.advancedRestoreType}/${this.selectedHitlist.id}/records${params}`;
    this.http.get(url).toPromise()
      .then(res => {
        e.hitlistId = res.json().id;
        this.grid.instance.collapseAll(-1);
        this.onClose.emit(e);
      })
      .catch(error => {
        notifyException(`Restoring the selected query failed due to a problem`, error, 5000);
      });
  }

  private restoreSelectedHitlist(e: IHitlistData) {
    if (this.hitlistId !== e.id) {
      this.onClose.emit(e);
    }
  }

  private refreshSelectedHitlist(e: IHitlistData) {
    let params = this.temporary ? '?temp=true' : '';
    let url = `${apiUrlPrefix}hitlists/${e.hitlistId}/performQuery${params}`;
    this.http.get(url).toPromise()
      .then(res => {
        e.hitlistId = res.json().id;
        this.onClose.emit(e);
      })
      .catch(error => {
        notifyException(`Restoring the selected query failed due to a problem`, error, 5000);
      });
  }

  private restoreQueryToForm(e: IHitlistData) {
    let url = `${apiUrlPrefix}hitlists/${e.hitlistId}/query${this.temporary ? '?temp=true' : ''}`;
    this.http.get(url).toPromise()
      .then(res => {
        let queryData = res.json() as IQueryData;
        this.onRestore.emit(queryData);
      })
      .catch(error => {
        notifyException(`Restoring the selected query failed due to a problem`, error, 5000);
      });
  }

  private cancel(e) {
    this.onClose.emit(e);
  }
};
