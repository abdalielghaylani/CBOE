import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgRedux, select } from '@angular-redux/store';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { getExceptionMessage, notify, notifyError, notifySuccess } from '../../common';
import { apiUrlPrefix } from '../../configuration';
import { ICustomTableData, IConfiguration, IAppState } from '../../store';
import { HttpService } from '../../services';

declare var jQuery: any;

@Component({
  selector: 'reg-templates',
  template: require('./templates.component.html'),
  styles: [require('./records.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegTemplates implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  @Output() onClose = new EventEmitter<any>();
  private rows: any[] = [];
  private dataSubscription: Subscription;
  private gridHeight: string;
  public dataSource: CustomStore;
  private columns = [{
    dataType: 'string',
    caption: 'Type',
    groupIndex: 0,
    allowEditing: false
  }, {
    dataField: 'id',
    visible: false,
    dataType: 'number',
    allowEditing: false
  }, {
    dataField: 'name',
    dataType: 'string',
    cellTemplate: 'loadCellTemplate'
  }, {
    dataField: 'description',
    dataType: 'string'
  }, {
    dataField: 'dateCreated',
    dataType: 'date',
    format: 'ShortDateShortTime',
    sortIndex: 0,
    sortOrder: 'desc',
    allowEditing: false
  }, {
    dataField: 'isPublic',
    caption: 'Public?',
    dataType: 'boolean',
    width: 60,
    allowEditing: false
  }, {
    dataField: 'data',
    dataType: 'string',
    allowEditing: false,
    allowFiltering: false,
    cellTemplate: 'structureCellTemplate'
  }];

  constructor(
    private ngRedux: NgRedux<IAppState>,
    private router: Router,
    private http: HttpService,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {
    let userName = this.ngRedux.getState().session.user.fullName.toUpperCase();
    let calculateCellValue = 'calculateCellValue';
    this.columns[0][calculateCellValue] = function (d) { return d.username.toUpperCase() === userName ? 'My Templates' : 'Shared Templates'; };
  }

  ngOnDestroy() {
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
  }

  public loadData() {
    this.dataSource = this.createCustomStore(this);
    this.changeDetector.markForCheck();
    this.gridHeight = this.getGridHeight();
  }

  private getGridHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  private onResize(event: any) {
    this.gridHeight = this.getGridHeight();
    this.grid.height = this.getGridHeight();
    this.grid.instance.repaint();
  }

  private onDocumentClick(event: any) {
    if (event.srcElement.title === 'Full Screen') {
      let fullScreenMode = event.srcElement.className === 'fa fa-compress fa-stack-1x white';
      this.gridHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
      this.grid.height = this.gridHeight;
      this.grid.instance.repaint();
    }
  }

  private onContentReady(e) {
    e.component.columnOption('command:edit', {
      visibleIndex: -1,
      width: 80
    });
  }

  private onCellPrepared(e) {
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
    }
  }

  private createCustomStore(parent: RegTemplates): CustomStore {
    let apiUrlBase = `${apiUrlPrefix}templates`;
    return new CustomStore({
      load: function (loadOptions) {
        let deferred = jQuery.Deferred();
        parent.http.get(apiUrlBase)
          .toPromise()
          .then(result => {
            let rows = result.json();
            deferred.resolve(rows, { totalCount: rows.length });
          })
          .catch(error => {
            let message = getExceptionMessage(`The submission templates were not retrieved properly due to a problem`, error);
            deferred.reject(message);
          });
        return deferred.promise();
      },

      remove: function (key) {
        let deferred = jQuery.Deferred();
        let id = key[Object.getOwnPropertyNames(key)[0]];
        parent.http.delete(`${apiUrlBase}/${id}`)
          .toPromise()
          .then(result => {
            notifySuccess(`The template ${id} was deleted successfully!`, 5000);
            deferred.resolve(result.json());
          })
          .catch(error => {
            let message = getExceptionMessage(`The template ${id} was not deleted due to a problem`, error);
            deferred.reject(message);
          });
        return deferred.promise();
      }
    });
  }

  private cancel(e) {
    this.onClose.emit(e);
  }

  private loadTemplate(templateId) {
    this.onClose.emit();
    this.router.navigate([`records/new/${templateId}`]);
  }
};
