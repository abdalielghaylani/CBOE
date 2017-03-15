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
  template: `<dx-popup
        class="popup"
        [width]="350"
        [height]="300"
        [showTitle]="true"
        title="Restore Hitlist"
        [dragEnabled]="false"
        [closeOnOutsideClick]="true"
        [(visible)]="popupVisible">
        <div *dxTemplate="let data of 'content'">
            <div class="panel" id="restore-hitlist" *ngIf="popupVisible">
                          <div class="panel-heading">
                            <b style="font-size:14px;">Please choose a restore type below</b>
                          </div>
                          <div class="panel-body">
                            <div class="radio">
                              <label><input type="radio" name="optradio">Replace current list</label>
                            </div>
                            <div class="radio">
                              <label><input type="radio" name="optradio">Intersect with current list</label>
                            </div>
                            <div class="radio">
                              <label><input type="radio" name="optradio">Subtract from current list</label>
                            </div>
                            <div class="radio">
                              <label><input type="radio" name="optradio">Union with current list</label>
                            </div>
                            <div style="margin-top: 15px">
                              <button class="mr1 btn btn-primary">Restore</button>
                              <a class="mr1 btn btn-primary" (click)="hideRestorePopup()">Cancel</a>
                            </div>
                          </div>
               </div>
        </div>
    </dx-popup>
      <dx-data-grid id="grdSearchQuery" [columns]=hitlistColumns
        [dataSource]=records [paging]='{pageSize: 20}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [10,15,20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' rowAlternationEnabled=true
        (onEditorPreparing)='onEditorPreparing($event)'
        (onRowRemoving)='onRowRemoving($event)'
        (onCellPrepared)='onCellPrepared($event)'
        (onEditingStart)='onEditingStart($event)'
        (onContentReady)='onContentReady($event)'
        (onRowUpdating)='onRowUpdating($event)' >
        <dxo-editing mode="row" [allowUpdating]="true" [allowDeleting]="true" [allowAdding]="false" 
        [texts]=" {
                confirmDeleteMessage: 'Are you sure you want to delete this hitlist?',
                confirmDeleteTitle: 'Remove Hitlist'
            }" ></dxo-editing>
       <div *dxTemplate="let d of 'restoreCellTemplate'" >
          <a class="fa fa-recycle grd-icon" aria-hidden="true" title='Restore Hitlist'></a>&nbsp;
          <a class="fa fa-bolt grd-icon" aria-hidden="true" title='Perform Query'></a>&nbsp;
          <a class="fa fa-eyedropper grd-icon" aria-hidden="true" title='Restore Query To Form'></a>&nbsp;
          <a class="fa fa fa-filter grd-icon" aria-hidden="true" title='Advanced' (click)="showRestorePopup($event)"></a>
       </div>
        <div *dxTemplate="let d of 'saveCellTemplate'" >
        <a *ngIf="d.data.HistlistType === 0" class="fa fa-plus-circle grd-icon" aria-hidden="true" 
        title='Add to saved hitlist' 
        (click)="moveToSaveHitlist(d.data)"></a>&nbsp;
        {{d.value}}
        </div>
      </dx-data-grid>
     `,
  styles: [require('./records.css')],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegQueryManagemnt implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @Input() hitlistColumns: any;
  private hitlistData$: Observable<ISearchRecords>;
  private records: any[];
  private recordsSubscription: Subscription;
  private popupVisible: boolean = false;

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
    this.actions.deleteHitlists(e.data.HistlistType, e.data.ID);
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
  onEditingStart(e) {

  }

  onRowUpdating(e) {
    this.actions.editHitlists(
      {
        Name: e.newData.Name ? e.newData.Name : e.oldData.Name,
        Description: e.newData.Description ? e.newData.Description : e.oldData.Description,
        IsPublic: (e.newData.IsPublic === undefined ? e.oldData.IsPublic : e.newData.IsPublic) === true ? 1 : 0,
        HitlistType: e.oldData.HistlistType,
        hitlistID: e.oldData.ID
      });
  }

  moveToSaveHitlist(e) {
    alert(e);
    this.actions.saveHitlists(
      {
        Name: e.Name,
        Description: e.Description,
        IsPublic: e.IsPublic,
        HitlistType: e.HistlistType,
        hitlistID: e.ID
      });

  }

  onEditorPreparing(e) {
    if (e.rowType === 'data') {
      e.editRow(e.rowIndex);
    }
  }

  showRestorePopup(e) {

    this.popupVisible = true;
  }
  hideRestorePopup() {
    this.popupVisible = false;
  }

};
