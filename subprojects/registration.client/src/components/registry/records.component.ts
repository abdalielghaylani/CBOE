import {
  Component,
  Input, ViewChild,
  Output,
  OnInit,
  OnDestroy,
  EventEmitter,
  ChangeDetectorRef, ChangeDetectionStrategy, ElementRef,
  Directive, HostListener
} from '@angular/core';
import { select, NgRedux } from '@angular-redux/store';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { RegistryActions, RegistrySearchActions } from '../../actions';
import { IAppState, CRecordsData, IRecords, ISearchRecords } from '../../store';
import { DxDataGridComponent } from 'devextreme-angular';
import { notify, notifyError, notifySuccess } from '../../common';
import * as regSearchTypes from './registry-search.types';
import { CRecords } from './registry.types';
import CustomStore from 'devextreme/data/custom_store';
import { fetchLimit } from '../../configuration';

@Component({
  selector: 'reg-records',
  template: require('./records.component.html'),
  styles: [require('./records.css')],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecords implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @Input() temporary: boolean;
  @Input() restore: boolean;
  @select(s => s.session.lookups) lookups$: Observable<any>;
  @select(s => !!s.session.token) loggedIn$: Observable<boolean>;
  private records$: Observable<IRecords>;
  private lookupsSubscription: Subscription;
  private recordsSubscription: Subscription;
  private hitlistSubscription: Subscription;
  private lookups: any;
  private popupVisible: boolean = false;
  private rowSelected: boolean = false;
  private selectedRows: any[] = [];
  private tempResultRows: any[];
  private hitlistVM: regSearchTypes.CQueryManagementVM = new regSearchTypes.CQueryManagementVM(this.ngRedux.getState());
  private hitlistData$: Observable<ISearchRecords>;
  private isMarkedQuery: boolean;
  private loadIndicatorVisible: boolean = false;
  private records: CRecords;
  private gridHeight: string;
  private currentIndex: number = 0;
  private dataStore: CustomStore;
  private sortCriteria: string;

  constructor(
    private router: Router,
    private ngRedux: NgRedux<IAppState>,
    private registryActions: RegistryActions,
    private actions: RegistrySearchActions,
    private element: ElementRef,
    private changeDetector: ChangeDetectorRef) {
    this.records = new CRecords(this.temporary, new CRecordsData(this.temporary));
    this.createCustomStore(this);
  }

  ngOnInit() {
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
    if (this.recordsSubscription) {
      this.recordsSubscription.unsubscribe();
    }
    if (this.hitlistSubscription) {
      this.hitlistSubscription.unsubscribe();
    }
  }

  // Trigger data retrieval for the view to show.
  retrieveContents(lookups: any) {
    this.lookups = lookups;
    if (this.loggedIn$) {
      this.loadIndicatorVisible = true;
    }
    if (this.restore) {
      this.restoreHitlist();
    } else {
      this.loadData();
    }
  }

  restoreHitlist() {
  }

  getGridHeight() {
    return ((this.element.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  loadData() {
    this.registryActions.openRecords({
      temporary: this.temporary,
      skip: this.records.data.rows.length,
      take: fetchLimit,
      sort: this.sortCriteria
    });
    this.records$ = this.ngRedux.select(['registry', this.temporary ? 'tempRecords' : 'records']);
    if (!this.recordsSubscription) {
      this.recordsSubscription = this.records$.subscribe(d => { this.openRegistryRecords(d); });
    }
    this.actions.openHitlists();
    this.hitlistData$ = this.ngRedux.select(['registrysearch', 'hitlist']);
    if (this.hitlistSubscription) {
      this.hitlistSubscription = this.hitlistData$.subscribe(() => {
        this.hitlistVM = new regSearchTypes.CQueryManagementVM(this.ngRedux.getState());
        this.changeDetector.markForCheck();
      });
    }
    this.gridHeight = this.getGridHeight();
  }

  openRegistryRecords(records: IRecords) {
    this.loadIndicatorVisible = false;
    if (records.data.startIndex === 0) {
      this.records.temporary = records.temporary;
      if (records.data.rows.length < records.data.totalCount) {
        this.records.filterRow.visible = false;
      }
      this.records.setRecordData(records.data);
      this.records.gridColumns = records.gridColumns.map(s => this.updateGridColumn(s));
    } else if (this.records.data.rows.length > 0 && this.records.data.rows.length === records.data.startIndex) {
      this.records.setRecordData(records.data);
    }
    this.createCustomStore(this);
    this.changeDetector.markForCheck();
  }

  createCustomStore(ref: RegRecords) {
    ref.dataStore = new CustomStore({
      load: function (loadOptions) {
        if (loadOptions.sort !== null) {
          let sortCriteria = !loadOptions.sort[0].desc ? loadOptions.sort[0].selector : loadOptions.sort[0].selector + ' DESC';
          if (ref.sortCriteria !== sortCriteria && ref.records.data.rows.length !== ref.records.data.totalCount) {
            ref.sortCriteria = sortCriteria;
            ref.updateContents();
          }
        }
        if (ref.records.data.rows.length === ref.records.data.totalCount) {
          ref.records.filterRow.visible = true;
        }
        return ref.records.getFetchedRows();
      }
    });
  }

  updateGridColumn(gridColumn) {
    if (gridColumn.lookup) {
      gridColumn.lookup = {
        dataSource: this.lookups.users,
        displayExpr: 'USERID',
        valueExpr: 'PERSONID'
      };
    }
    return gridColumn;
  }

  onResize(event: any) {
    this.gridHeight = this.getGridHeight();
    this.grid.height = this.getGridHeight();
    this.grid.instance.repaint();
  }

  onContentReady(e) {
    e.component.columnOption('command:edit', {
      visibleIndex: -1,
      width: 80
    });
  }

  updateContents() {
    this.registryActions.openRecords({
      temporary: this.temporary,
      skip: this.records.data.rows.length,
      take: fetchLimit,
      sort: this.sortCriteria
    });
  }

  onRowPrepared(e) {
    if (e.rowType === 'data') {
      if (this.records.data.rows.length < this.records.data.totalCount) {
        if (e.rowIndex === this.records.data.rows.length - 1) {
          this.updateContents();
        }
      }
    }
  }

  onCellPrepared(e) {
    if (e.rowType === 'data' && e.column.command === 'edit') {
      let isEditing = e.row.isEditing;
      let $links = e.cellElement.find('.dx-link');
      $links.text('');
      if (isEditing) {
        $links.filter('.dx-link-save').addClass('dx-icon-save');
        $links.filter('.dx-link-cancel').addClass('dx-icon-revert');
      } else {
        $links.filter('.dx-link-edit').addClass('dx-icon-edit');
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
      }
      // if (e.rowIndex === this.records.data.rows.length - 1
      //   && this.records.data.rows.length < this.records.data.totalCount) {
      //   new Promise((resolve, reject) => {
      //       this.updateContents();
      //       resolve(this.records.data.rows);
      //     });
      // }
    }
  }

  onToolbarPreparing(e) {
    e.toolbarOptions.items.unshift({
      location: 'before',
      template: 'toolbarContents'
    });
  }

  onSelectionChanged(e) {
    this.selectedRows = e.selectedRowKeys;
  }

  onSearchClick(e) {
    e.cancel = true;
    this.router.navigate([`search/${this.temporary ? 'temp' : ''}`]);
  }

  onInitNewRow(e) {
    e.cancel = true;
    this.router.navigate(['records/new']);
  }

  onEditingStart(e) {
    e.cancel = true;
    let id = e.data[Object.keys(e.data)[0]];
    this.router.navigate([`records/${this.temporary ? 'temp' : ''}/${id}`]);
  }

  manageQueries() {
    this.actions.openHitlists();
    this.currentIndex = 1;
  }

  newQuery() {
    this.currentIndex = 2;
  }

  saveQuery(isMarked: boolean) {
    this.isMarkedQuery = isMarked;
    this.hitlistVM.saveQueryVM.clear();
    this.popupVisible = true;
  }

  saveHitlist() {
    if (this.hitlistVM.saveQueryVM.data.Name && this.hitlistVM.saveQueryVM.data.Description) {
      if (this.isMarkedQuery === true) {
        this.hitlistVM.saveQueryVM.clear();
        notifySuccess('Marked records saved successfully!', 5000);
      } else {
        this.hitlistVM.saveQueryVM.clear();
        notifySuccess('Query saved successfully!', 5000);
      }
    } else {
      notifyError('Name and Description is required!', 5000);
    }
  }

  editQuery(id: Number) {
    // TODO: It should show the search page and populate the contents with hitlist query.
    // this.router.navigate([`search/${id}`]);
    this.currentIndex = 2;
  }

  cancelSaveQuery() {
    this.popupVisible = false;
  }

  headerClicked(e) {
    this.currentIndex = 0;
  }

  showMarked() {
    if (this.selectedRows && !this.rowSelected) {
      this.rowSelected = true;
      this.tempResultRows = this.records.data.rows;
      this.records.data.rows = this.selectedRows;
    }
  }

  showSearchResults() {
    if (this.rowSelected) {
      this.rowSelected = false;
      this.selectedRows = this.records.data.rows;
      this.records.data.rows = this.tempResultRows;
      this.tempResultRows = [];
    }
  }

  printRecords() {
    let printContents, popupWin;
    printContents = document.getElementById('grdRecords').innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(`<html>
        <head>
          <title>Print table</title>
          <style>
          table, th, td 
          {
              border:solid 1px #f0f0f0;
              font:15px Verdana;
          }
          .dx-header-row {
              font-weight:bold;
          }
          img {
            height:120px;
          }
          .dx-datagrid-header-panel,.dx-datagrid-filter-row,.dx-datagrid-pager,
          .dx-command-edit,.dx-command-select{
            display:none;
          }
          </style>
        </head>
    <body onload="window.print();window.close()">${printContents}</body>
      </html>`);
    popupWin.document.close();
  }
};
