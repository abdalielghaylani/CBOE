import { Component, ChangeDetectionStrategy, OnInit, OnDestroy, ChangeDetectorRef, ElementRef, ViewChild, Input } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import { RegInvContainerHandler } from '../../inventory-container-handler/inventory-container-handler';
import { NgRedux } from '@angular-redux/store';
import { IAppState, CSystemSettings } from '../../../../redux';
import { PrivilegeUtils } from '../../../../common';

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
  @Input() invContainersVM: any[] = [];
  private systemSettings: CSystemSettings;
  private columns: any[] = [];

  constructor(
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef,
    private ngRedux: NgRedux<IAppState>
  ) {
    let lookups = this.ngRedux.getState().session.lookups;
    let invSampleRequestPrivilege = PrivilegeUtils.hasBatchContainersRequestPrivilege(lookups.userPrivileges);
    let showRequestFromContainer = invSampleRequestPrivilege
      && new CSystemSettings(lookups.systemSettings).showRequestFromContainer;
    let showRequestFromBatch = invSampleRequestPrivilege && lookups.disabledControls.filter((i) => i.id === `RequestFromBatchURL`).length === 0
      && new CSystemSettings(lookups.systemSettings).showRequestFromBatch;
    this.columns = [{
      dataField: 'id',
      caption: 'ID',
      dataType: 'string',
      allowEditing: false,
      cellTemplate: 'containerCellTemplate'
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
    }, {
      dataField: '',
      caption: 'Request from Batch',
      dataType: 'string',
      alignment: 'center',
      cellTemplate: 'batchRequestCellTemplate',
      allowEditing: false,
      visible: showRequestFromBatch
    }, {
      dataField: '',
      caption: 'Request from Container',
      dataType: 'string',
      alignment: 'center',
      cellTemplate: 'containerRequestCellTemplate',
      allowEditing: false,
      visible: showRequestFromContainer
    }
    ];
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  private get totalQuantityAvailable(): string {
    if (this.invContainersVM && this.invContainersVM.length > 0) {
      let invContainer = this.invContainersVM[0];
      return `Total Quantity Available:  ${invContainer.totalQtyAvailable}`;
    }
    return '';
  }

  private get numberOfContainers(): string {
    if (this.invContainersVM) {
      return `Number of Containers:  ${this.invContainersVM.length}`;
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
    const target = event.target || event.srcElement;
    if (target.title === 'Full Screen') {
      let fullScreenMode = target.className === 'fa fa-compress fa-stack-1x white';
      this.gridHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
      this.grid.height = this.gridHeight;
      this.grid.instance.repaint();
    }
  }

  private onContentReady(e) {
  }

  private onCellPrepared(e) {
  }

  private viewContainerDetails(d) {
    let invHandler = new RegInvContainerHandler();
    invHandler.openContainerPopup(d.data.idUrl.split('\"')[1], null);
  }

  private openRequestForm(url) {
    let invHandler = new RegInvContainerHandler();
    invHandler.openContainerPopup(url, null);
  }
}
