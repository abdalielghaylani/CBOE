import { Component, OnDestroy, ElementRef, ViewChild, ViewChildren } from '@angular/core';
import { select, NgRedux } from '@angular-redux/store';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { DxDataGridComponent } from 'devextreme-angular';
import { HttpService } from '../../../services';
import { IAppState, ILookupData } from '../../../redux';

@Component({
  selector: 'reg-config-base',
  template: ``
})
export class RegConfigBaseComponent implements OnDestroy {
  @ViewChild(DxDataGridComponent) grid;
  @ViewChildren(DxDataGridComponent) grids;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;
  protected ngUnsubscribe: Subject<void> = new Subject<void>();
  protected elementRef: ElementRef;
  protected http: HttpService;
  protected gridHeight: string;
  protected lookups: ILookupData;
  protected lookupsSubscription: Subscription;

  constructor(elementRef: ElementRef, http: HttpService) {
    this.elementRef = elementRef;
    this.http = http;
  }

  ngOnInit() {
    if (this.lookupsSubscription == null) {
      this.lookupsSubscription = this.lookups$.subscribe(d => {
        if (d != null && this.lookups !== d) {
          this.lookups = d;
          this.loadData(d);
        }
      });
    }
  }

  ngOnDestroy() {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  loadData(lookups: ILookupData) {
  }

  getGridHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  togglePanel(event) {
    const target = event.target || event.srcElement;
    if (target.children.length > 0) {
      target.children[0].click();
    }
  }

  onResize(event: any) {
    this.gridHeight = this.getGridHeight();
    this.grid.height = this.getGridHeight();
    this.grid.instance.repaint();
  }

  onDocumentClick(event: any) {
    const target = event.target || event.srcElement;
    if (target.title === 'Full Screen') {
      let fullScreenMode = target.className === 'fa fa-compress fa-stack-1x white';
      this.gridHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
      this.grid.height = this.gridHeight;
      this.grid.instance.repaint();
    }
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
        $links.filter('.dx-link-save').addClass('dx-icon-save');
        $links.filter('.dx-link-cancel').addClass('dx-icon-revert');
      } else {
        $links.filter('.dx-link-edit').addClass('dx-icon-edit');
        $links.filter('.dx-link-delete').addClass('dx-icon-trash');
      }
    }
  }
}
