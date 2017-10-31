
import { Component } from '@angular/core';
import * as dxDialog from 'devextreme/ui/dialog';
import { invIntegrationBasePath } from '../../../configuration';
import { IRegInvModel } from '../registry.types';

@Component({
  selector: 'create-inventory-container',
  template: ''
})
export class RegInvContainerCreator {

  constructor() {
  }

  public createContainer(invModel: IRegInvModel) {
    if (!this.isBrowserIE()) {
      dxDialog.alert(`Inventory integration features are supported in Internet Explorer only.
        In order to create container(s) please use Internet Explorer web browser`, `Warning`);
      return;
    }
    let chemInvUrlSection: string = invModel.isBulkContainerCreation
      ? (`/cheminv/gui/ImportFromChemReg.asp?RegIDList=3,5&OpenAsModalFrame=false`)
      : (`/Cheminv/gui/CreateOrEditContainer.asp?GetData=new&vRegBatchID=${invModel.batchIDs[0]}&RefreshOpenerLocation=true`);
    let windowParams: string = invModel.isBulkContainerCreation
      ? (`width=1100,height=700`)
      : (`width=800,height=700`);
    let createContainerUrl: string = invIntegrationBasePath + chemInvUrlSection;
    window.open(createContainerUrl, 'Create New Container', windowParams);
  }

  private isBrowserIE() {
    return (!!navigator.userAgent.match(/Trident/g) || !!navigator.userAgent.match(/MSIE/g));
  }

};
