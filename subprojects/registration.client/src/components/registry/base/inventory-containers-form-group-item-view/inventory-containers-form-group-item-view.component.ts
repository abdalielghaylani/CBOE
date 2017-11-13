import { Component, ChangeDetectionStrategy, OnInit, OnDestroy, ChangeDetectorRef, ElementRef, ViewChild, Input } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
@Component({
  selector: 'inventory-containers-form-group-item-view',
  template: require('./inventory-containers-form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InventoryContainersFormGroupItemView implements OnInit, OnDestroy {
  @ViewChild(DxDataGridComponent) grid: DxDataGridComponent;
  private gridHeight: string;
  @Input() invContainers: any[];
  private columns = [{
    dataField: 'id',
    caption: 'ID',
    dataType: 'string',
    allowEditing: false
  }, {
    dataField: 'qtyAvailable',
    caption: 'Qty Available',
    dataType: 'string',
    allowEditing: false
  },
  {
    dataField: 'containerSize',
    caption: 'Container Size',
    dataType: 'string',
    allowEditing: false
  }, {
    dataField: 'location',
    caption: 'Location',
    dataType: 'string',
    allowEditing: false
  }, {
    dataField: 'containerType',
    caption: 'Container Type',
    dataType: 'string',
    allowEditing: false
  }, {
    dataField: 'regBatchID',
    caption: 'Reg Number',
    dataType: 'string',
    allowEditing: false
  }];

  constructor(
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {

  }

  ngOnDestroy() {
  }

  private get totalQuantityAvailable(): string {
    if (this.invContainers && this.invContainers.length > 0) {
      let invContainer = this.invContainers[0];
      return `Total Quantity Available:  ${invContainer.totalQtyAvailable}`;
    }
    return '';
  }

  private get numberOfContainers(): string {
    if (this.invContainers) {
      return `Number of Containers:  ${this.invContainers.length}`;
    }
    return '';
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
  }

  private onCellPrepared(e) {
  }
};
