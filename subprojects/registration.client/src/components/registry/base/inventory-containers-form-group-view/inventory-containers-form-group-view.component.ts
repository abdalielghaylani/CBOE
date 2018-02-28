import { Component, ChangeDetectionStrategy, OnInit, OnDestroy, ChangeDetectorRef, ElementRef, ViewChild, Input } from '@angular/core';
import { IInventoryContainerList, IAppState, CSystemSettings } from '../../../../redux/index';
import { RegInvContainerHandler } from '../../inventory-container-handler/inventory-container-handler';
import { NgRedux } from '@angular-redux/store';
import { PrivilegeUtils } from '../../../../common';
@Component({
  selector: 'inventory-containers-form-group-view',
  template: require('./inventory-containers-form-group-view.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InventoryContainersFormGroup implements OnInit {
  private id: string = 'invContainerForm';
  @Input() invContainers: IInventoryContainerList;
  private invHandler = new RegInvContainerHandler();
  private showRequestMaterialButton: boolean = false;

  constructor(private ngRedux: NgRedux<IAppState>) {
  }

  ngOnInit() {
    let lookups = this.ngRedux.getState().session.lookups;
    this.showRequestMaterialButton = PrivilegeUtils.hasBatchContainersRequestPrivilege(lookups.userPrivileges)
      && lookups.disabledControls.filter((i) => i.id === `ReqMaterial`).length === 0
      && new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings).showRequestMaterial
      && (this.invContainers.containers && (this.invContainers.containers.length > 0));
  }

  protected togglePanel(event) {
    const target = event.target || event.srcElement;
    if (target.children.length > 0) {
      target.children[0].click();
    }
  }

  requestMaterial(e) {
    e.stopPropagation();
    this.invHandler.openContainerPopup(this.invContainers.containers[this.invContainers.containers.length - 1].requestURL + `&RequestType=R`, null);
  }
};
